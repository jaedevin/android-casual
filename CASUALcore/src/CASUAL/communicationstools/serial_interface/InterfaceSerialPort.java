/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package CASUAL.communicationstools.serial_interface;

/**
 *
 * @author adamoutler
 */
public interface InterfaceSerialPort extends com.sun.jna.Library {
    public String[] getComPorts();
    public boolean checkPortStatus(String port);
    public boolean sendDataToPort(String port, String data, String expectedValue);
    public byte[] sendBinaryData(String port,  byte[] data, byte[] expectedValue);
    public String sendData(String port, String data);
}
