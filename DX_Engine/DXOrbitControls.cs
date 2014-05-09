using Arbaro2.Utilities;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.DX_Engine
{
    //
    // more or less based on three.js
    //  -> Orbit control
    //

    public class DXOrbitControls
    {    
        // ctrl is the control hooked for events (mouse & keyboard)
        public DXOrbitControls(DXCamera camera, Control ctrl)
        {           
            ctrl.MouseDown += ctrl_MouseDown;
            ctrl.MouseMove += ctrl_MouseMove;
            ctrl.MouseUp += ctrl_MouseUp;
            ctrl.MouseWheel += ctrl_MouseWheel;
            ctrl.KeyDown += ctrl_KeyDown;
        }

 

        void ctrl_KeyDown(object sender, KeyEventArgs e)
        {
        }

        void ctrl_MouseWheel(object sender, MouseEventArgs e)
        {
        }

        void ctrl_MouseUp(object sender, MouseEventArgs e)
        {
        }

        void ctrl_MouseMove(object sender, MouseEventArgs e)
        {
        }

        void ctrl_MouseDown(object sender, MouseEventArgs e)
        {
        }
    }
}
