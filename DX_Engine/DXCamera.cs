using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//  Loosely based on ThreeJS camera
//  The camera has to handle a "focal point"
//  So it is defined by 
//      it's fov (camera opening)
//      its position in space
//      its target (the actual point in 3D space the camera is focused on)
//      The look_at normalized vector is basical (Target - Position).normalize
//      Up and left vectors are initialized using (0,1,0) for "initial" up
// 
//  The camera can be controled by changing its position or target point
//  To do more than that, you need a CameraControler (like the OrbitControler)
//

namespace Arbaro2.DX_Engine
{
    public class DXCamera
    {
        private float _fov = (float)(45*Math.PI/180.0), _aspectRatio = 1, _znear = 0.1f, _zfar = 2000;
        private float _width = 1, _height = 1;
        private Matrix _viewMatrix, _projMatrix;

        private bool _ortho = false;
        public bool Orthographic { get { return _ortho; } set { _ortho = value; UpdateMatrices(); } }
        public bool Perspective { get { return !_ortho; } set { _ortho = !value; UpdateMatrices(); } }

        private void UpdateMatrices() {
            if (_ortho) _projMatrix = Matrix.OrthoLH(_width, _height, _znear, _zfar);
            else _projMatrix = Matrix.PerspectiveFovLH(_fov, _aspectRatio, _znear, _zfar);
            _viewMatrix = Matrix.LookAtLH(_position, _target, _up);
        }

        private Vector3 _position = new Vector3(0,0,0);
        public Vector3 Position { get { return _position; } set { _position = value; UpdateMatrices(); } }

        private Vector3 _target = new Vector3(0,0,float.MaxValue);
        public Vector3 Target { 
            get { return _target; } 
            set { 
                _target = value;
                UpdateMatrices();
            } 
        }

        private Vector3 _up = new Vector3(0, 1, 0);
        public Vector3 Up { 
            get { 
                Vector4 u = new Vector4(0, 1, 0, 0); 
                u = Vector4.Transform(u, _viewMatrix);
                return new Vector3(u.X, u.Y, u.Z);
            }
            set {
                _up = value;
            }
        }

        public Vector3 Left
        {
            get
            {
                Vector4 l = new Vector4(1, 0, 0, 0);
                l = Vector4.Transform(l, _viewMatrix);
                return new Vector3(l.X, l.Y, l.Z);
            }
        }

        public float Fov { get { return _fov; } set { _fov = value; UpdateMatrices(); } }
        public float AspectRatio { get { return _aspectRatio; } set { 
            _aspectRatio = value; 
            UpdateMatrices(); } 
        }
        public float ZNear { get { return _znear; } set { _znear = value; UpdateMatrices(); } }
        public float ZFar { get { return _zfar; } set { _zfar = value; UpdateMatrices(); } }
        public float Width { get { return _width; } set { _width = value; UpdateMatrices(); } }
        public float Height { get { return _height; } set { _height = value; UpdateMatrices(); } }

        // Look at changes the focal point
        public void LookAt(Vector3 lookAt) {
            throw new Exception("DXCamera.LookAt not implemented");
        }

        public Matrix ViewMatrix { get { return _viewMatrix; } }
        public Matrix ProjMatrix { get { return _projMatrix; } }

        private DXCamera(float a, float b, float c, float d, bool persp)
        {
            if (persp)
            {
                _fov = a;
                _znear = c;
                _zfar = d;
                _aspectRatio = b;
            }
            else {
                _width = a;
                _znear = c;
                _zfar = d;
                _height = b;
            }

            UpdateMatrices();
        }

        // Create a perspective camera
        public static DXCamera DXCameraPerspective(float fov, float aspect, float near, float far) 
        {           
            return new DXCamera(fov, aspect, near, far, true);         
        }

        // Creates an ortho camera
        public static DXCamera DXCameraOrtho(float width, float height, float near, float far)
        {
            return new DXCamera(width, height, near, far, false);           
        }
    }
}
