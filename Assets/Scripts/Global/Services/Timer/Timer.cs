using System;

namespace Global.Services.Timer {
    public class Timer {
        public bool TimerEnded;
        public float TimeLeft { get; private set; }
        
        public bool IsLoop;

        private readonly Action OnEnd;
        private readonly Action<int> OnTick;
        private float _initialTime;

        public Timer(float time, Action callback, bool isLoop = false, Action<int> callbackTick = null) {
            TimeLeft = time;
            _initialTime = time;
            IsLoop = isLoop;
            OnEnd = callback;
            OnTick = callbackTick;
        }

        public void SetTime(float time) {
            TimeLeft = time;
            _initialTime = time;
        }

        public void ResetTimer() {
            TimerEnded = false;
            TimeLeft = _initialTime;
        }

        public void ResetTimer(float time) {
            TimerEnded = false;
            _initialTime = time;
            TimeLeft = _initialTime;
        }

        public void Process() {
            if (TimerEnded) return;
            
            TimeLeft -= UnityEngine.Time.deltaTime;
            OnTick?.Invoke((int)Math.Ceiling(TimeLeft));

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