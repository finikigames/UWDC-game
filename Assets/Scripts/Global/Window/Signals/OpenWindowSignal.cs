using Core.MVP.Base.Interfaces;
using Global.Window.Enums;

namespace Global.Window.Signals {
    public struct OpenWindowSignal {
        public WindowKey Key;
        public IWindowData Data;

        public OpenWindowSignal(WindowKey key, IWindowData data) {
            Key = key;
            Data = data;
        }
    }
}
