using Arbaro2.Utilities;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

//
//  The controler should be provided with "something" to control
//  the speed of the zoom and pan... TODO
//

//
//  As the camera position & (mainly) look at can be changed
//  without going through the controler...
//  We need to add the yaw & pitch etc... based on the actual 
//  camera position & yaw/pitch
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
            float zoom = e.Delta;
            Vector3 rail = _camera.Target - _camera.Position;            
            UpdateCamera(0, 0, Math.Sign(zoom)*0.1f*rail, Vector3.Zero);
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
                UpdateCamera(_rx, _ry, Vector3.Zero, Vector3.Zero);
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Pan
                //  The camera and the target are translated with the same
                //  translation. The translation vector is in the tangent plane
                //  to the orbit (translation along left & up vector)

                Vector3 translation = 100*(-DX*_camera.Left+DY*_camera.Up);
                UpdateCamera(_rx, _ry, translation, translation);
            }
        }

        void ctrl_MouseDown(object sender, MouseEventArgs e)
        {
            _MouseX = e.X;
            _MouseY = e.Y;
            _MouseDown = true;
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

            _camera.Position = position;
            _camera.Target = p;            
        }

        void UpdateCamera(float dYaw, float dPitch, Vector3 dPosition, Vector3 dTarget)
        {
            _camera.Position += dPosition;
            _camera.Target += dTarget;
        }
    }
}
