using Arbaro2.Utilities;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//
//  This works because the initial camera is such that
//  It look along the positive Z axis
//  It is placed on the negative Z axis
//  it does not initially contains any rotation (yaw, pitch)
//  The camera controler does not handle roll moves
//

namespace Arbaro2.DX_Engine
{
    public class DXOrbitControls
    {       
        private float _MouseX, _MouseY;
        private bool _MouseDown = false;
        private Control _Ctrl;
        private float _rx, _ry;
        private DXCamera _camera;
   
        // ctrl is the control hooked for events (mouse & keyboard)
        public DXOrbitControls(DXCamera camera, Control ctrl)
        {
            _Ctrl = ctrl;
            _camera = camera;
            ctrl.MouseDown += ctrl_MouseDown;
            ctrl.MouseMove += ctrl_MouseMove;
            ctrl.MouseUp += ctrl_MouseUp;
            ctrl.MouseWheel += ctrl_MouseWheel;
            ctrl.KeyDown += ctrl_KeyDown;
         
            _MouseX = 0; _MouseY = 0;
            _rx = 0; _ry = 0;
        }

        public void Reset()
        {
            _MouseX = 0; _MouseY = 0;
            _rx = 0; _ry = 0;
        }
 

        void ctrl_KeyDown(object sender, KeyEventArgs e)
        {                     
        }


        void ctrl_MouseWheel(object sender, MouseEventArgs e)
        {
        }

        void ctrl_MouseUp(object sender, MouseEventArgs e)
        {
            _MouseX = 0;
            _MouseY = 0;
            _MouseDown = false;
        }

        void ctrl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_MouseDown) return;

            float DX = (e.X - _MouseX)/_Ctrl.ClientSize.Width;
            float DY = (e.Y - _MouseY)/_Ctrl.ClientSize.Height;
            _MouseX = e.X;
            _MouseY = e.Y;

            if (e.Button == MouseButtons.Left) {
                // Rotate
                DX = (float)Math.PI * DX;
                DY = (float)Math.PI * DY;               
                _rx += DX;
                _ry -= DY;
                UpdateCamera();
            }
        }

        void ctrl_MouseDown(object sender, MouseEventArgs e)
        {
            _MouseX = e.X;
            _MouseY = e.Y;
            _MouseDown = true;
        }

        void UpdateCamera() 
        {
            Vector3 translation = _camera.Target;

            
            Matrix M = Matrix.RotationYawPitchRoll(_rx, _ry, 0);
            Vector4 up = new Vector4(0, 1, 0,0); up = Vector4.Transform(up, M);
            Vector4 lookAt = new Vector4(0, 0, 1, 0); lookAt = Vector4.Transform(lookAt, M);
            Vector4 pos = new Vector4(0, 0, _camera.Position.Length(), 0); pos = Vector4.Transform(pos, M);
            _camera.ETU(new Vector3(pos.X, pos.Y, pos.Z), _camera.Target, new Vector3(up.X, up.Y, up.Z));
        }

        public void LookAt(BoundingBox BBox)
        {
            Reset();

            Vector3 lengths = BBox.Maximum - BBox.Minimum;
            float maxLen = Math.Max(lengths.Z, Math.Max(lengths.X, lengths.Y));
            maxLen = Math.Max(2 * _camera.ZNear, maxLen);

            Vector3 p = (BBox.Maximum + BBox.Minimum) / 2.0f;
            float distance = (1.05f * maxLen / 2.0f) / (float)Math.Tan(_camera.Fov / 2.0);

            Vector3 position = p; position.Z -= distance;
            _camera.ETU(position, p, new Vector3(0, 1, 0));
        }
    }
}
