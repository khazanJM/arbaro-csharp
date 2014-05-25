using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arbaro2.DX_Engine.DXMesh
{
    public partial class DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits>
    {
        /// <summary>
        /// Type allowing edges to be accessed like an array.
        /// </summary>
        public class DXEdgeCollection : IEnumerable<DXEdge>
        {
            readonly DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> mesh;

            internal DXEdgeCollection(DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> m)
            {
                mesh = m;
            }

            /// <summary>
            /// Accesses the edges in a mesh.
            /// </summary>
            /// <param name="index">The index of the edge.</param>
            /// <returns>The indexed <see cref="Edge"/>.</returns>
            public DXEdge this[int index]
            {
                get
                {
                    return mesh.edges[index];
                }
            }

            /// <summary>
            /// The number of edges in the mesh.
            /// </summary>
            public int Count
            {
                get
                {
                    return mesh.edges.Count;
                }
            }

            /// <summary>
            /// Provides an enumerator for the edges of the mesh.
            /// </summary>
            /// <returns>An edge of the mesh.</returns>
            public IEnumerator<DXEdge> GetEnumerator()
            {
                foreach (DXEdge e in mesh.edges)
                {
                    yield return e;
                }
            }

            /// <summary>
            /// Useless IEnumerable.GetEnumerator() implementation.
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        /// <summary>
        /// Type allowing faces to be accessed like an array.
        /// </summary>
        public class DXFaceCollection : IEnumerable<DXFace>
        {

            readonly DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> mesh;

            internal DXFaceCollection(DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> m)
            {
                mesh = m;
            }

            /// <summary>
            /// Accesses the faces in a mesh.
            /// </summary>
            /// <param name="index">The index of the face.</param>
            /// <returns>The indexed <see cref="Face"/>.</returns>
            public DXFace this[int index]
            {
                get
                {
                    return mesh.faces[index];
                }
            }

            /// <summary>
            /// The number of faces in the mesh.
            /// </summary>
            public int Count
            {
                get
                {
                    return mesh.faces.Count;
                }
            }

            /// <summary>
            /// Provides an enumerator for the faces of the mesh.
            /// </summary>
            /// <returns>A face of the mesh.</returns>
            public IEnumerator<DXFace> GetEnumerator()
            {
                foreach (DXFace f in mesh.faces)
                {
                    yield return f;
                }
            }

            /// <summary>
            /// Useless IEnumerable.GetEnumerator() implementation.
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }


            /// <summary>
            /// Adds a face to the mesh with default face traits.
            /// </summary>
            /// <param name="faceVertices">The vertices of the face in counterclockwise order.</param>
            /// <returns>The face created by this method.</returns>
            /// <exception cref="BadTopologyException">
            /// Thrown when fewer than three vertices are given or the vertices cannot form a valid face.
            /// </exception>
            /// <exception cref="ArgumentNullException">Thrown when a null vertex is given.</exception>
            public DXFace Add(params DXVertex[] faceVertices)
            {
                return Add(default(TFaceTraits), faceVertices);
            }

            /// <summary>
            /// Adds a face to the mesh with the specified face traits.
            /// </summary>
            /// <param name="faceTraits">The custom traits for the face to add to the mesh.</param>
            /// <param name="faceVertices">The vertices of the face in counterclockwise order.</param>
            /// <returns>The face created by this method.</returns>
            /// <exception cref="BadTopologyException">
            /// Thrown when fewer than three vertices are given or the vertices cannot form a valid face.
            /// </exception>
            /// <exception cref="ArgumentNullException">Thrown when a null vertex is given.</exception>
            public DXFace Add(TFaceTraits faceTraits, params DXVertex[] faceVertices)
            {
                if (mesh.trianglesOnly)
                {
                    return AddTriangles(faceTraits, faceVertices)[0];
                }
                else
                {
                    return CreateFace(faceTraits, faceVertices);
                }
            }

            /// <summary>
            /// Adds triangular faces to the mesh with default face traits.
            /// </summary>
            /// <param name="faceVertices">The vertices of the faces in counterclockwise order.</param>
            /// <returns>An array of faces created by this method.</returns>
            /// <exception cref="BadTopologyException">
            /// Thrown when fewer than three vertices are given or the vertices cannot form a valid face.
            /// </exception>
            /// <exception cref="ArgumentNullException">Thrown when a null vertex is given.</exception>
            public DXFace[] AddTriangles(params DXVertex[] faceVertices)
            {
                return AddTriangles(default(TFaceTraits), faceVertices);
            }

            /// <summary>
            /// Adds triangular faces to the mesh with the specified face traits.
            /// </summary>
            /// <param name="faceTraits">The custom traits for the faces to add to the mesh.</param>
            /// <param name="faceVertices">The vertices of the faces in counterclockwise order.</param>
            /// <returns>An array of faces created by this method.</returns>
            /// <exception cref="BadTopologyException">
            /// Thrown when fewer than three vertices are given or the vertices cannot form a valid face.
            /// </exception>
            /// <exception cref="ArgumentNullException">Thrown when a null vertex is given.</exception>
            public DXFace[] AddTriangles(TFaceTraits faceTraits, params DXVertex[] faceVertices)
            {
                int n = faceVertices.Length;

                // Require at least 3 vertices
                if (n < 3)
                {
                    throw new DXBadTopologyException("Cannot create a polygon with fewer than three vertices.");
                }

                DXFace[] addedFaces = new DXFace[n - 2];

                // Triangulate the face
                for (int i = 0; i < n - 2; ++i)
                {
                    addedFaces[i] = CreateFace(faceTraits, faceVertices[0], faceVertices[i + 1], faceVertices[i + 2]);
                }

                return addedFaces;
            }

            /// <summary>
            /// Adds a face to the mesh with the specified face traits.
            /// </summary>
            /// <param name="faceTraits">The custom traits for the face to add to the mesh.</param>
            /// <param name="faceVertices">The vertices of the face in counterclockwise order.</param>
            /// <returns>The face created by this method.</returns>
            /// <exception cref="BadTopologyException">
            /// Thrown when fewer than three vertices are given or the vertices cannot form a valid face.
            /// </exception>
            /// <exception cref="ArgumentNullException">Thrown when a null vertex is given.</exception>
            private DXFace CreateFace(TFaceTraits faceTraits, params DXVertex[] faceVertices)
            {
                int n = faceVertices.Length;

                // Require at least 3 vertices
                if (n < 3)
                {
                    throw new DXBadTopologyException("Cannot create a polygon with fewer than three vertices.");
                }

                DXEdge e;
                DXFace f;
                DXHalfedge[] faceHalfedges = new DXHalfedge[n];
                bool[] isNewEdge = new bool[n], isUsedVertex = new bool[n];

                // Make sure input is (mostly) acceptable before making any changes to the mesh
                for (int i = 0; i < n; ++i)
                {
                    int j = (i + 1) % n;

                    if (faceVertices[i] == null)
                    {
                        throw new ArgumentNullException("Can't add a null vertex to a face.");
                    }
                    if (!faceVertices[i].OnBoundary)
                    {
                        throw new DXBadTopologyException("Can't add an edge to a vertex on the interior of a mesh.");
                    }

                    // Find existing halfedges for this face
                    faceHalfedges[i] = faceVertices[i].FindHalfedgeTo(faceVertices[j]);
                    isNewEdge[i] = (faceHalfedges[i] == null);
                    isUsedVertex[i] = (faceVertices[i].Halfedge != null);

                    if (!isNewEdge[i] && !faceHalfedges[i].OnBoundary)
                    {
                        throw new DXBadTopologyException("Can't add more than two faces to an edge.");
                    }
                }

                // Create face
                f = new DXFace(faceTraits);
                mesh.AppendToFaceList(f);

                // Create new edges
                for (int i = 0; i < n; ++i)
                {
                    int j = (i + 1) % n;

                    if (isNewEdge[i])
                    {
                        // Create new edge
                        e = new DXEdge();
                        mesh.AppendToEdgeList(e);

                        // Create new halfedges
                        faceHalfedges[i] = new DXHalfedge();
                        mesh.AppendToHalfedgeList(faceHalfedges[i]);

                        faceHalfedges[i].Opposite = new DXHalfedge();
                        mesh.AppendToHalfedgeList(faceHalfedges[i].Opposite);

                        // Connect opposite halfedge to inner halfedge
                        faceHalfedges[i].Opposite.Opposite = faceHalfedges[i];

                        // Connect edge to halfedges
                        e.Halfedge0 = faceHalfedges[i];

                        // Connect halfedges to edge
                        faceHalfedges[i].Edge = e;
                        faceHalfedges[i].Opposite.Edge = e;

                        // Connect halfedges to vertices
                        faceHalfedges[i].ToVertex = faceVertices[j];
                        faceHalfedges[i].Opposite.ToVertex = faceVertices[i];

                        // Connect vertex to outgoing halfedge if it doesn't have one yet
                        if (faceVertices[i].Halfedge == null)
                        {
                            faceVertices[i].Halfedge = faceHalfedges[i];
                        }
                    }

                    if (faceHalfedges[i].Face != null)
                    {
                        throw new DXBadTopologyException("An inner halfedge already has a face assigned to it.");
                    }

                    // Connect inner halfedge to face
                    faceHalfedges[i].Face = f;

                    //Debug.Assert(faceHalfedges[i].FromVertex == faceVertices[i] && faceHalfedges[i].ToVertex == faceVertices[j]);
                }

                // Connect next/previous halfedges
                for (int i = 0; i < n; ++i)
                {
                    int j = (i + 1) % n;

                    // Outer halfedges
                    if (isNewEdge[i] && isNewEdge[j] && isUsedVertex[j])  // Both edges are new and vertex has faces connected already
                    {
                        DXHalfedge closeHalfedge = null;

                        // Find the closing halfedge of the first available opening
                        foreach (DXHalfedge h in faceVertices[j].Halfedges)
                        {
                            if (h.Face == null)
                            {
                                closeHalfedge = h;
                                break;
                            }
                        }

                        //Debug.Assert(closeHalfedge != null);

                        DXHalfedge openHalfedge = closeHalfedge.Previous;

                        // Link new outer halfedges into this opening
                        faceHalfedges[i].Opposite.Previous = openHalfedge;
                        openHalfedge.Next = faceHalfedges[i].Opposite;
                        faceHalfedges[j].Opposite.Next = closeHalfedge;
                        closeHalfedge.Previous = faceHalfedges[j].Opposite;
                    }
                    else if (isNewEdge[i] && isNewEdge[j])  // Both edges are new
                    {
                        faceHalfedges[i].Opposite.Previous = faceHalfedges[j].Opposite;
                        faceHalfedges[j].Opposite.Next = faceHalfedges[i].Opposite;
                    }
                    else if (isNewEdge[i] && !isNewEdge[j])  // This is new, next is old
                    {
                        faceHalfedges[i].Opposite.Previous = faceHalfedges[j].Previous;
                        faceHalfedges[j].Previous.Next = faceHalfedges[i].Opposite;
                    }
                    else if (!isNewEdge[i] && isNewEdge[j])  // This is old, next is new
                    {
                        faceHalfedges[i].Next.Previous = faceHalfedges[j].Opposite;
                        faceHalfedges[j].Opposite.Next = faceHalfedges[i].Next;
                    }
                    else if (!isNewEdge[i] && !isNewEdge[j] && faceHalfedges[i].Next != faceHalfedges[j])  // Relink faces before adding new edges if they are in the way of a new face
                    {
                        DXHalfedge closeHalfedge = faceHalfedges[i].Opposite;

                        // Find the closing halfedge of the opening opposite the opening halfedge i is on
                        do
                        {
                            closeHalfedge = closeHalfedge.Previous.Opposite;
                        } while (closeHalfedge.Face != null && closeHalfedge != faceHalfedges[j] && closeHalfedge != faceHalfedges[i].Opposite);

                        if (closeHalfedge == faceHalfedges[j] || closeHalfedge == faceHalfedges[i].Opposite)
                        {
                            throw new DXBadTopologyException("Unable to find an opening to relink an existing face.");
                        }

                        DXHalfedge openHalfedge = closeHalfedge.Previous;

                        // Remove group of faces between two openings, close up gap to form one opening
                        openHalfedge.Next = faceHalfedges[i].Next;
                        faceHalfedges[i].Next.Previous = openHalfedge;

                        // Insert group of faces into target opening
                        faceHalfedges[j].Previous.Next = closeHalfedge;
                        closeHalfedge.Previous = faceHalfedges[j].Previous;
                    }

                    // Inner halfedges
                    faceHalfedges[i].Next = faceHalfedges[j];
                    faceHalfedges[j].Previous = faceHalfedges[i];
                }

                // Connect face to an inner halfedge
                f.Halfedge = faceHalfedges[0];

                return f;
            }
        }


        /// <summary>
        /// Type allowing halfedges to be accessed like an array.
        /// </summary>
        public class DXHalfedgeCollection : IEnumerable<DXHalfedge> {
            
            readonly DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> mesh;

            internal DXHalfedgeCollection(DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> m)
            {
                mesh = m;
            }

            /// <summary>
            /// Accesses the halfedges in a mesh.
            /// </summary>
            /// <param name="index">The index of the halfedge.</param>
            /// <returns>The indexed <see cref="Halfedge"/>.</returns>
            public DXHalfedge this[int index]
            {
                get
                {
                    return mesh.halfedges[index];
                }
            }

            /// <summary>
            /// The number of halfedges in the mesh.
            /// </summary>
            public int Count
            {
                get
                {
                    return mesh.halfedges.Count;
                }
            }

            /// <summary>
            /// Provides an enumerator for the halfedges of the mesh.
            /// </summary>
            /// <returns>A halfedge of the mesh.</returns>
            public IEnumerator<DXHalfedge> GetEnumerator()
            {
                foreach (DXHalfedge h in mesh.halfedges)
                {
                    yield return h;
                }
            }

            /// <summary>
            /// Useless IEnumerable.GetEnumerator() implementation.
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }


        /// <summary>
        /// Type allowing vertices to be accessed like an array.
        /// </summary>
        public class DXVertexCollection : IEnumerable<DXVertex> {

            readonly DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> mesh;

            internal DXVertexCollection(DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> m)
            {
                mesh = m;
            }

            /// <summary>
            /// Accesses the vertices in a mesh.
            /// </summary>
            /// <param name="index">The index of the vertex.</param>
            /// <returns>The indexed <see cref="Vertex"/>.</returns>
            public DXVertex this[int index]
            {
                get
                {
                    return mesh.vertices[index];
                }
            }

            /// <summary>
            /// The number of vertices in the mesh.
            /// </summary>
            public int Count
            {
                get
                {
                    return mesh.vertices.Count;
                }
            }


            /// <summary>
            /// Provides an enumerator for the vertices of the mesh.
            /// </summary>
            /// <returns>A vertex of the mesh.</returns>
            public IEnumerator<DXVertex> GetEnumerator()
            {
                foreach (DXVertex v in mesh.vertices)
                {
                    yield return v;
                }
            }

            /// <summary>
            /// Useless IEnumerable.GetEnumerator() implementation.
            /// </summary>
            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }


            /// <summary>
            /// Adds a vertex to the mesh.
            /// </summary>
            /// <returns>The vertex created by this method.</returns>
            public DXVertex Add()
            {
                DXVertex v = new DXVertex();
                mesh.AppendToVertexList(v);
                return v;
            }

            /// <summary>
            /// Adds a vertex to the mesh with the specified vertex traits.
            /// </summary>
            /// <param name="vertexTraits">The custom traits for the vertex to add to the mesh.</param>
            /// <returns>The vertex created by this method.</returns>
            public DXVertex Add(TVertexTraits vertexTraits)
            {
                DXVertex v = new DXVertex(vertexTraits);
                mesh.AppendToVertexList(v);
                return v;
            }

        }
    }

}
