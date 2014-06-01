using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Arbaro2.DX_Engine.DXTreeMesh;
using Arbaro2.Arbaro.Tree;
using Arbaro2.Arbaro.Params;
using SharpDX;
using SharpDX.Direct3D11;

using Buffer = SharpDX.Direct3D11.Buffer;

using DXBaseArbaroTreeMesh = Arbaro2.DX_Engine.DXMesh.DXMesh<Arbaro2.DX_Engine.DXTreeMesh.DXArbaroNullTrait, 
                                                             Arbaro2.DX_Engine.DXTreeMesh.DXArbaroNullTrait, 
                                                             Arbaro2.DX_Engine.DXTreeMesh.DXArbaroHalfedgeTrait, 
                                                             Arbaro2.DX_Engine.DXTreeMesh.DXArbaroVertexTrait>;
using System.Runtime.InteropServices;
using Arbaro2.DX_Engine.DXCameras;
using SharpDX.DXGI;
using SharpDX.Direct3D;

namespace Arbaro2.DX_Engine.DXTreeMesh
{
    [StructLayout(LayoutKind.Sequential)]
    public struct DXMEV
    {
        public Vector4 P; // position     
    }

    //
    //  Creates a tree mesh
    //      from an ArbaroTree
    //      generating a DXMesh
    //

    public class DXArbaroTreeMesh :DXRenderable
    {
        private DXBaseArbaroTreeMesh[] meshes = { new DXBaseArbaroTreeMesh(), new DXBaseArbaroTreeMesh(), new DXBaseArbaroTreeMesh(), new DXBaseArbaroTreeMesh(), new DXBaseArbaroTreeMesh() };

        private Buffer[] _vertexBuffer2 = { null, null, null, null, null }, _indexBuffer2 = { null, null, null, null, null };
        private int[] IndexCount2 = { 0, 0, 0, 0, 0 };
        private InputElement[] _inputElements;
        private InputLayout _inputLayout;

        public DXArbaroTreeMesh(CS_Tree tree, CS_Params csParams)
            : base()
        {
            DXTreeMesh_TreeTraversal traversal = new DXTreeMesh_TreeTraversal(meshes, csParams);
            tree.traverseTree(traversal);

            InitShaders();

            BBox = new BoundingBox();
            BBox.Minimum = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
            BBox.Maximum = new Vector3(float.MinValue, float.MinValue, float.MinValue);

            for (int i = 0; i < 5; i++)
            {
                // Build vertex buffer
                if (meshes[i].Faces.Count != 0)
                {
                    var streamV = new DataStream(meshes[i].Vertices.Count * Marshal.SizeOf(typeof(DXMEV)), true, true);
                    var streamI = new DataStream(meshes[i].Faces.Count * 2 * sizeof(UInt32), true, true);

                    
                    
                }
            }
        }

        protected override void _Render(DXCamera camera)
        {
            EffectTechnique technique = _shader.DXEffect.GetTechniqueByIndex(0);

            for (int i = 0; i < 5; i++)
            {
                if (_vertexBuffer2[i] != null && Program.DXSceneOptions.LevelVisibility[i])
                {

                    DXContext.InputAssembler.SetVertexBuffers(0, new VertexBufferBinding(_vertexBuffer2[i], Marshal.SizeOf(typeof(DXMEV)), 0));
                    DXContext.InputAssembler.SetIndexBuffer(_indexBuffer2[i], Format.R32_UInt, 0);
                    DXContext.InputAssembler.PrimitiveTopology = PrimitiveTopology.TriangleList;
                    DXContext.InputAssembler.InputLayout = _inputLayout;

                    _shader.SetParameter("worldMatrix", Matrix.Identity);
                    _shader.SetParameter("viewMatrix", camera.ViewMatrix);
                    _shader.SetParameter("projectionMatrix", camera.ProjMatrix);
                    _shader.SetParameter("wvp", Matrix.Identity * camera.ViewMatrix * camera.ProjMatrix);


                    EffectPass usePass = technique.GetPassByIndex(0);
                    usePass.Apply(DXContext);
                    DXContext.DrawIndexed(IndexCount2[i], 0, 0);
                    usePass.Dispose();


                    usePass = technique.GetPassByIndex(1);
                    usePass.Apply(DXContext);
                    DXContext.DrawIndexed(IndexCount2[i], 0, 0);
                    usePass.Dispose();


                }
            }
            technique.Dispose();
        }

        private void InitShaders()
        {
            _shader = Program.DXShaderManager.MakeShader("SolidWireframe");

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


        // Tree traversal
        public class DXTreeMesh_TreeTraversal : CS_TreeTraversal
        {          
            private CS_Params _csParams;
            private DXBaseArbaroTreeMesh[] _meshes;

            public DXTreeMesh_TreeTraversal(DXBaseArbaroTreeMesh[] me, CS_Params csParams)
            {
                _csParams = csParams;
                _meshes = me;
            }

            public override bool enterStem(CS_Stem stem)
            {
                // 1. Create the first section                
                Vector3[] section_base = stem.getSections()[0].getSectionPoints();
                List<DXBaseArbaroTreeMesh.DXVertex> dxvv1 = new List<DXBaseArbaroTreeMesh.DXVertex>();
                foreach(Vector3 v in section_base) {
                    DXBaseArbaroTreeMesh.DXVertex dxv = _meshes[stem.getLevel()].Vertices.Add(new DXArbaroVertexTrait(v));
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
                        List<DXBaseArbaroTreeMesh.DXVertex> dxvv2 = new List<DXBaseArbaroTreeMesh.DXVertex>();
                        foreach (Vector3 v in section_base)
                        {
                            DXBaseArbaroTreeMesh.DXVertex dxv = _meshes[stem.getLevel()].Vertices.Add(new DXArbaroVertexTrait(v));
                            dxvv2.Add(dxv);
                        }
                        AddRangeVertex(_meshes[stem.getLevel()], dxvv1, dxvv2);
                        dxvv1 = dxvv2;
                    }
                    else
                    {
                        CS_SegmentImpl seg = stem.getSections()[i];
                        for (int j = 0; j < seg.getSubsegmentCount(); j++)
                        {
                            CS_SubsegmentImpl subseg = seg.subsegments[j];
                            Vector3[] section_next = subseg.getSectionPoints();
                            List<DXBaseArbaroTreeMesh.DXVertex> dxvv2 = new List<DXBaseArbaroTreeMesh.DXVertex>();
                            foreach (Vector3 v in section_base)
                            {
                                DXBaseArbaroTreeMesh.DXVertex dxv = _meshes[stem.getLevel()].Vertices.Add(new DXArbaroVertexTrait(v));
                                dxvv2.Add(dxv);
                            }
                            AddRangeVertex(_meshes[stem.getLevel()], dxvv1, dxvv2);
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

            private void AddRangeVertex(DXBaseArbaroTreeMesh mesh, List<DXBaseArbaroTreeMesh.DXVertex> v1, List<DXBaseArbaroTreeMesh.DXVertex> v2) 
            {
                // Create triangles between both vertices list
                for (int i = 0; i < v1.Count; i++) {
                    int j = i + 1; if (j == v1.Count) j = 0;
                    mesh.Faces.Add(v1[i], v2[i], v2[j], v1[j]);
                }
            }
        }
    }
}
