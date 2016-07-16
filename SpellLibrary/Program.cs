using System;
using System.Windows.Forms;

namespace SpellLibrary
{
    class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            Application.EnableVisualStyles();
            Application.Run(new MainWindow());
            return 0;
        }
    }
}
