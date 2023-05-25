using Global.Window.Enums;

namespace Global.Window.Signals {
    public class PreloadWindowSignal {
        public WindowKey Key;

        public PreloadWindowSignal(WindowKey key) {
            Key = key;
        }
    }
}