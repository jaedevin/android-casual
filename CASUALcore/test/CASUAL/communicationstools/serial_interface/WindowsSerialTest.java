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

import CASUAL.Log;
import org.junit.After;
import org.junit.AfterClass;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;
import static org.junit.Assert.*;

/**
 *
 * @author adamo
 */
public class WindowsSerialTest {
    
    public WindowsSerialTest() {
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
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of getComPorts method, of class WindowsSerial.
     */
    @Test
    public void testGetComPorts() {
        System.out.println("getComPorts");
        WindowsSerial instance = new WindowsSerial();
        
        String[] results = instance.getComPorts();
        assert(results.length>0);
        for (String result:results){
            Log.level3Verbose("Found Com Port: "+result);
        }
        
    }

    /**
     * Test of checkPortStatus method, of class WindowsSerial.
     */
    @Test
    public void testCheckPortStatus() {
        System.out.println("checkPortStatus");
        String port = "";
        WindowsSerial instance = new WindowsSerial();
        boolean expResult = false;
        boolean result = instance.checkPortStatus(port);
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of sendDataToPort method, of class WindowsSerial.
     */
    @Test
    public void testSendDataToPort() {
        System.out.println("sendDataToPort");
        String port = "";
        String data = "";
        String expectedValue = "";
        WindowsSerial instance = new WindowsSerial();
        boolean expResult = false;
        boolean result = instance.sendDataToPort(port, data, expectedValue);
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }

    /**
     * Test of sendData method, of class WindowsSerial.
     */
    @Test
    public void testSendData() {
        System.out.println("sendData");
        String port = "";
        String data = "";
        WindowsSerial instance = new WindowsSerial();
        String expResult = "";
        String result = instance.sendData(port, data);
        assertEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        fail("The test case is a prototype.");
    }
    
}
