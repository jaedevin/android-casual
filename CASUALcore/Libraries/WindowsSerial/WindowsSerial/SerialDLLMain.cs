/**
*
* To build this properly use the package manager console
* PM> Install-Package UnmanagedExports
*/

using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Text.RegularExpressions;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq;

namespace WindowsSerialJavaInterface
{

    interface InterfaceSerialPort
    {
        String getComPorts();
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
        static AutoResetEvent readerWait = new AutoResetEvent(false);

        /**
         * returns available com ports
         */
        [RGiesecke.DllExport.DllExport]
        public static String getComPorts()
        {
            portname = "";
            string[] ports = System.IO.Ports.SerialPort.GetPortNames();
            List<String> l = new List<String>();


            String returnvalue = "";
            foreach (String s in ports)
            {
                if (s.Length > 0) { 
                    returnvalue = returnvalue + s.Replace("\0", "") + ":;;;";
                }
            }
            return returnvalue;
        }

        /**
         *  checks the status of the com port
         */
        [RGiesecke.DllExport.DllExport("checkPortStatus", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static bool checkPortStatus(String port)
        {
            return sendDataToPort(port, "AT\n", "OK");
        }

        /**
         * returns true if expected value is found in call
         */

        //[RGiesecke.DllExport.DllExport("sendDataToPort", CallingConvention = CallingConvention.Cdecl)]
       // [return: MarshalAs(UnmanagedType.Bool)]

        public static bool sendDataToPort(string port, string data, string expectedValue)
        {
            setUpPortIfNeeded(port);
            serialPort.Open();

            Console.WriteLine("test");

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
            return getPortInfoz(comPort);
        }


        /**
         * Sends data returns a string. 
         */
        [RGiesecke.DllExport.DllExport]
        public static String sendData(string port, string data)
        {

            setUpPortIfNeeded(port);
            serialPort.Open();

            Thread t = new Thread(() => DataReceivedHandler(serialPort, "ADAM OUTLER - will never appear in data so it is a good string to use to not expect anything"));
            t.Start();
            readerWait.WaitOne();

            data = data.Replace("\n", serialPort.NewLine);
            serialPort.WriteLine(data);
            t.Join();
            //serialPort.Close();
            System.Diagnostics.Debug.WriteLine(dataReceived);
            return dataReceived;
        }    
        /**
         * Sends bytes returns bytes. 
         */
        [RGiesecke.DllExport.DllExport]
        public static char[] sendData(string port, byte[] data)
        {

            setUpPortIfNeeded(port);
            serialPort.Open();

            Thread t = new Thread(() => DataReceivedHandler(serialPort, "ADAM OUTLER - will never appear in data so it is a good string to use to not expect anything"));
            t.Start();
            readerWait.WaitOne();
            serialPort.Write(data,0,data.Length);
            t.Join();
            //serialPort.Close();
            System.Diagnostics.Debug.WriteLine(dataReceived);
            return dataReceived.ToCharArray();
        }


        static void setUpPortIfNeeded(String portName)
        {
            
            dataReceived = "";
            
            if (portname.Equals("") || !portName.Equals(portname))
            {
              //  portName = portName.Replace(":", "");
                serialPort = new System.IO.Ports.SerialPort(portName.Replace(":",""));
                getSerialProperties(serialPort);
                WindowsSerial.portname = portName;
            }
        }


        public static String getPortInfoz(String port)
        {

            port = port.Replace(":", "");
            foreach (COMPortInfo comPort in COMPortInfo.GetCOMPortsInfo())
            {
        
                if (comPort.Name.Equals(port))
                {
                    return (string.Format("{0} – {1}", comPort.Name, comPort.Description + validatePort(port)));
                }
            }
            
            return validatePort(port) ;


        }
      


        static string validatePort(String port)
        {
            var query = string.Format("SELECT * FROM Win32_PnPEntity WHERE Name LIKE '%{0}%' ", port);
            using (var searcher = new ManagementObjectSearcher(query))
            {
                ManagementObjectCollection objectCollection = searcher.Get();
                foreach (ManagementBaseObject managementBaseObject in objectCollection)
                {
                    return managementBaseObject.ToString();
                }
            }
            return "No PNP data available";
        }

        static void getSerialProperties(SerialPort sp)
        {
            serialPort.ReadTimeout = TIMEOUT;
            serialPort.WriteTimeout = TIMEOUT;
            System.Diagnostics.Debug.WriteLine("baud" + serialPort.BaudRate);

            //9600,n,8,1
            Microsoft.Win32.RegistryKey myKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Ports");

           
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
                    if (ex !=null) System.Diagnostics.Debug.WriteLine("Timeout");
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
                        if (exagain != null) System.Diagnostics.Debug.WriteLine("Timeout");

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
                Console.Write(" 0x" + (int)c);
            }
        }

    }



    /**
    * Process Connection Code from https://dariosantarelli.wordpress.com/2010/10/18/c-how-to-programmatically-find-a-com-port-by-friendly-name/
    */
    internal class ProcessConnection
    {

        public static ConnectionOptions ProcessConnectionOptions()
        {
            ConnectionOptions options = new ConnectionOptions();
            options.Impersonation = ImpersonationLevel.Impersonate;
            options.Authentication = AuthenticationLevel.Default;
            options.EnablePrivileges = true;
            return options;
        }

        public static ManagementScope ConnectionScope(string machineName, ConnectionOptions options, string path)
        {
            ManagementScope connectScope = new ManagementScope();
            connectScope.Path = new ManagementPath(@"\\" + machineName + path);
            connectScope.Options = options;
            connectScope.Connect();
            return connectScope;
        }
    }

    public class COMPortInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public COMPortInfo() { }

        public static List<COMPortInfo> GetCOMPortsInfo()
        {
            List<COMPortInfo> comPortInfoList = new List<COMPortInfo>();

            ConnectionOptions options = ProcessConnection.ProcessConnectionOptions();
            ManagementScope connectionScope = ProcessConnection.ConnectionScope(Environment.MachineName, options, @"\root\CIMV2");

            ObjectQuery objectQuery = new ObjectQuery("SELECT * FROM Win32_PnPEntity WHERE ConfigManagerErrorCode = 0");
            ManagementObjectSearcher comPortSearcher = new ManagementObjectSearcher(connectionScope, objectQuery);

            using (comPortSearcher)
            {
                string caption = null;
                foreach (ManagementObject obj in comPortSearcher.Get())
                {
                    if (obj != null)
                    {
                        object captionObj = obj["Caption"];
                        if (captionObj != null)
                        {
                            caption = captionObj.ToString();
                            if (caption.Contains("(COM"))
                            {
                                COMPortInfo comPortInfo = new COMPortInfo();
                                comPortInfo.Name = caption.Substring(caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")",string.Empty);
                                comPortInfo.Description = caption;
                                comPortInfoList.Add(comPortInfo);
                            }
                            if (caption.ToLower().Contains("modem"))
                            {
                                //here we can find another way to get information. 
                            }
                        }
                    }
                }
            }
            return comPortInfoList;
        }
    }

}
