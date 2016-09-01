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
import java.io.File;

/**
 *
 * @author adamoutler
 */
public class WindowsSerial implements InterfaceSerialPort {
    
    public interface InterfaceWindowsSerialPort extends com.sun.jna.Library {
    public String getComPorts();
    public boolean checkPortStatus(String port);
    public boolean sendDataToPort(String port, String data, String expectedValue);
    public String sendData(String port, String data);
   }
    
    InterfaceWindowsSerialPort windows;


    

    

    public static void main(String[] args) {
        WindowsSerial ws=new WindowsSerial();
        
        String[] ports=ws.getComPorts();
        for (String port: ports){
            System.out.println("java port:"+port);
        }
        System.out.println("received data:"+ws.sendData(ports[0],"AT\n"));
    }

    @Override
    public String[] getComPorts() {
        String[] returnValue=windows.getComPorts().split(";;;");
        return returnValue;
    }

    
    
    public WindowsSerial() {
       Log.level4Debug("JNA Path:"+System.getProperty("jna.library.path"));
        File f = new File(CASUALSessionData.getWindowsDLL());
        if (OSTools.isWindows()){
            if (OSTools.isWindows64Arch()){
                windows = (InterfaceWindowsSerialPort) Native.loadLibrary("/com/imeidocbox/WindowsSerial64.dll", InterfaceWindowsSerialPort.class);
            } else {
                windows = (InterfaceWindowsSerialPort) Native.loadLibrary("/com/imeidocbox/WindowsSerial32.dll", InterfaceWindowsSerialPort.class);
            }
        }
       
        
    }

    @Override
    public boolean checkPortStatus(String port) {
        return windows.checkPortStatus(port);
    }

    @Override
    public boolean sendDataToPort(String port, String data, String expectedValue) {
        return windows.sendDataToPort(port, data, expectedValue);
    }

    @Override
    public String sendData(String port, String data) {
        return windows.sendData(port, data);
    }

}
