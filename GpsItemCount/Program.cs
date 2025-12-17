using System;
using System.Windows.Forms;

namespace GpsItemCount
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            DateTime in331Days = DateTime.Now.AddDays(331);

            /*
            GpxCounter program = new GpxCounter();
            DirectoryInfo folder = new DirectoryInfo(@"D:\daten\cmaurer\doc\wandern\PCT-PacificCrestTrail\GPS");
            program.countNodes(folder);
            */
            Application.Run(new frmMain());

        }
    }
}
