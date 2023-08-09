using System;
using Checkers.Audio;
using Checkers.Board;
using Checkers.Interfaces;
using Checkers.Structs;
using DG.Tweening;
using Global.Enums;
using UnityEngine;

namespace Checkers.Pawns
{
    public class PawnProperties : MonoBehaviour, IPawnProperties
    {
        public GameObject PromotionParticles;
        public GameObject PawnSelection;
        public MeshRenderer PawnCanMove;
        public MeshRenderer PawnCantMove;
        public Sprite CrownGreen;
        public Sprite CrownRed;

        private Sequence _cantSequence;
        private Sequence _canSequence;

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
            
            KillSequences();
        }

        public void RemovePawnSelection() {
            if (activePawnSelection != null)
                Destroy(activePawnSelection);
        }

        public void AddPawnCantSelection() {
            KillSequences();
            PawnCantMove.material.DOFade(0, 0);
            _cantSequence = DOTween.Sequence();

            _cantSequence
                .AppendCallback(() => PawnCantMove.gameObject.SetActive(true))
                .Append(PawnCantMove.material.DOFade(1, 1f))
                .Append(PawnCantMove.material.DOFade(0, 1f))
                .onComplete += () => PawnCantMove.gameObject.SetActive(false);
        }
        
        public void AddPawnCanSelection() {
            KillSequences();
            PawnCanMove.material.DOFade(0, 0);
            _canSequence = DOTween.Sequence();

            _canSequence
                .SetAutoKill(false)
                .AppendCallback(() => PawnCanMove.gameObject.SetActive(true))
                .Append(PawnCanMove.material.DOFade(1, 1f))
                .onKill += () => PawnCanMove.gameObject.SetActive(false);
        }

        public void ClearSelection() {
            KillSequences();
        }

        private void KillSequences() {
            _cantSequence?.Kill(true);
            _canSequence?.Kill(true);
        }
    }
}