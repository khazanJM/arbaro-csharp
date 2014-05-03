using Arbaro2.DX_Engine;
using Arbaro2.Utilities;

using SharpDX.Windows;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2
{
    static class Program
    {
        public static ArbaroMainForm form;
        public static DXRendererClass Renderer = new DXRendererClass();

        /// <summary>
        /// Point d'entrée principal de l'application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            bool initialized = false;
            bool resize = false;

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            DXConfigClass DXConfig = new DXConfigClass("");

            form = new ArbaroMainForm();
            form.Width = DXConfig.FormDefaultWidth;
            form.Height = DXConfig.FormDefaultHeight;

            // Setup handler on resize form
            (form.renderCtrl as Control).Resize += (sender, args) =>
            {
                resize = true;
            };


            CS_PreciseTimer preciseTimer = new CS_PreciseTimer(10);
            DateTime tStart = preciseTimer.Now;

            RenderLoop.Run(form, () =>
            {
                if (!initialized)
                {
                    Rectangle r = form.renderCtrl.ClientRectangle;
                    Renderer.Initialize(r.Width, r.Height, form.renderCtrl.Handle, form, DXConfig);
                    initialized = true;
                }
                if (resize)
                {
                    resize = false;
                    Rectangle r = form.renderCtrl.ClientRectangle;
                    Renderer.Resize(r.Width, r.Height, DXConfig);
                }

                DateTime tEnd = preciseTimer.Now;
                float elapsed = (float)(tEnd.Subtract(tStart)).TotalMilliseconds;
                tStart = tEnd;

                Renderer.Frame();
            });

            Renderer.Dispose();
        }
    }
}
