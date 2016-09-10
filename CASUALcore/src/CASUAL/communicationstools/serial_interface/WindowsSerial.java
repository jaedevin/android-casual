/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package CASUAL.communicationstools.serial_interface;

import com.sun.jna.Native;
import CASUAL.Log;
import CASUAL.OSTools;
import CASUAL.CASUALSessionData;
import CASUAL.FileOperations;
import java.io.File;
import java.io.IOException;

/**
 *
 * @author adamoutler
 */
public class WindowsSerial implements InterfaceSerialPort {

    public interface InterfaceWindowsSerialPort extends com.sun.jna.Library {

        String getComports();

        String getPortInfo(String port);

        String sendDataToPort(String data);  //passed on checkPortStatus   Failed to assert with \r\r\at\r\r\r

        String sendData(String data);

        String sendBinData(String data);  //failed on sendBinData  Failed on main

    }

    InterfaceWindowsSerialPort windows;
    static String[] splitString = {"-=-=-=-=-=-=-=-=-"};

    public static void main(String[] args) {
        WindowsSerial ws = new WindowsSerial();

        String[] ports = ws.getComPorts();
        /* for (String port: ports){
            System.out.println("java port:"+port);
            System.out.println("port info:"+ws.getPortInfo(port));
        }
      
        System.out.println("received data:"+ws.sendData(ports[1],"AT\r"));
        System.out.println("received data:"+ws.sendDataToPort(ports[1],"AT\r","OK"));*/
        ws.checkPortStatus(ports[1]);

        byte[] bytes = ws.sendBinaryData(ports[2], new byte[]{0x7e, 0x0a, 0x63, 0x74, 0x7e}, new byte[]{0x7e});
        StringBuilder sb = new StringBuilder();
        for (byte b : bytes) {
            sb.append((char) b);
        }
    }

    @Override
    public String[] getComPorts() {
        String[] returnValue = windows.getComports().split(";;;");
        return returnValue;
    }

    public WindowsSerial() {
        Log.level4Debug("Loading WindowsSerial DLL");
        File f = new File(CASUALSessionData.getWindowsDLL());
        if (OSTools.isWindows()) {
            if (OSTools.isWindows64Arch()) {
                windows = (InterfaceWindowsSerialPort) Native.loadLibrary("CASUAL/communicationstools/serial_interface/resources/CASUALCommunicationsDLL64.dll", InterfaceWindowsSerialPort.class);

            } else {
                windows = (InterfaceWindowsSerialPort) Native.loadLibrary("/CASUAL/communicationstools/serial_interface/resources/CASUALCommunicationsDLL86.dll", InterfaceWindowsSerialPort.class);
            }
        }
        Log.level4Debug("loaded module");
    }

    public String getPortInfo(String port) {
        return windows.getPortInfo(port);
    }

    public byte[] sendBinaryData(String port, byte[] data, byte[] expectation) {
        try {
            FileOperations fo = new FileOperations();
            File dat = File.createTempFile("sendBinaryDataData", "");
            File exp = File.createTempFile("sendBinaryDataExpected", "");
            dat.deleteOnExit();
            exp.deleteOnExit();
            String dataFile = dat.getAbsolutePath();
            String expectedFile = exp.getAbsolutePath();
            fo.writeBytesToFile(dataFile, data);
            fo.writeBytesToFile(expectedFile, expectation);

            String returnfile = windows.sendBinData(port + splitString[0] + dataFile + splitString[0] + expectedFile);
            return fo.readBytesFromFile(returnfile);

        } catch (IOException ex) {
            Log.errorHandler(ex);
            return null;
        }

    }

    @Override
    public boolean checkPortStatus(String port) {
        return this.sendDataToPort(port, "AT\r\nAT\nAT\r\n", "OK");

    }

    @Override
    public boolean sendDataToPort(String port, String data, String expectedValue) {
        String s = windows.sendDataToPort(port + splitString[0] + "\r\nAT\r\nAT\r\n" + splitString[0] + expectedValue);
        final boolean retval = windows.sendDataToPort(port + splitString[0] + data + splitString[0] + expectedValue).contains("true");
        return retval;
    }

    @Override
    public String sendData(String port, String data) {
        return windows.sendData(port + splitString[0] + data);
    }

}
