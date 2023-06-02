using Core.MVP.Base.Interfaces;
using Global.Enums;

namespace Global.UI.Data {
    public class FlyTextData : IWindowData {
        public string FlyText { get; set; }
        public PawnColor PawnColor { get; set; }
    }
}