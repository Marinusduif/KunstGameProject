using UnityEngine;

public class Spiral : MonoBehaviour
{
    Mesh Mesh;
    const int numberOfSegments = 2000; //aantal segementen. Ieder segment bestaat uit twee driehoeken
    [SerializeField] float WidthInnerCircle = 3f; //breedte binnencirkel
    [SerializeField] float withOutherCircle = 5f; //breedte buitenscirkel
    [SerializeField] float Theta = 0;
    float dTheta = -0.05f; //nauwkeurigheid, hoe kleiner, hoe mooier
    [SerializeField] float y = 0;
    [SerializeField] float dy = 0.02f; //de helling die de spiraal maakt

    void Start()
    {
        Mesh = new Mesh();
        Mesh = GetComponent<MeshFilter>().mesh;

        GenerateSpiral();
        //SaveMesh.Save(Mesh, "Assets/art/models/Spiral2.asset");

    }

    void Update()
    {

    }

    private void GenerateSpiral()
    {
        const int numberOfVertices = (numberOfSegments * 2) + 2;
        const int numberOfTriangles = (numberOfSegments * 2 * 3);

        Vector3[] vertices = new Vector3[numberOfVertices];
        Vector3[] normals = new Vector3[numberOfVertices]; // array to store normals
        Vector2[] uvs = new Vector2[numberOfVertices]; // UV array for the vertices
        Color[] colors = new Color[numberOfVertices]; // colors array for the vertices

        // Bereken de totale hoogte voor het kleurverloop
        float minHeight = 0;
        float maxHeight = dy * (numberOfSegments - 1);

        float totalAngle = dTheta * numberOfSegments;

        for (int i = 0; i < numberOfVertices; i++)
        {
            if (i % 2 == 0)
            {
                vertices[i] = CylinderToCarthesian(WidthInnerCircle, Theta, y);
                uvs[i] = new Vector2(0f, Mathf.InverseLerp(minHeight, maxHeight, y)); // Inner circle UV
            }
            else
            {
                vertices[i] = CylinderToCarthesian(withOutherCircle, Theta, y);
                uvs[i] = new Vector2(1f, Mathf.InverseLerp(minHeight, maxHeight, y)); // Outer circle UV
                Theta += dTheta;
                y += dy;
            }

            // Calculate color based on height (y-position)
            float t = Mathf.InverseLerp(minHeight, maxHeight, vertices[i].y);
            colors[i] = Color.Lerp(Color.black, Color.white, t);
        }

        int[] triangles = new int[numberOfTriangles];

        for (int i = 0; i < numberOfSegments; i++)
        {
            int vertIndex = i * 2;
            int triIndex = i * 6;

            // First triangle
            triangles[triIndex] = vertIndex;
            triangles[triIndex + 1] = vertIndex + 1;
            triangles[triIndex + 2] = vertIndex + 3;

            // Second triangle
            triangles[triIndex + 3] = vertIndex;
            triangles[triIndex + 4] = vertIndex + 3;
            triangles[triIndex + 5] = vertIndex + 2;
        }

        // Calculate normals
        for (int i = 0; i < numberOfSegments; i++)
        {
            int vertIndex = i * 2;

            // Get the vertices of the two triangles in the current segment
            Vector3 v0 = vertices[vertIndex];
            Vector3 v1 = vertices[vertIndex + 1];
            Vector3 v2 = vertices[vertIndex + 2];
            Vector3 v3 = vertices[vertIndex + 3];

            // Calculate normal for first triangle
            Vector3 normal1 = Vector3.Cross(v1 - v0, v2 - v0).normalized;

            // Calculate normal for second triangle
            Vector3 normal2 = Vector3.Cross(v3 - v0, v2 - v0).normalized;

            // Assign normals to the vertices of the two triangles
            normals[vertIndex] = normal1;
            normals[vertIndex + 1] = normal1;
            normals[vertIndex + 2] = normal2;
            normals[vertIndex + 3] = normal2;
        }

        Mesh.Clear();
        Mesh.vertices = vertices;
        Mesh.triangles = triangles;
        Mesh.normals = normals; // Assign calculated normals
        Mesh.uv = uvs; // Assign calculated UVs
        Mesh.colors = colors; // Apply vertex colors
    }

    Vector3 CylinderToCarthesian(float radius, float Theta, float y_sp)
    {
        float x = radius * Mathf.Cos(Theta);
        float y = y_sp;
        float z = radius * Mathf.Sin(Theta);
        return new Vector3(x, y, z);
    }
}
