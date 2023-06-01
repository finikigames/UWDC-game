using Checkers.Audio;
using Checkers.Board;
using Checkers.Interfaces;
using Checkers.Structs;
using Global.Enums;
using UnityEngine;

namespace Checkers.Pawns
{
    public class PawnProperties : MonoBehaviour, IPawnProperties
    {
        public GameObject PromotionParticles;
        public GameObject PawnSelection;
        public Sprite CrownGreen;
        public Sprite CrownRed;

        public PawnColor PawnColor { get; set; }
        public bool IsKing { get; set; }

        private GameObject activePawnSelection;

        public TileIndex GetTileIndex() {
            return GetComponentInParent<TileProperties>().GetTileIndex();
        }

        public void PromoteToKing() {
            IsKing = true;
            CreatePromotionParticles();
            
            GetComponentInChildren<SpriteRenderer>().sprite = 
                PawnColor == PawnColor.White ? CrownGreen : CrownRed;
            
            PlayPromotionSound();
        }

        private void CreatePromotionParticles() {
        }

        private void PlayPromotionSound() {
            var gameAudio = GameObject.FindGameObjectWithTag("Audio").GetComponent<GameAudio>();
            gameAudio.PlayPromotionSound();
        }

        public void AddPawnSelection() {
            if (activePawnSelection != null) return;
            activePawnSelection = Instantiate(PawnSelection, transform);
        }

        public void RemovePawnSelection() {
            if (activePawnSelection != null)
                Destroy(activePawnSelection);
        }
    }
}