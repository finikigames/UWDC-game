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

        public int whitePawnCount;
        public int blackPawnCount;

        public PawnColor YourColor;
        [FormerlySerializedAs("turn")]
        public PawnColor Turn;

        private CPUPlayer cpuPlayer;

        private void Awake()
        {
            Turn = StartingPawnColor;
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
            CheckVictory();
        }

        private void CheckVictory()
        {
            if (whitePawnCount == 0)
                EndGame(PawnColor.Black);
            else if (blackPawnCount == 0)
                EndGame(PawnColor.White);
        }

        private void EndGame(PawnColor winnerPawnColor) {
            OnEndGame?.Invoke(winnerPawnColor, WinLoseReason.Rule);
        }
    }
}