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
        public class DXVertex {

            /// <summary>
            /// The custom traits for the vertex.
            /// </summary>
            public TVertexTraits Traits;

            private DXHalfedge halfedge;
            private DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> mesh;
            private int index;

            /// <summary>
            /// Creates a vertex with traits set to their default value.
            /// </summary>
            internal DXVertex() { }

            /// <summary>
            /// Creates a vertex with the given traits.
            /// </summary>
            /// <param name="vertexTraits">Traits for this vertex.</param>
            internal DXVertex(TVertexTraits vertexTraits)
            {
                Traits = vertexTraits;
            }

            /// <summary>
            /// The number of edges connected to the vertex.
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

            // <summary>
            /// The number of faces with the vertex.
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
            /// A halfedge that originates from the vertex.
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
            /// The number of halfedges from the vertex.
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
            /// The index of this in the mesh's interal vertex list.
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
            /// The mesh the vertex belongs to.
            /// </summary>
            public DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> Mesh
            {
                get
                {
                    return mesh;
                }
                internal set
                {
                    mesh = value;
                }
            }

            /// <summary>
            /// Checks if the vertex is on the boundary of the mesh.
            /// </summary>
            public bool OnBoundary
            {
                get
                {
                    if (this.halfedge == null)
                    {
                        return true;
                    }

                    // Search adjacent faces for any that are null
                    foreach (DXHalfedge h in Halfedges)
                    {
                        if (h.OnBoundary)
                        {
                            return true;
                        }
                    }

                    return false;
                }
            }

            /// <summary>
            /// The number of vertices in the one ring neighborhood.
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
            /// An iterator for edges connected to the vertex.
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
            /// An iterator for the faces with the vertex.
            /// </summary>
            public IEnumerable<DXFace> Faces
            {
                get
                {
                    foreach (DXHalfedge h in Halfedges)
                    {
                        if (h.Face != null)
                        {
                            yield return h.Face;
                        }
                    }
                }
            }

            /// <summary>
            /// An iterator for the halfedges originating from the vertex.
            /// </summary>
            public IEnumerable<DXHalfedge> Halfedges
            {
                get
                {
                    DXHalfedge h = this.halfedge;

                    if (h != null)
                    {
                        do
                        {
                            yield return h;
                            h = h.Opposite.Next;
                        } while (h != this.halfedge);
                    }
                }
            }

            /// <summary>
            /// An iterator for the vertices in the one ring neighborhood.
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

            /// <summary>
            /// Searches for the edge associated with the specified vertex.
            /// </summary>
            /// <param name="vertex">A vertex sharing an edge with this vertex.</param>
            /// <returns>The edge if it is found, otherwise null.</returns>
            public DXEdge FindEdgeTo(DXVertex vertex)
            {
                foreach (DXHalfedge h in Halfedges)
                {
                    if (h.ToVertex == vertex)
                    {
                        return h.Edge;
                    }
                }
                return null;
            }

            /// <summary>
            /// Searches for the halfedge pointing to the specified face from this vertex.
            /// </summary>
            /// <param name="face">The face the halfedge to find points to.</param>
            /// <returns>The halfedge if it is found, otherwise null.</returns>
            public DXHalfedge FindHalfedgeTo(DXFace face)
            {
                foreach (DXHalfedge h in Halfedges)
                {
                    if (h.Face == face)
                    {
                        return h;
                    }
                }
                return null;
            }

            /// <summary>
            /// Searches for a halfedge pointing to a vertex from this vertex.
            /// </summary>
            /// <param name="vertex">A vertex pointed to by the halfedge to search for.</param>
            /// <returns>The halfedge from this vertex to the specified vertex. If none exists, returns null.</returns>
            public DXHalfedge FindHalfedgeTo(DXVertex vertex)
            {
                foreach (DXHalfedge h in Halfedges)
                {
                    if (h.ToVertex == vertex)
                    {
                        return h;
                    }
                }
                return null;
            }

            /// <summary>
            /// Searches for an indexed edge by iterating.
            /// </summary>
            /// <param name="index">The index of the edge to return.</param>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative or too large.</exception>
            /// <returns>The specified edge.</returns>
            public DXEdge GetEdge(int index)
            {
                int count = 0;
                foreach (DXEdge e in Edges)
                {
                    if (count == index)
                    {
                        return e;
                    }
                    ++count;
                }
                throw new ArgumentOutOfRangeException("index");
            }

            /// <summary>
            /// Searches for an indexed face by iterating.
            /// </summary>
            /// <param name="index">The index of the face to return.</param>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative or too large.</exception>
            /// <returns>The specified face.</returns>
            public DXFace GetFace(int index)
            {
                int count = 0;
                foreach (DXFace f in Faces)
                {
                    if (count == index)
                    {
                        return f;
                    }
                    ++count;
                }
                throw new ArgumentOutOfRangeException("index");
            }

            /// <summary>
            /// Searches for an indexed halfedge by iterating.
            /// </summary>
            /// <param name="index">The index of the halfedge to return.</param>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative or too large.</exception>
            /// <returns>The specified halfedge.</returns>
            public DXHalfedge GetHalfedge(int index)
            {
                int count = 0;
                foreach (DXHalfedge h in Halfedges)
                {
                    if (count == index)
                    {
                        return h;
                    }
                    ++count;
                }
                throw new ArgumentOutOfRangeException("index");
            }

            /// <summary>
            /// Searches for an indexed vertex by iterating.
            /// </summary>
            /// <param name="index">The index of the vertex to return.</param>
            /// <exception cref="ArgumentOutOfRangeException">Thrown when <paramref name="index"/> is negative or too large.</exception>
            /// <returns>The specified vertex.</returns>
            public DXVertex GetVertex(int index)
            {
                int count = 0;
                foreach (DXVertex v in Vertices)
                {
                    if (count == index)
                    {
                        return v;
                    }
                    ++count;
                }
                throw new ArgumentOutOfRangeException("index");
            }
        }
    }
}
