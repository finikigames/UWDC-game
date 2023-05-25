using Global.Window.Enums;

namespace Global.Window.Signals {
    public struct CloseWindowSignal {
        public WindowKey Key;

        public CloseWindowSignal(WindowKey key) {
            Key = key;
        }
    }
}
