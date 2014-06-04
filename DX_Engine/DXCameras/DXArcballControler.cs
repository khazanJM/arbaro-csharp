using Arbaro2.DX_Engine.DXControls;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.DX_Engine.DXCameras
{
    class DXArcballControler : DXBaseCameraControler
    {
        private Matrix _cameraWorldTransform;
        private Vector3 _center;
        private BoundingBox _bbox;

        private int _arcballStartX;
        private int _arcballStartY;

        // ctrl is the control hooked for events (mouse & keyboard)
        public DXArcballControler(DXCamera camera, Control ctrl)
            : base(camera, ctrl)
        {

        }

        private void Reset()
        {
            _MouseX = 0; _MouseY = 0;
        }


        protected override void ctrl_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.X)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    LookAt(_bbox, LookAtDirection.POSITIVE_X);
                }
                else
                {
                    LookAt(_bbox, LookAtDirection.NEGATIVE_X);
                }
            }
            if (e.KeyCode == Keys.Y)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    LookAt(_bbox, LookAtDirection.POSITIVE_Y);
                }
                else
                {
                    LookAt(_bbox, LookAtDirection.NEGATIVE_Y);
                }
            }
            if (e.KeyCode == Keys.Z)
            {
                if (e.Modifiers == Keys.Shift)
                {
                    LookAt(_bbox, LookAtDirection.POSITIVE_Z);
                }
                else
                {
                    LookAt(_bbox, LookAtDirection.NEGATIVE_Z);
                }
            }
        }


        protected override void ctrl_MouseWheel(object sender, MouseEventArgs e)
        {
            // Delta absolute value is pretty unknown and depends on device driver
            // the only thing we should use is it sign           
            Vector3 translation = Math.Sign(e.Delta) * _camera.LookAt;
            _cameraWorldTransform *= Matrix.Translation(translation);
            _camera.WorldTransform = _cameraWorldTransform;
        }

        protected override void ctrl_MouseUp(object sender, MouseEventArgs e)
        {
            _MouseX = 0;
            _MouseY = 0;
            _MouseDown = false;

            _cameraWorldTransform = _camera.WorldTransform ;
        }

        protected override void ctrl_MouseMove(object sender, MouseEventArgs e)
        {
            if (!_MouseDown) return;

            float DX = (e.X - _MouseX) / _Ctrl.ClientSize.Width;
            float DY = (e.Y - _MouseY) / _Ctrl.ClientSize.Height;
            

            if (e.Button == MouseButtons.Left)
            {
                // Rotate
                float l = Math.Min(_Ctrl.ClientSize.Width, _Ctrl.ClientSize.Height);
                Vector3 s0 = 1.3f * 2*new Vector3(_arcballStartX - _Ctrl.ClientSize.Width/2f, _Ctrl.ClientSize.Height/2f - _arcballStartY, 0) / (float)l;
                if (s0.Length() > 1) s0.Z = 0; else s0.Z = (float)Math.Sqrt(1 - s0.LengthSquared());
                s0.Normalize();
               
                Vector3 s1 = 1.3f * 2 * new Vector3(e.X - _Ctrl.ClientSize.Width / 2f, _Ctrl.ClientSize.Height / 2f - e.Y, 0) / (float)l;
                if (s1.Length() > 1) s1.Z = 0; else s1.Z = (float)Math.Sqrt(1 - s1.LengthSquared());
                s1.Normalize();

                float angle = (float)Math.Acos(Vector3.Dot(s0, s1));
                Console.WriteLine(angle * 180 / 3.14f);
                Vector3 axis = Vector3.Cross(s0, s1); axis.Normalize();

                Matrix rot = Matrix.RotationAxis(axis, angle);

                _camera.WorldTransform = _cameraWorldTransform * Matrix.Translation(-_center) * rot * Matrix.Translation(_center);                
           
            }
            else if (e.Button == MouseButtons.Right)
            {
                // Pan
                // we translate the camera   
                // considering the object we are interested in is ~(_center) in world position
                // in order to ensure the objects stays "under" the mouse when panning

                Matrix M = _camera.ViewMatrix * _camera.ProjMatrix;
                Vector4 center = Vector3.Transform(_center, M);

                M.Invert();
                Vector4 p = new Vector4(center.X + DX * center.W, center.Y - DY * center.W, center.Z, center.W);
                Vector4 p4 = Vector4.Transform(p, M);
                p4 = p4 / p4.W;


                Vector3 translation = 2 * (_center - new Vector3(p4.X, p4.Y, p4.Z));

                // The center of the orbit is also moved
                _center += translation;


                _cameraWorldTransform = _cameraWorldTransform * Matrix.Translation(translation);
                _camera.WorldTransform = _cameraWorldTransform;

            }

            _MouseX = e.X;
            _MouseY = e.Y;
        }

        protected override void ctrl_MouseDown(object sender, MouseEventArgs e)
        {
            _MouseX = e.X;
            _MouseY = e.Y;

            _arcballStartX = e.X;
            _arcballStartY = e.Y;

            _MouseDown = true;
        }

        //
        //  The camera is placed in such a way
        //  it can display the content of the BBox 
        //  In all regards, it is also placed along the negative Z axis
        //  Looking towards the positive Z axis
        //  with a (0,1,0) Up vector 
        //

        public override void LookAt(BoundingBox BBox, LookAtDirection dir = LookAtDirection.POSITIVE_Z)
        {
            Reset();

            _bbox = BBox;

            _center = (BBox.Maximum + BBox.Minimum) / 2;
            Vector3 LMax = BBox.Maximum - BBox.Minimum;
            float lmax = Math.Max(LMax.Z, Math.Max(LMax.X, LMax.Y));

            Vector3 translation = _center;
            translation.Z = -(lmax / (float)Math.Tan(_camera.Fov) + lmax / 2 + _center.Z);

            _cameraWorldTransform = Matrix.Translation(translation);
            _camera.WorldTransform = _cameraWorldTransform;

            Matrix r = Matrix.Identity;
            if (dir == LookAtDirection.POSITIVE_Z)
            {
                r = Matrix.Identity;
            }
            else if (dir == LookAtDirection.NEGATIVE_Z)
            {
                r = Matrix.RotationY((float)Math.PI);
            }
            else if (dir == LookAtDirection.POSITIVE_X)
            {
                r = Matrix.RotationY((float)Math.PI / 2f);
            }
            else if (dir == LookAtDirection.NEGATIVE_X)
            {
                r = Matrix.RotationY(-(float)Math.PI / 2f);
            }
            else if (dir == LookAtDirection.POSITIVE_Y)
            {
                r = Matrix.Translation(-_center) * Matrix.RotationX(-(float)Math.PI / 2f) * Matrix.Translation(_center);
            }
            else if (dir == LookAtDirection.NEGATIVE_Y)
            {
                r = Matrix.Translation(-_center) * Matrix.RotationX((float)Math.PI / 2f) * Matrix.Translation(_center);
            }

            _cameraWorldTransform = Matrix.Translation(translation) * r;
            _camera.WorldTransform = _cameraWorldTransform;
        }
    }
}
