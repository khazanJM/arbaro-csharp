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
        private Vector3 s0 = Vector3.Zero;
        private Vector3 _cameraPos;

        public Matrix M0 = Matrix.Identity;
        public Matrix M = Matrix.Identity;

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
            M0 = M;
        }

        private Vector3 ArcballPos(int X, int Y)
        {
            Vector3 s = Vector3.Zero;

            // arcball sphere screen diameter
            float screenDiameter = Math.Min(_Ctrl.ClientSize.Width, _Ctrl.ClientSize.Height);

            // x& y position of the mouse on the sphere
            s.X = 3*(X - screenDiameter / 2f) / screenDiameter;
            s.Y = -3*(Y - screenDiameter / 2f) / screenDiameter;
            float n = s.LengthSquared();
            if (n < 1f) s.Z = -(float)Math.Sqrt(1 - n);

            s.Normalize();

            return s;
        }

        protected override void ctrl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_MouseDown) return;

            float DX = (e.X - _MouseX) / _Ctrl.ClientSize.Width;
            float DY = (e.Y - _MouseY) / _Ctrl.ClientSize.Height;           

            if (e.Button == MouseButtons.Left)
            {
                //
                // Rotate
                //    
                                        
                // Current mouse position on the unit sphere
                Vector3 s = ArcballPos(e.X, e.Y);              

                // make rotation              
                float angle = (float)Math.Acos(Vector3.Dot(s0, s));
                Vector3 axis = Vector3.Cross(s0, s); axis.Normalize();
                M = M0*Matrix.RotationAxis(axis, angle);          
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Pan
                //  The camera and the target are translated with the same
                //  translation. 
                Matrix VP = _camera.ViewMatrix * _camera.ProjMatrix;             
                Vector4 mpp = Vector3.Transform(_camera.Target, VP);
                VP.Invert();
                Vector4 delta = Vector4.Transform(new Vector4(-2 * DX, 2 * DY, mpp.Z / mpp.W, 1), VP);              
                Vector3 delta3 = new Vector3(delta.X, delta.Y, delta.Z)/delta.W;

                _camera.Position += delta3 - _camera.Target;
                _camera.Target = delta3;
            }

            _MouseX = e.X;
            _MouseY = e.Y;
        }

        protected override void ctrl_MouseDown(object sender, MouseEventArgs e)
        {
            _MouseX = e.X;
            _MouseY = e.Y;
            _MouseDown = true;

            // for intial quaternion
            // Mouse down position on the unit sphere
            s0 = ArcballPos(e.X, e.Y);

            Console.WriteLine(s0);

            // Camera initial position
            _cameraPos = _camera.Position;     
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
