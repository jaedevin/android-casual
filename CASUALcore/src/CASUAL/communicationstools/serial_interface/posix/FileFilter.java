/*
 * To change this license header, choose License Headers in Project Properties.
 * To change this template file, choose Tools | Templates
 * and open the template in the editor.
 */

package CASUAL.communicationstools.serial_interface.posix;

import java.io.File;

/**
 *
 * @author adamoutler
 */
public class FileFilter {

    public String[] selectFilesInPathBasedOnName(String pathname, String fileContains) {
        return new File(pathname).list((File dir, String name) -> {
            if (name.contains(fileContains)) {
                return true;
            }
            return false;
        });
    }
    
}
