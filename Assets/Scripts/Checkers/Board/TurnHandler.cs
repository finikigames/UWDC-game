using System;
using Checkers.AI;
using Checkers.Interfaces;
using Checkers.Pawns;
using Global.Enums;
using Global.Window.Enums;
using UnityEngine;
using UnityEngine.Serialization;

namespace Checkers.Board
{
    public class TurnHandler : MonoBehaviour
    {
        public PawnColor StartingPawnColor;

        public Action<PawnColor, GameObject> OnPawnCheck;
        public Action<PawnColor, WinLoseReason> OnEndGame;
        public Action<PawnColor> OnTurnChange;

        public int whitePawnCount;
        public int blackPawnCount;

        public PawnColor YourColor;
        [FormerlySerializedAs("turn")]
        public PawnColor Turn;

        private CPUPlayer cpuPlayer;
        private PawnsGenerator _pawnsGenerator;
        private PawnMover _pawnMover;

        private void Awake()
        {
            Turn = StartingPawnColor;
            _pawnsGenerator = GetComponent<PawnsGenerator>();
            _pawnMover = GetComponent<PawnMover>();
        }

        private void Start()
        {
            int boardSize = GetComponent<ITilesGenerator>().BoardSize;
            int pawnRows = GetComponent<PawnsGenerator>().PawnRows;
            whitePawnCount = blackPawnCount = Mathf.CeilToInt(boardSize * pawnRows / 2f);
            cpuPlayer = GetComponent<CPUPlayer>();
        }

        public void NextTurn()
        {
            Turn = Turn == PawnColor.White ? PawnColor.Black : PawnColor.White;
            
            OnTurnChange?.Invoke(Turn);
            CheckToilet(Turn);
        }

        public PawnColor GetTurn()
        {
            return Turn;
        }

        public void DecrementPawnCount(GameObject pawn)
        {
            var pawnColor = pawn.GetComponent<IPawnProperties>().PawnColor;

            OnPawnCheck?.Invoke(pawnColor, pawn);
        
            if (pawnColor == PawnColor.White)
                --whitePawnCount;
            else
                --blackPawnCount;
            
            _pawnsGenerator.Pawns[pawnColor].Remove(pawn);
            CheckVictory();
        }

        private void CheckVictory() {
            if (whitePawnCount == 0) {
                EndGame(PawnColor.Black);
                return;
            }

            if (blackPawnCount != 0) return;
            
            EndGame(PawnColor.White);
        }
        
        private void CheckToilet(PawnColor pawnColor) {
            var pawns = _pawnsGenerator.Pawns[pawnColor];
            
            if (pawns.Count != 1) return;
            if (_pawnMover.CanPawnBeSelected(pawns[0])) return;
            
            EndGame(pawnColor == PawnColor.Black ? PawnColor.White : PawnColor.Black);
        }

        public void EndGame(PawnColor winnerPawnColor, WinLoseReason reason = WinLoseReason.Rule) {
            OnEndGame?.Invoke(winnerPawnColor, reason);
        }
    }
}