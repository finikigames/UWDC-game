using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Core.Extensions.UI {
    public class SliderTextValueRepresenter : MonoBehaviour {
        public Slider Slider;
        public TextMeshProUGUI Text;

        public void Start() {
            SetValue();

            Slider.onValueChanged.AddListener(_ => SetValue());
        }

        // Todo garbage
        private void SetValue() {
            Text.text = $"{((int)Slider.value).ToString()}/{Slider.maxValue.ToString()}";
        }
    }
}