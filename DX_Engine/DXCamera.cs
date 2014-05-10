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
//      The look_at normalized vector is basical Target - Position
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
        }

        private Vector3 _position = new Vector3(0,0,0);
        public Vector3 Position { get { return _position; } }

        private Vector3 _target = new Vector3(0,0,float.MaxValue);
        public Vector3 Target { get { return _target; } }

        private Vector3 _up = new Vector3(0, 0, float.MaxValue);
        public Vector3 Up { get { return _up; } }

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
        // and in fact is not that easy... todo
        public void LookAt(Vector3 lookAt) {
            throw new Exception("DXCamera.LookAt not implemented");
        }

        public void ETU(Vector3 eye, Vector3 target, Vector3 up) {

            _position = eye;
            _target = target;
            _viewMatrix = Matrix.LookAtLH(_position, target, up);
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
