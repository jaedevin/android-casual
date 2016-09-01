/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package CASUAL.communicationstools.serial_interface.posix;

import CASUAL.communicationstools.serial_interface.SerialInterface;
import java.io.BufferedInputStream;
import java.io.File;
import java.io.FileInputStream;
import java.io.FileNotFoundException;
import java.io.FileOutputStream;
import java.io.IOException;
import java.util.concurrent.atomic.AtomicBoolean;
import java.util.logging.Level;
import java.util.logging.Logger;

/**
 *
 * @author adamoutler
 */
public class PosixSerialConnectivity {

    private static final AtomicBoolean stopRead = new AtomicBoolean(false);
    private static final Object readerWait = new Object();
    private static String returnValue = "";
    final static String ack = "OK";
    final static int maxDuration = 5000;

    private static void resetReturnValue() {
        returnValue = "";
    }

    synchronized static private String writeData(String file, String dataToSend) throws FileNotFoundException, IOException, InterruptedException {
        File f = new File(file);
        BufferedInputStream bis=new BufferedInputStream(new FileInputStream(file));
        Thread t = initializeReader(bis);
        t.setName("reader Thread");
        FileOutputStream fos = new FileOutputStream(f);
       resetReturnValue();

        t.start();
        Thread.sleep(100);
        fos.write((dataToSend+"\n").getBytes());
        synchronized (readerWait) {
            readerWait.wait(maxDuration);
        }
        fos.close();
        stopRead.set(true);
        return returnValue;
    }

    synchronized static private Thread initializeReader(BufferedInputStream bis) {
        stopRead.set(false);
        Thread t = getReader(bis);
        t.setDaemon(true);
        t.setName("Reader Thread");

        return t;
    }

    synchronized static private Thread getReader(final BufferedInputStream bis) {
        return new Thread(() -> {
            try {
                while (!stopRead.get()) {
                    if (bis.available() > 0) {
                        synchronized (readerWait) {
                            returnValue = returnValue + (char) bis.read();
                            if (returnValue.contains(ack)) {
                                stopRead.set(true);
                                readerWait.notify();
                               
                            }
                        }
                    }

                }
                stopRead.set(false);
            } catch (IOException ex) {
                returnValue = "IOEXCEPTION!";
            } finally {
                try {
                   System.out.println(bis.available()+" chars left");
                   while (bis.available()>0){
                       System.out.print((char)bis.read());
                   }
                    bis.close();
                    System.out.println("received: " + returnValue);
                } catch (IOException ex) {
                    Logger.getLogger(PosixSerialConnectivity.class.getName()).log(Level.SEVERE, null, ex);
                }
            }

        });
    }

    private static void flushInput(BufferedInputStream bis) throws IOException{
        while (bis.available()>0){
            System.out.print("+");
            System.out.print((char)bis.read());
        }
    }
    
    public synchronized String sendCommandToSerial(String port, String data) throws IOException, FileNotFoundException, InterruptedException {
        return writeData(port, data);
    }

    public synchronized boolean sendCommandToSerial(String port, String data, String expectedValue) throws IOException, FileNotFoundException, InterruptedException {
        return sendCommandToSerial(port, data).contains(expectedValue);
    }

    public synchronized boolean verifyConnectivity(String port) throws IOException, FileNotFoundException, InterruptedException {
        String returnvalue = writeData(port, "AT\n");
        return returnvalue.contains("OK");
    }

    public static void main(String[] args) throws Exception {
 //       try {
            System.out.println("START");
            SerialInterface si = new SerialInterface();
            String com = si.getComPorts()[0];
            boolean status=si.checkPortStatus(com);
            System.out.println("status:" +status);
            if (!status){
                throw new Exception("could not verify connectivity");
            } else {
                System.out.println("passed");
            }
           // System.out.print("Status" + si.sendData(com, "AT+IMEITEST=1,0\n"));
         //   System.out.println("status " + si.checkPortStatus(com) + si.checkPortStatus(com));

       //     System.out.print(new PosixSerialConnectivity().verifyConnectivity(com));
     //   } catch (IOException ex) {
     //       Logger.getLogger(PosixSerialConnectivity.class.getName()).log(Level.SEVERE, null, ex);
  //    /  } catch (InterruptedException ex) {
            //Logger.getLogger(PosixSerialConnectivity.class.getName()).log(Level.SEVERE, null, ex);
      //  }
    }
}
