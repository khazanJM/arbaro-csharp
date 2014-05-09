using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//  Loosely based on ThreeJS camera
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

        private Vector3 _position;
        public Vector3 Position { get { return _position; } set { _position = value; UpdateMatrices(); } }

        public float Fov { get { return _fov; } set { _fov = value; UpdateMatrices(); } }
        public float AspectRatio { get { return _aspectRatio; } set { 
            _aspectRatio = value; 
            UpdateMatrices(); } 
        }
        public float ZNear { get { return _znear; } set { _znear = value; UpdateMatrices(); } }
        public float ZFar { get { return _zfar; } set { _zfar = value; UpdateMatrices(); } }
        public float Width { get { return _width; } set { _width = value; UpdateMatrices(); } }
        public float Height { get { return _height; } set { _height = value; UpdateMatrices(); } }

        public void LookAt(Vector3 lookAt) {
            _viewMatrix = Matrix.LookAtLH(_position, lookAt, new Vector3(0,1,0));
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

        // Makes it such the Bounding box is "fully" within the camera frustrum.
        // basically the camera is placed in sucha a way that the bounding box 
        // will stay in the camera frustrum wherever the camera orbit to
        public void LookAt(BoundingBox BBox) 
        { 
            Vector3 lengths = BBox.Maximum - BBox.Minimum;
            float maxLen = Math.Max(lengths.Z, Math.Max(lengths.X, lengths.Y));
            maxLen = Math.Max(2 * _znear, maxLen);

            Vector3 p = (BBox.Maximum + BBox.Minimum) / 2.0f;
            float distance = (1.05f*maxLen/2.0f) / (float)Math.Tan(_fov/2.0);
            
            _position = p; _position.Z -= distance;
            LookAt(p);
        }
    }
}
