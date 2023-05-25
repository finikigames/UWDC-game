using System;
using Core.Extensions;
using UnityEngine;

namespace Core {
    [RequireComponent(typeof(RectTransform))]
    public class TargetScaler : MonoBehaviour {
        public ScaleType ScaleType;

        private Vector3 _defaultScale;
        private bool _initialized;
        
        private void Awake() {
            if (_initialized) return;
            
            _defaultScale = transform.localScale;
            _initialized = true;
        }

        private void Start() {
            var screenWidth = (float)Screen.width;
            var screenHeight = (float)Screen.height;

            var windowWidth = transform.AsRectTransform().sizeDelta.x;
            var windowHeight = transform.AsRectTransform().sizeDelta.y;

            var screenAspectRatio = UnityExtensions.GetAspectRatio(screenHeight, screenWidth);
            var windowAspectRatio = UnityExtensions.GetAspectRatio(windowHeight, windowWidth);
            
            if (CheckAbilityToHeightScale(screenAspectRatio, windowAspectRatio)) {
                screenHeight = ExtrapolateHeight(windowWidth, screenWidth, screenHeight, ref windowHeight);

                var targetScale = GetTargetHeightScale(screenHeight, windowHeight);

                ApplyScale(targetScale);
            }
            else if (CheckAbilityToWidthScale()) {
                screenWidth = ExtrapolateWidth(windowHeight, screenHeight, screenWidth, ref windowWidth);

                var targetScale = GetTargetWidthScale(screenWidth, windowWidth);

                ApplyScale(targetScale);
            }
        }

        private bool CheckAbilityToHeightScale(float screenAspectRatio, float windowAspectRatio) {
            return screenAspectRatio > windowAspectRatio && (ScaleType == ScaleType.Both || ScaleType == ScaleType.OnlyHeight);
        }

        private bool CheckAbilityToWidthScale() {
            return ScaleType == ScaleType.Both || ScaleType == ScaleType.OnlyWidth;
        }

        private static float GetTargetWidthScale(float screenWidth, float windowWidth) {
            var screenWidthDelta = Mathf.Abs(screenWidth - windowWidth);
            var targetScale = screenWidthDelta / windowWidth;
            return targetScale;
        }

        private static float GetTargetHeightScale(float screenHeight, float windowHeight) {
            var screenHeightDelta = Mathf.Abs(screenHeight - windowHeight);
            var targetScale = screenHeightDelta / windowHeight;
            return targetScale;
        }

        private float ExtrapolateHeight(float windowWidth, float screenWidth, float screenHeight,
            ref float windowHeight) {
            if (windowWidth > screenWidth) {
                var widthAspect = windowWidth / screenWidth;

                screenHeight *= widthAspect;
            }
            else {
                var widthAspect = screenWidth / windowWidth;

                windowHeight *= widthAspect;
            }

            return screenHeight;
        }

        private float ExtrapolateWidth(float windowHeight, float screenHeight, float screenWidth,
            ref float windowWidth) {
            if (windowHeight > screenHeight) {
                var heightAspect = windowHeight / screenHeight;

                screenWidth *= heightAspect;
            }
            else {
                var heightAspect = screenHeight / windowHeight;

                windowWidth *= heightAspect;
            }

            return screenWidth;
        }

        private void ApplyScale(float targetScale) {
            transform.localScale =
                new Vector3(_defaultScale.x + targetScale, _defaultScale.y + targetScale, _defaultScale.z + targetScale);
        }
    }
}