using Checkers.Structs;
using UnityEngine;

namespace Checkers.Board
{
    public class TileGetter : MonoBehaviour
    {
        public GameObject GetTile(int columnIndex, int rowIndex)
        {
            return transform.GetChild(columnIndex).GetChild(rowIndex).gameObject;
        }

        public GameObject GetTile(TileIndex tileIndex)
        {
            return transform.GetChild(tileIndex.Column).GetChild(tileIndex.Row).gameObject;
        }
    }
}