using System;
using System.Diagnostics;
using System.Windows.Forms;

namespace Caffeine
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            CaffeineApplicationContext app = null;
            try
            {
                app = new CaffeineApplicationContext();
                Application.Run(app);
            }
            finally
            {
                app?.Dispose();
            }
        }
    }
}
