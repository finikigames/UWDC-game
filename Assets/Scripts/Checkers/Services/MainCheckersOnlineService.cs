using System.Linq;
using Checkers.Board;
using Checkers.ConfigTemplate;
using Checkers.Settings;
using Checkers.UI.Data;
using Core.Extensions;
using Core.Primitives;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Global.ConfigTemplate;
using Global.Enums;
using Global.Scheduler.Base;
using Global.Window.Enums;
using Global.Window.Signals;
using Nakama;
using Newtonsoft.Json;
using Server.Services;
using UnityEngine;
using Zenject;

namespace Checkers.Services {
    public struct TurnData {
        public Coords To;
        public Coords From;
        public bool Capture;
    }
    
    public struct StartedSignal {
        public PawnColor StartColor;
    }

    public struct YouAreBlack {
        
    }
    
    public class MainCheckersOnlineService : ITickable {
        private readonly MainCheckerSceneSettings _sceneSettings;
        private readonly ISchedulerService _schedulerService;
        private readonly NakamaService _nakamaService;
        private readonly SignalBus _signalBus;
        private readonly AppConfig _appConfig;
        private readonly CheckersConfig _checkersConfig;
        private PawnColor _mainColor;
        private UnityEngine.Camera _cam;

        private TurnData _turnData;
        private bool _hasInput;
        private bool _someoneLeaved;
        private bool _endGame;

        public MainCheckersOnlineService(MainCheckerSceneSettings sceneSettings,
                                         ISchedulerService schedulerService, 
                                         NakamaService nakamaService,
                                         CheckersConfig checkersConfig,
                                         SignalBus signalBus,
                                         AppConfig appConfig) {
            _sceneSettings = sceneSettings;
            _schedulerService = schedulerService;
            _nakamaService = nakamaService;
            _signalBus = signalBus;
            _appConfig = appConfig;
            _checkersConfig = checkersConfig;
        }
        
        public void Initialize() {
            _signalBus.Fire(new OpenWindowSignal(WindowKey.MatchWindow, new MatchWindowData()));
            
            _mainColor = (PawnColor)_appConfig.PawnColor;
            
            var turnHandler = _sceneSettings.TurnHandler;

            RotateBoardToBlack();

            _sceneSettings.TurnHandler.YourColor = _mainColor;
            
            _nakamaService.SubscribeToMatchState(OnMatchState);
            _nakamaService.SubscribeToMatchPresence(OnMatchPresence);
            _sceneSettings.TurnHandler.OnEndGame += (s) => _endGame = true;
            
            turnHandler.OnPawnCheck += OnPawnCheck;
            turnHandler.OnEndGame += OnEndGame;

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

        private void OnMatchPresence(IMatchPresenceEvent obj) {
            if (obj.Leaves.Any()) {
                _someoneLeaved = true;
            }
        }

        public void Tick() {
            if (_endGame) return;
            
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
            if (!_someoneLeaved) return;

            _someoneLeaved = false;
            
            _signalBus.Fire(new OpenWindowSignal(WindowKey.WinWindow, new WinWindowData()));
        }

        private void CheckInput() {
            if (!Input.GetMouseButtonDown(0)) return;

            if (_sceneSettings.TurnHandler.Turn != _mainColor) return;
            var mouseInput = Input.mousePosition;

            var worldPosition = UnityEngine.Camera.main.ScreenToWorldPoint(mouseInput);

            var column = Mathf.RoundToInt(worldPosition.x);
            var row = Mathf.RoundToInt(worldPosition.z);
            
            if ((column < 0 || column > 7) || (row < 0 || row > 7)) return;
            var tileGetter = _sceneSettings.Getter;

            if (_appConfig.PawnColor == (int) PawnColor.Black) {
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
     
            var topPoint = new Vector3(endPosition.x, startPosition.y + 0.5f, endPosition.z); 
            
            Vector3[] waypoints = new[] {topPoint, startPosition, topPoint, endPosition, topPoint, endPosition};
            
            copy.transform.DOPath(waypoints, 1, PathType.CubicBezier, PathMode.Ignore).SetEase(Ease.Linear).onComplete += () => { Object.Destroy(copy); };
        }

        private void OnEndGame(PawnColor color) {
            if (color != _mainColor) {
                _schedulerService
                    .StartSequence()
                    .Append(0.3f, () => { _signalBus.Fire(new OpenWindowSignal(WindowKey.WinWindow, new WinWindowData()));});
            }
            else {
                _schedulerService
                    .StartSequence()
                    .Append(0.3f, () => { _signalBus.Fire(new OpenWindowSignal(WindowKey.LoseWindow, new LoseWindowData()));});
            }
        }
    }
}