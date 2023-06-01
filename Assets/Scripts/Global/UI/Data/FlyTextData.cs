using Core.MVP.Base.Interfaces;

namespace Global.UI.Data {
    public class FlyTextData : IWindowData {
        public string FlyText { get; set; }

        public FlyTextData(string text) {
            FlyText = text;
        }
    }
}