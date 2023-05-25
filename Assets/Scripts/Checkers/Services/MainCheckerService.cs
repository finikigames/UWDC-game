using Checkers.Settings;
using Core.Extensions;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Global.Extensions;
using Global.Scheduler.Base;
using UnityEngine;
using Zenject;

namespace Checkers.Services {
    public class MainCheckerService : IInitializable {
        private readonly MainCheckerSceneSettings _sceneSettings;
        private readonly ISchedulerService _schedulerService;

        public MainCheckerService(MainCheckerSceneSettings sceneSettings,
                                  ISchedulerService schedulerService) {
            _sceneSettings = sceneSettings;
            _schedulerService = schedulerService;
        }
        
        public void Initialize() {
            var turnHandler = _sceneSettings.TurnHandler;

            turnHandler.OnPawnCheck += OnPawnCheck;
            turnHandler.OnEndGame += OnEndGame;

            _sceneSettings.HeroHealthSlider.maxValue = 12;
            _sceneSettings.HeroHealthSlider.value = 12;

            _sceneSettings.EnemyHealthSlider.maxValue = 12;
            _sceneSettings.EnemyHealthSlider.value = 12;
            
            _sceneSettings.EnemyAnimation.ResetAnimation("idle", true);
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

        private async UniTask StartAsync() {
            await UnityExtensions.LoadSceneAsync("Simple");
            await UnityExtensions.LoadSceneAsync("CheckersMain");
        }
    }
}