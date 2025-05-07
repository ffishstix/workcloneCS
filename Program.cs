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
            SQL.tmep();
            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            //ApplicationConfiguration.Initialize();
            //Application.Run(new Form1());
            Console.ReadLine();
        }
    }
}