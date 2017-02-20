using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;
using CASUALSerialCommunications;
using System.IO;

namespace ComTesting
{
    [TestClass]
    public class ComTesting
    {
        const int SERIAL = 2;
        const int OTHER = 1;

        static String[] splitString = SerialCommunications.splitString;
        [TestMethod]
        public void TestGetComports()
        {
            String portlist = SerialCommunications.getComports();
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Reset();
            stopwatch.Start();
            String[] ports = portlist.Split(new string[] { ";;;" }, StringSplitOptions.RemoveEmptyEntries);
            stopwatch.Stop();
            reportTime(stopwatch);
            foreach (string s in ports)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
            Assert.IsTrue(ports.Length > 0);
        }

        [TestMethod]
        public void TestSendString()
        {
            String[] ports = SerialCommunications.getComports().Split(new String[] { ";;;" }, StringSplitOptions.None);
            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < 3; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                String dataReceived = SerialCommunications.sendData(ports[SERIAL], "\r\nAT\r\nAT\r\n");
                dataReceived = SerialCommunications.sendData(ports[SERIAL]+splitString[0]+ "\r\nAT\r\nAT\r\n");
                stopwatch.Stop();
                System.Diagnostics.Debug.WriteLine("Got data:" + dataReceived);
                reportTime(stopwatch);
                Assert.IsTrue(dataReceived.Contains("OK"));
            }

        }

        [TestMethod]
        public void TestSendBytes()
        {
            string[] ports = SerialCommunications.getComports().Split(new string[] { ";;;" }, StringSplitOptions.None);
            Stopwatch stopwatch = new Stopwatch();
            for (int i = 0; i < 10; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                String sendData = Path.GetTempFileName()+"s";
                SerialCommunications.writeByteArrayToFile(sendData, new byte[] { 0x7e, 0x00, 0x78, 0xf0, 0x7e });
                String expectData = Path.GetTempFileName()+"e";
                SerialCommunications.writeByteArrayToFile(expectData, new byte[] { 0x7e });

                
                String dataReceived = SerialCommunications.sendBinData(ports[OTHER] + splitString[0]+ sendData + splitString[0] + expectData);
                byte[] bytes=SerialCommunications.readFileToByteArray(dataReceived);
                stopwatch.Stop();
                String filename = "";
                foreach (char d in bytes)
                {
                    Console.Write((char)d);
                    filename = filename + (char)d;
                }

                reportTime(stopwatch);
                Assert.IsTrue(bytes.Length > 1);
            }
            

        }


        static private void reportTime(Stopwatch stopwatch)
        {
            Console.WriteLine("\n---TIME ELAPSED FROM SERIAL CALL: " + stopwatch.ElapsedMilliseconds + "millis");
        }

        [TestMethod]
        public void TestsendDataToPort()
        {
            String[] ports = SerialCommunications.getComports().Split(new String[] { ";;;" }, StringSplitOptions.None);
            Stopwatch stopwatch = new Stopwatch();

            for (int i = 0; i < 30; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                Assert.IsTrue(SerialCommunications.sendDataToPort(ports[SERIAL]+splitString[0]+ "\r\nAT\r\n"+splitString[0]+"OK").Contains("true"));
                stopwatch.Stop();
                reportTime(stopwatch);
            }
        }
  

  
        [TestMethod]
        public void testGetInformation()
        {
            string ports = SerialCommunications.getComports();
            Stopwatch stopwatch = new Stopwatch();

            string[] portArray = ports.Split(new string[] { ";;;" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i = 0; i < portArray.Length; i++)
            {
                stopwatch.Reset();
                stopwatch.Start();
                Console.WriteLine(SerialCommunications.getPortInfo(portArray[i]));
                stopwatch.Stop();
                reportTime(stopwatch);
            }

        }
    }
}
