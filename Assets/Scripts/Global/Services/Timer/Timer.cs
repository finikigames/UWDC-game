using System;

namespace Global.Services.Timer {
    public class Timer {
        public bool TimerEnded;
        public float TimeLeft { get; private set; }
        
        public bool IsLoop;

        private readonly Action OnEnd;
        private float _initialTime;

        public Timer(float time, Action callback, bool isLoop = false) {
            TimeLeft = time;
            _initialTime = time;
            IsLoop = isLoop;
            OnEnd = callback;
        }

        public void SetTime(float time) {
            TimeLeft = time;
            _initialTime = time;
        }

        public void ResetTimer() {
            TimerEnded = false;
            TimeLeft = _initialTime;
        }

        public void Process() {
            if (TimerEnded) return;
            
            TimeLeft -= UnityEngine.Time.deltaTime;

            if (TimeLeft <= 0) {
                EndTimer();
            }
        }
 
        private void EndTimer() {
            if (!IsLoop) {
                TimerEnded = true;
                OnEnd?.Invoke();
            }
            else {
                TimeLeft = _initialTime;
                OnEnd?.Invoke();
            }
        }
    }
}