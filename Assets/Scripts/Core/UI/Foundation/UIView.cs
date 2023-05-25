/*using System;
using System.Reflection;
using ProjectBattler.Source.Scripts.Core.UI.Attributes;
using UnityEngine.UIElements;

namespace ProjectBattler.Source.Scripts.Core.UI.Foundation
{
    public class UIView : VisualElement
    {
        public Action OnReady { get; set; }

        protected virtual void Initialize() { }

        protected UIView()
        {
            RegisterCallback<GeometryChangedEvent>(OnGeometryChange);
        }

        private void OnGeometryChange(GeometryChangedEvent evt)
        {
            UnregisterCallback<GeometryChangedEvent>(OnGeometryChange);
            ReflectDataAndInject();

            OnReady?.Invoke();
            OnReady = null;

            Initialize();
        }

        private void ReflectDataAndInject()
        {
            var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
            var isErrorFound = false;
            var error = "";

            foreach (var info in fields)
            {
                var attribute = (UIInject) Attribute.GetCustomAttribute(info, typeof (UIInject));
                if(attribute == null) continue;

                if (!VerifyAndGetElement<VisualElement>(attribute.Name, out var value))
                {
                    isErrorFound = true;
                    error += $"Element {attribute.Name} is missing!\n";
                }

                info.SetValue(this, value);
            }

            if (!isErrorFound) return;
            throw new Exception($"Error in {GetType().Name}. Some elements in view is missing. Check UXML layout.\n{error}\n");
        }

        private bool VerifyAndGetElement<TElement>(string elementName, out TElement value) where TElement : VisualElement
        {
            value = this.Q<TElement>(elementName);
            return value != null;
        }
    }
}*/