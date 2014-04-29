using Arbaro2.Utilities;
using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2
{
    static class Program
    {
        public static ArbaroMainForm form;

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            form = new ArbaroMainForm();

            CS_PreciseTimer preciseTimer = new CS_PreciseTimer(10);
            DateTime tStart = preciseTimer.Now;

            RenderLoop.Run(form, () =>
            {
                DateTime tEnd = preciseTimer.Now;
                float elapsed = (float)(tEnd.Subtract(tStart)).TotalMilliseconds;
                tStart = tEnd;                
            });
         
        }
    }
}
