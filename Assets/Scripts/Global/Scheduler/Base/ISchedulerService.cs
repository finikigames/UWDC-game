using System;

namespace Global.Scheduler.Base {
    public interface ISchedulerService {
        SequenceWrapper StartSequence();
        SequenceWrapper StartSequence(Guid guid);
        void Dispose(Guid guid);
    }
}