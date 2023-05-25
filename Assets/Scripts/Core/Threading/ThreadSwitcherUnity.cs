using System;
using System.Threading;
using Core.Threading.Interfaces;
using JetBrains.Annotations;

namespace Core.Threading
{
    internal struct ThreadSwitcherUnity : IThreadSwitcher
    {
        public IThreadSwitcher GetAwaiter()
        {
            return this;
        }

        public bool IsCompleted => SynchronizationContext.Current == UnityThread.Context;

        public void GetResult()
        {
        }

        public void OnCompleted([NotNull] Action continuation)
        {
            if (continuation == null)
                throw new ArgumentNullException(nameof(continuation));

            UnityThread.Context.Post(s => continuation(), null);
        }
    }
}