using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Keyloggerfy
{
    static class Program
    {
        /// <summary>
        /// FALTA PODER TERMINAR LOS HILOS Y CERRAR LA APLICACION
        /// O CERRAR LA APLICACION Y QUE LOS HILOS SIGAN CORRIENDO EN SEGUNDO PLANO
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new main());
        }
    }
}
