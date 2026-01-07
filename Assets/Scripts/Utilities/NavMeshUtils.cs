using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static bool RandomPointOnNavMesh(out Vector3 result)
    {
        var triangulation = NavMesh.CalculateTriangulation();

        if (triangulation.vertices.Length == 0)
        {
            result = Vector3.zero;
            return false;
        }

        // Pick random triangle
        int index = Random.Range(0, triangulation.indices.Length / 3) * 3;

        Vector3 v1 = triangulation.vertices[triangulation.indices[index]];
        Vector3 v2 = triangulation.vertices[triangulation.indices[index + 1]];
        Vector3 v3 = triangulation.vertices[triangulation.indices[index + 2]];

        // Random barycentric point in triangle
        float r1 = Random.value;
        float r2 = Random.value;

        // Make sure point stays inside triangle
        if (r1 + r2 >= 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        result = v1 + r1 * (v2 - v1) + r2 * (v3 - v1);
        return true;
    }
}
