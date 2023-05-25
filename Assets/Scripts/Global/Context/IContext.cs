using Global.StateMachine.Base.Enums;

namespace Global.Context {
    public interface IContext {
        GameContext Context();
        void RegisterContext();
        void UnRegisterContext();
    }
}