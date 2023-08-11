using Checkers.Board;
using Checkers.Interfaces;
using Checkers.Structs;
using Global.Enums;
using UnityEngine;

namespace Checkers.Pawns
{
    public class PawnMoveValidator : MonoBehaviour
    {
        private TileGetter tileGetter;
        private GameObject pawn;
        private GameObject targetTile;
        private GameObject pawnToCapture;
        private TileIndex targetTileIndex;
        private TileIndex currentTileIndex;
        private TileIndex positionDifferenceInIndex;
        private GameObject potentialPawn;
        private ValidatorData _ghostData;
        private bool fromCapture;
        private bool forCapture;
        private bool _isGhost;

        private MoveChecker _moveChecker;

        private void Awake()
        {
            tileGetter = GetComponent<TileGetter>();
            _ghostData = new ValidatorData();
            _moveChecker = GetComponent<MoveChecker>();
        }

        public bool IsValidMove(GameObject pawnToCheck, GameObject targetTileToCheck)
        {
            SetValues(pawnToCheck, targetTileToCheck);
            if (!IsMoveDiagonal() || IsTileOccupied(targetTileIndex))
                return false;
            if (!IsPawnKing())
                return positionDifferenceInIndex.Row == GetPawnRowMoveDirection();
            else
                return IsPathCollidingWithOtherPawns();
        }

        private void SetValues(GameObject pawnToCheck, GameObject targetTileToCheck)
        {
            pawn = pawnToCheck;
            targetTile = targetTileToCheck;
            targetTileIndex = targetTile.GetComponent<TileProperties>().GetTileIndex();
            currentTileIndex = pawn.GetComponent<IPawnProperties>().GetTileIndex();
            positionDifferenceInIndex = targetTileIndex - currentTileIndex;
            potentialPawn = null;
            fromCapture = false;
        }
        
        private void SetGhostValues(TileIndex index, GameObject targetTileToCheck, PawnColor color)
        {
            _ghostData.TargetTile = targetTileToCheck;
            _ghostData.TargetTileIndex = _ghostData.TargetTile.GetComponent<TileProperties>().GetTileIndex();
            _ghostData.CurrentTileIndex = index;
            _ghostData.PositionDifferenceInIndex = _ghostData.TargetTileIndex - _ghostData.CurrentTileIndex;
            _ghostData.PotentialPawn = null;
            _ghostData.PawnColor = color;
        }

        private bool IsMoveDiagonal() {
            if (!_isGhost) return Mathf.Abs(positionDifferenceInIndex.Column) == Mathf.Abs(positionDifferenceInIndex.Row);
            return Mathf.Abs(_ghostData.PositionDifferenceInIndex.Column) == Mathf.Abs(_ghostData.PositionDifferenceInIndex.Row);
        }

        private bool IsPawnKing()
        {
            return pawn.GetComponent<IPawnProperties>().IsKing;
        }

        private int GetPawnRowMoveDirection()
        {
            var pawnProperties = pawn.GetComponent<IPawnProperties>();
            return pawnProperties.PawnColor == PawnColor.White ? 1 : -1;
        }

        private bool IsPathCollidingWithOtherPawns()
        {
            var moveDirectionInIndex = GetDiagonalMoveDirectionInIndex();
            for (var checkedTileIndex = currentTileIndex + moveDirectionInIndex;
                 checkedTileIndex != targetTileIndex;
                 checkedTileIndex += moveDirectionInIndex)
                if (IsTileOccupied(checkedTileIndex))
                    return false;

            return true;
        }

        private TileIndex GetDiagonalMoveDirectionInIndex()
        {
            //Move direction means TileIndex with both values equal to +-1.
            if (!_isGhost) return new TileIndex(positionDifferenceInIndex.Column / Mathf.Abs(positionDifferenceInIndex.Column),
                positionDifferenceInIndex.Row / Mathf.Abs(positionDifferenceInIndex.Row));
            return new TileIndex(_ghostData.PositionDifferenceInIndex.Column / Mathf.Abs(_ghostData.PositionDifferenceInIndex.Column),
                _ghostData.PositionDifferenceInIndex.Row / Mathf.Abs(_ghostData.PositionDifferenceInIndex.Row));
        }

        private bool IsTileOccupied(TileIndex tileIndex)
        {
            return tileGetter.GetTile(tileIndex).GetComponent<TileProperties>().IsOccupied();
        }

        public bool IsCapturingMove(GameObject pawnToCheck, GameObject targetTileToCheck, bool forCapture = false) {
            this.forCapture = forCapture;
            _isGhost = false;
            SetValues(pawnToCheck, targetTileToCheck);
            if (!IsMoveDiagonal() || IsTileOccupied(targetTileIndex))
                return false;
            return IsCapturePositionChangeValid() && IsOpponentsPawnOnOneBeforeTargetTile();
        }
        
        public bool IsGhostCapturingMove(TileIndex index, PawnColor color, GameObject targetTileToCheck) {
            _isGhost = true;
            SetGhostValues(index, targetTileToCheck, color);
            if (!IsMoveDiagonal() || IsTileOccupied(targetTileIndex)) {
                _isGhost = false;
                return false;
            }

            var result = IsCapturePositionChangeValid() && IsOpponentsPawnOnOneBeforeTargetTile();
            _isGhost = false;
            
            return result;
        }

        private bool IsCapturePositionChangeValid() {
            if (!_isGhost) return (!IsPawnKing() && Mathf.Abs(positionDifferenceInIndex.Row) == 2) ||
                                 (IsPawnKing() && Mathf.Abs(positionDifferenceInIndex.Row) >= 2);
            return Mathf.Abs(_ghostData.PositionDifferenceInIndex.Row) >= 2;
        }

        private bool IsOpponentsPawnOnOneBeforeTargetTile()
        {
            if (!IsPawnOnOneBeforeTargetTile())
                return false;
            
            var potentialPawnToCapture = GetPotentialPawnToCapture();
            if (!IsPawnDifferentColorThanLastClickedPawn(potentialPawnToCapture))
                return false;
            
            if (_isGhost) return true;
            
            pawnToCapture = potentialPawnToCapture;
            return true;
        }

        private bool IsPawnOnOneBeforeTargetTile()
        {
            var moveDirectionInIndex = GetDiagonalMoveDirectionInIndex();
            int occupiedCount = 0;
            int ghostOccupiedCount = 0;
            var pawnColor = pawn.GetComponent<IPawnProperties>().PawnColor;
            var currentIndex = _isGhost ? _ghostData.CurrentTileIndex : currentTileIndex;
            var targetIndex = _isGhost ? _ghostData.TargetTileIndex : targetTileIndex;
            
            for (var checkedTileIndex = currentIndex + moveDirectionInIndex;
                checkedTileIndex != targetIndex;
                checkedTileIndex += moveDirectionInIndex) {

                if (_isGhost) {
                    if (!IsTileOccupied(checkedTileIndex)) continue;
                    var checkedPawn = tileGetter.GetTile(checkedTileIndex).GetComponent<TileProperties>().GetPawn();
                    if (potentialPawn == checkedPawn) continue;
                    
                    ghostOccupiedCount++;
                    if (ghostOccupiedCount > 1) return false;
                    
                    _ghostData.PotentialPawn = checkedPawn;
                } 
                else if (IsPawnKing() && forCapture) {
                    if (potentialPawn != null && !IsTileOccupied(checkedTileIndex)) {
                        if (_moveChecker.PawnHasCapturingMove(checkedTileIndex, pawnColor)) {
                            return false;
                        }
                    }
                    
                    if (!IsTileOccupied(checkedTileIndex)) continue;
                    occupiedCount++;
                    if (occupiedCount > 1) return false;
                    
                    potentialPawn = tileGetter.GetTile(checkedTileIndex).GetComponent<TileProperties>().GetPawn();
                    fromCapture = true;
                }
                else if (IsTileOccupied(checkedTileIndex) && checkedTileIndex != targetTileIndex - moveDirectionInIndex) {
                    return false;
                }
            }

            if (_isGhost) {
                return ghostOccupiedCount != 0;
            }

            return (IsPawnKing() && fromCapture) || IsTileOccupied(targetTileIndex - moveDirectionInIndex);
        }

        private GameObject GetPotentialPawnToCapture()
        {
            if (_isGhost) {
                return _ghostData.PotentialPawn;
            }
            
            if (IsPawnKing() && fromCapture) {
                return potentialPawn;
            }
            
            var moveDirectionInIndex = GetDiagonalMoveDirectionInIndex();
            return tileGetter.GetTile(targetTileIndex - moveDirectionInIndex).GetComponent<TileProperties>().GetPawn();
        }

        private bool IsPawnDifferentColorThanLastClickedPawn(GameObject pawnToCheck) {
            if (!_isGhost) return pawnToCheck.GetComponent<IPawnProperties>().PawnColor !=
                   pawn.GetComponent<IPawnProperties>().PawnColor;
            return pawnToCheck.GetComponent<IPawnProperties>().PawnColor != _ghostData.PawnColor;
        }

        public GameObject GetPawnToCapture()
        {
            return pawnToCapture;
        }
    }
}