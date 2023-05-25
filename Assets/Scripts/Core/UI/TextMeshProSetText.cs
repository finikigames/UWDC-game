using System.Globalization;
using TMPro;
using UnityEngine;

namespace Core.UI
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class TextMeshProSetText : UnityComponent
    {
        private TextMeshProUGUI _text;
        public string Format = "{}";

        private void OnEnable()
        {
            _text = GetComponent<TextMeshProUGUI>();
        }

        public void SetString(string value)
        {
            _text.text = string.Format(Format, value);
        }

        public void SetFloat(float value)
        {
            SetString(value.ToString(CultureInfo.InvariantCulture));
        }
    }
}