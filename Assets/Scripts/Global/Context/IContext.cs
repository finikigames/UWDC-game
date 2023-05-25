using Global.StateMachine.Base.Enums;

namespace Global {
    public interface IContext {
        GameContext Context();
        void RegisterContext();
        void UnRegisterContext();
    }
}