using Microsoft.VisualStudio.TestTools.UnitTesting;
using TracerApp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace TracerApp.Tests
{
    [TestClass()]
    public class TracerTests
    {
        private int waitTime = 100;

        [TestMethod()]
        public void AnyMethod()
        {
            Tracer tracer = new Tracer();
            tracer.StartTrace();
            Thread.Sleep(waitTime);
            tracer.StopTrace();
        }

        [TestMethod()]
        public void AnyMethod2()
        {
            Tracer tracer = new Tracer();
            tracer.StartTrace();
            AnyMethod3();
            AnyMethod3();
            tracer.StopTrace();
        }

        [TestMethod()]
        public void AnyMethod3()
        {
            Tracer tracer = new Tracer();
            tracer.StartTrace();
            Thread.Sleep(waitTime / 4);
            tracer.StopTrace();
        }
    
    }
}