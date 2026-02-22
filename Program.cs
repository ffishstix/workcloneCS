using System;
using System.Diagnostics;
using System.IO;

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



            Task.Run(() =>
            {
                SQL.initSQL();
                database.tryLoadLocalDatabase();
            });


            if (!firstRun.ranBefore())
            {
                ApplicationConfiguration.Initialize();
                FirstRunWindow firstRunWindow = new FirstRunWindow();
                Application.Run(firstRunWindow);
            }
            else
            {
                ApplicationConfiguration.Initialize();
                Application.Run(new Form1());
            }

            Logger.Log("exiting program, last line of code\n\n\n");
            
            do { Thread.Sleep(100);} 
            while (!SQL.initCompleted);

            Logger.Log(SQL.getDatabaseVNum().ToString());


            

        }

    }
}

