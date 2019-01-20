namespace SDE.Mesh
{
    using UnityEngine;
    
    public static class PolygonGeneration
    {
        public static EPolygonResult Create(Vector3[] vertices, out Mesh mesh, string polygonName = "Custom Polygon")
        {
            mesh = null;
            // needs to have 3 or more vertices
            if (vertices.Length < 3) return EPolygonResult.LESS_THAN_MIN_VERTEX;

            mesh = new Mesh() { name = polygonName };
            Triangulator tri = new Triangulator(vertices);

            mesh.vertices = vertices;
            mesh.triangles = tri.Triangulate();

            mesh.RecalculateNormals();
            
            // Check to see if any normal is facing the wrong way
            foreach (Vector3 norm in mesh.normals)
                if (norm.z > 0.0f) return EPolygonResult.NORMAL_INCORRECT_FACING;

            mesh.RecalculateBounds();

            return EPolygonResult.SUCCESSFUL;
        }
    }
}


