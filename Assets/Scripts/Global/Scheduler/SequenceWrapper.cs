using System;
using System.Threading.Tasks;
using DG.Tweening;

namespace Global.Services.Scheduler {
    public class SequenceWrapper {
        public Action<Guid> OnCompleteSequence;

        private Sequence _sequence;
        private Guid _guid;
        
        public void StartSequence(Guid guid) {
            _sequence = DOTween.Sequence();
            _guid = guid;
            DelayFrames(1);
        }

        public SequenceWrapper Append(float delay, Action callback) {
            _sequence
                .AppendInterval(delay)
                .AppendCallback(() => callback?.Invoke());
            return this;
        }

        public SequenceWrapper AppendCallback(Action callback) {
            _sequence
                .AppendCallback(() => callback?.Invoke());
            return this;
        }
        
        public void Kill() {
            _sequence.Kill();
        }

        private async void DelayFrames(int frames) {
            await Task.Delay(frames);
            _sequence.onComplete += DisposeSequence;
        }

        private void DisposeSequence() {
            OnCompleteSequence?.Invoke(_guid);
            OnCompleteSequence = null;
        }
    }
}
