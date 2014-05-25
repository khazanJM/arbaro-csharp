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
        public class DXFace
        {
            /// <summary>
            /// The custom traits for the face.
            /// </summary>
            public TFaceTraits Traits;

            private DXHalfedge halfedge;
            private int index;

            /// <summary>
            /// Creates a face with traits set to their default value.
            /// </summary>
            internal DXFace() { }

            /// <summary>
            /// Creates a face with the given traits.
            /// </summary>
            /// <param name="faceTraits">Traits for this face.</param>
            internal DXFace(TFaceTraits faceTraits)
            {
                Traits = faceTraits;
            }

            /// <summary>
            /// The number of edges for the face.
            /// </summary>
            public int EdgeCount
            {
                get
                {
                    int count = 0;
                    foreach (DXEdge e in Edges)
                    {
                        ++count;
                    }
                    return count;
                }
            }

            /// <summary>
            /// The number of adjacent faces for the face.
            /// </summary>
            public int FaceCount
            {
                get
                {
                    int count = 0;
                    foreach (DXFace f in Faces)
                    {
                        ++count;
                    }
                    return count;
                }
            }

            /// <summary>
            /// A halfedge that belongs to the face.
            /// </summary>
            public DXHalfedge Halfedge
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
            /// The number of halfedges for the face.
            /// </summary>
            public int HalfedgeCount
            {
                get
                {
                    int count = 0;
                    foreach (DXHalfedge h in Halfedges)
                    {
                        ++count;
                    }
                    return count;
                }
            }

            /// <summary>
            /// The index of this in the mesh's interal face list.
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
            /// The mesh the face belongs to.
            /// </summary>
            public DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> Mesh
            {
                get
                {
                    return halfedge.Mesh;
                }
            }

            /// <summary>
            /// Checks if the face is on the boundary of the mesh.
            /// </summary>
            public bool OnBoundary
            {
                get
                {
                    foreach (DXHalfedge h in Halfedges)
                    {
                        if (h.Opposite.OnBoundary)
                        {
                            return true;
                        }
                    }
                    return false;
                }
            }

            /// <summary>
            /// The number of vertices for the face.
            /// </summary>
            public int VertexCount
            {
                get
                {
                    int count = 0;
                    foreach (DXVertex v in Vertices)
                    {
                        ++count;
                    }
                    return count;
                }
            }


            /// <summary>
            /// An iterator for edges on the face.
            /// </summary>
            public IEnumerable<DXEdge> Edges
            {
                get
                {
                    foreach (DXHalfedge h in Halfedges)
                    {
                        yield return h.Edge;
                    }
                }
            }

            /// <summary>
            /// An iterator for faces adjacent to the face.
            /// </summary>
            public IEnumerable<DXFace> Faces
            {
                get
                {
                    foreach (DXHalfedge h in Halfedges)
                    {
                        yield return h.Opposite.Face;
                    }
                }
            }

            /// <summary>
            /// An iterator for halfedges for the face.
            /// </summary>
            public IEnumerable<DXHalfedge> Halfedges
            {
                get
                {
                    DXHalfedge h = this.halfedge;

                    do
                    {
                        yield return h;
                        h = h.Next;
                    } while (h != this.halfedge);
                }
            }

            /// <summary>
            /// An iterator for vertices for the face.
            /// </summary>
            public IEnumerable<DXVertex> Vertices
            {
                get
                {
                    foreach (DXHalfedge h in Halfedges)
                    {
                        yield return h.ToVertex;
                    }
                }
            }
        }
    }
}
