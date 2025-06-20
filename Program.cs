using System.Windows.Forms.VisualStyles;
using Microsoft.Data.SqlTypes;

namespace WorkCloneCS
{
    internal static class Program
    {
        static bool localOnly = false;
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            
            if (!firstRun.ranBefore())
            {
                ApplicationConfiguration.Initialize();
                FirstRunWindow firstRunWindow = new FirstRunWindow();
                Application.Run(firstRunWindow);
            }
            else
            {
                ApplicationConfiguration.Initialize();
                sync.syncAll();
                Application.Run(new Form1());
            }
            Logger.Log("exiting program, last line of code\n\n\n");
        
        }
    }
}