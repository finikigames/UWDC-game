using System;
using System.Collections.Generic;
using Core.Pools;
using Global.Scheduler.Base;

namespace Global.Scheduler {
    public class SchedulerService : ISchedulerService {
        private readonly Dictionary<Guid, SequenceWrapper> _sequences = new();
        private readonly Pool<SequenceWrapper> _pool = new();

        public SequenceWrapper StartSequence() {
            return Start(Guid.NewGuid());
        }
        
        public SequenceWrapper StartSequence(Guid guid) {
            return Start(guid);
        }
        
        public void Dispose(Guid guid) {
            if (_sequences.TryGetValue(guid, out var seq)) {
                _sequences.Remove(guid);
                seq.Kill();
                _pool.Release(seq);
            }
        }

        private SequenceWrapper Start(Guid guid) {
            var sequenceWrapper = _pool.Get();
            _sequences.Add(guid, sequenceWrapper);
            sequenceWrapper.StartSequence(guid);
            sequenceWrapper.OnCompleteSequence += Dispose;
            return sequenceWrapper;
        }
    }
}