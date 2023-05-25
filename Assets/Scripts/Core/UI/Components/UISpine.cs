using UnityEngine.UIElements;

namespace Core.UI.Components
{
    public class UISpine : VisualElement
    {
        public new class UxmlFactory : UxmlFactory<UISpine, UxmlTraits> { }
        public new class UxmlTraits : VisualElement.UxmlTraits
        {
            UxmlStringAttributeDescription _stringData = new UxmlStringAttributeDescription { name = "Name", defaultValue = "default_value" };

            public override void Init(VisualElement element, IUxmlAttributes bag, CreationContext cc)
            {
                base.Init(element, bag, cc);
                if(!(element is UISpine spine)) return;

                spine.stringAttr = _stringData.GetValueFromBag(bag, cc);
            }
        }

        public string stringAttr { get; set; }
    }
}