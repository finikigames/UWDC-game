using UnityEngine;

namespace Core.Extensions {
    public static class NumericsExtensions {
        public static Vector3 ToVector3(this float number) {
            return new Vector3(number, number, number);
        }

        public static Vector3 RandomPositionInRadius(this Renderer renderer, float percent = 1) {
            var radius = renderer.bounds.extents.x * percent;
            var endPointY = Random.Range(-radius, radius);
            var endPointX = Random.Range(-radius, radius);
            
            var position = renderer.bounds.center;
            return new Vector3(position.x + endPointX,
                position.y + endPointY,
                position.z);
        }
        
        public static Vector3 RandomPositionInRadiusAtPoint(this Renderer renderer, float xCoord, float radius = 1f) {
            var endPointY = Random.Range(-radius, radius);
            var endPointX = Random.Range(-radius, radius);
            
            var position = renderer.bounds.center;
            position.x = xCoord;
            return new Vector3(position.x + endPointX,
                position.y + endPointY,
                position.z);
        }
    }
}