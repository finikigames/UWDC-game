using Checkers.Structs;
using Global.Enums;
using UnityEngine;

namespace Checkers.Interfaces
{
    public interface IPawnProperties
    {
        GameObject gameObject { get; }
        PawnColor PawnColor { get; set; }
        bool IsKing { get; set; }

        TileIndex GetTileIndex();

        void PromoteToKing();

        void AddPawnSelection();

        void RemovePawnSelection();
    }
}