using Global.Window.Enums;
using Global.Window.Signals;
using Main.UI.Data;
using Zenject;

namespace Main.Services.Startup {
    public class MainSceneService : IInitializable {
        private readonly SignalBus _signalBus;

        public MainSceneService(SignalBus signalBus) {
            _signalBus = signalBus;
        }
        
        public void Initialize() {
            _signalBus.Fire(new OpenWindowSignal(WindowKey.StartWindow, new StartWindowData()));
        }
    }
}