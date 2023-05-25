using System;
using System.Diagnostics;
using System.Text;
using TMPro;
using Unity.Profiling;
using UnityEngine;
using UnityEngine.Profiling;

namespace Core
{
    public class FrameRateTiming : MonoBehaviour
    {
        public TextMeshProUGUI CPU;
        public TextMeshProUGUI GPU;
        public TextMeshProUGUI EcsMS;
        public TextMeshProUGUI AverageEcsMSText;

        public static TextMeshProUGUI meow;

        Recorder UpdateRecorder;
        ProfilerRecorder mainThreadTimeRecorder;
        FrameTiming[] frameTimings = new FrameTiming[3];
        const uint kNumFrameTimings = 1;
        uint m_frameCount = 0;
        public static Stopwatch m_stopwatch = new Stopwatch();
        private static long averageEcsMS;
        private Stopwatch stopwatch = new Stopwatch();
        private int frameCount;
        private float frameSampleRate = 0.1f;
        private string[] cpuFrameRateStrings;
        private string[] gpuFrameRateStrings;
        private int displayedDecimalDigits = 2;

        private void Awake()
        {
            UpdateRecorder = Recorder.Get("Gfx.PresentFrame");
            UpdateRecorder.enabled = true;
            meow = EcsMS;
            BuildFrameRateStrings();
            stopwatch.Reset();
            stopwatch.Start();
        }

        private void Update()
        {
            ++m_frameCount;
            if (m_frameCount <= kNumFrameTimings)
            {
                return;
            }
            FrameTimingManager.CaptureFrameTimings();
            FrameTimingManager.GetLatestTimings(1, frameTimings);
            if (frameTimings.Length < kNumFrameTimings)
            {
                //Debug.LogFormat("Skipping frame {0}, didn't get enough frame timings.",
                   // m_frameCount);

                return;
            }
            mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, "Gfx.PresentFrame");

            AverageEcsMSText.text = averageEcsMS.ToString() + "ms";
            var frameTime = mainThreadTimeRecorder.LastValue;
            //if (UpdateRecorder.isValid && SystemInfo.supportsGpuRecorder)
                //Debug.Log("BehaviourUpdate time: " + UpdateRecorder.gpuElapsedNanoseconds);
            //CPU.text = FrameTimingManager.GetCpuTimerFrequency().ToString();
            //GPU.text = FrameTimingManager.GetGpuTimerFrequency().ToString();
            //GPU.text = RenderTiming.instance.deltaTime.ToString();
            var result = FrameTimingManager.GetGpuTimerFrequency();
            //Debug.LogFormat("result: {0}", result);
            //Debug.LogFormat("result: {0}", frameTime);
            //Debug.Log(frameTime);
            var text = string.Format(
                "cpu frame time: {0}\ncpu time frame complete: {1}\ncpu time present called: {2}\ngpu frame time: {3}",
                frameTimings[0].cpuFrameTime,
                frameTimings[0].cpuTimeFrameComplete,
                frameTimings[0].cpuTimePresentCalled,
                frameTimings[0].gpuFrameTime);
            //Debug.Log(text);
            //GPU.text = RenderTiming.instance.deltaTime.ToString();
            float elapsedSeconds = stopwatch.ElapsedMilliseconds * 0.001f;
            if (elapsedSeconds >= frameSampleRate)
            {
                int cpuFrameRate = (int)(1.0f / (elapsedSeconds / m_frameCount));
                int gpuFrameRate = (int)(1.0f / (RenderTiming.instance.deltaTime / m_frameCount));
                m_frameCount = 0;
                CPU.text = cpuFrameRateStrings[Mathf.Clamp(cpuFrameRate, 0, Application.targetFrameRate)];
                GPU.text = gpuFrameRateStrings[Mathf.Clamp(gpuFrameRate, 0, Application.targetFrameRate)];
                stopwatch.Reset();
                stopwatch.Start();
            }
        }
        
        private void BuildFrameRateStrings()
        {
            cpuFrameRateStrings = new string[Application.targetFrameRate + 1];
            gpuFrameRateStrings = new string[Application.targetFrameRate + 1];
            string displayedDecimalFormat = string.Format("{{0:F{0}}}", displayedDecimalDigits);

            StringBuilder stringBuilder = new StringBuilder(32);
            StringBuilder milisecondStringBuilder = new StringBuilder(16);

            for (int i = 0; i < cpuFrameRateStrings.Length; ++i)
            {
                float miliseconds = (i == 0) ? 0.0f : (1.0f / i) * 1000.0f;
                milisecondStringBuilder.AppendFormat(displayedDecimalFormat, miliseconds);
                stringBuilder.AppendFormat("CPU: {0} fps ({1} ms)", i.ToString(), milisecondStringBuilder.ToString());
                cpuFrameRateStrings[i] = stringBuilder.ToString();
                stringBuilder.Length = 0;
                stringBuilder.AppendFormat("GPU: {0} fps ({1} ms)", i.ToString(), milisecondStringBuilder.ToString());
                gpuFrameRateStrings[i] = stringBuilder.ToString();
                milisecondStringBuilder.Length = 0;
                stringBuilder.Length = 0;
            }
        }
        
        public static void MeasureByStopwatch(Action action)
        {
            m_stopwatch.Reset();
            m_stopwatch.Start();

            action();

            m_stopwatch.Stop();

            averageEcsMS += m_stopwatch.ElapsedMilliseconds;
            averageEcsMS /= 2;
            meow.text = m_stopwatch.ElapsedMilliseconds.ToString() + "ms";
            //UnityEngine.Debug.Log(logName + ": " + m_stopwatch.ElapsedMilliseconds);
        }
        
        public static void MeasureByDateTime(Action action, string logName)
        {
            var startTime = DateTime.Now;

            action();

            var elapsed = (DateTime.Now - startTime).Milliseconds;

            UnityEngine.Debug.Log(logName + ": " + elapsed);
        }

        public static void MeasureByEnvironmentTickCount(Action action, string logName)
        {
            var startTick = Environment.TickCount;

            action();

            var elapsed = Environment.TickCount - startTick;

            UnityEngine.Debug.Log(logName + ": " + elapsed);
        }
    }
}