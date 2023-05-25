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

        public void StartTimer(string timerId, float time, Action onEnd, bool isLoop = false) {
            var timer = new Timer(time, onEnd, isLoop);
            
            _timers.Add(timerId, timer);
        }

        public void Tick() {
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