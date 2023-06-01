
namespace Global.Services.Timer {
    public abstract class Timer {
        public bool TimerEnded;
        public bool IsLoop;

        public abstract void SetTime(float time);

        public abstract void ResetTimer();

        public abstract void Process();
    }
}