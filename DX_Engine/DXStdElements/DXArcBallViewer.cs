using SharpDX;
using SharpDX.Direct3D;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Arbaro2.DX_Engine.DXMesh;

using Buffer = SharpDX.Direct3D11.Buffer;
using Arbaro2.DX_Engine.DXControls;

namespace Arbaro2.DX_Engine.DXStdElements
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DXAV
    {
        public Vector4 P; // position     
        public Vector4 C; // Color
    }

    class DXArcBallViewer : DXRenderable
    {
        private Buffer _vertexBuffer;
        private Buffer _indexBuffer;
        private int IndexCount;
        private InputElement[] _inputElements;
        private InputLayout _inputLayout;

        private DXArcBallControls _controler;

        public DXArcBallViewer(DXArcBallControls ctrl)
            : base()
        {
            _controler = ctrl;
            InitGeometry();
            InitShaders();
        }

        static float rot = 0;
        protected override void _Render(DXCamera camera)
        {
            EffectTechnique technique = _shader.DXEffect.GetTechniqueByIndex(0);
            EffectPass usePass = technique.GetPassByIndex(0);

            if (_vertexBuffer != null)
            {
                DXContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Marshal.SizeOf(typeof(DXAV)), 0));
                DXContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
                DXContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
                DXContext.InputAssembler.InputLayout = _inputLayout;

                Matrix ProjMatrix = Matrix.OrthoLH(camera.AspectRatio * 3, 3, camera.ZNear, camera.ZFar);
                Matrix ViewMatrix = camera.ViewMatrix;                

                _shader.SetParameter("worldMatrix", Matrix.Identity);
                _shader.SetParameter("viewMatrix", ViewMatrix);
                _shader.SetParameter("projectionMatrix", ProjMatrix);
                _shader.SetParameter("wvp", ViewMatrix * ProjMatrix);

                rot += 0.0001f;
                Matrix rotation = Matrix.RotationX(rot);
                Matrix translation = Matrix.Translation(camera.Target);
                _shader.SetParameter("rotation", _controler.M * translation);

                usePass.Apply(DXContext);

                DXContext.DrawIndexed(IndexCount, 0, 0);
            }


            technique.Dispose();
            usePass.Dispose();
        }

        private void InitShaders()
        {
            _shader = Program.DXShaderManager.MakeShader("ArcballViewer");

            // Create the InputElement
            _inputElements = new InputElement[]
					{
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 16, 0)
					};

            // Create the InputLayout
            EffectTechnique technique = _shader.DXEffect.GetTechniqueByIndex(0);
            EffectPass usePass = technique.GetPassByIndex(0);
            _inputLayout = new InputLayout(DXDevice, usePass.Description.Signature, _inputElements);
            technique.Dispose();
            usePass.Dispose();
        }

        private void InitGeometry()
        {
            int VCount = 60;

            List<DXAV> vertices = new List<DXAV>();

            float angle = (float)(2 * Math.PI / (float)VCount);

            for (int i = 0; i < VCount; i++)
            {
                DXAV v = new DXAV();
                v.P = new Vector4((float)Math.Cos(i * angle), (float)Math.Sin(i * angle), 0, 1);
                v.C = new Vector4(1, 0, 0, 1);
                vertices.Add(v);

                v = new DXAV();
                v.P = new Vector4((float)Math.Cos((i + 1) * angle), (float)Math.Sin((i + 1) * angle), 0, 1);
                v.C = new Vector4(1, 0, 0, 1);
                vertices.Add(v);
            }
            for (int i = 0; i < VCount; i++)
            {
                DXAV v = new DXAV();
                v.P = new Vector4((float)Math.Cos(i * angle), 0, (float)Math.Sin(i * angle), 1);
                v.C = new Vector4(0, 1, 0, 1);
                vertices.Add(v);

                v = new DXAV();
                v.P = new Vector4((float)Math.Cos((i + 1) * angle), 0, (float)Math.Sin((i + 1) * angle), 1);
                v.C = new Vector4(0, 1, 0, 1);
                vertices.Add(v);
            }
            for (int i = 0; i < VCount; i++)
            {
                DXAV v = new DXAV();
                v.P = new Vector4(0, (float)Math.Cos(i * angle), (float)Math.Sin(i * angle), 1);
                v.C = new Vector4(0, 0, 1, 1);
                vertices.Add(v);

                v = new DXAV();
                v.P = new Vector4(0, (float)Math.Cos((i + 1) * angle), (float)Math.Sin((i + 1) * angle), 1);
                v.C = new Vector4(0, 0, 1, 1);
                vertices.Add(v);
            }

            var stream = new DataStream(vertices.Count * Marshal.SizeOf(typeof(DXAV)), true, true);

            stream.WriteRange(vertices.ToArray());
            stream.Position = 0;
            _vertexBuffer = new Buffer(DXDevice, stream, new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = vertices.Count * Marshal.SizeOf(typeof(DXAV)),
                Usage = ResourceUsage.Default
            });
            stream.Dispose();

            List<UInt32> indices = new List<UInt32>();
            for (int k = 0; k < vertices.Count; k++) { indices.Add((UInt32)k); }

            stream = new DataStream(indices.Count * sizeof(UInt32), true, true);
            stream.WriteRange(indices.ToArray());

            stream.Position = 0;

            _indexBuffer = new Buffer(DXDevice, stream, new BufferDescription()
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = indices.Count * sizeof(UInt32),
                Usage = ResourceUsage.Default
            });
            stream.Dispose();

            IndexCount = indices.Count;
        }

        //
        // IDisposable
        //
        private bool disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    // cleanup managed stuff
                }

                // cleanup unmanaged stuff
                if (_vertexBuffer != null) _vertexBuffer.Dispose();
                if (_indexBuffer != null) _indexBuffer.Dispose();
                if (_inputLayout != null) _inputLayout.Dispose();

                disposed = true;
            }

            base.Dispose(disposing);
        }

        ~DXArcBallViewer()
        {
            Dispose(false);
        }
    }
}
