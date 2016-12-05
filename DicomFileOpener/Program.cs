using ClearCanvas.Dicom;
using Microsoft.Win32;
using System;
using System.IO;
using System.Text;


namespace DicomFileOpener
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if(!Properties.Settings.Default.SendToCreated)//If not initialized create a shortcut in the sendto
            {
                string sendToFolder = Environment.GetFolderPath(Environment.SpecialFolder.SendTo);
                string filename = Path.GetFileNameWithoutExtension(System.Reflection.Assembly.GetExecutingAssembly().Location);
                string execPath = Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location);
                IWshRuntimeLibrary.WshShell shell = new IWshRuntimeLibrary.WshShell();
                IWshRuntimeLibrary.IWshShortcut link = (IWshRuntimeLibrary.IWshShortcut)
                    shell.CreateShortcut(Path.Combine(sendToFolder, filename + ".lnk"));
                link.TargetPath = Path.Combine(execPath, filename + ".exe");
                link.Save();
                Properties.Settings.Default.SendToCreated = true;
                Properties.Settings.Default.Save();
            }
            Console.WriteLine("Started DicomFile Opener");
            if (args.Length > 0)
            {
                foreach (string arg in args)
                {
                    PrintFile(arg);
                }
            }
            else
            {
                Console.WriteLine("Enter a file path:");
                string file = Console.ReadLine();
                PrintFile(file);
            }
            Console.WriteLine("Press the any-key to continue");
            Console.ReadLine();
        }

        public static void PrintFile(string filename)
        {
            if (File.Exists(filename))
            {
                try
                {
                    DicomFile file = new DicomFile(filename);
                    file.Load(DicomReadOptions.DoNotStorePixelDataInDataSet | DicomReadOptions.ReadNonPart10Files);
                    StringBuilder sb = new StringBuilder();
                    file.Dump(sb, "", DicomDumpOptions.Default);
                    Console.WriteLine(sb.ToString());
                }
                catch (Exception)
                {
                    Console.WriteLine("Invalid File(Error, maybe not a dicomfile)");
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(filename))
                    Console.WriteLine("No Filename given");
                else
                    Console.WriteLine("Invalid File(File does not exist):" + filename);
            }
        }

    }
}