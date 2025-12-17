using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ZipItemCount
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            /*
            GpxCounter program = new GpxCounter();
            DirectoryInfo folder = new DirectoryInfo(@"D:\daten\cmaurer\doc\wandern\PCT-PacificCrestTrail\GPS");
            program.countNodes(folder);
            */
            Application.Run(new frmMain());

        }
    }
}
