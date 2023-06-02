using System.Linq;
using Checkers.Board;
using Checkers.ConfigTemplate;
using Checkers.Settings;
using Checkers.UI.Data;
using Core.Primitives;
using DG.Tweening;
using Global.ConfigTemplate;
using Global.Enums;
using Global.Scheduler.Base;
using Global.UI.Data;
using Global.Window;
using Global.Window.Enums;
using Global.Window.Signals;
using Nakama;
using Newtonsoft.Json;
using Server;
using Server.Services;
using UnityEngine;
using Zenject;

namespace Checkers.Services {
    public struct TurnData {
        public Coords To;
        public Coords From;
        public bool Capture;
    }

    public class MainCheckersOnlineService : ITickable {
        private readonly MainCheckerSceneSettings _sceneSettings;
        private readonly ISchedulerService _schedulerService;
        private readonly NakamaService _nakamaService;
        private readonly SignalBus _signalBus;
        private readonly AppConfig _appConfig;
        private readonly WindowService _windowService;
        private readonly MessageService _messageService;
        private readonly CheckersConfig _checkersConfig;
        private PawnColor _mainColor;
        private UnityEngine.Camera _cam;

        private TurnData _turnData;
        private bool _hasInput;

        public MainCheckersOnlineService(MainCheckerSceneSettings sceneSettings,
                                         ISchedulerService schedulerService, 
                                         NakamaService nakamaService,
                                         CheckersConfig checkersConfig,
                                         SignalBus signalBus,
                                         AppConfig appConfig,
                                         WindowService windowService,
                                         MessageService messageService) {
            _sceneSettings = sceneSettings;
            _schedulerService = schedulerService;
            _nakamaService = nakamaService;
            _signalBus = signalBus;
            _appConfig = appConfig;
            _windowService = windowService;
            _messageService = messageService;
            _checkersConfig = checkersConfig;
        }
        
        public void Initialize() {
            _signalBus.Fire(new OpenWindowSignal(WindowKey.MatchWindow, new MatchWindowData()));
            
            _mainColor = _appConfig.PawnColor;
            
            var turnHandler = _sceneSettings.TurnHandler;

            RotateBoardToBlack();

            _sceneSettings.TurnHandler.YourColor = _mainColor;
            
            _nakamaService.SubscribeToMatchState(OnMatchState);
            
            turnHandler.OnPawnCheck += OnPawnCheck;
            turnHandler.OnEndGame += OnEndGame;
            turnHandler.OnTurnChange += CheckTurn;
            
            CheckTurn(turnHandler.GetTurn());

            _sceneSettings.HeroHealthSlider.maxValue = 12;
            _sceneSettings.HeroHealthSlider.value = 12;

            _sceneSettings.EnemyHealthSlider.maxValue = 12;
            _sceneSettings.EnemyHealthSlider.value = 12;

            _sceneSettings.PawnMover.OnTurnEnd += () => {

            };
            
            _sceneSettings.PawnMover.OnTurn += (turnData) => {
                var currentTurn = _sceneSettings.TurnHandler.Turn;

                if (currentTurn != _mainColor) return;
                
                var matchId = _nakamaService.GetCurrentMatchId();
                
                var json = JsonConvert.SerializeObject(turnData);

                Debug.Log($"Send turn with data from {turnData.From} and to {turnData.To}");
                _nakamaService.SendMatchStateAsync(matchId, (long) CheckersMatchState.Turn, json);
            };
        }

        public void Tick() {
            if (_appConfig.GameEnded) return;
            
            CheckInput();

            CheckLeave();
            
            if (!_hasInput) return;

            _hasInput = false;
            
            var toCoords = _turnData.To;
            var tileTo = _sceneSettings.Getter.GetTile(toCoords.Column, toCoords.Row);

            var fromCoords = _turnData.From;
            var tileFrom = _sceneSettings.Getter.GetTile(fromCoords.Column, fromCoords.Row);

            Debug.Log($"Trying to move tile with coords from {fromCoords} and to coords {toCoords}");
            
            tileFrom.GetComponent<TileClickDetector>().MouseDown();
            tileTo.GetComponent<TileClickDetector>().MouseDown();
        }

        private void CheckLeave() {
            if (!_appConfig.Leave) return;

            _appConfig.Leave = false;
            _signalBus.Fire(new OpenWindowSignal(WindowKey.WinWindow, new WinWindowData{Reason = WinLoseReason.Concide}));
        }

        private void CheckInput() {
            if (!Input.GetMouseButtonDown(0)) return;
            if (_windowService.IsWindowOpened(WindowKey.PauseWindow)) return;
            if (_windowService.IsWindowOpened(WindowKey.WinWindow)) return;
            if (_windowService.IsWindowOpened(WindowKey.LoseWindow)) return;
            if (_windowService.IsWindowOpened(WindowKey.FleeWindow)) return;
            if (_windowService.IsWindowOpened(WindowKey.RulesWindow)) return;
            
            if (_sceneSettings.TurnHandler.Turn != _mainColor) return;
            var mouseInput = Input.mousePosition;

            var worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(mouseInput);

            var column = Mathf.RoundToInt(worldPosition.x);
            var row = Mathf.RoundToInt(worldPosition.z);
            
            if ((column < 0 || column > 7) || (row < 0 || row > 7)) return;
            var tileGetter = _sceneSettings.Getter;

            if (_appConfig.PawnColor == PawnColor.Black) {
                column = 7 - column;
                row = 7 - row;
            }
            var tile = tileGetter.GetTile(column, row);
            var clickDetector = tile.GetComponent<TileClickDetector>();
            clickDetector.MouseDown();
        }

        private void OnMatchState(IMatchState state) {
            var enc = System.Text.Encoding.UTF8;
            var content = enc.GetString(state.State);

            switch (state.OpCode) {
                case (long) CheckersMatchState.Turn: {
                    var turnData = JsonConvert.DeserializeObject<TurnData>(content);

                    Debug.Log($"Received raw data with from {turnData.From} and to {turnData.To}");
                    _turnData = turnData;
                    
                    Debug.Log($"Inverted data with from {_turnData.From} and to {_turnData.To}");

                    _hasInput = true;
                    break;
                }
                case (long) CheckersMatchState.WhiteTurnEnded: {
                    break;
                }
                case (long) CheckersMatchState.BlackTurnEnded: {
                    break;
                }
            }
        }

        private void RotateBoardToBlack() {
            if (_mainColor == PawnColor.White) return;
            _sceneSettings.BoardRoot.position = _checkersConfig.BoardBlackMainPosition;
            _sceneSettings.BoardRoot.rotation = Quaternion.Euler(_checkersConfig.BoardBlackMainRotation);
        }

        private void CheckTurn(PawnColor pawnColor) {
            string flyText = _mainColor == pawnColor ? "твой ход" : "ход соперника";
            var data = new FlyTextData {
                FlyText = flyText,
                PawnColor = pawnColor
            };
            _signalBus.Fire(new OpenWindowSignal(WindowKey.FlyText, data));
        }

        private void OnPawnCheck(PawnColor color, GameObject pawn) {
            var copy = Object.Instantiate(pawn, pawn.transform.position, pawn.transform.rotation);
            if (color == PawnColor.Black) {
                
                _sceneSettings.HeroHealthSlider.value -= 1;

                SendPawn(_sceneSettings.HeroTransform.position, copy);
            }
            else {
                _sceneSettings.EnemyHealthSlider.value -= 1;

                SendPawn(_sceneSettings.HeroTransform.position, copy);
            }
            
            Object.Destroy(pawn);
        }

        private void SendPawn(Vector3 endPosition, GameObject copy) {
            var startPosition = copy.transform.position;
            bool isPlayer = _sceneSettings.TurnHandler.Turn == _sceneSettings.TurnHandler.YourColor;
            var checkerPosition = isPlayer ? _sceneSettings._opponentBar : _sceneSettings._playerBar;

            var WP0 = new Vector3((checkerPosition.x + startPosition.x) / 2, startPosition.y, (startPosition.z + checkerPosition.z) / 2);
            var A = new Vector3((checkerPosition.x + startPosition.x) / 2, startPosition.y, startPosition.z);
            var B = new Vector3(WP0.x, WP0.y, WP0.z - 0.5f);
            var C = new Vector3(WP0.x, WP0.y, WP0.z + 0.5f);
            var D = new Vector3(WP0.x, startPosition.y, checkerPosition.z);
            var WP1 = new Vector3(checkerPosition.x, startPosition.y, checkerPosition.z);
            
            Vector3[] waypoints = new[] {WP0, A, B, WP1, C, D};
            
            copy.transform.DOPath(waypoints, 1, PathType.CubicBezier, PathMode.Ignore).SetEase(Ease.Linear).onComplete += () => { Object.Destroy(copy); };
            copy.transform.DOScale(Vector3.one / 1.5f, 1f);
        }

        private void OnEndGame(PawnColor color, WinLoseReason reason) {
            _appConfig.GameEnded = true;
            if (color != _mainColor) {
                _schedulerService
                    .StartSequence()
                    .Append(0.3f, () => { _signalBus.Fire(new OpenWindowSignal(WindowKey.WinWindow, new WinWindowData{Reason =  reason}));});
            }
            else {
                _schedulerService
                    .StartSequence()
                    .Append(0.3f, () => { _signalBus.Fire(new OpenWindowSignal(WindowKey.LoseWindow, new LoseWindowData{Reason = reason}));});
            }
        }
    }
}