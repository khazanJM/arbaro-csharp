using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbaro2.DX_Engine.DXTreeMesh;
using Arbaro2.Arbaro.Tree;
using Arbaro2.Arbaro.Params;


using DXBaseArbaroTreeMesh = Arbaro2.DX_Engine.DXMesh.DXMesh<Arbaro2.DX_Engine.DXTreeMesh.DXArbaroNullTrait, 
                                                             Arbaro2.DX_Engine.DXTreeMesh.DXArbaroNullTrait, 
                                                             Arbaro2.DX_Engine.DXTreeMesh.DXArbaroHalfedgeTrait, 
                                                             Arbaro2.DX_Engine.DXTreeMesh.DXArbaroVertexTrait>;
using SharpDX;


namespace Arbaro2.DX_Engine.DXTreeMesh
{
    //
    //  Creates a tree mesh
    //      from an ArbaroTree
    //      generating a DXMesh
    //      we need to generate 1 mesh per level + 1 mesh for the leaves
    //      thus DXArbaroTreeMesh cannot derive from DXBaseArbaroTreeMesh
    //

    public class DXArbaroTreeMesh : DXBaseArbaroTreeMesh
    {
        public DXArbaroTreeMesh(CS_Tree tree, CS_Params csParams)
            : base()
        {
            DXTreeMesh_TreeTraversal traversal = new DXTreeMesh_TreeTraversal(this, csParams);
            tree.traverseTree(traversal);
            this.VerifyTopology();
            Console.WriteLine("coucou");
        }


        // Tree traversal
        public class DXTreeMesh_TreeTraversal : CS_TreeTraversal
        {          
            private CS_Params _csParams;
            private DXArbaroTreeMesh _me;

            public DXTreeMesh_TreeTraversal(DXArbaroTreeMesh me, CS_Params csParams)
            {
                _csParams = csParams;
                _me = me;
            }

            public override bool enterStem(CS_Stem stem)
            {
                // 1. Create the first section                
                Vector3[] section_base = stem.getSections()[0].getSectionPoints();
                List<DXVertex> dxvv1 = new List<DXVertex>();
                foreach(Vector3 v in section_base) {
                    DXVertex dxv = _me.Vertices.Add(new DXArbaroVertexTrait(v));
                    dxvv1.Add(dxv);
                }                                        

                // 2. for each section but the last one - generate the upper part of the section
                //      and connect it with triangles to the previous section
                //      we use the lower section of next segment as upper section of current segment
                for (int i = 0; i < stem.getSections().Count; i++)
                {
                    if (stem.getSections()[i].getSubsegmentCount() == 1 && i != stem.getSections().Count - 1)
                    {
                        Vector3[] section_next = stem.getSections()[i + 1].getSectionPoints();
                        List<DXVertex> dxvv2 = new List<DXVertex>();
                        foreach (Vector3 v in section_base)
                        {
                            DXVertex dxv = _me.Vertices.Add(new DXArbaroVertexTrait(v));
                            dxvv2.Add(dxv);
                        }
                        AddRangeVertex(dxvv1, dxvv2, stem.getLevel());
                        dxvv1 = dxvv2;
                    }
                    else
                    {
                        CS_SegmentImpl seg = stem.getSections()[i];
                        for (int j = 0; j < seg.getSubsegmentCount(); j++)
                        {
                            CS_SubsegmentImpl subseg = seg.subsegments[j];
                            Vector3[] section_next = subseg.getSectionPoints();
                            List<DXVertex> dxvv2 = new List<DXVertex>();
                            foreach (Vector3 v in section_base)
                            {
                                DXVertex dxv = _me.Vertices.Add(new DXArbaroVertexTrait(v));
                                dxvv2.Add(dxv);
                            }
                            AddRangeVertex(dxvv1, dxvv2, stem.getLevel());
                            dxvv1 = dxvv2;
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
                return true;
            }

            private void AddRangeVertex(List<DXVertex> v1, List<DXVertex> v2, int level) 
            {
                // Create triangles between both vertices list
                for (int i = 0; i < v1.Count; i++) {
                    int j = i + 1; if (j == v1.Count) j = 0;
                    _me.Faces.Add(v1[i], v2[i], v2[j], v1[j]);
                }
            }
        }
    }
}
