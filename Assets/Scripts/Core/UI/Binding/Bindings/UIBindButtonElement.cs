using System;
using Core.UI.Binding.Interfaces;
using UnityEngine.UIElements;

namespace Core.UI.Binding.Bindings
{
    public class UIBindButtonElement : IUIUnbind, IUIBinding<Button>
    {
        public Button Target { get; set; }
        private Action _handler;

        public void OnClick(Action handler)
        {
            _handler = handler;

            Target.RegisterCallback<ClickEvent>(Handle);
        }

        private void Handle(ClickEvent clickEvent)
        {
            _handler?.Invoke();
        }

        public void Unbind()
        {
            Target.UnregisterCallback<ClickEvent>(Handle);
        }
    }
}