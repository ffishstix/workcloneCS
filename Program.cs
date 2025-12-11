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
            bool deepDebugging = false;
            
            Task.Run(() =>
            {
                SQL.initSQL();
                database.initLocalDatabase();
            });

            if (!deepDebugging)
            {
                if (!firstRun.ranBefore())
                {
                    ApplicationConfiguration.Initialize();
                    FirstRunWindow firstRunWindow = new FirstRunWindow();
                    Application.Run(firstRunWindow);
                }
                else
                {
                    ApplicationConfiguration.Initialize();
                    sync.getFiles();
                    Application.Run(new Form1());
                }

                Logger.Log("exiting program, last line of code\n\n\n");
                
            }
            else
            {
                // custom code
                Logger.Log("deep debugging enabled");
                
                do {
                    Thread.Sleep(100);
                    
                }while (!SQL.initCompleted);
                Logger.Log(SQL.getDatabaseVNum().ToString());    
                
                
            }
        }
    }
}

