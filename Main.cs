using System;
using System.IO;
using System.Xml;

namespace ShelteredSaveEditorCLI
{
    class Program
    {

        public static string path, savePath, saveType;
        static void Main(string[] args)
        {
            Console.WriteLine("=================================");
            Console.WriteLine("@ Sheltered Save Editor CLI v1.0");
            Console.WriteLine("@ Author: Emre Cebeci");
            Console.WriteLine("@ Date: 04/05/21 (dd/mm/yy)");
            Console.WriteLine("=================================");

            // Check if it has args
            if (args.Length >= 1)
            {
                // Check if there is a file in given location
                if (File.Exists(args[0]))
                {
                    // Open file and check its extension
                    var file = File.Open(args[0], FileMode.Open);
                    if (file.Name.EndsWith(".dat") || file.Name.EndsWith(".xml"))
                    {
                        // Save path variable, close file and decode or encode file.
                        path = file.Name;
                        file.Close();
                        if (ProcessData())
                        {
                            Console.WriteLine("[!] Your save file successfully converted to " + saveType);
                            Console.WriteLine("[!] Save file path: " + savePath);
                        }
                        else
                        {
                            Console.WriteLine("[!] File couldn't be converted.");
                        }
                    }
                    else
                    {
                        Console.WriteLine("[!] File is not a valid .dat or .xml file: " + file.Name);
                    }
                }
                else
                {
                    Console.WriteLine("[!] File not exists!");
                }
            }
            else
            {
                Console.WriteLine("[!] Please specify file path or drag & drop your save file on CLI executable.");
            }
            Console.WriteLine("Press any key to close.");
            Console.ReadKey(true);
        }
        private static bool ProcessData()
        {
            // Check if given file is xml
            if (path.EndsWith(".xml"))
            {
                // If it can load xml and find root node its okay else throws exception and returns false
                XmlDocument doc = new XmlDocument();
                try
                {
                    doc.Load(path);
                    XmlElement root = doc.DocumentElement;
                    if (root.Name != "root") throw new Exception("File is not a valid XML file.");
                }
                catch
                {
                    Console.WriteLine("[!] File is not a valid XML file.");
                    return false;
                }
            }
            // Read file as byte array
            var data = File.ReadAllBytes(path);
            if (data.Length > 0)
            {
                // Do some magic
                int num = 0;
                byte[] array = new byte[]
                {
                172,
                242,
                115,
                58,
                254,
                222,
                170,
                33,
                48,
                13,
                167,
                21,
                139,
                109,
                74,
                186,
                171
                };
                byte[] array2 = new byte[]
                {
                0,
                2,
                4,
                1,
                6,
                15,
                13,
                16,
                8,
                3,
                12,
                10,
                5,
                9,
                11,
                7,
                14
                };
                for (int i = 0; i < data.Length; i++)
                {
                    data[i] ^= array[(int)array2[num++]];
                    if (num >= array2.Length)
                    {
                        num = 0;
                    }
                }
                // Decleare some variables
                savePath = path.EndsWith(".dat") ? path.Replace(".dat", ".xml") : path.Replace(".xml", ".dat");
                saveType = path.EndsWith(".dat") ? "DECODED XML FILE" : "ENCODED DAT FILE";

                // Create new file
                using (FileStream fileStream = File.Create(savePath))
                {
                    if (fileStream != null)
                    {
                        fileStream.Write(data, 0, data.Length);
                        fileStream.Flush();
                    }
                    else
                    {
                        return false;
                    }
                }
                if (savePath.EndsWith(".xml"))
                {
                    try
                    {
                        var checkXml = new XmlDocument();
                        checkXml.Load(savePath);
                        var root = checkXml.DocumentElement;
                        if (root.Name != "root")
                        {
                            Console.WriteLine("[!] File is not a valid .DAT save file.");
                            File.Delete(savePath);
                            return false;
                        }
                    }
                    catch
                    {
                        Console.WriteLine("[!] File is not a valid .DAT save file.");
                        File.Delete(savePath);
                        return false;
                    }
                }
                return true;
            }
            else
            {
                Console.WriteLine("[!] File is empty!");
                return false;
            }
        }
    }
}
