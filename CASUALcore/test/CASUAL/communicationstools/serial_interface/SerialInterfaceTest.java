/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */
package CASUAL.communicationstools.serial_interface;

import CASUAL.OSTools;
import CASUAL.language.Command;
import java.util.Arrays;
import org.junit.After;
import org.junit.AfterClass;
import static org.junit.Assume.assumeTrue;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;

/**
 *
 * @author adamoutler
 */
public class SerialInterfaceTest {

    final public static String MODEM = "COM6:";
    final public static String SERIAL = "COM5:";

    public SerialInterfaceTest() {
    }

    @BeforeClass
    public static void setUpClass() throws Exception {
    }

    @AfterClass
    public static void tearDownClass() throws Exception {
    }

    @Before
    public void setUp() {
        assumeTrue(!java.awt.GraphicsEnvironment.isHeadless());
    }

    @After
    public void tearDown() throws Exception {
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
            assert (classname.equals("class CASUAL.communicationstools.serial_interface.LinuxSerial"));
        }
        if (OSTools.isWindows()) {
            assert (classname.equals("class CASUAL.communicationstools.serial_interface.WindowsSerial"));
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
        boolean result = instance.checkPortStatus(MODEM);
        assert (result);
        for (int i = 0; i < 3; i++) {
            result = instance.checkPortStatus(MODEM);
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
        String port = MODEM;
        String data = "\rAT\r\r";
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
        String port = MODEM;
        String data = "\rAT\r";
        for (int i = 0; i < 3; i++) {
            String result = instance.sendData(MODEM, data);
            System.out.println(result);
            assert (result.length() > 2);
        }
    }

    /**
     * Test of sendBinaryData method, of class SerialInterface.
     */
    @Test
    public void testSendBinaryData() {
        System.out.println("sendBinaryData");
        String port = SERIAL; //this is "COM5:"
        byte[] data = SerialInterface.hexStringToByteArray("7e 00 78 f0 7e");
        byte[] expected = new byte[]{0x7e};
        byte[] result = new SerialInterface().sendBinaryData(port, data, expected);
        for (byte b : result) {
            System.out.print((char) b);
        }
        System.out.println();
        assert (result.length > 20);
    }

}
