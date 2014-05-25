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

        // L 151
    }
}
