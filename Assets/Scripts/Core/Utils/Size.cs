using UnityEngine;

namespace Core.Utils {
    public struct Size {
        public static Size One = new Size(1, 1);
        public sbyte x;
        public sbyte y;

        public Vector2 Value {
            get => new Vector2(x, y);
            set {
                x = (sbyte)value.x;
                y = (sbyte) value.y;
            }
        }

    public Size(float horizontal, float vertical) {
            x = (sbyte)horizontal;
            y = (sbyte) vertical;
        }

        public static implicit operator Size(Vector2Int size) {
            return new Size((sbyte)size.x, (sbyte)size.y);
        } 
        
        public static implicit operator Vector2Int(Size size) {
            return new Vector2Int(size.x, size.y);
        } 
    }
}