using System;

namespace Global.Services.Timer {
    public class DownTimer : Timer {
        private readonly Action _onEnd;
        private readonly Action<float> _onTick;
        private float _initialTime;
        private float _timeLeft;

        public DownTimer(float time, Action callback, bool isLoop = false, Action<float> callbackTick = null) {
            _timeLeft = time;
            _initialTime = time;
            IsLoop = isLoop;
            _onEnd = callback;
            _onTick = callbackTick;
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
        
        public override void ResetTimer(float time) {
            TimerEnded = false;
            _initialTime = time;
            _timeLeft = _initialTime;
        }

        public override void Process() {
            if (TimerEnded) return;
            
            _timeLeft -= UnityEngine.Time.deltaTime;
            _onTick?.Invoke((int)Math.Ceiling(_timeLeft));

            if (_timeLeft <= 0) {
                EndTimer();
            }
        }
 
        private void EndTimer() {
            if (!IsLoop) {
                TimerEnded = true;
                _onEnd?.Invoke();
            }
            else {
                _timeLeft = _initialTime;
                _onEnd?.Invoke();
            }
        }
    }
}