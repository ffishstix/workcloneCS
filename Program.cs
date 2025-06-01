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
            sync.syncAll();
            if (sync.catagories != null)
            {
                // To customize application configuration such as set high DPI settings or default font,
                // see https://aka.ms/applicationconfiguration.
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
                Logger.Log("exiting program, last line of code\n\n\n");
            }
            else
            {
                Logger.Log("\n\n\n\ni have absolutely no idea why but the fucker wont work\n\n\n\n\n\n");
            }
        }
    }
}