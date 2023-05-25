using System;
using Core.Extensions;
using UnityEngine;

namespace Core.Utils {
    public class ScreenSpaceUIElement : UnityComponent {
        public RectTransform WorldRT;
        
        private void Awake() {
            var canvasRT = GetComponentInParent<Canvas>().transform.AsRectTransform();
            float ww = WorldRT.rect.width;

            var cameraMain = Camera.main;
            var position = WorldRT.position;
            Vector2 wScreenPos = cameraMain.WorldToScreenPoint(position);
            Debug.Log("*** Word Screen Position: " + cameraMain.WorldToScreenPoint(position));
 
            wScreenPos.x = wScreenPos.x + ((ww/2f) - wScreenPos.x);
            Debug.Log("*** word new Screen pos: " + wScreenPos);
            Vector3 outV;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(canvasRT, wScreenPos, Camera.current, out outV);
            Vector2 wWorldPos = outV;
            position = wWorldPos;
            WorldRT.position = position;
        }
    }
}