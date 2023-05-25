using System;
using DG.Tweening;
using Spine;
using Spine.Unity;

namespace Global.Extensions {
    public static class SpineExtensions {
        public static float GetAnimationTime(this SkeletonAnimation animation) {
            return animation.skeletonDataAsset.GetSkeletonData(true).Animations.Items[0].Duration;
        }

        public static float GetAnimationTime(this SkeletonAnimation skeleton, string animationName) {
            var animation = skeleton.GetAnimation(animationName);
            
            if (animation == null) return 0f;
            return animation.Duration;
        }
        
        public static float GetAnimationTime(this SkeletonGraphic skeleton, string animationName) {
            var animation = skeleton.GetAnimation(animationName);
            
            if (animation == null) return 0f;
            return animation.Duration;
        }
        
        public static float GetAnimationTime(this SkeletonGraphic skeleton) {
            return skeleton.skeletonDataAsset.GetSkeletonData(true).Animations.Items[0].Duration;
        }
        
        public static void ChangeAnimation(this SkeletonAnimation skeleton, string animationName, bool loop = false) {
            skeleton.AnimationState.SetAnimation(0, animationName, loop); 
        }

        public static Animation GetAnimation(this SkeletonAnimation skeleton, string animationName) {
            return skeleton.skeletonDataAsset.GetSkeletonData(true).Animations.Find(x =>
                x.Name == animationName
            );
        }
        
        public static Animation GetAnimation(this SkeletonGraphic skeleton, string animationName) {
            return skeleton.skeletonDataAsset.GetSkeletonData(true).Animations.Find(x =>
                x.Name == animationName
            );
        }
        public static void ResetAnimation(this SkeletonGraphic animation, string animationName) {
            animation.gameObject.SetActive(true);
            animation.AnimationState.ClearTracks();
            animation.AnimationState.SetAnimation(0, animationName, false);
        }
        
        public static void ResetAnimation(this SkeletonAnimation animation, string animationName, bool loop = false) {
            animation.gameObject.SetActive(true);
            animation.AnimationState.ClearTracks();
            animation.AnimationState.SetAnimation(0, animationName, loop);
        }
        
        public static void ResetAnimationAndDeactivate(this SkeletonAnimation animation, string animationName) {
            animation.AnimationState.ClearTracks();
            animation.AnimationState.SetAnimation(0, animationName, false);
            animation.DeactivateSpineAfterAnimation();
        }


        public static void DeactivateSpineAfterAnimation(this SkeletonAnimation animation, Action action = null) {
            var animationTime = animation.GetAnimationTime();
            DOTween.Sequence().AppendInterval(animationTime).AppendCallback(() => {
                action?.Invoke();
                animation.gameObject.SetActive(false);
            });
        }

        public static void DeactivateSpineAfterAnimation(this SkeletonGraphic animation, Action action = null) {
            var animationTime = animation.GetAnimationTime();
            DOTween.Sequence().AppendInterval(animationTime).AppendCallback(() => {
                action?.Invoke();
                animation.gameObject.SetActive(false);
            });
        }

        public static void DoAfterComplete(this SkeletonAnimation animation, Action action) {
            var animationTime = animation.GetAnimationTime();
            DOTween.Sequence().AppendInterval(animationTime).AppendCallback(() => {
                action?.Invoke();
            });
        }
        
        public static void ChangeSpine(this SkeletonAnimation animation, SkeletonDataAsset skeletonData) {
            animation.ClearState();
            animation.skeletonDataAsset = skeletonData;
            animation.Initialize(true, false);
        }
        
        public static void ChangeSpine(this SkeletonGraphic animation, SkeletonDataAsset skeletonData) {
            animation.Clear();
            animation.skeletonDataAsset = skeletonData;
            animation.Initialize(true);
        }
        
        public static void ChangeAnimation(this SkeletonGraphic skeleton, string animationName, bool loop = false) {
            skeleton.AnimationState.SetAnimation(0, animationName, loop);
        }
    }
}