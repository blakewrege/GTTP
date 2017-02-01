using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
namespace ServerClient
{
    static class AppSettings
    {

       
       private const  int DefaultPortNumber = 50500; // default port number
       private static string SettingFileName = "setting.cfg";
       private static int PortNumber = DefaultPortNumber;
       static public int Port
       {
           get
           {
               if (isSettingExit())
               {
                  String[] allSettings = File.ReadAllLines(GetSettingFilePath());
                  PortNumber = int.Parse(allSettings[0]);
                  return PortNumber;
               }
               return DefaultPortNumber;
           }


           set
           {
               PortNumber = value;
               var writeStream = new StreamWriter(File.OpenWrite(GetSettingFilePath()));
               writeStream.WriteLine(value);
               writeStream.Close();
                               
               
           }

       }


       private static string GetSettingFilePath()
       {
           String filePath = "";
           String directoryName = Path.GetDirectoryName(Application.ExecutablePath);
           filePath = Path.Combine(directoryName, SettingFileName);
           return filePath;

       }
       private static bool isSettingExit()
       {
           return File.Exists(GetSettingFilePath());
       }
    }
}
