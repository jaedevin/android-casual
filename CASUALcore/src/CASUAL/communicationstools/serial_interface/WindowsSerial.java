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
import java.util.Arrays;

/**
 *
 * @author adamoutler
 */
public class WindowsSerial implements InterfaceSerialPort {
    
    
    
    public interface InterfaceWindowsSerialPort extends com.sun.jna.Library {
            
            String getComports();
            String getPortInfo(String port);
            boolean checkPortStatus(String port);
            Boolean sendDataToPort(String port, String data, String expectedValue);
            String sendData(String port, String data);
            byte[] sendBinData(String port, byte[] data);
            
   }
    
    InterfaceWindowsSerialPort windows;

    

    

    public static void main(String[] args) {
        WindowsSerial ws=new WindowsSerial();
        
        String[] ports=ws.getComPorts();
        for (String port: ports){
            System.out.println("java port:"+port);
            System.out.println("port info:"+ws.getPortInfo(port));
        }
      
        System.out.println("received data:"+ws.sendData(ports[1],"AT\r"));
        System.out.println("received data:"+ws.sendDataToPort(ports[1],"AT\r","OK"));
          ws.checkPortStatus(ports[1]);
         System.out.println("received data:"+Arrays.toString(ws.sendBinData(ports[2],new byte[]{ 0x7e ,0x0a ,0x63 ,0x74 ,0x7e })));
    }

    @Override
    public String[] getComPorts() {
        String[] returnValue=windows.getComports().split(";;;");
        return returnValue;
    }

    
    
    public WindowsSerial() {
       Log.level4Debug("Loading WindowsSerial DLL");
        File f = new File(CASUALSessionData.getWindowsDLL());
        if (OSTools.isWindows()){
            if (OSTools.isWindows64Arch()){
                windows = (InterfaceWindowsSerialPort) Native.loadLibrary("CASUAL/communicationstools/serial_interface/resources/WindowsSerial32.dll", InterfaceWindowsSerialPort.class);
  
            } else {
                windows = (InterfaceWindowsSerialPort) Native.loadLibrary("/CASUAL/communicationstools/serial_interface/resources/WindowsSerial32.dll", InterfaceWindowsSerialPort.class);
            }
        }
    }
    
    public String getPortInfo(String port){
        return windows.getPortInfo(port);
    }
    
    public byte[] sendBinData(String port, byte[] data){
        return windows.sendBinData(port, data);
        
    }

    @Override
    public boolean checkPortStatus(String port) {
        return windows.checkPortStatus(port)==true;
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
