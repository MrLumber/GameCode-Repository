using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

using BasicApp.Basic;
using BasicApp.WordBrain;

namespace BasicApp
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

            BasicWindow mainWindow = new WBWindow();
            mainWindow.CreateRecord();

            ApplicationContext theApplicationRecord = new ApplicationContext(mainWindow.record);
            Application.Run(theApplicationRecord);

        }
    }
}
