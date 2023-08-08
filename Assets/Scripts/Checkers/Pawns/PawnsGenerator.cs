using System.Collections.Generic;
using Checkers.Board;
using Checkers.Interfaces;
using Global.Enums;
using UnityEngine;

namespace Checkers.Pawns
{
    public class PawnsGenerator : MonoBehaviour
    {
        public int PawnRows { get; private set; } = 3;
        public GameObject Pawn;
        public Sprite WhiteSprite;
        public Sprite BlackSprite;
        
        public Dictionary<PawnColor, List<GameObject>> Pawns = new Dictionary<PawnColor, List<GameObject>> {
            {PawnColor.White, new List<GameObject>()},
            {PawnColor.Black, new List<GameObject>()}
        };

        private TileGetter tileGetter;

        private int boardSize;

        private void Awake()
        {
            tileGetter = GetComponent<TileGetter>();
            boardSize = GetComponent<ITilesGenerator>().BoardSize;
            if (PlayerPrefs.HasKey("PawnRows"))
                PawnRows = PlayerPrefs.GetInt("PawnRows");
        }

        private void Start()
        {
            GenerateWhitePawns();
            GenerateBlackPawns();
        }

        private void GenerateWhitePawns()
        {
            for (var rowIndex = 0; rowIndex < boardSize && rowIndex < PawnRows; ++rowIndex)
            {
                for (var columnIndex = 0; columnIndex < boardSize; ++columnIndex)
                    if ((columnIndex + rowIndex) % 2 == 0)
                        GeneratePawn(columnIndex, rowIndex, PawnColor.White);
            }
        }

        private void GeneratePawn(int columnIndex, int rowIndex, PawnColor pawnColor)
        {
            Transform tileTransform = tileGetter.GetTile(columnIndex, rowIndex).transform;
            GameObject instantiatedPawn = Instantiate(Pawn, tileTransform.position, Pawn.transform.rotation, tileTransform);
            instantiatedPawn.GetComponentInChildren<SpriteRenderer>().sprite =
                pawnColor == PawnColor.White ? WhiteSprite : BlackSprite;
            instantiatedPawn.GetComponent<IPawnProperties>().PawnColor = pawnColor;
            
            Pawns[pawnColor].Add(instantiatedPawn);
        }

        private void GenerateBlackPawns()
        {
            for (var rowIndex = boardSize - 1; rowIndex >= 0 && rowIndex >= boardSize - PawnRows; --rowIndex)
            {
                for (var columnIndex = boardSize - 1; columnIndex >= 0; --columnIndex)
                    if ((rowIndex + columnIndex) % 2 == 0)
                        GeneratePawn(columnIndex, rowIndex, PawnColor.Black);
            }
        }
    }
}