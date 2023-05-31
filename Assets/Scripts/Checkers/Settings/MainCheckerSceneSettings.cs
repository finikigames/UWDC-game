using Checkers.Board;
using Checkers.Pawns;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.Settings {
    public class MainCheckerSceneSettings : MonoBehaviour {
        public Slider HeroHealthSlider;
        public Slider EnemyHealthSlider;
        public Transform HeroTransform;
        public Transform EnemyTransform;
        public TurnHandler TurnHandler;
        public TileGetter Getter;
        public Transform BoardRoot;
        public PawnMover PawnMover;
        
        public Vector3 _playerBar;
        public Vector3 _opponentBar;
    }
}