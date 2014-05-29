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
        public static DXRendererClass Renderer;
        public static DXConfigClass DXConfig;
        public static DXSceneOptions DXSceneOptions = new DXSceneOptions();
        public static DXShaderManager DXShaderManager;

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

            DXConfig = new DXConfigClass("");
            Renderer = new DXRendererClass(DXConfig);

            form = new ArbaroMainForm();
            form.Width = DXConfig.FormDefaultWidth;
            form.Height = DXConfig.FormDefaultHeight;

            // Setup handler on resize form
            (form.renderCtrl as Control).Resize += (sender, args) =>
            {
                resize = true;
            };

            DXShaderManager = new DXShaderManager();

            CS_PreciseTimer preciseTimer = new CS_PreciseTimer(10);
            DateTime tStart = preciseTimer.Now;

            RenderLoop.Run(form, () =>
            {
                if (!initialized)
                {
                    Rectangle r = form.renderCtrl.ClientRectangle;
                    Renderer.Initialize(r.Width, r.Height, form.renderCtrl.Handle, form, DXConfig);
                    initialized = true;
                    form.RendererInitialized();
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
