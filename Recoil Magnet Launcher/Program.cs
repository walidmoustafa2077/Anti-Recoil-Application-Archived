using System.Diagnostics;

namespace Recoil_Magnet_Launcher
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Get the correct build directory (where MouseController.exe is compiled)
            string originalDir = AppDomain.CurrentDomain.BaseDirectory; // More reliable than Directory.GetCurrentDirectory()
            try
            {
                // get each exe file in the directory
                foreach (string file in Directory.GetFiles(originalDir, "*.exe"))
                {
                    // if exe not contains Luncher, launch it
                    if (!file.Contains("Launcher"))
                    {
                        Process.Start(file);
                    }
                    break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error launching Mouse Controller: " + ex.Message);
            }

            Environment.Exit(0);
        }
    }
}
