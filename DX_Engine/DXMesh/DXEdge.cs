using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

//
//
//      DXMesh and all related classes are an implementation
//      of an half-edge data structure to store polygonal mesh
//      It is derived from the work of Alexander Kolliopoulos
//      http://www.dgp.toronto.edu/~alexk/halfedge.pdf
//      
//      My implementation is highly inspired from what he did
//      
//

namespace Arbaro2.DX_Engine.DXMesh
{

    public partial class DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits>
    {
        public class DXEdge
        {
            /// <summary>
            /// The custom traits for the edge.
            /// </summary>
            public TEdgeTraits Traits;

            private DXHalfedge halfedge;
            private int index;

            /// <summary>
            /// Creates an edge with traits set to their default value.
            /// </summary>
            internal DXEdge() { }

            /// <summary>
            /// Creates an edge with the given traits.
            /// </summary>
            /// <param name="edgeTraits">Traits for this edge.</param>
            internal DXEdge(TEdgeTraits edgeTraits)
            {
                Traits = edgeTraits;
            }

            /// <summary>
            /// One face adjacent to the edge, or null if there is no face.
            /// </summary>
            public DXFace Face0
            {
                get
                {
                    return halfedge.Face;
                }
            }

            /// <summary>
            /// The other face adjacent to the edge, or null if there is no face.
            /// </summary>
            public DXFace Face1
            {
                get
                {
                    return halfedge.Opposite.Face;
                }
            }

            /// <summary>
            /// One halfedge that corresponds to the edge.
            /// </summary>
            public DXHalfedge Halfedge0
            {
                get
                {
                    return halfedge;
                }
                internal set
                {
                    halfedge = value;
                }
            }

            /// <summary>
            /// The other halfedge that corresponds to the edge.
            /// </summary>
            public DXHalfedge Halfedge1
            {
                get
                {
                    return halfedge.Opposite;
                }
                internal set
                {
                    halfedge.Opposite = value;
                }
            }

            /// <summary>
            /// The index of this in the mesh's interal edge list.
            /// </summary>
            public int Index
            {
                get
                {
                    return index;
                }
                internal set
                {
                    index = value;
                }
            }

            /// <summary>
            /// Checks if the edge is on the boundary of the mesh.
            /// </summary>
            public bool OnBoundary
            {
                get
                {
                    return halfedge.OnBoundary || halfedge.Opposite.OnBoundary;
                }
            }

            /// <summary>
            /// The mesh the edge belongs to.
            /// </summary>
            public DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> Mesh
            {
                get
                {
                    return halfedge.Mesh;
                }
            }

            /// <summary>
            /// One vertex on the edge.
            /// </summary>
            public DXVertex Vertex0
            {
                get
                {
                    return halfedge.ToVertex;
                }
            }

            /// <summary>
            /// The other vertex on the edge.
            /// </summary>
            public DXVertex Vertex1
            {
                get
                {
                    return halfedge.Opposite.ToVertex;
                }
            }
        }
    }
}
