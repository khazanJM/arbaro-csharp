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
    /// <summary>
    /// A halfedge mesh data structure that stores mesh topology.
    /// </summary>
    /// <typeparam name="TEdgeTraits">The custom traits type for the edges.</typeparam>
    /// <typeparam name="TFaceTraits">The custom traits type for the faces.</typeparam>
    /// <typeparam name="THalfedgeTraits">The custom traits type for the halfeges.</typeparam>
    /// <typeparam name="TVertexTraits">The custom traits type for the vertices.</typeparam>
    /// <remarks>
    /// The trait classes allow the user to specify custom trait types on mesh elements.
    /// </remarks>
    public partial class DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits>
    {
        private List<DXEdge> edges = new List<DXEdge>();
        private List<DXFace> faces = new List<DXFace>();
        private List<DXHalfedge> halfedges = new List<DXHalfedge>();
        private List<DXVertex> vertices = new List<DXVertex>();

        /// <summary>
        /// If set to true, adding a face with more than three vertices will cause
        /// it to be triangulated regardless of which method is used.
        /// </summary>
        protected bool trianglesOnly;

        /// <summary>
        /// The edges of the mesh.
        /// </summary>
        public readonly DXEdgeCollection Edges;

        /// <summary>
        /// The faces of the mesh.
        /// </summary>
        public readonly DXFaceCollection Faces;

        /// <summary>
        /// The halfedges of the mesh.
        /// </summary>
        public readonly DXHalfedgeCollection Halfedges;

        /// <summary>
        /// The vertices of the mesh.
        /// </summary>
        public readonly DXVertexCollection Vertices;

        /// <summary>
        /// Initializes a new, empty mesh.
        /// </summary>
        public DXMesh()
        {
            Edges = new DXEdgeCollection(this);
            Faces = new DXFaceCollection(this);
            Halfedges = new DXHalfedgeCollection(this);
            Vertices = new DXVertexCollection(this);
        }

        /// <summary>
        /// Adds an edge to the edge list.
        /// </summary>
        /// <param name="edge">The edge to add.</param>
        protected void AppendToEdgeList(DXEdge edge)
        {
            edge.Index = edges.Count;
            edges.Add(edge);
        }

        /// <summary>
        /// Adds a face to the face list.
        /// </summary>
        /// <param name="face">The face to add.</param>
        protected void AppendToFaceList(DXFace face)
        {
            face.Index = faces.Count;
            faces.Add(face);
        }

        /// <summary>
        /// Adds a halfedge to the halfedge list.
        /// </summary>
        /// <param name="halfedge">The halfedge to add.</param>
        protected void AppendToHalfedgeList(DXHalfedge halfedge)
        {
            halfedge.Index = halfedges.Count;
            halfedges.Add(halfedge);
        }

        /// <summary>
        /// Adds a vertex to the vertex list.
        /// </summary>
        /// <param name="vertex">The vertex to add.</param>
        protected void AppendToVertexList(DXVertex vertex)
        {
            vertex.Index = vertices.Count;
            vertex.Mesh = this;
            vertices.Add(vertex);
        }


        /// <summary>
        /// Removes all elements from the mesh.
        /// </summary>
        /// <remarks>
        /// This is not exposed publicly because it's too easy to cause damage when using dynamic traits.
        /// </remarks>
        protected virtual void Clear()
        {
            edges.Clear();
            faces.Clear();
            halfedges.Clear();
            vertices.Clear();
        }

        /// <summary>
        /// Triangulates a mesh.
        /// </summary>
        /// <returns>A triangulated copy of the mesh.</returns>
        /// <remarks>
        /// Any edge and halfedge traits are not copied to the new mesh. Face traits are copied
        /// to all faces triangulated from a face.
        /// </remarks>
        public DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> TriangularCopy()
        {
            DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits> triangulatedMesh = new DXMesh<TEdgeTraits, TFaceTraits, THalfedgeTraits, TVertexTraits>();
            Dictionary<DXVertex, DXVertex> newVertices = new Dictionary<DXVertex, DXVertex>();

            foreach (DXVertex v in Vertices)
            {
                newVertices[v] = triangulatedMesh.Vertices.Add(v.Traits);
            }

            foreach (DXFace f in Faces)
            {
                DXVertex[] vertices = new DXVertex[f.VertexCount];
                int i = 0;
                foreach (DXVertex v in f.Vertices)
                {
                    vertices[i] = newVertices[v];
                    ++i;
                }
                triangulatedMesh.Faces.AddTriangles(f.Traits, vertices);
            }

            return triangulatedMesh;
        }

        /// <summary>
        /// Trims internal data structures to their current size.
        /// </summary>
        /// <remarks>
        /// Call this method to prevent excess memory usage when the mesh is done being built.
        /// </remarks>
        public void TrimExcess()
        {
            edges.TrimExcess();
            faces.TrimExcess();
            halfedges.TrimExcess();
            vertices.TrimExcess();
        }

        /// <summary>
        /// Checks halfedge connections to verify that a valid mesh was constructed.
        /// </summary>
        /// <remarks>
        /// Checking for proper topology in every case when a face is added would slow down
        /// mesh construction significantly, so this method may be called once when a mesh
        /// is complete to ensure that topology is manifold (with boundary).
        /// If the mesh is non-manifold, a BadTopologyException will be thrown.
        /// </remarks>
        public void VerifyTopology()
        {
            foreach (DXHalfedge h in halfedges)
            {
                //Debug.Assert(h == h.Opposite.Opposite);
                //Debug.Assert(h.Edge == h.Opposite.Edge);
                //Debug.Assert(h.ToVertex.Halfedge.Opposite.ToVertex == h.ToVertex);

                if (h.Previous.Next != h)
                {
                    throw new DXBadTopologyException("A halfedge's previous next is not itself.");
                }

                if (h.Next.Previous != h)
                {
                    throw new DXBadTopologyException("A halfedge's next previous is not itself.");
                }

                if (h.Next.Face != h.Face)
                {
                    throw new DXBadTopologyException("Adjacent halfedges do not belong to the same face.");
                }

                // Make sure each halfedge is reachable from the vertex it originates from
                bool found = false;

                foreach (DXHalfedge hIt in h.FromVertex.Halfedges)
                {
                    if (hIt == h)
                    {
                        found = true;
                        break;
                    }
                }

                if (found == false)
                {
                    throw new DXBadTopologyException("A halfedge is not reachable from the vertex it originates from.");
                }
            }
        }

        /// <summary>
        /// Determines if two faces are adjacent.
        /// </summary>
        /// <param name="faceA">One of the faces to search for.</param>
        /// <param name="faceB">The other face to search for.</param>
        /// <returns>True if the faces are adjacent, false if they are not.</returns>
        public static bool FacesShareEdge(DXFace faceA, DXFace faceB)
        {
            foreach (DXFace f in faceA.Faces)
            {
                if (f == faceB)
                {
                    return true;
                }
            }

            return false;
        }



    }
}
