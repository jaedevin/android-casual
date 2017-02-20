using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Runtime.InteropServices;
using System.IO.Ports;
using System.Management;
using System.IO;

namespace CASUALSerialCommunications

{
    [ComVisible(true)]
    [StructLayout(LayoutKind.Explicit, CharSet = CharSet.Ansi, Size = 96, Pack = 1)]
    static public class SerialCommunications
    {

        public static String[] splitString = { "-=-=-=-=-=-=-=-=-" };
        static SerialPort serialPort;
        static int TIMEOUT = 750;
        static String dataReceived = "";
        static List<byte> dataReceivedBytes = new List<byte>();
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
         * returns true if expected value is found in call
         */
        [RGiesecke.DllExport.DllExport("sendDataToPort", CallingConvention = CallingConvention.Cdecl)]
   
        public static String sendDataToPort(String input)
        {
            String[]dataExpected= input.Split(splitString,StringSplitOptions.None);
            String port = dataExpected[0];
            String data;
            String expectedValue = dataExpected[2];
            try
            {
                data = dataExpected[1].Replace("\n", serialPort.NewLine);
            }catch (Exception ex)
            {
                data = dataExpected[1];
                Console.WriteLine("got exception " + ex + "in send data to port");
            }
            Console.WriteLine("data:"+data);

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
            String retval= dataReceived.Contains(expectedValue) ? "true" : "false";
            return retval;
        }

        [RGiesecke.DllExport.DllExport]
        public static string sendData(String input) { 

        String[] portData = input.Split(splitString, StringSplitOptions.None);
        String port = portData[0];
        String data = portData[1];
        return sendData(port, data);
    }


    /**
     * Sends data returns a string. 
     */

        public static String sendData(String port, String data)
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
            Console.WriteLine(dataReceived);
            return dataReceived;
        }




        /**
         * Sends bytes returns bytes. 
         */
        [RGiesecke.DllExport.DllExport]
        public static String sendBinData(String data)
        {
            Console.WriteLine("got data: " + data);
            String[] dataExpected = data.Split(splitString, StringSplitOptions.None);
            String port = dataExpected[0];
            Console.WriteLine("port" + port);

            byte[] returnval=sendBinData(port, readFileToByteArray(dataExpected[1]), readFileToByteArray(dataExpected[2]));
            String tempfile = Path.GetTempFileName();
            bool written=writeByteArrayToFile(tempfile, returnval);
            return (written ? tempfile : null);
        }


        private static byte[] sendBinData(String port, byte[] data, byte[] expected)
        {

            dataReceivedBytes = new List<byte>(); 

            setUpPortIfNeeded(port);
            serialPort.Open();

            Thread t = new Thread(() => DataByteHandler(serialPort, expected));
            t.Start();
            readerWait.WaitOne();
            serialPort.Write(data, 0, data.Length);
            t.Join();
            //serialPort.Close();
            Console.WriteLine(dataReceived);
            String tempfile = Path.GetTempFileName();
            return  dataReceivedBytes.ToArray<byte>();
            
        }


        public static bool writeByteArrayToFile(String FileName, byte[] ByteArray)
        {
            try
            {
                System.IO.FileStream FileStream = new System.IO.FileStream(FileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
                FileStream.Write(ByteArray, 0, ByteArray.Length);
                FileStream.Close();
                return true;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception.ToString());
            }
            return false;
        }


        public static byte[] readFileToByteArray(String filename)
        {
            return File.ReadAllBytes(filename);

        }



        static void setUpPortIfNeeded(String portName)
        {

            dataReceived = "";

            if (!string.IsNullOrEmpty(portName) )
            {
                //  portName = portName.Replace(":", "");
                serialPort = new System.IO.Ports.SerialPort(portName.Replace(":", ""));
                getSerialProperties(serialPort);
                SerialCommunications.portname = portName;
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
                   
                        return (string.Format("{0} - {1}\n {2} - {3}\n {4}", comPort.Name, comPort.Description, comPort.Manufacturer, comPort.PNPClass, comPort.DeviceID));
                }
            }

            return searchForPortInfo(port);


        }



        static string searchForPortInfo(String port)
        {
            String query = "SELECT * FROM Win32_PnPEntity WHERE Name LIKE ' " + port.Replace(":", "") + "'";
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
            Console.WriteLine("baud" + serialPort.BaudRate);

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
            Console.WriteLine(serialPort.BaudRate + "," + sp.DataBits + sp.Parity + sp.StopBits);

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
                    if (ex != null) Console.WriteLine("Timeout");
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
                        if (exagain != null) Console.WriteLine("Fatal timeout 1000ms without response");

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
                    if (checkPatternInArray(list.ToArray<byte>(), expected))
                    {
                        continueReading = false;
                    }
                }
                catch (TimeoutException ex)
                {
                    if (ex != null) Console.WriteLine("Timeout");
                    try
                    {
                        int input = sp.ReadByte();
                        list.Add(Convert.ToByte(input));
                        if (checkPatternInArray(list.ToArray<byte>(), expected))
                        {
                            continueReading = false;
                        }
                    }
                    catch (TimeoutException exagain)
                    {
                        if (exagain != null) Console.WriteLine("Timeout");
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
            foreach (byte b in list)
            {
                dataReceivedBytes.Add(b);

            }


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
    public string DeviceID { get; set; }
    public string Manufacturer { get; set; }
    public string PNPClass { get; set; }
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
                            object devid = obj["DeviceID"];
                            object mfg = obj["Manufacturer"];
                            object pnpclass= obj["PNPClass"]; 


                            comPortInfo.Name = caption.Substring(caption.LastIndexOf("(COM")).Replace("(", string.Empty).Replace(")", string.Empty);
                            comPortInfo.Description = caption;
                            comPortInfo.DeviceID = devid.ToString();
                            comPortInfo.Manufacturer = mfg.ToString();
                            comPortInfo.PNPClass = pnpclass.ToString();
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
