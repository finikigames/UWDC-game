using System;
using Core.Observable;
using Core.UI.Binding.Interfaces;
using UnityEngine.UIElements;

namespace Core.UI.Binding.Bindings
{
    public class UIBindWithModelField<TValueType, TModelType> : IUIUnbind
    {
        private readonly INotifyValueChanged<TValueType> _element;
        private readonly Observable<TModelType> _field;

        private Func<TModelType, TValueType> _convertModelToElement;
        private Func<TValueType, TModelType> _convertElementToModel;

        public UIBindWithModelField(INotifyValueChanged<TValueType> element, Observable<TModelType> field)
        {
            _element = element;
            _field = field;
        }

        public void Apply()
        {
            _field.Subscribe(HandleModel);
            _element.RegisterValueChangedCallback(HandleElement);
        }


        public UIBindWithModelField<TValueType, TModelType> AttachConverters(Func<TValueType, TModelType> elementToModel, Func<TModelType, TValueType> modelToElement)
        {
            _convertElementToModel = elementToModel;
            _convertModelToElement = modelToElement;
            return this;
        }

        private void HandleModel(TModelType value)
        {
            if (typeof(TValueType) == typeof(TModelType))
            {
                _element.value = (TValueType) Convert.ChangeType(value, typeof(TValueType));
                return;
            }

            if (_convertModelToElement == null)
            {
                throw new Exception($"Declare converter from {typeof(TValueType)} to {typeof(TModelType)}");
            }

            _element.value = _convertModelToElement(value);
        }

        private void HandleElement(ChangeEvent<TValueType> handler)
        {
            if (typeof(TValueType) == typeof(TModelType))
            {
                _field.Value = (TModelType) Convert.ChangeType(handler.newValue, typeof(TModelType));
                return;
            }
            if(_convertElementToModel == null)
            {
                throw new Exception($"Declare converter from {typeof(TModelType)} to {typeof(TValueType)}");
            }

            _field.Value = _convertElementToModel(handler.newValue);
        }

        public void Unbind()
        {
            _field.Unsubscribe(HandleModel);
            _element.UnregisterValueChangedCallback(HandleElement);
        }
    }
}