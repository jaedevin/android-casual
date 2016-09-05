/**
*
* To build this properly use the package manager console
* PM> Install-Package UnmanagedExports
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Management;
using System.Threading;
using System.Runtime.InteropServices;
using System.Linq;


[assembly: CLSCompliant(true)]
namespace WindowsSerialJavaInterface
{

    [ComVisible(true)]
    interface InterfaceSerialPort
    {
        string getComports();
        string getPortInfo(String port);
        bool checkPortStatus(String port);
        Boolean sendDataToPort(String port, String data, String expectedValue);
        string sendData(String port, String data);
        byte?[] sendBinData(String port, byte?[] data);
    }

    [ComVisible(true)]
    static public class WindowsSerialCSharp
    {

        
        static SerialPort serialPort;
        static int TIMEOUT = 750;
        static String dataReceived = "";
        static List<int> dataReceivedBytes = new List<int>();
        static String portname = "";
        static AutoResetEvent readerWait = new AutoResetEvent(false);


        /**
         * returns available com ports
         */
        [RGiesecke.DllExport.DllExport("getComports", CallingConvention = CallingConvention.Cdecl)]
        public static string getComports()
        {
            string[] ports = SerialPort.GetPortNames();
            String returnvalue = "";
            foreach (String s in ports)
            {
                if (s.Length > 0)
                {
                    returnvalue = returnvalue + s.Replace("\0", "") + ":;;;";
                }
            }
            return returnvalue;
        }




        /**
         *  checks the status of the com port
         */
        [ComVisibleAttribute(true)]
        [RGiesecke.DllExport.DllExport("checkPortStatus", CallingConvention = CallingConvention.Cdecl)]
        [return: MarshalAs(UnmanagedType.HString)]
        public static bool checkPortStatus(String port)
        {
            return sendDataToPort(port, "AT\n", "OK");
        }

        /**
         * returns true if expected value is found in call
         */
        [RGiesecke.DllExport.DllExport("sendDataToPort", CallingConvention = CallingConvention.Cdecl)]
        
        public static bool sendDataToPort(string port, string data, string expectedValue)
        {
            setUpPortIfNeeded(port);
            serialPort.Open();
            char[] chars = data.ToCharArray();
            Thread t = new Thread(() => DataReceivedHandler(serialPort, expectedValue));
            t.Start();
            readerWait.WaitOne();
            Console.Write("WriteStart");
            foreach (char c in chars)
            {
                Console.Write(" 0x" + (int)c);
            }
            serialPort.WriteLine(data);
            Console.WriteLine("WriteStop");
            t.Join();
            return dataReceived.Contains(expectedValue);
        }





        /**
         * Sends data returns a string. 
         */
        [RGiesecke.DllExport.DllExport("sendData", CallingConvention = CallingConvention.Cdecl)]
        public static string sendData(String port, String data)
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
        [RGiesecke.DllExport.DllExport("sendBinData", CallingConvention = CallingConvention.Cdecl)]
        public static int[] sendBinData(String port, byte[] data, byte[] expected)
        {

            
            setUpPortIfNeeded(port);
            serialPort.Open();

            Thread t = new Thread(() => DataByteHandler(serialPort, expected));
            t.Start();
            readerWait.WaitOne();
            serialPort.Write(data, 0, data.Length);
            t.Join();
            //serialPort.Close();
            System.Diagnostics.Debug.WriteLine(dataReceived);
            return dataReceivedBytes.ToArray<int>();
        }




        static void setUpPortIfNeeded(String portName)
        {

            dataReceived = "";

            if ( string.IsNullOrEmpty(portname) || !portName.Equals(portname) )
            {
                //  portName = portName.Replace(":", "");
                serialPort = new System.IO.Ports.SerialPort(portName.Replace(":", ""));
                getSerialProperties(serialPort);
                WindowsSerialCSharp.portname = portName;
            }
        }


        [RGiesecke.DllExport.DllExport("getPortInfo", CallingConvention = CallingConvention.Cdecl)]
        public static String getPortInfo(String port)
        {

            port = port.Replace(":", "");
            foreach (ComportInfo comPort in ComportInfo.GetCOMPortsInfo())
            {

                if (comPort.Name.Equals(port))
                {
                    return (string.Format("{0} – {1}", comPort.Name, comPort.Description + validatePort(port)));
                }
            }

            return validatePort(port);


        }



        static string validatePort(String port)
        {
            String query = "SELECT * FROM Win32_PnPEntity WHERE Name LIKE ' "+ port.Replace(":","")+"'";
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
//          Microsoft.Win32.RegistryKey myKey = Microsoft.Win32.Registry.LocalMachine.OpenSubKey("SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion\\Ports");


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
            Console.Write("ReadStart");
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
                catch (TimeoutException ex) //we waited 500ms with no response
                {
                    if (ex != null) System.Diagnostics.Debug.WriteLine("Timeout");
                    try
                    {
                        dataReceived = dataReceived + sp.ReadLine();
                        if (dataReceived.Contains(expected))
                        {
                            continueReading = false;
                        }
                    }
                    catch (TimeoutException exagain) //we waited 1000ms with no response, we're done!
                    {
                        if (exagain != null) System.Diagnostics.Debug.WriteLine("Fatal timeout 1000ms without response");

                        continueReading = false;
                    }
                }


                if (!sp.BaseStream.CanRead)
                {
                    continueReading = false;
                }
            }
            Console.Write("ReadStop");
            sp.Close();

            Console.WriteLine(dataReceived);
        }



        private static bool checkPatternInArray(byte[] array, byte[] pattern)
        {
            int fidx = 0;
            int result = Array.FindIndex(array, 0, array.Length, (byte b) =>
            {
                fidx = (b == pattern[fidx]) ? fidx + 1 : 0;
                return (fidx == pattern.Length);
            });
            return (result >= pattern.Length - 1);
        }


        private static void DataByteHandler(SerialPort sp, byte[] expected)
        {
            continueReading = true;
            Console.Write("r");
            readerWait.Set();
            List<byte> list = new List<byte>();
            while (continueReading)
            {
                try
                {
                    list.Add(Convert.ToByte(sp.ReadByte()));
                    if (checkPatternInArray(list.ToArray<byte>(),expected))
                    {
                        continueReading = false;
                    }
                }
                catch (TimeoutException ex)
                {
                    if (ex != null) System.Diagnostics.Debug.WriteLine("Timeout");
                    try
                    {
                        list.Add(Convert.ToByte(sp.ReadByte()));
                        if (checkPatternInArray(list.ToArray<byte>(), expected))
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
            Console.Write(dataReceived);
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

    public class ComportInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ComportInfo() { }

        public static List<ComportInfo> GetCOMPortsInfo()
        {
            List<ComportInfo> ComPortInfoList = new List<ComportInfo>();

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
                            ComportInfo comPortInfo = new ComportInfo();
                                comPortInfo.Name = caption.Substring(caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);
                                comPortInfo.Description = caption;
                                ComPortInfoList.Add(comPortInfo);
                            }
                            if (caption.ToLower().Contains("modem"))
                            {
                                //here we can find another way to get information. 
                            }
                        }
                    }
                }
            }
            return ComPortInfoList;
        }
    }


