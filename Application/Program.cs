﻿
using TracerApp;

namespace MainProram
{
    class Program
    {

        static void Main(string[] args)
        {
            Tracer tracer = new Tracer();
            Foo foo = new Foo(tracer);
            Bar bar = new Bar(tracer);
            WriterResult wr = new WriterResult();
            Thread myThread = new Thread(new ThreadStart(bar.InnerMethodForSecondThread));
            myThread.Start();
            foo.MyMethod();
            bar.InnerMethod();
            bar.InnerMethod1();
            bar.InnerMethod2();

            wr.JsonXmlToConsole(tracer.GetTraceResult());
            wr.JsonXmlToFile(tracer.GetTraceResult());
        }

    }
    public class Foo
    {
        private Bar _bar;
        private ITracer _tracer;

        internal Foo(ITracer tracer)
        {
            _tracer = tracer;
            _bar = new Bar(_tracer);
        }
        public void MyMethod2()
        {
            _tracer.StartTrace();

            _bar.InnerMethod();

            _tracer.StopTrace();
        }
        public void MyMethod()
        {
            _tracer.StartTrace();

            _bar.InnerMethod();

            _tracer.StopTrace();
        }
    }

    public class Bar
    {
        private ITracer _tracer;

        internal Bar(ITracer tracer)
        {
            _tracer = tracer;
        }

        public void InnerMethod()
        {
            _tracer.StartTrace();
            Thread.Sleep(50);
            _tracer.StopTrace();
        }
        public void InnerMethod1()
        {
            _tracer.StartTrace();
            Thread.Sleep(50);
            _tracer.StopTrace();
        }
        public void InnerMethod2()
        {
            _tracer.StartTrace();
            Thread.Sleep(50);
            _tracer.StopTrace();
        }
        public void InnerMethodForSecondThread()
        {
            _tracer.StartTrace();
            Thread.Sleep(50);
            _tracer.StopTrace();
        }
    }
    class WriterResult
    {
        public void JsonXmlToConsole(string[] jsonAndXml)
        {
            foreach (var item in jsonAndXml)
            {
                Console.WriteLine(item);
            }

        }

        public void JsonXmlToFile(string[] jsonAndXml)
        {
            File.WriteAllText("E:/result.json", jsonAndXml[0]);
            File.WriteAllText("E:/result.xml", jsonAndXml[1]);
        }
    }
}
