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
using Arbaro2.DX_Engine.DXCameras;

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
        private InputElement[] _inputElements;
        private InputLayout _inputLayout;

        private Buffer[] _vertexBuffer2 = { null, null, null, null, null };
        private Buffer[] _indexBuffer2 = { null, null, null, null, null };
        private int[] IndexCount2 = { 0, 0, 0, 0, 0 };

        public DXTreeSkeleton(CS_Tree tree, CS_Params csParams)
        {
            InitShaders();

            DXTreeSkeleton_TreeTraversal traversal = new DXTreeSkeleton_TreeTraversal(csParams);
            tree.traverseTree(traversal);
            BBox = traversal.BBox;

            for (int i = 0; i < 5; i++)
            {
                if (traversal.Vertices2[i].Count != 0)
                {

                    var stream = new DataStream(traversal.Vertices2[i].Count * Marshal.SizeOf(typeof(DXSKV)), true, true);

                    stream.WriteRange(traversal.Vertices2[i].ToArray());
                    stream.Position = 0;
                    _vertexBuffer2[i] = new Buffer(DXDevice, stream, new BufferDescription()
                    {
                        BindFlags = BindFlags.VertexBuffer,
                        CpuAccessFlags = CpuAccessFlags.None,
                        OptionFlags = ResourceOptionFlags.None,
                        SizeInBytes = traversal.Vertices2[i].Count * Marshal.SizeOf(typeof(DXSKV)),
                        Usage = ResourceUsage.Default
                    });
                    stream.Dispose();

                    List<UInt32> indices = new List<UInt32>();
                    for (int k = 0; k < traversal.Vertices2[i].Count; k++) { indices.Add((UInt32)k); }

                    stream = new DataStream(indices.Count * sizeof(UInt32), true, true);
                    stream.WriteRange(indices.ToArray());

                    stream.Position = 0;

                    _indexBuffer2[i] = new Buffer(DXDevice, stream, new BufferDescription()
                    {
                        BindFlags = BindFlags.IndexBuffer,
                        CpuAccessFlags = CpuAccessFlags.None,
                        OptionFlags = ResourceOptionFlags.None,
                        SizeInBytes = indices.Count * sizeof(UInt32),
                        Usage = ResourceUsage.Default
                    });
                    stream.Dispose();

                    IndexCount2[i] = indices.Count;
                }
            }
        }
      
        protected override void _Render(DXCamera camera)
        {
            EffectTechnique technique = _shader.DXEffect.GetTechniqueByIndex(0);
            EffectPass usePass = technique.GetPassByIndex(0);

            for (int i = 0; i < 5; i++)
            {
                if (_vertexBuffer2[i] != null && Program.DXSceneOptions.LevelVisibility[i])
                {
                    DXContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer2[i], Marshal.SizeOf(typeof(DXSKV)), 0));
                    DXContext.InputAssembler.SetIndexBuffer(_indexBuffer2[i], Format.R32_UInt, 0);
                    DXContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.LineList;
                    DXContext.InputAssembler.InputLayout = _inputLayout;

                    _shader.SetParameter("worldMatrix", Matrix.Identity);
                    _shader.SetParameter("viewMatrix", camera.ViewMatrix);
                    _shader.SetParameter("projectionMatrix", camera.ProjMatrix);
                    _shader.SetParameter("vp", camera.ViewMatrix * camera.ProjMatrix);                   

                    usePass.Apply(DXContext);

                    DXContext.DrawIndexed(IndexCount2[i], 0, 0);
                }
            }



            technique.Dispose();
            usePass.Dispose();
        }

        private void InitShaders()
        {
            _shader = Program.DXShaderManager.MakeShader("TreeSkeleton");

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
                for (int i = 0; i < 5; i++)
                {
                    if (_vertexBuffer2[i] != null) _vertexBuffer2[i].Dispose();
                    if (_indexBuffer2[i] != null) _indexBuffer2[i].Dispose();
                }
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
        private int LEAFLEVEL = 4;
        private CS_Params _csParams;

        Vector3[] colors = { new Vector3(0,0,0), 
                             new Vector3(63/255.0f, 29.0f/255.0f, 11.0f/255.0f), 
                             new Vector3(133.0f/255.0f, 84.0f/255.0f, 57.0f/255.0f), 
                             new Vector3(176f/255f, 167f/255f,110f/255f), 
                             new Vector3(207f/255f, 104f/255f,0),
                             new Vector3(5f/255f, 96f/255f,1f/255f)};
      
        public List<DXSKV>[] Vertices2 = { new List<DXSKV>(), new List<DXSKV>(), new List<DXSKV>(), new List<DXSKV>(), new List<DXSKV>() };
        public BoundingBox BBox = new BoundingBox(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue),
                                                  new Vector3(float.MinValue, float.MinValue, float.MinValue));

        public DXTreeSkeleton_TreeTraversal(CS_Params csParams)
        {
            _csParams = csParams;
        }

        public override bool enterStem(CS_Stem stem)
        {
            foreach (CS_SegmentImpl seg in stem.getSections())
            {
                if (seg.getSubsegmentCount() == 1)
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
                
                    Vertices2[Math.Min(stem.getLevel(),3)].Add(v0);
                    Vertices2[Math.Min(stem.getLevel(),3)].Add(v1);
                }
                else
                {
                    Vector3 currPos = seg.getLowerPosition();

                    for (int subindex = 0; subindex < seg.getSubsegmentCount(); subindex++)
                    {
                        DXSKV v0, v1;

                        v0.P = new Vector3(currPos.X, currPos.Z, currPos.Y);

                        v1.P = new Vector3(0, 0, 0);
                        v1.P = seg.subsegments[subindex].getTransformation().apply(v1.P);
                        currPos = v1.P;
                        v1.P = new Vector3(v1.P.X, v1.P.Z, v1.P.Y);

                        v0.C = colors[Math.Min(3, stem.getLevel())];
                        v1.C = colors[Math.Min(3, stem.getLevel())];                  

                        Vertices2[Math.Min(stem.getLevel(),3)].Add(v0);
                        Vertices2[Math.Min(stem.getLevel(),3)].Add(v1);


                        BBox.Maximum = Vector3.Max(BBox.Maximum, v0.P);
                        BBox.Maximum = Vector3.Max(BBox.Maximum, v1.P);
                        BBox.Minimum = Vector3.Min(BBox.Minimum, v0.P);
                        BBox.Minimum = Vector3.Min(BBox.Minimum, v1.P);
                    }
                }
            }

            return true;
        }

        public override bool enterTree(CS_Tree tree)
        {            
            return true;
        }

        public override bool leaveStem(CS_Stem stem)
        {
            return true;
        }

        public override bool leaveTree(CS_Tree tree)
        {         
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

            Vertices2[LEAFLEVEL].Add(v0);
            Vertices2[LEAFLEVEL].Add(v1);

            return true;
        }
    }
}
