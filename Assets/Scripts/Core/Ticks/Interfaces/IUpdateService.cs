namespace Core.Ticks.Interfaces
{
    public interface IUpdateService
    {
        void RegisterUpdate(IUpdatable updatable);
        void RegisterFixedUpdate(IFixedUpdateRunner fixedUpdateRunner);
        void RegisterLateUpdate(ILateUpdate lateUpdate);
        void RegisterGizmoUpdate(IGizmoRunner gizmoRunner);
        void UnregisterUpdate(IUpdatable updatable);
        void UnregisterFixedUpdate(IFixedUpdateRunner fixedUpdateRunner);
        void UnregisterLateUpdate(ILateUpdate lateUpdate);
        void UnregisterGizmoUpdate(IGizmoRunner gizmoRunner);
        void StopAll();
        void StartAll();
    }
}