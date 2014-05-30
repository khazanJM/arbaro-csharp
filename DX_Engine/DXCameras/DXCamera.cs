﻿using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.DX_Engine.DXCameras
{
    //
    //  The camera is here to store the view and projection matrices
    //  alongside with near and far distances...
    //  A controler is required to do anything remoely useful with a camera.
    //

    public abstract class DXCamera
    {
        protected abstract void UpdateProjectionMatrix();

        protected float _fov = (float)(45 * Math.PI / 180.0), _aspectRatio = 1, _znear = 0.1f, _zfar = 2000;
        protected Matrix _viewMatrix, _projMatrix;

        public float Fov { get { return _fov; } set { _fov = value; UpdateProjectionMatrix(); } }

        public float AspectRatio
        {
            get { return _aspectRatio; }
            set
            {
                _aspectRatio = value;
                UpdateProjectionMatrix();
            }
        }

        public float ZNear { get { return _znear; } set { _znear = value; UpdateProjectionMatrix(); } }
        public float ZFar { get { return _zfar; } set { _zfar = value; UpdateProjectionMatrix(); } }

        public Matrix ViewMatrix { get { return _viewMatrix; } set { _viewMatrix = value; } }
        public Matrix ProjMatrix { get { return _projMatrix; } }

        public DXCamera(float fov, float width, float height, float near, float far)
        {
            _fov = fov;
            _aspectRatio = width / height;
            _znear = near;
            _zfar = far;
        }
    }

    public class DXPerspectiveCamera : DXCamera
    {
        public DXPerspectiveCamera(float fov, float width, float height, float near, float far)
            : base(fov, width, height, near, far)
        {
        }

        protected override void UpdateProjectionMatrix()
        {
            _projMatrix = Matrix.PerspectiveFovLH(_fov, _aspectRatio, _znear, _zfar);
        }
    }

    public class DXOrthographicCamera : DXCamera
    {
        private float _width = 1, _height = 1;

        public float Width { get { return _width; } set { _width = value; UpdateProjectionMatrix(); } }
        public float Height { get { return _height; } set { _width = value; UpdateProjectionMatrix(); } }

        public DXOrthographicCamera(float fov, float width, float height, float near, float far) : base(fov, width, height, near, far) 
        {
            _width = width;
            _height = height;
        }

        protected override void UpdateProjectionMatrix()
        {
            Matrix.OrthoLH(_width, _height, _znear, _zfar);
        }
    }
}
