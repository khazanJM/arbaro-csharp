using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Arbaro2.DX_Engine
{
    public class DXRendererClass : IDisposable
    {
        private DXConfigClass _config;
        private D3DClass _D3D = null;
        private DXCamera _camera= null;

        public DXRendererClass(DXConfigClass config) { _config = config; }
       
        public void RenderScene() 
        {
        }

        public void Initialize(int viewWidth, int viewHeight, IntPtr handle, Form form, DXConfigClass DXConfig)
        {
            DestroyAllResources();

            _D3D = new D3DClass();
            _D3D.Initialize(viewWidth, viewHeight, handle, form, DXConfig);
        }

        public void Resize(int viewWidth, int viewHeight, DXConfigClass DXConfig)
        {
            if (_D3D != null)
            {
                // notify everyone that size changed              
                _D3D.HandleResize(viewWidth, viewHeight, DXConfig);
            }
        }


        public void Frame()
        {
            // This is here to remove the warning about 
            // OMSetRenderTargetsAndUnorderedAccessViews: Resource being set to OM RenderTarget slot 0 is still bound on input!
            _D3D.DXDevice.ImmediateContext.PixelShader.SetShaderResources(0, 1, null as ShaderResourceView);

            _D3D.BeginScene();

            RenderScene();

            _D3D.EndScene();
        }

        private void DestroyAllResources()
        {
            if (_D3D != null) _D3D.Dispose();
        }

        public void KeyPress(KeyPressEventArgs e)
        {
            if (e.KeyChar == _config.SmartCameraKeyCode_NZ) CameraReset(DX_AXIS.NEGATIVE_Z);
            else if (e.KeyChar == _config.SmartCameraKeyCode_PZ) CameraReset(DX_AXIS.POSITIVE_Z);
            else if (e.KeyChar == _config.SmartCameraKeyCode_PX) CameraReset(DX_AXIS.POSITIVE_X);
            else if (e.KeyChar == _config.SmartCameraKeyCode_NX) CameraReset(DX_AXIS.NEGATIVE_X);
            else if (e.KeyChar == _config.SmartCameraKeyCode_PY) CameraReset(DX_AXIS.POSITIVE_Y);
            else if (e.KeyChar == _config.SmartCameraKeyCode_NY) CameraReset(DX_AXIS.NEGATIVE_Y);
        }

        private void CameraReset(DX_AXIS axis)
        {

        }

        private int _mouseX, _mouseY;
        public void MouseDown(MouseEventArgs e)
        {
            _mouseX = e.X;
            _mouseY = e.Y;
        }

        public void MouseWheel(MouseEventArgs e)
        {
            // TODO configure, enhance... too many magic numbers
            //  in non linear version... 0.1 and (d-10) should be based on
            //  bounding box size
            float zoomFactor = _config.CameraZoomFactor;
            if (_config.NonLinearCameraZoom)
            {
                float d = Math.Abs(_camera.Distance);
                zoomFactor = _config.CameraZoomFactor / (1 + 0.1f * Math.Max(0, (d - 10)));
            }

            _camera.Zoom((float)(e.Delta / zoomFactor));
        }

        public void MouseMove(MouseEventArgs e)
        {
            int Dx = e.X - _mouseX;
            int Dy = e.Y - _mouseY;

            // TODO configure, enhance
            if (e.Button == MouseButtons.Right)
            {
                // Rotate camera
                _camera.Rotate((float)(Dx / _config.CameraRotateFactor), (float)(Dy / _config.CameraRotateFactor));
            }
            else if (e.Button == MouseButtons.Middle)
            {
                // Pan camera
                _camera.Pan(Dx / _config.CameraPanFactor, Dy / _config.CameraPanFactor);
            }

            _mouseX = e.X;
            _mouseY = e.Y;
        }

        //
        // IDisposable
        //
        private bool disposed = false;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // cleanup managed stuff
                }

                // cleanup unmanaged stuff
                DestroyAllResources();

                disposed = true;
            }
        }

        ~DXRendererClass()
        {
            Dispose(false);
        }
    }
}
