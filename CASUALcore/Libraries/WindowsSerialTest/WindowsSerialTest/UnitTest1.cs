using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsSerialJavaInterface;
using System.Diagnostics;

namespace WindowsSerialTest
{
    [TestClass]
    public class UnitTest1
    {
        const int SERIAL = 1;
        const int OTHER = 2;


        [TestMethod]
        public void TestGetComports() {
            String portlist = WindowsSerialCSharp.getComports();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            String[] ports= portlist.Split(new string[] { ";;;" }, StringSplitOptions.RemoveEmptyEntries);
            stopwatch.Stop();
            reportTime(stopwatch);
            foreach (string s in ports)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
            Assert.IsTrue(ports.Length > 0);
        }

        [TestMethod]
        public void TestSendString() {
            String[] ports = WindowsSerialCSharp.getComports().Split(new String[] { ";;;" }, StringSplitOptions.None);
            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < 3; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                String dataReceived = WindowsSerialCSharp.sendData(ports[SERIAL], "\r\nAT\r\nAT\r\n");
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine("Got data:" + dataReceived );
                reportTime(stopwatch);
                Assert.IsTrue(dataReceived.Contains("OK"));
            }

        }

        [TestMethod]
        public void TestSendBytes()
        {
            string[] ports = WindowsSerialCSharp.getComports().Split(new string[] { ";;;" }, StringSplitOptions.None);
            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < 10; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                int[] dataReceived = WindowsSerialCSharp.sendBinData(ports[OTHER], new byte[] { 0x7e, 0x0c, 0x14, 0x3a, 0x7e, }, new byte[] { 0x7e });
                stopwatch.Stop();
                foreach (int d in dataReceived)
                {
                    Console.Write("0x"+d);

                }
                reportTime(stopwatch);
            }

        }


        static private void reportTime(Stopwatch stopwatch)
        {
            Console.WriteLine("\n---TIME ELAPSED FROM SERIAL CALL: " + stopwatch.ElapsedMilliseconds + "millis");
        }

        [TestMethod]
        public void TestSendDataWithExpectation() {
            String[] ports = WindowsSerialCSharp.getComports().Split(new String[] { ";;;" }, StringSplitOptions.None);
            Stopwatch stopwatch = new Stopwatch();

            for (int i = 0; i < 30; i++) {
                stopwatch.Reset();
                stopwatch.Start();
                Assert.IsTrue( WindowsSerialCSharp.sendDataToPort(ports[SERIAL], "\r\nAT\r\n", "OK"));
                stopwatch.Stop();
                reportTime(stopwatch);
            }
        }
        [TestMethod]
        public void testConnection() {
            String portlist = WindowsSerialCSharp.getComports();
            Stopwatch stopwatch = new Stopwatch();

            String[] ports = portlist.Split(new String[] { ";;;" }, StringSplitOptions.None);
            Assert.IsTrue(WindowsSerialCSharp.checkPortStatus(ports[SERIAL]));
            for (int i = 0; i < 30; i++)
            {
                System.Diagnostics.Debug.Write((char)i + ";");

                stopwatch.Reset();
                stopwatch.Start();
                Assert.IsTrue(WindowsSerialCSharp.checkPortStatus(ports[SERIAL]));
                stopwatch.Stop();
                reportTime(stopwatch);
            }
        }
        [TestMethod]
        public void testGetInformation() {
            string ports = WindowsSerialCSharp.getComports();
            Stopwatch stopwatch = new Stopwatch();

            string[] portArray=ports.Split(new string[] { ";;;" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i= 0; i < portArray.Length; i++) {
                stopwatch.Reset();
                stopwatch.Start();
                Console.WriteLine(WindowsSerialCSharp.getPortInfo(portArray[i]));
                stopwatch.Stop();
                reportTime(stopwatch);
            }

        }
    }
}
