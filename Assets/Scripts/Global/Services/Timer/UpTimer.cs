using System;

namespace Global.Services.Timer {
    public class UpTimer : Timer
    {
        private readonly Action OnEnd;
        private readonly Action<int> OnTick;
        private float _currentTime;
        private float _timeDuration;
        
        public UpTimer(float time, Action callback, bool isLoop = false, Action<int> callbackTick = null) {
            _timeDuration = time;
            _currentTime = 0;
            IsLoop = isLoop;
            OnEnd = callback;
            OnTick = callbackTick;
        }

        public override void SetTime(float time) {
            _timeDuration = time;
            _currentTime = 0;
        }

        public override void ResetTimer() {
            TimerEnded = false;
            _currentTime = 0;
        }
        
        public override void ResetTimer(int time) {
            TimerEnded = false;
            _timeDuration = time;
            _currentTime = 0;
        }

        public override void Process() {
            if (TimerEnded) return;
            
            _currentTime += UnityEngine.Time.deltaTime;
            OnTick?.Invoke((int)Math.Floor(_currentTime));

            if (_currentTime >= _timeDuration) {
                EndTimer();
            }
        }
 
        private void EndTimer() {
            if (!IsLoop) {
                TimerEnded = true;
                OnEnd?.Invoke();
            }
            else {
                _currentTime = _timeDuration;
                OnEnd?.Invoke();
            }
        }
    }
}