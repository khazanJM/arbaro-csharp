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

        public DXTreeSkeleton(CS_Tree tree) {

            InitShaders();

            DXTreeSkeleton_TreeTraversal traversal = new DXTreeSkeleton_TreeTraversal();
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
    }


    //
    //  Tree traversal dedicated to create skeleton
    //

    public class DXTreeSkeleton_TreeTraversal : CS_TreeTraversal
    {
        Vector3[] colors = { new Vector3(0,0,0), 
                             new Vector3(102.0f/255.0f, 196.0f/255.0f, 0), 
                             new Vector3(150.0f/255.0f, 156.0f/255.0f, 4.0f/255.0f), 
                             new Vector3(2f/255f, 73f/255f,17f/255f), 
                             new Vector3(239f/255f, 104f/255f,0)};
        public List<DXSKV> Vertices = new List<DXSKV>();
        public BoundingBox BBox = new BoundingBox(new Vector3(float.MaxValue, float.MaxValue, float.MaxValue), 
                                                  new Vector3(float.MinValue, float.MinValue, float.MinValue));

        public override bool enterStem(CS_Stem stem)
        {
            // Only generating the segments... needs to do the same for the subsegments
            foreach (CS_SegmentImpl seg in stem.getSections()) {
                DXSKV v0, v1;             
                CS_Vector cv0 = seg.getLowerPosition();
                CS_Vector cv1 = seg.getUpperPosition();              

                v0.P = new Vector3((float)cv0.getX(), (float)cv0.getY(), (float)cv0.getZ());
                v1.P = new Vector3((float)cv1.getX(), (float)cv1.getY(), (float)cv1.getZ());
                v0.C = colors[stem.getLevel()];
                v1.C = colors[stem.getLevel()];

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
            return true;
        }
    }
}
