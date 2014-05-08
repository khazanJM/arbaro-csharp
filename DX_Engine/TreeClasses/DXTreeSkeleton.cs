using Arbaro2.Arbaro.Transformation;
using Arbaro2.Arbaro.Tree;
using SharpDX;
using SharpDX.Direct3D11;
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

    public class DXTreeSkeleton
    {
        private Buffer _vertexBuffer;

        public DXTreeSkeleton(Device DXDevice, CS_Tree tree) {

            DXTreeSkeleton_TreeTraversal traversal = new DXTreeSkeleton_TreeTraversal();
            tree.traverseTree(traversal);

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
