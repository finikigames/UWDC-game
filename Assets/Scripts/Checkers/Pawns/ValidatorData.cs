using Checkers.Structs;
using Global.Enums;
using UnityEngine;

namespace Checkers.Pawns {
    public class ValidatorData {
        public GameObject TargetTile;
        public TileIndex TargetTileIndex;
        public TileIndex CurrentTileIndex;
        public TileIndex PositionDifferenceInIndex;
        public GameObject PotentialPawn;
        public PawnColor PawnColor;
    }
}