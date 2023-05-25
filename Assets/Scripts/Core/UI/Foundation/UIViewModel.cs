/*using ProjectBattler.Source.Scripts.Core.UI.Binding;
using UnityEngine;
using UnityEngine.UIElements;
using Zenject;

namespace ProjectBattler.Source.Scripts.Core.UI.Foundation
{
    [RequireComponent(typeof(UIDocument))]
    public class UIViewModel<TView, TModel> : UnityComponent where TView : UIView
                                                             where TModel : UIModel, new()
    {
        [Inject] protected App App;

        protected readonly TModel Model = new TModel();
        protected readonly UIBinder Binder = new UIBinder();

        protected UIDocument Document;
        protected TView View;

        private void OnEnable()
        {
            Document = GetComponent<UIDocument>();
            View = Document.rootVisualElement.Q<TView>();
            View.OnReady += OnViewReady;
        }

        private void OnDisable()
        {
            View.OnReady -= OnViewReady;
        }

        protected virtual void OnViewReady() {}
    }
}*/