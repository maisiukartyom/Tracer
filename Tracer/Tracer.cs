using System.Diagnostics;

namespace TracerApp
{
    public class TraceResult
    {
        public Stopwatch stopwatch;

        public long time { get; set; }

        public string methodName { get; set; }

        public string className { get; set; }

        public List<TraceResult> traceResultList { get; set; }
    }
    public interface ITracer
    {

        void StartTrace();

        void StopTrace();

        string[] GetTraceResult();

    }
}
