using System.Threading.Tasks;
using Checkers.ConfigTemplate;
using Checkers.Settings;
using Core.Extensions;
using Core.Primitives;
using DG.Tweening;
using Global.Extensions;
using Global.Services.Scheduler.Base;
using Nakama;
using Newtonsoft.Json;
using Server.Services;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace Checkers.Services {
    public struct TurnData {
        public Coords To;
        public Coords From;
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
        private readonly CheckersConfig _checkersConfig;
        private PawnColor _mainColor;

        private TurnData _turnData;
        private bool _hasInput;
        
        public MainCheckersOnlineService(MainCheckerSceneSettings sceneSettings,
                                         ISchedulerService schedulerService, 
                                         NakamaService nakamaService,
                                         CheckersConfig checkersConfig) {
            _sceneSettings = sceneSettings;
            _schedulerService = schedulerService;
            _nakamaService = nakamaService;
            _checkersConfig = checkersConfig;
        }
        
        public void Initialize() {
            _mainColor = PlayerPrefsX.GetEnum<PawnColor>("YourColor");
            
            var turnHandler = _sceneSettings.TurnHandler;

            //RotateBoardToBlack();

            _sceneSettings.TurnHandler.YourColor = _mainColor;
            
            _nakamaService.SubscribeToMatchState(OnMatchState);
            _nakamaService.SubscribeToMatchPresence(OnMatchPresence);
            
            turnHandler.OnPawnCheck += OnPawnCheck;
            turnHandler.OnEndGame += OnEndGame;

            _sceneSettings.HeroHealthSlider.maxValue = 12;
            _sceneSettings.HeroHealthSlider.value = 12;

            _sceneSettings.EnemyHealthSlider.maxValue = 12;
            _sceneSettings.EnemyHealthSlider.value = 12;
            
            _sceneSettings.EnemyAnimation.ResetAnimation("idle", true);

            _sceneSettings.PawnMover.OnTurnEnd += () => {

            };
            
            _sceneSettings.PawnMover.OnTurn += (turnData) => {
                var currentTurn = _sceneSettings.TurnHandler.Turn;

                if (currentTurn != _mainColor) return;
                
                var matchId = _nakamaService.GetCurrentMatchId();
                
                var json = JsonConvert.SerializeObject(turnData);

                _nakamaService.SendMatchStateAsync(matchId, (long) CheckersMatchState.Turn, json);
            };
        }

        private void OnMatchPresence(IMatchPresenceEvent obj) {
            if (_nakamaService.GetCurrentMatchPlayers() >= 2) return;

            var signal = new YouAreBlack();

            var json = JsonConvert.SerializeObject(signal);
            _nakamaService.SendMatchStateAsync(obj.MatchId, (long) CheckersMatchState.Started, json);
        }

        public void Tick() {
            if (!_hasInput) return;

            _hasInput = false;
            
            var toCoords = _turnData.To;
            var tileTo = _sceneSettings.Getter.GetTile(toCoords.Column, toCoords.Row);

            var fromCoords = _turnData.From;
            var tileFrom = _sceneSettings.Getter.GetTile(fromCoords.Column, fromCoords.Row);

            tileFrom.GetComponent<TileClickDetector>().ManualPawnClick();
            tileTo.GetComponent<TileClickDetector>().ManualTileClick();
            
        }

        private void OnMatchState(IMatchState state) {
            var enc = System.Text.Encoding.UTF8;
            var content = enc.GetString(state.State);

            switch (state.OpCode) {
                case (long) CheckersMatchState.Turn: {
                    _hasInput = true;
                    var turnData = JsonConvert.DeserializeObject<TurnData>(content);
                    _turnData = turnData;

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
                _sceneSettings.EnemyAnimation.ResetAnimation("attack");
                _sceneSettings.EnemyAnimation.DoAfterComplete(() => _sceneSettings.EnemyAnimation.ResetAnimation("idle", true));
                
                _sceneSettings.HeroHealthSlider.value -= 1;

                SendPawn(_sceneSettings.HeroTransform.position, copy);
            }
            else {
                _sceneSettings.EnemyHealthSlider.value -= 1;

                _sceneSettings.EnemyAnimation.ResetAnimation("hurt");
                _sceneSettings.EnemyAnimation.DoAfterComplete(() => _sceneSettings.EnemyAnimation.ResetAnimation("idle", true));

                var position = _sceneSettings.EnemyAnimation.GetComponent<MeshRenderer>().bounds.center;
                
                SendPawn(position, copy);
            }
            
            Object.Destroy(pawn);
        }

        private void SendPawn(Vector3 endPosition, GameObject copy) {
            var startPosition = copy.transform.position;
     
            var topPoint = new Vector3(endPosition.x, startPosition.y + 0.5f, endPosition.z); 
            
            Vector3[] waypoints = new[] {topPoint, startPosition, topPoint, endPosition, topPoint, endPosition};
            
            copy.transform.DOPath(waypoints, 1, PathType.CubicBezier, PathMode.Ignore).SetEase(Ease.Linear).onComplete += () => { Object.Destroy(copy); };
        }

        private async void OnEndGame(PawnColor color) {
            _schedulerService
                .StartSequence()
                .Append(0.3f, () => { StartAsync(); });
        }

        private async Task StartAsync() {
            await UnityExtensions.LoadSceneAsync("Simple");
            await UnityExtensions.LoadSceneAsync("CheckersMain");
        }
    }
}