using Checkers.Board;
using Checkers.Interfaces;
using Checkers.Structs;
using Global.Enums;
using UnityEngine;

namespace Checkers.AI.Pawns
{
    public class AIPawnProperties : MonoBehaviour, IPawnProperties
    {
        public PawnColor PawnColor { get; set; }
        public bool IsKing { get; set; }

        public TileIndex GetTileIndex()
        {
            return GetComponentInParent<TileProperties>().GetTileIndex();
        }

        public void PromoteToKing()
        {
            IsKing = true;
        }

        public void AddPawnSelection()
        {
        }

        public void RemovePawnSelection()
        {
        }
    }
}