/*
 * To change this template, choose Tools | Templates
 * and open the template in the editor.
 */
package CASUAL;

import java.io.BufferedInputStream;
import java.io.ByteArrayInputStream;
import java.io.File;
import java.io.FileNotFoundException;
import java.io.IOException;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.logging.Level;
import java.util.logging.Logger;
import org.junit.After;
import org.junit.AfterClass;
import static org.junit.Assert.assertArrayEquals;
import static org.junit.Assert.assertEquals;
import org.junit.Before;
import org.junit.BeforeClass;
import org.junit.Test;

/**
 *
 * @author adam
 */
public class FileOperationsTest {
    private static final String adbIniResource="/CASUAL/communicationstools/adb/resources/adb_usb.ini";
    @BeforeClass
    public static void setUpClass(){
    }
    @AfterClass
    public static void tearDownClass() {
    }

    @Before
    public void setUp() throws Exception {
    }

    @After
    public void tearDown() throws Exception {
    }

    CASUALSessionData sd = CASUALSessionData.newInstance();

    public FileOperationsTest() {
    }

    /**
     * Test of recursiveDelete method, of class FileOperations.
     */
    @Test
    public void testRecursiveDelete_String() {
        System.out.println("recursiveDelete");
        String path =sd.getTempFolder();
        String testpath=sd.getTempFolder()+"woot"+CASUALSessionData.slash+"woot"+CASUALSessionData.slash+"woot"+CASUALSessionData.slash+"woot";
        FileOperations instance = new FileOperations();
        new File(testpath).mkdirs();
        instance.recursiveDelete(path);
        assert(!instance.verifyExists(testpath));
    }

    /**
     * Test of recursiveDelete method, of class FileOperations.
     */
    @Test
    public void testRecursiveDelete_File() {
        System.out.println("recursiveDelete");
        File testpath = new File(sd.getTempFolder()+"woot"+CASUALSessionData.slash+"woot"+CASUALSessionData.slash+"woot"+CASUALSessionData.slash+"woot");
        FileOperations instance = new FileOperations();
        instance.recursiveDelete(new File(sd.getTempFolder()+"woot"));
        assert(!instance.verifyExists(testpath.getAbsolutePath()));
    }

    /**
     * Test of verifyWritePermissionsRecursive method, of class FileOperations.
     * @throws java.io.IOException
     */
    @Test
    public void testVerifyWritePermissionsRecursive() throws IOException {
        System.out.println("verifyWritePermissionsRecursive");
        String path =sd.getTempFolder()+"woot";
        new File(path).createNewFile();
        FileOperations instance = new FileOperations();
        boolean result = instance.verifyWritePermissionsRecursive(sd.getTempFolder());
        assert(result);
    }


    /**
     * Test of findRecursive method, of class FileOperations.
     * @throws java.io.IOException
     */
    @Test
    public void testFindRecursive() throws IOException {
        System.out.println("findRecursive");
        File testpath = new File(sd.getTempFolder()+"ss"+CASUALSessionData.slash+"ss"+CASUALSessionData.slash+"ss"+CASUALSessionData.slash+"test");
        testpath.mkdirs();
        File testFile=new File(testpath.getAbsolutePath()+CASUALSessionData.slash+"testFindRecursive");
        new File(sd.getTempFolder()+"ss"+CASUALSessionData.slash+"test").createNewFile();
        testFile.createNewFile();
        if (!testFile.exists()){
            try {
                Thread.sleep(3000);
                
            } catch (InterruptedException ex) {
                Logger.getLogger(FileOperationsTest.class.getName()).log(Level.SEVERE, null, ex);
            }
        }
        String PathToSearch =sd.getTempFolder();
        FileOperations instance =new FileOperations();
        System.out.println("performing recursive search");
        String result = instance.findRecursive(PathToSearch,"testFindRecursive");
        System.out.println("result: "+result);
        System.out.println("result: "+result);
        assertEquals(testpath.getCanonicalPath()+CASUALSessionData.slash+"testFindRecursive" ,result);
        instance.recursiveDelete(sd.getTempFolder());

    }

    /**
     * Test of verifyExists method, of class FileOperations.
     * @throws java.io.IOException
     */
    @Test
    public void testVerifyExists() throws IOException {
        File f= new File(sd.getTempFolder()+"new");
        f.createNewFile();
        assertEquals(true, new CASUAL.FileOperations().verifyExists(sd.getTempFolder() + "new" + CASUAL.CASUALSessionData.slash));
        assertEquals(false, new CASUAL.FileOperations().verifyExists(sd.getTempFolder() + "asfdadfasfd" + CASUAL.CASUALSessionData.slash));
        f.delete();
    }

    /**
     * Test of makeFolder method, of class FileOperations.
     */
    @Test
    public void testMakeFolder() {
        assertEquals(true, new CASUAL.FileOperations().makeFolder(sd.getTempFolder() + "new" + CASUAL.CASUALSessionData.slash));
        assertEquals(false, new CASUAL.FileOperations().makeFolder(null));
        
    }

    /**
     * Test of writeStreamToFile method, of class FileOperations.
     * @throws java.lang.Exception
     */
    @Test
    public void testWriteStreamToFile() throws Exception {
        System.out.println("writeStreamToFile");
        String expectedResult="woot";
        ByteArrayInputStream bas=new ByteArrayInputStream(expectedResult.getBytes());
        BufferedInputStream stream = new BufferedInputStream(bas);
        String destination =sd.getTempFolder()+"file";
        FileOperations instance = new FileOperations();
        instance.writeStreamToFile(stream, destination);
        String result=instance.readFile(destination);
        assertEquals(result,expectedResult);        
        
    }

    /**
     * Test of writeToFile method, of class FileOperations.
     * @throws java.lang.Exception
     */
    @Test
    public void testWriteToFile() throws Exception {
        System.out.println("writeToFile");
        String Text = "woot";
        String f =sd.getTempFolder()+CASUALSessionData.slash+"newFile";
        FileOperations instance = new FileOperations();
        instance.writeToFile(Text, f);
        assertEquals(Text,instance.readFile(f));
        assert(instance.deleteFile(f));
    }

    /**
     * Test of deleteStringArrayOfFiles method, of class FileOperations.
     * @throws java.io.IOException
     */
    @Test
    public void testDeleteStringArrayOfFiles() throws IOException {
        System.out.println("deleteStringArrayOfFiles");
        String[] cleanUp = new String[]{sd.getTempFolder()+"cool",sd.getTempFolder()+"woot",sd.getTempFolder()+"neat"};
        for (String s:cleanUp){
            new File(s).createNewFile();
        }
        FileOperations instance = new FileOperations();
        boolean expResult = true;
        boolean result = instance.deleteStringArrayOfFiles(cleanUp);
        assertEquals(expResult, result);
        
    }

    /**
     * Test of copyFile method, of class FileOperations.
     * @throws java.lang.Exception
     */
    @Test
    public void testCopyFile_File_File() throws Exception {
        System.out.println("copyFile");
        
        File sourceFile = new File(sd.getTempFolder()+"woot");
        sourceFile.createNewFile();
        File destFile = new File(sd.getTempFolder()+"woot2");
        FileOperations instance = new FileOperations();
        instance.copyFile(sourceFile, destFile);
        assert(instance.verifyExists(sourceFile.getAbsolutePath()));
        assert(instance.verifyExists(destFile.getAbsolutePath()));
        sourceFile.delete();
        destFile.delete();
        
        
    }

    /**
     * Test of currentDir method, of class FileOperations.
     */
    @Test
    public void testCurrentDir() {
        System.out.println("currentDir");
        FileOperations instance = new FileOperations();
        String result = instance.currentDir();
        assert(!result.isEmpty());

    }


    /**
     * Test of copyFile method, of class FileOperations.
     */
    @Test
    public void testCopyFile_String_String() {
        System.out.println("copyFile");
        String FromFile = "";
        String ToFile = "";
        FileOperations instance = new FileOperations();
        boolean expResult = false;
        boolean result = instance.copyFile(FromFile, ToFile);
        assertEquals(expResult, result);

    }

    /**
     * Test of setExecutableBit method, of class FileOperations.
     * @throws java.io.IOException
     */
    @Test
    public void testSetExecutableBit() throws IOException {
        System.out.println("setExecutableBit");
        String Executable =sd.getTempFolder()+"new";
        File f=new File(Executable);
        f.createNewFile();
        FileOperations instance = new FileOperations();
        boolean result = instance.setExecutableBit(Executable);
        assert(f.canExecute());
        f.delete();
        if (OSTools.isLinux()||OSTools.isMac()){
            f.createNewFile();
            assert(!f.canExecute());
            f.delete();
        }
        

    }

    /**
     * Test of verifyResource method, of class FileOperations.
     */
    @Test
    public void testVerifyResource() {
        System.out.println("verifyResource");
        String res = adbIniResource;
        FileOperations instance = new FileOperations();
        boolean expResult = true;
        boolean result = instance.verifyResource(res);
        assertEquals(expResult, result);

    }

    /**
     * Test of readTextFromResource method, of class FileOperations.
     * @throws java.io.FileNotFoundException
     */
    @Test
    public void testReadTextFromResource() throws FileNotFoundException, IOException {
        System.out.println("writeStreamToFile");
        String expectedResult="woot";
        ByteArrayInputStream bas=new ByteArrayInputStream(expectedResult.getBytes());
        BufferedInputStream stream = new BufferedInputStream(bas);
        String destination =sd.getTempFolder()+"file";
        FileOperations instance = new FileOperations();
        instance.writeStreamToFile(stream, destination);
        String result=instance.readFile(destination);
        assertEquals(expectedResult,result);        
    }

    /**
     * Test of readTextFromStream method, of class FileOperations.
     */
    @Test
    public void testReadTextFromStream() {
        System.out.println("readTextFromStream");
        String expectedResult="woot";
        ByteArrayInputStream bas=new ByteArrayInputStream(expectedResult.getBytes());
        BufferedInputStream stream = new BufferedInputStream(bas);
        FileOperations instance = new FileOperations();
        String result=instance.readTextFromStream(stream);
        assertEquals(expectedResult, result);
    }

    /**
     * Test of readFile method, of class FileOperations.
     */
    @Test
    public void testReadFile() {
        System.out.println("readFile");
        String FileOnDisk = "README.txt";
        FileOperations instance = new FileOperations();
        String expResult = "YOU'RE DOING IT WRONG!";
        String result = instance.readFile(FileOnDisk);
        assert(result.contains(expResult));

    }

    /**
     * Test of listFolderFiles method, of class FileOperations.
     */
    @Test
    public void testListFolderFiles() {
        System.out.println("listFolderFiles");
        String folder = "./";
        FileOperations instance = new FileOperations();
        String[] expResult = new File(folder).list();
        String[] result = instance.listFolderFiles(folder);
        assertArrayEquals(expResult, result);

    }

    /**
     * Test of listFolderFilesCannonically method, of class FileOperations.
     */
    @Test
    public void testListFolderFilesCannonically() {
        System.out.println("listFolderFilesCannonically");
        String folder = "./";
        FileOperations instance = new FileOperations();
        String expResult="";
        try {
            expResult = new File("../src/META-INF").getCanonicalPath();
        } catch (IOException ex) {
            Logger.getLogger(FileOperationsTest.class.getName()).log(Level.SEVERE, null, ex);
        }
        String[] result = instance.listFolderFilesCannonically(folder);
        assert(Arrays.asList(result).contains(expResult));
    }

    /**
     * Test of moveFile method, of class FileOperations
     * @throws java.lang.Exception
     */
    @Test
    public void testMoveFile_File_File() throws Exception {
        System.out.println("moveFile");
        File sourceFile = new File(sd.getTempFolder()+"newfile");
        File destFile = new File(sd.getTempFolder()+"newfile2");
        sourceFile.createNewFile();
        FileOperations instance = new FileOperations();
        destFile.delete();
        boolean expResult = true;
        boolean result = instance.moveFile(sourceFile, destFile);
        assertEquals(expResult, result);
        assert(!instance.verifyExists(sourceFile.getAbsolutePath()));
        assert(instance.verifyExists(destFile.getAbsolutePath()));
        destFile.delete();
        sourceFile.delete();
    }

   

    /**
     * Test of writeBytesToFile method, of class FileOperations.
     */
    @Test
    public void testWriteBytesToFile() throws Exception {
        System.out.println("writeBytesToFile");
        File f=new File("myfile");
        String path=f.getAbsolutePath();
        byte[] data = new byte[]{100, 101};
        FileOperations instance = new FileOperations();
        instance.writeBytesToFile(path, data);
        byte[] output=instance.readBytesFromFile(path);
        f.deleteOnExit();
        assert(output[0]==data[0]);
        assert(output[1]==data[1]);
        // TODO review the generated test code and remove the default call to fail.
    }

    /**
     * Test of readBytesFromFile method, of class FileOperations.
     */
    @Test
    public void testReadBytesFromFile() throws Exception {
        System.out.println("readBytesFromFile");
        String file = "";
        FileOperations instance = new FileOperations();
        byte[] expResult = null;
        byte[] result = instance.readBytesFromFile(file);
        assertArrayEquals(expResult, result);
        // TODO review the generated test code and remove the default call to fail.
        
        
    }
}