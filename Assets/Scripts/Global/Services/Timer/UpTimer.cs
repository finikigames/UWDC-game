using System;

namespace Global.Services.Timer {
    public class UpTimer : Timer
    {
        private readonly Action _onEnd;
        private readonly Action<float> _onTick;
        private float _currentTime;
        private float _timeDuration;
        
        public UpTimer(float time, Action callback, bool isLoop = false, Action<float> callbackTick = null) {
            _timeDuration = time;
            _currentTime = 0;
            IsLoop = isLoop;
            _onEnd = callback;
            _onTick = callbackTick;
        }

        public override void SetTime(float time) {
            _timeDuration = time;
            _currentTime = 0;
        }

        public override float GetTime() {
            return _currentTime;
        }

        public override void ResetTimer() {
            TimerEnded = false;
            _currentTime = 0;
        }
        
        public override void ResetTimer(float time) {
            TimerEnded = false;
            _timeDuration = time;
            _currentTime = 0;
        }

        public override void Process() {
            if (TimerEnded) return;
            
            _currentTime += UnityEngine.Time.deltaTime;
            _onTick?.Invoke((int)Math.Floor(_currentTime));

            if (_currentTime >= _timeDuration) {
                EndTimer();
            }
        }
 
        private void EndTimer() {
            if (!IsLoop) {
                TimerEnded = true;
                _onEnd?.Invoke();
            }
            else {
                _currentTime = _timeDuration;
                _onEnd?.Invoke();
            }
        }
    }
}