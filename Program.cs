using System.Windows.Forms.VisualStyles;
using Microsoft.Data.SqlTypes;

namespace WorkCloneCS
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Logger.Log("This is a normal log.");  // Normal (black text)
            Logger.Log("This is an informational log.", 1);  // Info (blue text)
            Logger.Log("This is a success log.", 2);  // Success (green text)
            Logger.Log("This is a warning log.", 3);  // Warning (orange text)
            Logger.Log("This is an error log.", 4);  // Error (red text)
            sync.syncAll();
            if (sync.catagories != null)
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
                Console.ReadLine();
            }
            else
            {
                Logger.Log("i have absolutely no idea why but the fucker wont work");
            }
        }
    }
}