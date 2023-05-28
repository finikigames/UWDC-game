using Checkers.Board;
using UnityEngine;

namespace Checkers.Pawns
{
    public class PawnClickDetector : MonoBehaviour
    {
        private void OnMouseDown()
        {
            GetComponentInParent<TileClickDetector>().ChildPawnClicked();
        }
    }
}
