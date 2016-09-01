/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package CASUAL.communicationstools.serial_interface;

import CASUAL.OSTools;
import org.junit.Before;
import org.junit.Test;

/**
 *
 * @author adamoutler
 */
public class SerialInterfaceIT {

    public SerialInterfaceIT() {
    }

    @Before
    public void setUp() {
    }

    /**
     * Test of getInterface method, of class SerialInterface.
     */
    @Test
    public void testGetInterface() {
        System.out.println("getInterface");
        SerialInterface instance = new SerialInterface();
        InterfaceSerialPort expResult = null;
        InterfaceSerialPort result = instance.getInterface();
        assert (expResult != result);
        String classname = result.getClass().toString();
        if (OSTools.isLinux()) {
            assert (classname.equals("class com.imeidocbox.serial_interface.LinuxSerial"));
        }
        if (OSTools.isWindows()) {
            assert (classname.equals("class com.imeidocbox.serial_interface.WindowsSerial"));
        }

    }

    /**
     * Test of getComPorts method, of class SerialInterface.
     */
    @Test
    public void testGetComPorts() {
        System.out.println("getComPorts");
        SerialInterface instance = new SerialInterface();
        String[] result = instance.getComPorts();
        assert (result.length > 0);
        if (OSTools.isLinux()) {
            assert (result[0].contains("tty"));
        }
        if (OSTools.isWindows()) {
            assert (result[0].contains("COM"));
        }
    }

    /**
     * Test of checkPortStatus method, of class SerialInterface.
     */
    @Test
    public void testCheckPortStatus() {
        System.out.println("checkPortStatus");
        SerialInterface instance = new SerialInterface();
        String port = instance.getComPorts()[0];
        boolean result = instance.checkPortStatus(port);
        assert (result);
        for (int i = 0; i < 500; i++) {
            result = instance.checkPortStatus(port);
            assert (result);
        }
        // TODO review the generated test code and remove the default call to fail.

    }

    /**
     * Test of sendDataToPort method, of class SerialInterface.
     */
    @Test
    public void testSendDataToPort() {
        System.out.println("sendDataToPort");
        SerialInterface instance = new SerialInterface();
        String port = instance.getComPorts()[0];
        String data = "AT\n";
        String expectedValue = "OK";

        boolean result = instance.sendDataToPort(port, data, expectedValue);
        assert (result);
        data = "atat";
        assert (!instance.sendDataToPort(port, data, expectedValue));
        data = "\nat\n";
        assert (!instance.sendDataToPort(port, data, expectedValue));
        // TODO review the generated test code and remove the default call to fail.
    }

    /**
     * Test of sendData method, of class SerialInterface.
     */
    @Test
    public void testSendData() {
        System.out.println("sendData");
        SerialInterface instance = new SerialInterface();
        String port = instance.getComPorts()[0];
        String data = "AT+IMEITEST=1,0\n";
        for (int i = 0; i < 400; i++) {
            String result = instance.sendData(port, data);
            System.out.println(result);
            assert (result.length() > 30);
        }
    }

}
