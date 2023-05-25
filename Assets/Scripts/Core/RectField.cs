using UnityEngine;

namespace Core
{
    public struct RectField
    {
        public int X;
        public int Y;

        public int YTop;
        public int YBottom;
        public int XLeft;
        public int XRigth;

        public int Width => XRigth - XLeft + 1;
        public int Height => YBottom - YTop + 1;
        
        public RectField(Vector2Int center, Vector2Int size)
        {
            X = center.x;
            Y = center.y;
            YTop = Y - size.x;
            YBottom = Y + size.x;
            XLeft = X - size.y;
            XRigth = X + size.y;
        }
        
        public RectField(Vector2Int center, int size)
        {
            X = center.x;
            Y = center.y;
            YTop = Y - size;
            YBottom = Y + size;
            XLeft = X - size;
            XRigth = X + size;
        }
    }
}