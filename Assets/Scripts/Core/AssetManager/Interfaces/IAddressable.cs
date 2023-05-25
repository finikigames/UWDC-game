using System.Threading.Tasks;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Core.AssetManager.Interfaces
{
    public interface IAddressable<T> where T : class
    {
       Task<T> Task { get; }
       AsyncOperationStatus Status { get; }
    }
}