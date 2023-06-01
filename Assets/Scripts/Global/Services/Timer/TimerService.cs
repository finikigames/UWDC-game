using System;
using System.Collections.Generic;
using Zenject;

namespace Global.Services.Timer {
    public class TimerService : ITickable {
        private Dictionary<string, Timer> _timers;
        private HashSet<string> _timersToDelete;

        public TimerService() {
            _timers = new Dictionary<string, Timer>();
            _timersToDelete = new HashSet<string>();
        }

        public void StartTimer(string timerId, float time, Action onEnd, bool isLoop = false, Action<int> onTick = null) {
            StartDownTimer(timerId, time, onEnd, isLoop, onTick);
        }

        public void StartDownTimer(string timerId, float time, Action onEnd, bool isLoop = false, Action<int> onTick = null) {
            var timer = new DownTimer(time, onEnd, isLoop, onTick);
            
            _timers.Add(timerId, timer);
        }

        public float GetTime(string timerId) {
            if (!_timers.ContainsKey(timerId)) return 0f;

            return _timers[timerId].GetTime();
        }

        public void StartUpTimer(string timerId, float time, Action onEnd, bool isLoop = false, Action<int> onTick = null)
        {
            var timer = new UpTimer(time, onEnd, isLoop, onTick);
            _timers.Add(timerId, timer);
        }

        public void RemoveTimer(string timerId) {
            _timersToDelete.Add(timerId);
        }

        public void ResetTimer(string timerId) {
            if (!_timers.ContainsKey(timerId)) return;
            
            _timers[timerId].ResetTimer();
        }

        public void ResetTimer(string timerId, int time) {
            if (!_timers.ContainsKey(timerId)) return;
            
            _timers[timerId].ResetTimer(time);
        }

        public void Tick() {
            foreach (var key in _timersToDelete) {
                _timers.Remove(key);
            }

            foreach (var timerPair in _timers) {
                timerPair.Value.Process();

                if (timerPair.Value.TimerEnded) {
                    if (timerPair.Value.IsLoop) {
                        timerPair.Value.ResetTimer();   
                    }
                    else {
                        _timersToDelete.Add(timerPair.Key);
                    }
                }
            }

            foreach (var key in _timersToDelete) {
                _timers.Remove(key);
            }
            
            _timersToDelete.Clear();
        }
    }
}