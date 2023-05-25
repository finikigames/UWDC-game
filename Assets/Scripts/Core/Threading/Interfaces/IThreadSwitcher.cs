using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Core.Threading.Interfaces
{
    /// <summary>
    ///     Defines an object that switches to a thread.
    /// </summary>
    [PublicAPI]
    public interface IThreadSwitcher : INotifyCompletion
    {
        bool IsCompleted { get; }

        IThreadSwitcher GetAwaiter();

        void GetResult();
    }
}