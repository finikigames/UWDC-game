using UnityEngine.UIElements;

namespace Core.UI.Binding.Interfaces
{
    public interface IUIBindingValuable<T>
    {
        INotifyValueChanged<T> Target { get; set; }
    }
}