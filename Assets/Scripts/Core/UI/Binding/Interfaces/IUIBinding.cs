using UnityEngine.UIElements;

namespace Core.UI.Binding.Interfaces
{
    public interface IUIBinding<TElement> where TElement : VisualElement
    {
        TElement Target { get; set; }
    }
}