using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.IO.Ports;  

namespace WindowsSerialJavaInterface
{

    interface InterfaceSerialPort
    {
        String[] getComPorts();
        Boolean checkPortStatus(String port);
        Boolean sendDataToPort(String port, String data, String expectedValue);
        String sendData(String port, String data);

    }
    public class WindowsSerial
    {
        static SerialPort serialPort;
        static int TIMEOUT = 500;
        static String dataReceived = "";
        static String portname = "";
        static AutoResetEvent readerWait=new AutoResetEvent(false);

        /**
         * returns available com ports
         */
        [RGiesecke.DllExport.DllExport]
        public static String getComPorts()
        {
            portname = "";
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            List<String> l = new List<String>();
            // Display each port name to the System.Diagnostics.Debug. 
            foreach (string port in ports)
            {
                System.Diagnostics.Debug.Write(port);
                if (validatePort(port))
                {
                    l.Add(port);
                }
            }

            String returnvalue = "";
            foreach (String s in l.ToArray())
            {
                returnvalue = returnvalue + s + ";;;";
            }
            return returnvalue;
        }

        /**
         *  checks the status of the com port
         */
        [RGiesecke.DllExport.DllExport("checkPortStatus", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static bool checkPortStatus(string port)
        {
            return sendDataToPort(port, "AT\n", "OK");
        }

        /**
         * returns true if expected value is found in call
         */
        [RGiesecke.DllExport.DllExport("sendDataToPort", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static bool sendDataToPort(string port, string data, string expectedValue)
        {

            setUpPortIfNeeded(port);
            serialPort.Open();
            Thread t = new Thread(() => DataReceivedHandler(serialPort, expectedValue));
            t.Start();
            readerWait.WaitOne();
            Console.Write("w");
            foreach (char c in data.ToCharArray())
            {
                Console.Write(" 0x" + (int)c);
            }
            serialPort.WriteLine(data);
            Console.WriteLine("W");
            t.Join();
            return dataReceived.Contains(expectedValue);
        }













        [RGiesecke.DllExport.DllExport]
        public static string getPortInfo(String comPort)
        {
            return "";
        }


        /**
         * Sends data returns a string. 
         */
        [RGiesecke.DllExport.DllExport]
        public static String sendData(string port, string data)
        {

            setUpPortIfNeeded(port);
            serialPort.Open();

            Thread t = new Thread(() => DataReceivedHandler(serialPort, "ADAM OUTLER - will never appear in data"));
            t.Start();
            readerWait.WaitOne();
            
            data = data.Replace("\n", serialPort.NewLine);
            serialPort.WriteLine(data);
            t.Join();
            //serialPort.Close();
            System.Diagnostics.Debug.WriteLine(dataReceived);
            return dataReceived;
        }


        static void setUpPortIfNeeded(String portName)
        {
            dataReceived = "";

            if (portname.Equals("") || !portName.Equals(portname))
            {
                serialPort = new System.IO.Ports.SerialPort(portName);
                getSerialProperties(serialPort);
                WindowsSerial.portname = portName;
            }
        }


        public static String getPortInfoz(String port){
           
            String result="";
             using (var searcher = new ManagementObjectSearcher("SELECT * FROM WIN32_SerialPort"))
        {
            string[] portnames = SerialPort.GetPortNames();
            var ports = searcher.Get().Cast<ManagementBaseObject>().ToList();
            var tList = (from n in portnames join p in ports on n equals p["DeviceID"].ToString() select n + " - " + p["Caption"]).ToList();

            foreach (string s in tList)
            {
                result = result + s;
            }
        
                return result;
            }

            // pause program execution to review results...
            Console.WriteLine("Press enter to exit");
            Console.ReadLine();
        }

        static Boolean validatePort(String port)
        {
            var query = string.Format("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%{0}%' AND Manufacturer LIKE 'FTDI' ", port);
            using (var searcher = new ManagementObjectSearcher(query))
            {
                ManagementObjectCollection objectCollection = searcher.Get();
                foreach (ManagementBaseObject managementBaseObject in objectCollection)
                {
                    if (managementBaseObject.ToString().Contains("FTDIBUS"))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        static void getSerialProperties(SerialPort sp)
        {
            serialPort.ReadTimeout = TIMEOUT;
            serialPort.WriteTimeout = TIMEOUT;
            System.Diagnostics.Debug.WriteLine("baud" + serialPort.BaudRate);

            //9600,n,8,1
            Microsoft.Win32.RegistryKey myKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Ports");
            String[] portarray = myKey.GetValue(sp.PortName + ":").ToString().Split(',');
            //sp.BaudRate = int.Parse(portarray[0]);
            sp.BaudRate = 115200;
            //sp.DataBits = int.Parse(portarray[2].ToUpperInvariant());
            sp.DataBits = 8;
            //sp.StopBits = (StopBits)Enum.Parse(typeof(StopBits), portarray[3], true);
            sp.StopBits = StopBits.One;

            sp.RtsEnable = false;
           

            Parity p;
            p = Parity.None;
            /*
            switch (portarray[2])
            {
                case "e":
                    p = Parity.Even;
                    break;
                case "m":
                    p = Parity.Mark;
                    break;
                case "n":
                    p = Parity.None;
                    break;
                case "o":
                    p = Parity.Odd;
                    break;
                case "s":
                    p = Parity.Space;
                    break;
                default:
                    p = Parity.None;
                    break;
            }*/
            sp.Parity = p;
            System.Diagnostics.Debug.WriteLine(serialPort.BaudRate + "," + sp.DataBits + sp.Parity + sp.StopBits);

        }


        /**
         *gets the UID and VID of the COM port from the UIDVID string 
         * 
         * 
         */
        static String[] getUIDVID(String szDeviceID)
        {
            String[] retval = new String[2];
            // FTDIBUS\VID_0403+PID_0000+A9GZV9T9A\0000
            string[] aszToken = szDeviceID.Split(new char[] { '\\' });
            int nTemp = szDeviceID.IndexOf(@"VID_");
            nTemp += 4;
            retval[0] = szDeviceID.Substring(nTemp, 4);
            nTemp += 4;
            nTemp = szDeviceID.IndexOf(@"PID_", nTemp);
            nTemp += 4;
            retval[1] = szDeviceID.Substring(nTemp, 4);
            return retval;

        }

        static Boolean continueReading = true;
        private static void DataReceivedHandler(SerialPort sp, String expected)
        {
            continueReading = true;
            Console.Write("r");
            readerWait.Set();
            while (continueReading)
            {
                try
                {
                    String s = sp.ReadLine();
                    dataReceived = dataReceived + s + "\n";
                    if (dataReceived.Contains(expected))
                    {
                        continueReading = false;
                    }
                }
                catch (TimeoutException ex)
                {
                    System.Diagnostics.Debug.WriteLine("Timeout");
                    try
                    {
                        dataReceived = dataReceived + sp.ReadLine();
                        if (dataReceived.Contains(expected))
                        {
                            continueReading = false;
                        }
                    }
                    catch (TimeoutException exagain)
                    {
                        System.Diagnostics.Debug.WriteLine("Timeout");

                        continueReading = false;
                    }
                }


                if (!sp.BaseStream.CanRead)
                {
                    continueReading = false;
                }
            }
            Console.Write("R");
            sp.Close();
            foreach (char c in dataReceived.ToCharArray())
            {
               Console.Write(" 0x"+(int)c);
            }
        }

    }
}