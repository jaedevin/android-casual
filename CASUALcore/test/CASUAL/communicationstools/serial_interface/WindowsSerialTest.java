/*
 * Copyright (C) 2016 adamo
 *
 * This program is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program.  If not, see <https://www.gnu.org/licenses/>.
 */
package CASUAL.communicationstools.serial_interface;

import CASUAL.CASUALTools;
import CASUAL.Log;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import static org.junit.Assert.*;
import static org.junit.Assume.assumeTrue;

/**
 *
 * @author adamo
 */
public class WindowsSerialTest {
    
    final static String MODEM="COM6:";
    final static String SERIAL="COM5:";
    CASUALTools ct=new CASUALTools();
    public WindowsSerialTest() {
       assumeTrue(!java.awt.GraphicsEnvironment.isHeadless());
    }
    
    @BeforeClass
    public static void setUpClass() {
    }
    
    @AfterClass
    public static void tearDownClass() {
    }
    
    @Before
    public void setUp() {
    }
    
    @After
    public void tearDown() {
    }

    /**
     * Test of main method, of class WindowsSerial.
     */
    @Test
    public void testMain() {
        System.out.println("main");
        String[] args = null;
        WindowsSerial.main(args);

    }

    /**
     * Test of getComPorts method, of class WindowsSerial.
     */
    @Test
    public void testGetComPortsInformation() {
        System.out.println("getComPorts");
        WindowsSerial instance = new WindowsSerial();
        String[] results = instance.getComPorts();
        assert(results.length>0);
        for (String result:results){
            Log.level3Verbose("Found Com Port: "+result);
            Log.level3Verbose(instance.getPortInfo(result));
        }
        
    }

    /**
     * Test of checkPortStatus method, of class WindowsSerial.
     */
    @Test
    public void testCheckPortStatus() {
        System.out.println("checkPortStatus");
        String port = MODEM;
        WindowsSerial instance = new WindowsSerial();
        boolean expResult = true;
        boolean result = instance.checkPortStatus(port);
        assertEquals(expResult, result);

    }

    /**
     * Test of sendDataToPort method, of class WindowsSerial.
     */
    @Test
    public void testSendDataToPort() {
        System.out.println("sendDataToPort");
        String port = MODEM;
        String data = "\r\nAT\r\nAT\r\nAT\r\nAT\nAT\n";
        String expectedValue = "OK";
        WindowsSerial instance = new WindowsSerial();
        String expResult = "OK";
        final boolean result = instance.sendDataToPort(port, data, expectedValue);
        assert( result);

    }

    /**
     * Test of sendData method, of class WindowsSerial.
     */
    @Test
    public void testSendData() {
        System.out.println("sendData");
        String port = MODEM;
        String data = "\r\n\r\bAT\r\n";
        WindowsSerial instance = new WindowsSerial();
        String expResult = "OK";
        String result = instance.sendData(port, data);
        assert( result.contains(expResult));
        

    }

    /**
     * Test of getComPorts method, of class WindowsSerial.
     */
    @Test
    public void testGetComPorts() {
        System.out.println("getComPorts");
        WindowsSerial instance = new WindowsSerial();
        int expResult = 0;
        String[] result = instance.getComPorts();
        assert( result.length>expResult);
        // TODO review the generated test code and remove the default call to fail.
    
    }

    /**
     * Test of getPortInfo method, of class WindowsSerial.
     */
    @Test
    public void testGetPortInfo() {
        System.out.println("getPortInfo");
        
        WindowsSerial instance = new WindowsSerial();
        String[] ports = instance.getComPorts();
        for (String port : ports){
            assert(instance.getPortInfo(port).length()>0);
        }
    }

    /**
     * Test of sendBinData method, of class WindowsSerial.
     */
    @Test
    public void testSendBinData() {
        System.out.println("sendBinData");
        
        byte[] data = new byte[] { (byte)0x7e, (byte)0x00, (byte)0x78, (byte)0xf0, (byte)0x7e };
        byte[] expected=new byte[]{ 0x7e };
        WindowsSerial instance = new WindowsSerial();
        String port =SERIAL;
        int expResult = 1;
        byte[] result = instance.sendBinData(port, data,expected);
        for (byte c: result){
            System.out.print((char)c);
        }
        System.out.println();
        for (byte c: result){
            System.out.print((char)c + " "+ (int)c+ " ");
        }
        assert( result.length>expResult);

    }
    
}
