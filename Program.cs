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
            ApplicationConfiguration.Initialize();

            if (!firstRun.ranBefore())
            {
                Application.Run(new FirstRunWindow());

                // User closed first-run without saving valid settings.
                if (!firstRun.ranBefore())
                {
                    Logger.Log("first-run configuration not completed; exiting.");
                    return;
                }
            }

            SQL.initSQL();
            database.tryLoadLocalDatabase();

            Application.Run(new Form1());


            Logger.Log("exiting program, last line of code\n\n\n");
        }
    }
}