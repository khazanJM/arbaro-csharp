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
        public class DXHalfedge {

            /// <summary>
            /// The custom traits for the halfedge.
            /// </summary>
            public THalfedgeTraits Traits;

            private DXEdge edge;
            private DXFace face;
            private DXHalfedge nextHalfedge, oppositeHalfedge, previousHalfedge;
            private DXVertex vertex;
            private int index;

            /// <summary>
            /// Creates a halfedge with traits set to their default value.
            /// </summary>
            internal DXHalfedge() { }

            /// <summary>
            /// Creates a halfedge with the given traits.
            /// </summary>
            /// <param name="halfedgeTraits">Traits for this halfedge.</param>
            internal DXHalfedge(THalfedgeTraits halfedgeTraits)
            {
                Traits = halfedgeTraits;
            }

            /// <summary>
            /// The edge corresponding to the halfedge.
            /// </summary>
            public DXEdge Edge
            {
                get
                {
                    return edge;
                }
                internal set
                {
                    edge = value;
                }
            }

            /// <summary>
            /// The face corresponding to the halfedge.
            /// </summary>
            public DXFace Face
            {
                get
                {
                    return face;
                }
                internal set
                {
                    face = value;
                }
            }

            /// <summary>
            /// The vertex the halfedge originates from.
            /// </summary>
            public DXVertex FromVertex
            {
                get
                {
                    return Opposite.ToVertex;
                }
            }

            /// <summary>
            /// The index of this in the mesh's interal halfedge list.
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
            /// The mesh the halfedge belongs to.
            /// </summary>
            public DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> Mesh
            {
                get
                {
                    return vertex.Mesh;
                }
            }

            /// <summary>
            /// The next halfedge.
            /// </summary>
            public DXHalfedge Next
            {
                get
                {
                    return nextHalfedge;
                }
                internal set
                {
                    nextHalfedge = value;
                }
            }

            /// <summary>
            /// Checks if the halfedge is on the boundary of the mesh.
            /// </summary>
            public bool OnBoundary
            {
                get
                {
                    return face == null;
                }
            }

            /// <summary>
            /// The opposite halfedge.
            /// </summary>
            public DXHalfedge Opposite
            {
                get
                {
                    return oppositeHalfedge;
                }
                internal set
                {
                    oppositeHalfedge = value;
                }
            }

            /// <summary>
            /// The previous halfedge.
            /// </summary>
            public DXHalfedge Previous
            {
                get
                {
                    return previousHalfedge;
                }
                internal set
                {
                    previousHalfedge = value;
                }
            }

            /// <summary>
            /// The vertex pointed to by the halfedge.
            /// </summary>
            public DXVertex ToVertex
            {
                get
                {
                    return vertex;
                }
                internal set
                {
                    vertex = value;
                }
            }
        }
    }
}
