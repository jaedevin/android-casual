/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package CASUAL.communicationstools.serial_interface;

import CASUAL.CASUALSessionData;
import CASUAL.language.Command;
import CASUAL.Log;

import CASUAL.OSTools;
import javafx.application.Platform;
import javafx.scene.control.ProgressBar;

/**
 *
 * @author adamoutler
 */
public class SerialInterface implements InterfaceSerialPort {

    final static int MAXREPEATS = 10;
    final InterfaceSerialPort selectedInterface;

    public SerialInterface() {
        if (OSTools.isLinux()) {
            selectedInterface = SYSTEM.LINUX.getInterface();
        } else if (OSTools.isWindows()) {
            selectedInterface = SYSTEM.WINDOWS.getInterface();
        } else {
            selectedInterface = new LinuxSerial();
        }
    }

    private enum SYSTEM {

        LINUX(new LinuxSerial()),
        LINUX64(new LinuxSerial()),
        MAC(new LinuxSerial()),
        MAC64(new LinuxSerial()),
        WINDOWS(new WindowsSerial()),
        WINDOWS64(new WindowsSerial());

        private final InterfaceSerialPort iface;

        SYSTEM(InterfaceSerialPort c) {
            iface = c;

        }

        InterfaceSerialPort getInterface() {
            return iface;
        }

    };

    public InterfaceSerialPort getInterface() {
        return selectedInterface;
    }

    @Override
    public String[] getComPorts() {
        return selectedInterface.getComPorts();
    }

    @Override
    public boolean checkPortStatus(String port) {
        return selectedInterface.checkPortStatus(port);
    }

    @Override
    public boolean sendDataToPort(String port, String data, String expectedValue) {
        return selectedInterface.sendDataToPort(port, data, expectedValue);
    }

    @Override
    public String sendData(String port, String data) {
        return selectedInterface.sendData(port, data);
    }
    final class StatusOject{
       boolean complete = true;
       final Object lock=new Object();
    }

    public void sendScriptByLineToSerialPort(final String port, final String data) {
        final StatusOject c=new StatusOject();
        Thread t;
        t = new Thread(new Runnable(){ 
             public void run(){
            Thread.currentThread().setName("Sending Script by line");
            String[] script = data.split("\n");
            float progressMax = script.length;
            float progress = 0;
            Log.level2Information("Starting");

            for (String line : script) {
                if (!line.contains(":::")) continue;
                String[] sendexpect = line.split(":::");
                if (sendexpect.length==3){
                    Log.LiveUpdate(sendexpect[2]);
                }
                if (!sendLineUntilMaxRepeatsExceeded(port, sendexpect[0], sendexpect[1])) {
                    Log.level2Information("Critical Error while processing information");
                    c.complete=false;
                    break;
                }
                Log.LiveUpdate("OK");
                progress++;
            }
            Log.level2Information("Job complete. All sequences closed.");
            synchronized (c.lock){
                c.lock.notifyAll();
            }
          }
        });
        t.start();
    }

    public String parseUARTCommand(String port, Command cmd){
        
        String line=cmd.get();
                String[] sendExpect = line.split(":::");

                    
                    
                   for (int i=0; i<MAXREPEATS;i++){
                        if (sendExpect.length==3){
                            if (i>0){
                                Log.progress("RETRY");
                            }
                            Log.progress(sendExpect[2]);
                        }
                        String s;
                       s=this.sendData( port, sendExpect[0]);
                       cmd.setReturn(false, s);

                       
                       if  ((sendExpect.length>1 && cmd.getReturn().contains(sendExpect[1])) ||sendExpect.length==1){
                           cmd.setReturn(true,cmd.getReturn());
                            break;
                       } else {
                           cmd.setReturn(false,cmd.getReturn());
                           break;
                       }
                   }
                   
                   
                    if (!cmd.getReturnPassedOrFailed()) {
                        Log.level2Information("Critical Error while processing information");
                        cmd.set("$ERROR!!!! COULD NOT COMPLETE"+cmd);
                        cmd.setReturn(false, "");
                    Log.progress("FAIL");

                    }
                
                return cmd.getReturn();
    }
    
   

    private boolean sendLineUntilMaxRepeatsExceeded(String port, String data, String expected) {
        for (int i = 0; i < MAXREPEATS; i++) {
            if (sendDataToPort(port, data + "\n", expected)) {
                return true;
            }
            Log.level2Information("Retry");
        }
        return false;
    }

}
