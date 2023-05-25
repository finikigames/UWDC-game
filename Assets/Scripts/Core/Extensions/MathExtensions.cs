namespace Core.Extensions {
    public static class MathExtensions {
        public static int ClampMax(this int value, int max) {
            return value > max ? max : value;
        }
        
        public static float ClampMax(this float value, float max) {
            return value > max ? max : value;
        }
        
        public static long ClampMax(this long value, long max) {
            return value > max ? max : value;
        }
    }
}