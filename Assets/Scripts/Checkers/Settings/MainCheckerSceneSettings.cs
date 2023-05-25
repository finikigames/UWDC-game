using Spine.Unity;
using UnityEngine;
using UnityEngine.UI;

namespace Checkers.Settings {
    public class MainCheckerSceneSettings : MonoBehaviour {
        public SkeletonAnimation EnemyAnimation;
        public Slider HeroHealthSlider;
        public Slider EnemyHealthSlider;
        public Transform HeroTransform;
        public Transform EnemyTransform;
        public TurnHandler TurnHandler;
        public TileGetter Getter;
        public Transform BoardRoot;
        public PawnMover PawnMover;
    }
}