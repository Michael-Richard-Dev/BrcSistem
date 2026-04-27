using System;
using System.Windows.Forms;
using BRCSISTEM.Desktop.Bootstrap;

namespace BRCSISTEM.Desktop
{
    internal static class Program
    {
        [STAThread]
        private static void Main()
        {
            System.Windows.Forms.Application.EnableVisualStyles();
            System.Windows.Forms.Application.SetCompatibleTextRenderingDefault(false);

            var compositionRoot = CompositionRoot.Create();
            System.Windows.Forms.Application.Run(new Interface.LoginForm(compositionRoot));
        }
    }
}
