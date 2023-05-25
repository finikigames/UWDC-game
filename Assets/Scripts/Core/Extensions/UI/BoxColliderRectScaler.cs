using UnityEngine;

namespace Core.Extensions.UI {
    public class BoxColliderRectScaler : MonoBehaviour {
        public BoxCollider2D BoxCollider2D;
        public RectTransform ConstraintTransform;

        private void Start() {
            AssignSize();
        }

        private void Update() {
            AssignSize();
        }

        private void AssignSize() {
            BoxCollider2D.size = new Vector2(BoxCollider2D.size.x, ConstraintTransform.rect.height);
            BoxCollider2D.offset = new Vector2(0, ConstraintTransform.rect.height / 2);
        }
    }
}