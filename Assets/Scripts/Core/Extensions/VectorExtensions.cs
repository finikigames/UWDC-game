using UnityEngine;

namespace Core.Extensions {
    public static class VectorExtensions {
        public static readonly string Left = "left";
        public static readonly string Right = "right";
        public static readonly string Down = "down";
        public static readonly string Up = "up";
        
        public static float Distance(this Vector3[] array) {
            if (array.Length == 1) return array[0].magnitude;
            float distance = 0f;
            for (int i = 0; i < array.Length - 1; i++) {
                distance += Vector3.Distance(array[i], array[i + 1]);
            }

            return distance;
        }
        
        public static string GetDirectionName(Vector3 start, Vector3 end) {
            var difference = start - end;
            difference.Normalize();

            var x = difference.x;
            var y = difference.y;
            
            var xPos = x > 0;
            var yPos = y > 0;

            var xAbs = Mathf.Abs(x);
            var yAbs = Mathf.Abs(y);

            if (xPos && xAbs > yAbs) return Left;
            if (!xPos&& xAbs > yAbs) return Right;
            if (yPos && yAbs > xAbs) return Down;
            if (!yPos && yAbs > xAbs) return Up;

            throw new System.ArgumentException($"There is no such animation for input vectors {start} amd {end}");
        }
    }
}