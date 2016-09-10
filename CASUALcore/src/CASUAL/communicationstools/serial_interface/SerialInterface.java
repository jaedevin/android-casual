/*
 * Copyright (C) 2016 Adam Outler adamoutler@gmail.com
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

import CASUAL.language.Command;
import CASUAL.Log;
import CASUAL.OSTools;

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
    public byte[] sendBinaryData(String port, byte[] data, byte[] expectedValue) {
        return selectedInterface.sendBinaryData(port, data, expectedValue);
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

    final class StatusOject {

        boolean complete = true;
        final Object lock = new Object();
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

    public static byte[] hexStringToByteArray(String s) {
        s = s.replace(" ", "");
        int len = s.length();
        byte[] data = new byte[len / 2];
        for (int i = 0; i < len; i += 2) {
            data[i / 2] = (byte) ((Character.digit(s.charAt(i), 16) << 4)
                    + Character.digit(s.charAt(i + 1), 16));
        }
        return data;
    }
}
