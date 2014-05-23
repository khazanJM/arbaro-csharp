using Arbaro2.Arbaro.Params;
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
    //  
    //
    [StructLayout(LayoutKind.Sequential)]
    public struct DXMEV
    {
        public Vector4 P; // position     
    }

    public class DXTreeMesh : DXRenderable
    {
        private Buffer _vertexBuffer, _indexBuffer;
        private int IndexCount = 0;
        private InputElement[] _inputElements;
        private InputLayout _inputLayout;

        public DXTreeMesh(CS_Tree tree, CS_Params csParams)
        {
            InitShaders();

            DXTreeMesh_TreeTraversal traversal = new DXTreeMesh_TreeTraversal(csParams);
            tree.traverseTree(traversal);
            BBox = traversal.BBox;

            // Build vertex buffer
            if (traversal.Vertices.Count == 0) return;

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

            stream = new DataStream(traversal.Indices.Count * sizeof(UInt32), true, true);
            stream.WriteRange(traversal.Indices.ToArray());

            stream.Position = 0;

            _indexBuffer = new Buffer(DXDevice, stream, new BufferDescription()
            {
                BindFlags = BindFlags.IndexBuffer,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = traversal.Indices.Count * sizeof(UInt32),
                Usage = ResourceUsage.Default
            });
            stream.Dispose();

            IndexCount = traversal.Indices.Count;
        }


        protected override void _Render(DXCamera camera)
        {
            if (_vertexBuffer == null) return;

            DXContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer, Marshal.SizeOf(typeof(DXMEV)), 0));
            DXContext.InputAssembler.SetIndexBuffer(_indexBuffer, Format.R32_UInt, 0);
            DXContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
            DXContext.InputAssembler.InputLayout = _inputLayout;

            _shader.SetParameter("worldMatrix", Matrix.Identity);
            _shader.SetParameter("viewMatrix", camera.ViewMatrix);
            _shader.SetParameter("projectionMatrix", camera.ProjMatrix);
            _shader.SetParameter("wvp", camera.ViewMatrix * camera.ProjMatrix);

            EffectTechnique technique = _shader.DXEffect.GetTechniqueByIndex(0);
            EffectPass usePass = technique.GetPassByIndex(0);
            usePass.Apply(DXContext);
            DXContext.DrawIndexed(IndexCount, 0, 0);

            usePass = technique.GetPassByIndex(1);
            usePass.Apply(DXContext);
            DXContext.DrawIndexed(IndexCount, 0, 0);

            technique.Dispose();
            usePass.Dispose();
        }

        private void InitShaders()
        {
            _shader = new DXShader("SolidWireframe");

            // Create the InputElement
            _inputElements = new InputElement[]
					{
                        new InputElement("POSITION", 0, SharpDX.DXGI.Format.R32G32B32A32_Float, 0, 0),
                        new InputElement("TEXCOORD", 0, SharpDX.DXGI.Format.R32G32_Float, 16, 0),
                        new InputElement("NORMAL", 0, SharpDX.DXGI.Format.R32G32B32_Float, 24, 0),
                        new InputElement("TANGENT", 0, SharpDX.DXGI.Format.R32G32B32_Float, 36, 0)
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

        ~DXTreeMesh()
        {
            Dispose(false);
        }
    }

    //
    //  Tree traversal dedicated to create skeleton
    //

    public class DXTreeMesh_TreeTraversal : CS_TreeTraversal
    {
        private CS_Params _csParams;

        public List<DXMEV> Vertices = new List<DXMEV>();
        public List<int> Indices = new List<int>();
        public BoundingBox BBox = new BoundingBox(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
                                                  new Vector3(float.MinValue, float.MinValue, float.MinValue));

        public DXTreeMesh_TreeTraversal(CS_Params csParams)
        {
            _csParams = csParams;
        }

        // Pb getSectionPoints retourne les points de le section "basse"
        public bool enterStem_old(CS_Stem stem)
        {
            //Console.WriteLine("enter stem");


            bool first = true;
            Vector3[] section_base = stem.getSections()[0].getSectionPoints();
            AddRangeVector3(section_base, first); first = false;

            //Console.WriteLine("stem section count "+stem.getSections().Count);
            for (int i = 0; i < stem.getSections().Count; i++)
            {
                CS_SegmentImpl seg = stem.getSections()[i];

                //Console.WriteLine("stem sub section count " + seg.getSubsegmentCount());

                if (seg.getSubsegmentCount() == 1)
                {
                    Vector3[] section_next = stem.getSections()[i].getSectionPoints();                   
                    AddRangeVector3(section_next, first);         
                }
                else
                {
                    for (int j = 0; j < seg.getSubsegmentCount(); j++)
                    {
                        CS_SubsegmentImpl subseg = seg.subsegments[j];
                        Vector3[] section_next = subseg.getSectionPoints();
                        AddRangeVector3(section_next, first);                     
                    }
                }

            }

            return true;
        }

        public override bool enterStem(CS_Stem stem)
        {
            // 1. Create the first section
            bool first = true;
            Vector3[] section_base = stem.getSections()[0].getSectionPoints();
            AddRangeVector3(section_base, first); first = false;

            // 2. for each section but the last one - generate the upper part of the section
            //      and connect it with triangles to the previous section
            //      we use the lower section of next segment as upper section of current segment
            for (int i = 0; i < stem.getSections().Count-1; i++)
            {               
                if (stem.getSections()[i].getSubsegmentCount() == 1)
                {
                    Vector3[] section_next = stem.getSections()[i + 1].getSectionPoints();
                    AddRangeVector3(section_next, first);
                }
                else {
                    CS_SegmentImpl seg = stem.getSections()[i];
                    for (int j = 0; j < seg.getSubsegmentCount(); j++)
                    {
                        CS_SubsegmentImpl subseg = seg.subsegments[j];
                        Vector3[] section_next = subseg.getSectionPoints();
                        AddRangeVector3(section_next, first);
                    }
                }
            }

            // 3. The last section I don't know how it gets calculated in initial Arbaro implementation :-(
            //      So I modified getSectionPoints to retrieve explicitely this last section
            Vector3[] section_last = stem.getSections()[stem.getSections().Count - 1].getSectionPoints(false);
            AddRangeVector3(section_last, first);  

            return true;
        }


        private void AddRangeVector3(Vector3[] v, bool isFirst)
        {
            int N = Vertices.Count - 1;
            int C = v.Count();

            foreach (Vector3 v3 in v)
            {
                DXMEV p = new DXMEV();
                p.P = new Vector4(v3.X, v3.Z, v3.Y, 1);
                Vertices.Add(p);

                BBox.Maximum = Vector3.Max(BBox.Maximum, new Vector3(p.P.X, p.P.Y, p.P.Z));                
                BBox.Minimum = Vector3.Min(BBox.Minimum, new Vector3(p.P.X, p.P.Y, p.P.Z));                
            }

            if (isFirst) { }
            else
            {
                // Create the triangles
                for (int i = 0; i < C; i++)
                {
                    // first triangle for the "quad"
                    Indices.Add(N - C + 1 + i); Indices.Add(N + i + 1);
                    if (i == C - 1) Indices.Add(N - C + 1); else Indices.Add(N - C + 2 + i);

                    // second triangle for the "quad"
                    if (i == C - 1) Indices.Add(N - C + 1); else Indices.Add(N - C + 2 + i);
                    Indices.Add(N + i + 1);
                    if (i == C - 1) Indices.Add(N + 1); else Indices.Add(N + 2 + i);
                }
            }
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
            DXMEV v0, v1, v2, v3;
            DX_Transformation transf = leaf.getTransformation();

            // each leaf is just a quad
            v0 = new DXMEV(); v0.P = _csParams.LeafScale * new Vector4(-0.5f * _csParams.LeafScaleX, 0, 0, 1);
            v1 = new DXMEV(); v1.P = _csParams.LeafScale * new Vector4(-0.5f * _csParams.LeafScaleX, 0, 0.5f, 1);
            v2 = new DXMEV(); v2.P = _csParams.LeafScale * new Vector4(0.5f * _csParams.LeafScaleX, 0, 0.5f, 1);
            v3 = new DXMEV(); v3.P = _csParams.LeafScale * new Vector4(0.5f * _csParams.LeafScaleX, 0, 0, 1);

            // the tree is caculated in openGL coordinates with Z "up" so...
            v0.P = transf.apply(v0.P); v0.P = new Vector4(v0.P.X, v0.P.Z, v0.P.Y, 1);
            v1.P = transf.apply(v1.P); v1.P = new Vector4(v1.P.X, v1.P.Z, v1.P.Y, 1);
            v2.P = transf.apply(v2.P); v2.P = new Vector4(v2.P.X, v2.P.Z, v2.P.Y, 1);
            v3.P = transf.apply(v3.P); v3.P = new Vector4(v3.P.X, v3.P.Z, v3.P.Y, 1);


            BBox.Maximum = Vector3.Max(BBox.Maximum, new Vector3(v0.P.X, v0.P.Y, v0.P.Z));
            BBox.Maximum = Vector3.Max(BBox.Maximum, new Vector3(v1.P.X, v1.P.Y, v1.P.Z));
            BBox.Maximum = Vector3.Max(BBox.Maximum, new Vector3(v2.P.X, v2.P.Y, v2.P.Z));
            BBox.Maximum = Vector3.Max(BBox.Maximum, new Vector3(v3.P.X, v3.P.Y, v3.P.Z));

            BBox.Minimum = Vector3.Min(BBox.Minimum, new Vector3(v0.P.X, v0.P.Y, v0.P.Z));
            BBox.Minimum = Vector3.Min(BBox.Minimum, new Vector3(v1.P.X, v1.P.Y, v1.P.Z));
            BBox.Minimum = Vector3.Min(BBox.Minimum, new Vector3(v2.P.X, v2.P.Y, v2.P.Z));
            BBox.Minimum = Vector3.Min(BBox.Minimum, new Vector3(v3.P.X, v3.P.Y, v3.P.Z));

            int c = Vertices.Count;
            Indices.Add(c); Indices.Add(c + 1); Indices.Add(c + 3);
            Indices.Add(c + 3); Indices.Add(c + 1); Indices.Add(c + 2);

            Vertices.Add(v0);
            Vertices.Add(v1);
            Vertices.Add(v2);
            Vertices.Add(v3);



            return true;
        }
    }
}
