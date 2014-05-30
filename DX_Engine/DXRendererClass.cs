using Arbaro2.DX_Engine.DXControls;
using Arbaro2.DX_Engine.DXStdElements;
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
        
        public DXCamera Camera= null;
        public DXBaseControls CameraControler = null;

        public Dictionary<string, DXRenderable> RenderableList = new Dictionary<string, DXRenderable>();

        public Device DXDevice { get { return _D3D.DXDevice; } }

        public DXRendererClass(DXConfigClass config)
        { 
            _config = config;           
        }
       
        public void RenderScene() 
        {
            Program.DXShaderManager.Refresh();

            _D3D.SetBackBufferRenderTarget();
            foreach (DXRenderable r in RenderableList.Values)
                r.Render(Camera);
        }

        public void Initialize(int viewWidth, int viewHeight, IntPtr handle, Form form, DXConfigClass DXConfig)
        {
            DestroyAllResources();

            _D3D = new D3DClass();
            _D3D.Initialize(viewWidth, viewHeight, handle, form, DXConfig);
            Camera = DXCamera.DXCameraPerspective(_config.FOV, (float)viewWidth/(float)viewHeight, _config.ScreenNear, _config.ScreenDepth);
            //CameraControler = new DXOrbitControls(Camera, (form as ArbaroMainForm).renderCtrl);
            CameraControler = new DXArcBallControls(Camera, (form as ArbaroMainForm).renderCtrl);

            if (CameraControler is DXArcBallControls) { 
                // Add an arcball viewer
                RenderableList.Add("ArcballViewer", new DXArcBallViewer(CameraControler as DXArcBallControls));
            }
            else
                RenderableList.Add("ArcballViewer", new DXArcBallViewer(new DXArcBallControls(Camera, (form as ArbaroMainForm).renderCtrl)));
        }

        public void Resize(int viewWidth, int viewHeight, DXConfigClass DXConfig)
        {
            if (_D3D != null)
            {
                float aspect = (float)viewWidth / (float)viewHeight;
                Camera.AspectRatio = aspect;

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
