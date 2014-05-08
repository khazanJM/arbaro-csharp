using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using Device = SharpDX.Direct3D11.Device;

namespace Arbaro2.DX_Engine
{
    public enum DX_AXIS { POSITIVE_X, NEGATIVE_X, POSITIVE_Y, NEGATIVE_Y, POSITIVE_Z, NEGATIVE_Z };

    public class D3DClass : IDisposable
    {
        private DXConfigClass _DXConfig;

        private Device _device;
        public Device DXDevice { get { return _device; } }

        private IntPtr _handle;
        public IntPtr Handle { get { return _handle; } }

        private Form _form;
        public Form Form { get { return _form; } }

        private int _viewWidth;
        public int ViewWidth { get { return _viewWidth; } }

        private int _viewHeight;
        public int ViewHeight { get { return _viewHeight; } }

        private Matrix _projectionMatrix;
        public Matrix ProjectionMatrix { get { return _projectionMatrix; } }

        private Matrix _worldMatrix;
        public Matrix WorldMatrix { get { return _worldMatrix; } }

        private Matrix _orthoMatrix;
        public Matrix OrthoMatrix { get { return _orthoMatrix; } }

        private SwapChain _swapChain = null;
        private DeviceContext _context = null;
        private RenderTargetView _renderTargetView = null;
        private Texture2D _depthStencilBuffer = null;

        private DepthStencilView _depthStencilView = null;
        public DepthStencilView DXDepthStencilView { get { return _depthStencilView; } }

        private DepthStencilState _depthStencilState = null;
        private BlendState _alphaEnableBlendingState = null;
        private BlendState _alphaDisabledBlendingState = null;

        public D3DClass()
        {
        }

        //
        //  Initialize        
        //  
        public void Initialize(int viewWidth, int viewHeight, IntPtr handle, Form form, DXConfigClass DXConfig)
        {
            _DXConfig = DXConfig;
            _handle = handle;
            _form = form;
            _viewWidth = viewWidth;
            _viewHeight = viewHeight;

            SwapChainDescription swapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                ModeDescription = new ModeDescription(viewWidth, viewHeight, new Rational(60, 1), Format.R8G8B8A8_UNorm),
                IsWindowed = true,
                OutputHandle = handle,
                SampleDescription = new SampleDescription(DXConfig.MSAA_SampleCount, DXConfig.MSAA_SampleDesc),
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

#if DEBUG
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.Debug, swapChainDesc, out _device, out _swapChain);
#else
            Device.CreateWithSwapChain(DriverType.Hardware, DeviceCreationFlags.None, swapChainDesc, out _device, out _swapChain);
#endif
            _context = _device.ImmediateContext;

            // Ignore all windows events
            Factory factory = _swapChain.GetParent<Factory>();
            factory.MakeWindowAssociation(handle, WindowAssociationFlags.IgnoreAll);

            HandleResize(viewWidth, viewHeight, DXConfig);
        }

        public void SetBackBufferRenderTarget()
        {
            // Bind the render target view and depth stencil buffer to the output render pipeline.
            _device.ImmediateContext.OutputMerger.SetRenderTargets(_depthStencilView, _renderTargetView);
            _device.ImmediateContext.Rasterizer.SetViewport(new Viewport(0, 0, _viewWidth, _viewHeight, 0.0f, 1.0f));
        }

        // end of init... and whatever is required to handel window resize
        public void HandleResize(int viewWidth, int viewHeight, DXConfigClass DXConfig)
        {
            // happens when window is minimized
            if (viewWidth * viewHeight == 0) return;

            _viewWidth = viewWidth;
            _viewHeight = viewHeight;

            // dispose whatever needs to be disposed first
            if (_renderTargetView != null) _renderTargetView.Dispose();
            if (_depthStencilBuffer != null) _depthStencilBuffer.Dispose();
            if (_depthStencilView != null) _depthStencilView.Dispose();

            // Resize the backbuffer
            _swapChain.ResizeBuffers(1, viewWidth, viewHeight, Format.R8G8B8A8_UNorm, SwapChainFlags.None);

            // Get the backbuffer from the swapchain
            var backBuffer = Texture2D.FromSwapChain<Texture2D>(_swapChain, 0);

            // RenderTargetView on the backbuffer
            _renderTargetView = new RenderTargetView(_device, backBuffer);
            _renderTargetView.DebugName = "MainRenderView";

            backBuffer.Dispose();

            // Create the depth buffer
            _depthStencilBuffer = new Texture2D(_device, new Texture2DDescription()
            {
                Format = Format.D32_Float,
                ArraySize = 1,
                MipLevels = 1,
                Width = viewWidth,
                Height = viewHeight,
                SampleDescription = new SampleDescription(DXConfig.MSAA_SampleCount, DXConfig.MSAA_SampleDesc),
                Usage = ResourceUsage.Default,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None
            });
            _depthStencilBuffer.DebugName = "MainDepthBuffer";

            _depthStencilView = new DepthStencilView(_device, _depthStencilBuffer);

            _context.OutputMerger.SetTargets(_depthStencilView, _renderTargetView);
            _context.Rasterizer.SetViewport(new Viewport(0, 0, viewWidth, viewHeight, 0.0f, 1.0f));

            // Setup the projection matrix.           
            float screenAspect = (float)viewWidth / (float)viewHeight;

            // Create the projection matrix for 3D rendering.
            _projectionMatrix = Matrix.PerspectiveFovLH(_DXConfig.FOV, screenAspect, _DXConfig.ScreenNear, _DXConfig.ScreenDepth);
            _worldMatrix = Matrix.Identity;
            _orthoMatrix = Matrix.OrthoLH(viewWidth, viewHeight, _DXConfig.ScreenNear, _DXConfig.ScreenDepth);
        }

        public void BeginScene()
        {
            _context.ClearRenderTargetView(_renderTargetView, _DXConfig.BackgroundColor);
            _context.ClearDepthStencilView(_depthStencilView, DepthStencilClearFlags.Depth, 1.0f, 0);
        }

        public void EndScene()
        {
            _swapChain.Present(_DXConfig.VSync ? 1 : 0, PresentFlags.None);
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

        private void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // cleanup managed stuff
                }

                // cleanup unmanaged stuff
                if (_device != null) _device.Dispose();
                if (_swapChain != null) _swapChain.Dispose();
                if (_renderTargetView != null) _renderTargetView.Dispose();
                if (_depthStencilBuffer != null) _depthStencilBuffer.Dispose();
                if (_depthStencilView != null) _depthStencilView.Dispose();
                if (_depthStencilState != null) _depthStencilState.Dispose();
                if (_alphaEnableBlendingState != null) _alphaEnableBlendingState.Dispose();
                if (_alphaDisabledBlendingState != null) _alphaDisabledBlendingState.Dispose();

                _device = null;
                _swapChain = null;
                _renderTargetView = null;
                _depthStencilView = null;
                _depthStencilBuffer = null;

                disposed = true;
            }
        }

        ~D3DClass()
        {
            Dispose(false);
        }
    }
}
