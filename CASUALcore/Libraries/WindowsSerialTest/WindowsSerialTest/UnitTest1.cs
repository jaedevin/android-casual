using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WindowsSerialJavaInterface;

namespace WindowsSerialTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestGetComPorts() {
            String portlist = WindowsSerial.getComPorts();
            String[] ports= portlist.Split(new string[] { ";;;" }, StringSplitOptions.None);
            System.Diagnostics.Debug.Write("got serial ports");
            foreach (string s in ports)
            {
                System.Diagnostics.Debug.WriteLine(s);
            }
            Assert.IsTrue(ports.Length > 0);
        }
        [TestMethod]
        public void TestSendData() {
            String[] ports = WindowsSerial.getComPorts().Split(new String[] { ";;;" }, StringSplitOptions.None); for (int i = 0; i < 10; i++)
            {
                String dataReceived = WindowsSerial.sendData(ports[1], "\r\nAT\r\nAT\r\n");
                System.Diagnostics.Debug.WriteLine("Got data:" + dataReceived);
                Assert.IsTrue(dataReceived.Contains("OK"));
            }

        }
        [TestMethod]
        public void TestSendDataWithExpectation() {
            String[] ports = WindowsSerial.getComPorts().Split(new String[] { ";;;" }, StringSplitOptions.None); for (int i = 0; i < 30; i++)
            {
                Assert.IsTrue( WindowsSerial.sendDataToPort(ports[1], "\r\nAT\r\n", "OK"));
            }
        }
        [TestMethod]
        public void testConnection() {

            String[] ports = WindowsSerial.getComPorts().Split(new String[] { ";;;" }, StringSplitOptions.None);
            Assert.IsTrue(WindowsSerial.checkPortStatus(ports[2]));
            for (int i = 0; i < 30; i++)
            {
                System.Diagnostics.Debug.Write(i + ";");
                Assert.IsTrue(WindowsSerial.checkPortStatus(ports[0]));
            }
        }
        [TestMethod]
        public void testGetInformation() {
            string ports = WindowsSerial.getComPorts();
            Console.WriteLine(ports);

            string[] portArray=ports.Split(new string[] { ";;;" }, StringSplitOptions.RemoveEmptyEntries);
            for (int i= 0; i < portArray.Length; i++) { 
               Console.WriteLine(WindowsSerial.getPortInfoz(portArray[i]));
            }

        }
    }
}
