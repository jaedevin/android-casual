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

import CASUAL.Log;
import CASUAL.communicationstools.serial_interface.posix.FileFilter;
import CASUAL.communicationstools.serial_interface.posix.PosixSerialConnectivity;
import java.io.IOException;

/**
 *
 * @author adamoutler
 */
public class LinuxSerial implements InterfaceSerialPort {

    String pathname = "/dev/";
    String fileContains = "ttyUSB";

    @Override
    public String[] getComPorts() {
        String[] ports = new FileFilter().selectFilesInPathBasedOnName(pathname, fileContains);
        for (int i = 0; i < ports.length; i++) {
            if (!ports[i].startsWith(pathname)) {
                ports[i] = pathname + ports[i];
            }
        }
        return ports;
    }

    @Override
    public boolean checkPortStatus(String port) {
        try {
            return new PosixSerialConnectivity().verifyConnectivity(port);
        } catch (IOException | InterruptedException ex) {
            Log.errorHandler(ex);

        }
        return false;
    }

    @Override
    public boolean sendDataToPort(String port, String data, String expectedValue) {
        try {
            return (new PosixSerialConnectivity().sendCommandToSerial(port, data).contains(expectedValue));
        } catch (IOException | InterruptedException ex) {
            Log.errorHandler(ex);
        }
        return false;
    }

    @Override
    public String sendData(String port, String data) {
        try {
            String s = new PosixSerialConnectivity().sendCommandToSerial(port, data);
            return s;
        } catch (IOException | InterruptedException ex) {
            Log.errorHandler(ex);
        }
        return null;
    }

    @Override
    public byte[] sendBinaryData(String port, byte[] data, byte[] expectedValue) {
        throw new UnsupportedOperationException("Linux binary in Windows Serial is not yet supported");

    }

}
