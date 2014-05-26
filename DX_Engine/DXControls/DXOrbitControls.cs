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

namespace Arbaro2.DX_Engine.DXControls
{
    public class DXOrbitControls : DXBaseControls
    {
        // ctrl is the control hooked for events (mouse & keyboard)
        public DXOrbitControls(DXCamera camera, Control ctrl)
            : base(camera, ctrl)
        {
        }

        private void Reset()
        {
            _MouseX = 0; _MouseY = 0;
        }


        protected override void ctrl_KeyDown(object sender, KeyEventArgs e)
        {
        }


        protected override void ctrl_MouseWheel(object sender, MouseEventArgs e)
        {
            float zoom = e.Delta;
            Vector3 rail = _camera.Target - _camera.Position;
            UpdateCamera(0, 0, Math.Sign(zoom) * 0.1f * rail, Vector3.Zero);
        }

        protected override void ctrl_MouseUp(object sender, MouseEventArgs e)
        {
            _MouseX = 0;
            _MouseY = 0;
            _MouseDown = false;
        }

        protected override void ctrl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_MouseDown) return;

            float DX = (e.X - _MouseX) / _Ctrl.ClientSize.Width;
            float DY = (e.Y - _MouseY) / _Ctrl.ClientSize.Height;
            _MouseX = e.X;
            _MouseY = e.Y;

            if (e.Button == MouseButtons.Left)
            {
                // Rotate
                DX = (float)Math.PI * DX;
                DY = (float)Math.PI * DY;
                UpdateCamera(-DX, DY, Vector3.Zero, Vector3.Zero);
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Pan
                //  The camera and the target are translated with the same
                //  translation. 
                Vector3 l = Vector3.Cross((_camera.Target - _camera.Position), new Vector3(0, 1, 0));
                l.Normalize();
                Vector3 u = Vector3.Cross((_camera.Target - _camera.Position), l);
                u.Normalize();

                Vector3 translation = 10 * (DX * l + -DY * u);
                UpdateCamera(0, 0, translation, translation);
            }
        }

        protected override void ctrl_MouseDown(object sender, MouseEventArgs e)
        {
            _MouseX = e.X;
            _MouseY = e.Y;
            _MouseDown = true;
        }

        public override void LookAt(BoundingBox BBox)
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

        private void UpdateCamera(float dYaw, float dPitch, Vector3 dPosition, Vector3 dTarget)
        {
            _camera.Position += dPosition;
            _camera.Target += dTarget;

            if (dYaw != 0 || dPitch != 0)
            {
                Vector3 P = _camera.Position - _camera.Target;
                float r = P.Length();
                P /= r;
                float a = (float)Math.Atan2(P.Z, P.X);
                float b = (float)Math.Asin(P.Y);

                a += dYaw;
                b += dPitch;

                P.Y = (float)Math.Sin(b);
                P.X = (float)(Math.Cos(b) * Math.Cos(a));
                P.Z = (float)(Math.Cos(b) * Math.Sin(a));
                P *= r;
                P += _camera.Target;
                _camera.Position = P;
            }
        }
    }
}
