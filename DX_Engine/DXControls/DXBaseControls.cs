using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.DX_Engine.DXControls
{
    public abstract class DXBaseControls
    {
        protected float _MouseX, _MouseY;
        protected bool _MouseDown = false;
        protected Control _Ctrl;
        protected DXCamera _camera;

        //
        // ctrl is the windows control hooked for events (mouse & keyboard)
        //

        public DXBaseControls(DXCamera camera, Control ctrl)
        {
            _Ctrl = ctrl;
            _camera = camera;
            ctrl.MouseDown += ctrl_MouseDown;
            ctrl.MouseMove += ctrl_MouseMove;
            ctrl.MouseUp += ctrl_MouseUp;
            ctrl.MouseWheel += ctrl_MouseWheel;
            ctrl.KeyDown += ctrl_KeyDown;

            _MouseX = 0; _MouseY = 0;
        }
    
        protected abstract void ctrl_KeyDown(object sender, KeyEventArgs e);
        protected abstract void ctrl_MouseWheel(object sender, MouseEventArgs e);
        protected abstract void ctrl_MouseUp(object sender, MouseEventArgs e);
        protected abstract void ctrl_MouseMove(object sender, MouseEventArgs e);
        protected abstract void ctrl_MouseDown(object sender, MouseEventArgs e);
        
        public abstract void LookAt(BoundingBox BBox);
    }

}
