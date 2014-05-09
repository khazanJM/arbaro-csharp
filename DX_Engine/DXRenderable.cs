using SharpDX;
using SharpDX.Direct3D11;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.DX_Engine
{
    public abstract class DXRenderable : IDisposable
    {
        public BoundingBox BBox = new BoundingBox(Vector3.Zero, Vector3.Zero);
        public bool Visible = true;

        protected DXShader _shader;

        protected abstract void _Render(DXCamera camera);

        public Device DXDevice { get { return Program.Renderer.DXDevice; } }
        public DeviceContext DXContext { get { return Program.Renderer.DXDevice.ImmediateContext; } }

        public void Render(DXCamera camera) {
            if(Visible) _Render(camera);
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

                disposed = true;
            }
        }

        ~DXRenderable()
        {
            Dispose(false);
        }
    }
}
