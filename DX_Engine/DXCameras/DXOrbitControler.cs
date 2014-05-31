using Arbaro2.DX_Engine.DXCameras;
using Arbaro2.Utilities;
using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Arbaro2.DX_Engine.DXControls
{
    //
    //  An orbit camera arbit around a given point in space
    //  this point can be changed by panning the camera
    // 
    // 

    public class DXOrbitControler : DXBaseCameraControler
    {
        private Matrix _cameraWorldTransform;
        private Vector3 _center;

        // ctrl is the control hooked for events (mouse & keyboard)
        public DXOrbitControler(DXCamera camera, Control ctrl)
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

                // DX generate a rotation around the camera Y axis
                // DY around the camera X axis                                          
                Vector4 xAxis = new Vector4(1, 0, 0, 0); xAxis = Vector4.Transform(xAxis, _cameraWorldTransform);

                Matrix rotY = Matrix.RotationAxis(new Vector3(0, 1, 0), DX);
                Matrix rotX = Matrix.RotationAxis(new Vector3(xAxis.X, xAxis.Y, xAxis.Z), -DY);
                Matrix rot = rotY * rotX;

                Matrix M = _cameraWorldTransform * Matrix.Translation(-_center) * rot * Matrix.Translation(_center);
                _cameraWorldTransform = M;
                
                _camera.WorldTransform = _cameraWorldTransform;
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

                
                Vector3 translation = 2*(_center - new Vector3(p4.X, p4.Y, p4.Z));
                
                // The center of the orbit is also moved
                _center += translation;


                _cameraWorldTransform = _cameraWorldTransform * Matrix.Translation(translation);
                _camera.WorldTransform = _cameraWorldTransform;
                 
            }
        }

        protected override void ctrl_MouseDown(object sender, MouseEventArgs e)
        {
            _MouseX = e.X;
            _MouseY = e.Y;
            _MouseDown = true;
        }

        //
        //  The camera is placed in such a way
        //  it can display the content of the BBox 
        //  In all regards, it is also placed along the negative Z axis
        //  Looking towards the positive Z axis
        //  with a (0,1,0) Up vector 
        //
        public override void LookAt(BoundingBox BBox)
        {          
            _center = (BBox.Maximum + BBox.Minimum) / 2;            
            Vector3 LMax = BBox.Maximum - BBox.Minimum;
            float lmax = Math.Max(LMax.Z, Math.Max(LMax.X, LMax.Y));

            Vector3 translation = _center; 
            translation.Z = -(lmax/(float)Math.Tan(_camera.Fov) + lmax/2 + _center.Z);
            
            _cameraWorldTransform = Matrix.Translation(translation);
            _camera.WorldTransform = _cameraWorldTransform;
        }
    }
}
