using System;

namespace Global.Services.Timer {
    public class DownTimer : Timer {
        private readonly Action OnEnd;
        private readonly Action<int> OnTick;
        private float _initialTime;
        private float _timeLeft;

        public DownTimer(float time, Action callback, bool isLoop = false, Action<int> callbackTick = null) {
            _timeLeft = time;
            _initialTime = time;
            IsLoop = isLoop;
            OnEnd = callback;
            OnTick = callbackTick;
        }

        public override float GetTime() {
            return _timeLeft;
        }

        public override void SetTime(float time) {
            _timeLeft = time;
            _initialTime = time;
        }

        public override void ResetTimer() {
            TimerEnded = false;
            _timeLeft = _initialTime;
        }
        
        public override void ResetTimer(int time) {
            TimerEnded = false;
            _initialTime = time;
            _timeLeft = _initialTime;
        }

        public override void Process() {
            if (TimerEnded) return;
            
            _timeLeft -= UnityEngine.Time.deltaTime;
            OnTick?.Invoke((int)Math.Ceiling(_timeLeft));

            if (_timeLeft <= 0) {
                EndTimer();
            }
        }
 
        private void EndTimer() {
            if (!IsLoop) {
                TimerEnded = true;
                OnEnd?.Invoke();
            }
            else {
                _timeLeft = _initialTime;
                OnEnd?.Invoke();
            }
        }
    }
}