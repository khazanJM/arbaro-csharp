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
        protected D3DClass _D3D = null;

        public DXRendererClass() { }
       
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
