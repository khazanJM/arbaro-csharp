using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.DX_Engine.DXControls
{
    class DXArcBallControls : DXBaseControls
    {
        // ctrl is the control hooked for events (mouse & keyboard)
        public DXArcBallControls(DXCamera camera, Control ctrl)
            : base(camera, ctrl)
        {
        }

        protected override void ctrl_KeyDown(object sender, KeyEventArgs e)
        {
        }


        protected override void ctrl_MouseWheel(object sender, MouseEventArgs e)
        {
            float zoom = e.Delta;
            Vector3 rail = _camera.Target - _camera.Position;
            _camera.Position += Math.Sign(zoom) * 0.1f * rail;
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
                
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Pan
                //  The camera and the target are translated with the same
                //  translation. 
                Matrix VP = -_camera.ViewMatrix * _camera.ProjMatrix;
                Vector3 MP = _camera.Target;
                Vector4 mpp = Vector3.Transform(MP, VP);
                Vector4 delta = new Vector4(-2*DX, 2*DY, mpp.Z/mpp.W, 1);
                Matrix vp = _camera.ViewMatrix * _camera.ProjMatrix;
                vp.Invert();
                delta = Vector4.Transform(delta, vp);
                
                Vector3 delta3 = new Vector3(delta.X, delta.Y, delta.Z)/delta.W;
                _camera.Position += delta3 - _camera.Target;
                _camera.Target = delta3;
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

        private void Reset()
        {
            _MouseX = 0; _MouseY = 0;
        }
    }
}
