﻿using Checkers.Board;
using Checkers.Structs;
using UnityEngine;

namespace Checkers.AI
{
    public class CPUPlayer : MonoBehaviour
    {
        public MoveTreeBuilder MoveTreeBuilder;

        private TileGetter tileGetter;
        private TurnHandler turnHandler;

        private void Start()
        {
            tileGetter = GetComponent<TileGetter>();
            turnHandler = GetComponent<TurnHandler>();
        }

        public void DoPlayerMove(Move move)
        {
            MoveTreeBuilder.DoPlayerMove(move);
        }

        public void DoCPUMove()
        {
            if (MoveTreeBuilder.HasNextMove())
                ChooseAndDoMove();
        }

        private void ChooseAndDoMove()
        {
            Move move = MoveTreeBuilder.ChooseNextCPUMove();
            var fromTileClickDetector = tileGetter.GetTile(move.From).GetComponent<TileClickDetector>();
            var toTileClickDetector = tileGetter.GetTile(move.To).GetComponent<TileClickDetector>();
            fromTileClickDetector.ClickTile();
            toTileClickDetector.ClickTile();
        }
    }
}