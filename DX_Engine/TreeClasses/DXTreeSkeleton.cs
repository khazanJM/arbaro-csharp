﻿using Arbaro2.Arbaro.Params;
using Arbaro2.Arbaro.Transformation;
using Arbaro2.Arbaro.Tree;
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
using Buffer = SharpDX.Direct3D11.Buffer;

namespace Arbaro2.DX_Engine.TreeClasses
{
    //
    //  Each segment will be made of 2 points + a color for each level
    //
    [StructLayout(LayoutKind.Sequential)]
    public struct DXSKV
    {
        public Vector3 P; // position
        public Vector3 C; // color
    }

    //
    //  Build a DX set of lines representing all the segments
    //  and subsegments of the tree
    //

    public class DXTreeSkeleton : DXRenderable
    {
        private Buffer _vertexBuffer, _indexBuffer;
        private int IndexCount = 0;
        private InputElement[] _inputElements;
        private InputLayout _inputLayout;

        public DXTreeSkeleton(CS_Tree tree, CS_Params csParams)
        {

            InitShaders();

            DXTreeSkeleton_TreeTraversal traversal = new DXTreeSkeleton_TreeTraversal(csParams);
            tree.traverseTree(traversal);
            BBox = traversal.BBox;

            // Build vertex buffer
            var stream = new DataStream(traversal.Vertices.Count * Marshal.SizeOf(typeof(DXSKV)), true, true);

            stream.WriteRange(traversal.Vertices.ToArray());
            stream.Position = 0;
            _vertexBuffer = new Buffer(DXDevice, stream, new BufferDescription()
            {
                BindFlags = BindFlags.VertexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = traversal.Vertices.Count * Marshal.SizeOf(typeof(DXSKV)),
                Usage = ResourceUsage.Default
            });
            stream.Dispose();

            List<UInt32> indices = new List<UInt32>();
            for (int i = 0; i < traversal.Vertices.Count; i++) { indices.Add((UInt32)i); }

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

        protected override void _Render(DXCamera camera)
        {
            DXContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Marshal.SizeOf(typeof(DXSKV)), 0));
            DXContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            DXContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
            DXContext.InputAssembler.InputLayout = _inputLayout;

            _shader.SetParameter("worldMatrix", Matrix.Identity);
            _shader.SetParameter("viewMatrix", camera.ViewMatrix);
            _shader.SetParameter("projectionMatrix", camera.ProjMatrix);
            _shader.SetParameter("wvp", camera.ViewMatrix * camera.ProjMatrix);

            EffectTechnique technique = _shader.DXEffect.GetTechniqueByIndex(0);
            EffectPass usePass = technique.GetPassByIndex(0);
            usePass.Apply(DXContext);

            DXContext.DrawIndexed(IndexCount, 0, 0);

            technique.Dispose();
            usePass.Dispose();
        }

        private void InitShaders()
        {
            _shader = new DXShader("TreeSkeleton");

            // Create the InputElement
            _inputElements = new InputElement[]
					{
						new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32_Float, 0, 0),
                        new InputElement("COLOR", 0, SharpDX.DXGI.Format.R32G32B32_Float, 12, 0)
					};

            // Create the InputLayout
            EffectTechnique technique = _shader.DXEffect.GetTechniqueByIndex(0);
            EffectPass usePass = technique.GetPassByIndex(0);
            _inputLayout = new InputLayout(DXDevice, usePass.Description.Signature, _inputElements);
            technique.Dispose();
            usePass.Dispose();
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

        ~DXTreeSkeleton()
        {
            Dispose(false);
        }
    }


    //
    //  Tree traversal dedicated to create skeleton
    //

    public class DXTreeSkeleton_TreeTraversal : CS_TreeTraversal
    {
        private CS_Params _csParams;

        Vector3[] colors = { new Vector3(0,0,0), 
                             new Vector3(63/255.0f, 29.0f/255.0f, 11.0f/255.0f), 
                             new Vector3(133.0f/255.0f, 84.0f/255.0f, 57.0f/255.0f), 
                             new Vector3(176f/255f, 167f/255f,110f/255f), 
                             new Vector3(207f/255f, 104f/255f,0),
                             new Vector3(5f/255f, 96f/255f,1f/255f)};

        public List<DXSKV> Vertices = new List<DXSKV>();
        public BoundingBox BBox = new BoundingBox(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
                                                  new Vector3(float.MinValue, float.MinValue, float.MinValue));

        public DXTreeSkeleton_TreeTraversal(CS_Params csParams) {
            _csParams = csParams;
        }

        public override bool enterStem(CS_Stem stem)
        {
            // Only generating the segments... needs to do the same for the subsegments
            foreach (CS_SegmentImpl seg in stem.getSections())
            {
                DXSKV v0, v1;
                Vector3 cv0 = seg.getLowerPosition();
                Vector3 cv1 = seg.getUpperPosition();

                // the tree is caculated in openGL coordinates with Z "up" so...
                v0.P = new Vector3(cv0.X, cv0.Z, cv0.Y);
                v1.P = new Vector3(cv1.X, cv1.Z, cv1.Y);
                v0.C = colors[Math.Min(3, stem.getLevel())];
                v1.C = colors[Math.Min(3, stem.getLevel())];

                BBox.Maximum = Vector3.Max(BBox.Maximum, v0.P);
                BBox.Maximum = Vector3.Max(BBox.Maximum, v1.P);
                BBox.Minimum = Vector3.Min(BBox.Minimum, v0.P);
                BBox.Minimum = Vector3.Min(BBox.Minimum, v1.P);

                Vertices.Add(v0);
                Vertices.Add(v1);
            }

            return true;
        }

        public override bool enterTree(CS_Tree tree)
        {
            //Console.WriteLine("enter tree");
            return true;
        }

        public override bool leaveStem(CS_Stem stem)
        {
            return true;
        }

        public override bool leaveTree(CS_Tree tree)
        {
            //Console.WriteLine("leave tree");
            return true;
        }

        public override bool visitLeaf(CS_Leaf leaf)
        {
            DXSKV v0, v1;
            DX_Transformation transf = leaf.getTransformation();


            // the tree is caculated in openGL coordinates with Z "up" so...
            v0.P = new Vector3(0, 0, 0);
            v1.P = new Vector3(0, 0, _csParams.LeafScale);
            v0.P = transf.apply(v0.P); v0.P = new Vector3(v0.P.X, v0.P.Z, v0.P.Y);
            v1.P = transf.apply(v1.P); v1.P = new Vector3(v1.P.X, v1.P.Z, v1.P.Y);
            v0.C = colors[5];
            v1.C = colors[5];

            BBox.Maximum = Vector3.Max(BBox.Maximum, v0.P);
            BBox.Maximum = Vector3.Max(BBox.Maximum, v1.P);
            BBox.Minimum = Vector3.Min(BBox.Minimum, v0.P);
            BBox.Minimum = Vector3.Min(BBox.Minimum, v1.P);

            Vertices.Add(v0);
            Vertices.Add(v1);


            return true;
        }
    }
}
