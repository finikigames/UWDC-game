using System;
using Core.Observable;
using Core.UI.Binding.Interfaces;
using UnityEngine.UIElements;

namespace Core.UI.Binding.Bindings
{
    public class UIBindValuableElement<T>: IUIUnbind, IUIBindingValuable<T>
    {
        public INotifyValueChanged<T> Target { get; set; }
        private IUIUnbind _unbind;
        private Action<T> _handler;

       public UIBindWithModelField<T, N> With<N>(Observable<N> field)
        {
            var with = new UIBindWithModelField<T, N>(Target, field);
            _unbind = with;
            return with;
        }

       public void OnValueChanged(Action<T> handler)
       {
           Target.RegisterValueChangedCallback(HandleElement);
           _handler = handler;
       }

       private void HandleElement(ChangeEvent<T> value)
       {
          _handler?.Invoke(value.newValue);
       }

        public void Unbind()
        {
            Target.UnregisterValueChangedCallback(HandleElement);
            _unbind.Unbind();
        }
    }
}