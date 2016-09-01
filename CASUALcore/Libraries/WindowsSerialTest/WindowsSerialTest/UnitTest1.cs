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
            String[] ports = WindowsSerial.getComPorts().Split(new String[] { ";;;" }, StringSplitOptions.None);
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
                String dataReceived = WindowsSerial.sendData(ports[0], "\r\nAT\r\nAT\r\n");
                System.Diagnostics.Debug.WriteLine("Got data:" + dataReceived);
                Assert.IsTrue(dataReceived.Contains("OK"));
            }

        }
        [TestMethod]
        public void TestSendDataWithExpectation() {
            String[] ports = WindowsSerial.getComPorts().Split(new String[] { ";;;" }, StringSplitOptions.None); for (int i = 0; i < 30; i++)
            {
                Assert.IsTrue(WindowsSerial.sendDataToPort(ports[0], "\r\nAT\r\n", "OK"));
            }
        }
        [TestMethod]
        public void testConnection() {

            String[] ports = WindowsSerial.getComPorts().Split(new String[] { ";;;" }, StringSplitOptions.None);
            Assert.IsTrue(WindowsSerial.checkPortStatus(ports[0]));
            for (int i = 0; i < 30; i++)
            {
                System.Diagnostics.Debug.Write(i + ";");
                Assert.IsTrue(WindowsSerial.checkPortStatus(ports[0]));
            }
        }
        [TestMethod]
        public void testGetInformation() {


            String[] ports = WindowsSerial.getComPorts().Split(new String[] { ";;;" }, StringSplitOptions.None);
            String s = WindowsSerial.getPortInfoz(ports[1]);
            Console.Write(s);

        }
    }
}
