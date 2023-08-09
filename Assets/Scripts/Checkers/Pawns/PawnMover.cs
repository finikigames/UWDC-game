using System;
using System.Collections;
using System.Collections.Generic;
using Checkers.AI;
using Checkers.Board;
using Checkers.Interfaces;
using Checkers.Services;
using Checkers.Structs;
using Core.Primitives;
using Global.Enums;
using UnityEngine;

namespace Checkers.Pawns
{
    public class PawnMover : MonoBehaviour {
        public Action OnTurnEnd;
        public Action<TurnData> OnTurn;
    
        public float HorizontalMovementSmoothing;
        public float VerticalMovementSmoothing;
        public float PositionDifferenceTolerance;

        private GameObject lastClickedTile;
        private GameObject lastClickedPawn;
        private PawnMoveValidator pawnMoveValidator;
        private MoveChecker moveChecker;
        private PromotionChecker promotionChecker;
        private TurnHandler turnHandler;
        private CPUPlayer cpuPlayer;
        private PawnsGenerator _pawnsGenerator;

        private bool isPawnMoving;
        private bool isMoveMulticapturing;
        private List<IPawnProperties> _pawnsWithSelections = new();

        public bool TurnState => isPawnMoving;
        
        private void Awake()
        {
            pawnMoveValidator = GetComponent<PawnMoveValidator>();
            moveChecker = GetComponent<MoveChecker>();
            promotionChecker = GetComponent<PromotionChecker>();
            turnHandler = GetComponent<TurnHandler>();
            cpuPlayer = GetComponent<CPUPlayer>();
            _pawnsGenerator = GetComponent<PawnsGenerator>();
        }

        public void PawnClicked(GameObject pawn)
        {
            if (!CanPawnBeSelected(pawn))
                return;
            if (pawn != lastClickedPawn)
                SelectPawn(pawn);
            else
                UnselectPawn();
        }

        public bool CanPawnBeSelected(GameObject pawn)
        {
            PawnColor turn = turnHandler.GetTurn();
            if (isPawnMoving || turn != GetPawnColor(pawn) || isMoveMulticapturing ||
                !moveChecker.PawnHasAnyMove(pawn)) {
                pawn.GetComponent<IPawnProperties>().AddPawnCantSelection();
                return false;
            }

            if (moveChecker.PawnsHaveCapturingMove(turn) && !moveChecker.PawnHasCapturingMove(pawn)) {
                pawn.GetComponent<IPawnProperties>().AddPawnCantSelection();
                return false;
            }
            return true;
        }

        private void SelectPawn(GameObject pawn)
        {
            if (lastClickedPawn != null)
                UnselectPawn();
            lastClickedPawn = pawn;
            AddPawnSelection();
        }

        private void AddPawnSelection()
        {
            lastClickedPawn.GetComponent<IPawnProperties>().AddPawnSelection();
        }

        private void UnselectPawn()
        {
            RemoveLastClickedPawnSelection();
            lastClickedPawn = null;
        }

        private void RemoveLastClickedPawnSelection()
        {
            lastClickedPawn.GetComponent<IPawnProperties>().RemovePawnSelection();
        }

        private PawnColor GetPawnColor(GameObject pawn)
        {
            return pawn.GetComponent<IPawnProperties>().PawnColor;
        }

        public void TileClicked(GameObject tile)
        {
            //TODO: Add available moves highlight.
            if (!CanTileBeClicked()) return;
            lastClickedTile = tile;
            if (IsMoveNoncapturingAndValid()) {
                MovePawn();
            }
            else if (IsMoveCapturingAndValid()) {
                CapturePawn();
            }
        }

        private bool CanTileBeClicked()
        {
            return !isPawnMoving && lastClickedPawn != null;
        }

        private bool IsMoveNoncapturingAndValid()
        {
            if (moveChecker.PawnHasCapturingMove(lastClickedPawn))
                return false;
            return pawnMoveValidator.IsValidMove(lastClickedPawn, lastClickedTile);
        }

        private void MovePawn()
        {
            SendTurnEvent(false);
            SendMoveToCPU();
            ChangeMovedPawnParent();
            StartCoroutine(AnimatePawnMove());
            RemoveLastClickedPawnSelection();
        }

        private void SendTurnEvent(bool isCapture) {
            var toIndex = lastClickedTile.GetComponent<TileProperties>().GetTileIndex();
            var fromIndex = lastClickedPawn.GetComponent<IPawnProperties>().GetTileIndex();

            var toCoords = new Coords {
                Column = (sbyte) toIndex.Column,
                Row = (sbyte) toIndex.Row
            };

            var fromCoords = new Coords {
                Column = (sbyte) fromIndex.Column,
                Row = (sbyte) fromIndex.Row
            };

            var turnData = new TurnData {
                To = toCoords,
                From = fromCoords,
                Capture = isCapture
            };

            OnTurn?.Invoke(turnData);
        }

        private void SendMoveToCPU()
        {
            if (!ShouldMoveBeSentToCPU()) return;
            TileIndex fromIndex = lastClickedPawn.GetComponent<IPawnProperties>().GetTileIndex();
            TileIndex toIndex = lastClickedTile.GetComponent<TileProperties>().GetTileIndex();
            cpuPlayer.DoPlayerMove(new Move(fromIndex, toIndex));
        }

        private bool ShouldMoveBeSentToCPU()
        {
            return cpuPlayer != null && cpuPlayer.enabled && GetPawnColor(lastClickedPawn) == PawnColor.White;
        }

        private void ChangeMovedPawnParent()
        {
            lastClickedPawn.transform.SetParent(lastClickedTile.transform);
        }

        private IEnumerator AnimatePawnMove()
        {
            isPawnMoving = true;
            var targetPosition = lastClickedPawn.transform.parent.position;
            yield return MoveHorizontal(targetPosition);
            promotionChecker.CheckPromotion(lastClickedPawn);
            isPawnMoving = false;
            EndTurn();
        }

        private void EndTurn()
        {
            OnTurnEnd?.Invoke();

            lastClickedPawn = null;
            isMoveMulticapturing = false;

            foreach (IPawnProperties pawn in _pawnsWithSelections) {
                pawn.ClearSelection();
            }
            
            turnHandler.NextTurn();

            ShowPawnPossibility();
        }

        private IEnumerator MoveHorizontal(Vector3 targetPosition)
        {
            var pawnTransform = lastClickedPawn.transform;
            while (Vector3.Distance(pawnTransform.position, targetPosition) > PositionDifferenceTolerance)
            {
                pawnTransform.position = Vector3.Lerp(pawnTransform.position, targetPosition,
                    HorizontalMovementSmoothing * Time.deltaTime);
                yield return null;
            }
        }

        private bool IsMoveCapturingAndValid()
        {
            return pawnMoveValidator.IsCapturingMove(lastClickedPawn, lastClickedTile, true);
        }

        private void CapturePawn()
        {
            SendTurnEvent(true);
            SendMoveToCPU();
            ChangeMovedPawnParent();
            StartCoroutine(AnimatePawnCapture());
            RemoveLastClickedPawnSelection();
        }

        private IEnumerator AnimatePawnCapture()
        {
            isPawnMoving = true;
            yield return DoCaptureMovement();
            RemoveCapturedPawn();
            yield return null; //Waiting additional frame for captured pawn destruction.
            promotionChecker.CheckPromotion(lastClickedPawn);
            isPawnMoving = false;
            MulticaptureOrEndTurn();
        }

        private IEnumerator DoCaptureMovement()
        {
            var targetPosition = lastClickedPawn.transform.position + Vector3.up;
            yield return MoveVertical(targetPosition);
            targetPosition = lastClickedPawn.transform.parent.position + Vector3.up;
            yield return MoveHorizontal(targetPosition);
            targetPosition = lastClickedPawn.transform.position - Vector3.up;
            yield return MoveVertical(targetPosition);
        }

        private IEnumerator MoveVertical(Vector3 targetPosition)
        {
            var pawnTransform = lastClickedPawn.transform;
            while (Vector3.Distance(pawnTransform.position, targetPosition) > PositionDifferenceTolerance)
            {
                pawnTransform.position = Vector3.Lerp(pawnTransform.position, targetPosition,
                    VerticalMovementSmoothing * Time.deltaTime);
                yield return null;
            }
        }

        private void RemoveCapturedPawn()
        {
            GameObject pawnToCapture = pawnMoveValidator.GetPawnToCapture();
            turnHandler.DecrementPawnCount(pawnToCapture);
        }

        private void MulticaptureOrEndTurn()
        {
            if (moveChecker.PawnHasCapturingMove(lastClickedPawn))
                Multicapture();
            else
                EndTurn();
        }

        private void Multicapture()
        {
            isMoveMulticapturing = true;
            AddPawnSelection();
            if (IsMoveByCPUAndMulticapturing())
                cpuPlayer.DoCPUMove();
        }

        private bool IsMoveByCPUAndMulticapturing()
        {
            return cpuPlayer != null && cpuPlayer.enabled && GetPawnColor(lastClickedPawn) == PawnColor.Black;
        }

        private void ShowPawnPossibility() {
            var turn = turnHandler.GetTurn();
            
            if (turn != turnHandler.YourColor) return;
            if (!moveChecker.PawnsHaveCapturingMove(turn)) return;

            _pawnsWithSelections.Clear();
            var pawns = _pawnsGenerator.Pawns[turn];
            foreach (var pawn in pawns) {
                if (!moveChecker.PawnHasCapturingMove(pawn)) continue;

                var properties = pawn.GetComponent<IPawnProperties>();
                properties.AddPawnCanSelection();
                _pawnsWithSelections.Add(properties);
            }
        }
    }
}