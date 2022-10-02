using System.Collections.Concurrent;
using System.Diagnostics;
using System.Reflection;
using System.Text.Json;
using System.Xml.Serialization;

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
    public class Tracer : ITracer
    {

        ConcurrentDictionary<int, Stack<TraceResult>> threadsResults;

        public Tracer()
        {
            threadsResults = new ConcurrentDictionary<int, Stack<TraceResult>>();
        }

        public void StartTrace()
        {
            TraceResult traceResult = new TraceResult();
            Stack<TraceResult> stackTraceResult = new Stack<TraceResult>();
            //Получаем имя метода и класс к кторому он принадлежит
            StackTrace stackTrace = new StackTrace();
            StackFrame frame = stackTrace.GetFrame(1);
            MethodBase method = frame.GetMethod();
            Type type = method.DeclaringType;
            traceResult.time = 0;
            traceResult.methodName = method.Name;
            traceResult.className = type.Name;
            //Запускаем stopwatch и помещаем его в структуру traceResult
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            traceResult.stopwatch = stopwatch;
            traceResult.traceResultList = new List<TraceResult>();
            //Создаем новый стек для потока, если он еще не был создан
            threadsResults.GetOrAdd(Thread.CurrentThread.ManagedThreadId, _ => new Stack<TraceResult>());
            threadsResults.TryGetValue(Thread.CurrentThread.ManagedThreadId, out stackTraceResult);
            //Создаем корневую структуру, которая будет считать общее время выполнения.
            if (stackTraceResult.Count == 0)
            {
                TraceResult root = new TraceResult();
                root.methodName = Thread.CurrentThread.ManagedThreadId.ToString();
                root.className = "Thread";
                root.time = 0;
                Stopwatch stopwatch1 = new Stopwatch();
                stopwatch1.Start();
                root.stopwatch = stopwatch1;

                root.traceResultList = new List<TraceResult>();

                stackTraceResult.Push(root);
                stackTraceResult.Push(traceResult);
            }
            else
            {
                stackTraceResult.Push(traceResult);
            }

            threadsResults.TryAdd(Thread.CurrentThread.ManagedThreadId, stackTraceResult);
        }
        public void StopTrace()
        {
            Stack<TraceResult> stackTraceResult = new Stack<TraceResult>();
            threadsResults.TryGetValue(Thread.CurrentThread.ManagedThreadId, out stackTraceResult);
            TraceResult traceResult = stackTraceResult.Pop();
            Stopwatch stopwatch = traceResult.stopwatch;
            stopwatch.Stop();
            traceResult.time = stopwatch.ElapsedMilliseconds;
            stackTraceResult.Peek().traceResultList.Add(traceResult);
            if (stackTraceResult.Count == 1)
            {
                Stopwatch stopwatch1 = stackTraceResult.Peek().stopwatch;
                stopwatch1.Stop();
                stackTraceResult.Peek().time = stopwatch1.ElapsedMilliseconds;
                stopwatch1.Start();
                stackTraceResult.Peek().stopwatch = stopwatch1;
            }
            threadsResults.TryAdd(Thread.CurrentThread.ManagedThreadId, stackTraceResult);
        }
        public string[] GetTraceResult()
        {
            List<TraceResult> TRL = new List<TraceResult>();
            foreach (KeyValuePair<int, Stack<TraceResult>> item in threadsResults)
            {
                TRL.Add(item.Value.Pop());
                item.Value.Push(TRL[TRL.Count - 1]);
            }
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
            };
            string json = JsonSerializer.Serialize<List<TraceResult>>(TRL, options);

            XmlSerializer formatter = new XmlSerializer(typeof(List<TraceResult>));
            StringWriter textWriter = new StringWriter();
            formatter.Serialize(textWriter, TRL);

            string[] result = { json, textWriter.ToString() };
            return result;
        }
    }
}
