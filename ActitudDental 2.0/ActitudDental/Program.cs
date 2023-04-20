using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ActitudDental
{
    static class Program
    {
        /// <summary>
        /// Punto de entrada principal para la aplicación.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            FrmLogin fLogin = new FrmLogin();
            if (fLogin.ShowDialog() == DialogResult.OK)
            {
                Application.Run(new FrmDental());
            }
            else
            {
                Application.Exit();
            }
        }
    }
}
