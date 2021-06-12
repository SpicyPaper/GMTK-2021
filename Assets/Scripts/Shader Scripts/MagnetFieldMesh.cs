using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MagnetFieldMesh : MonoBehaviour
{
    private Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
    }

    public void UpdateMesh(Vector3[] coordinates)
    {
        mesh.Clear();

        mesh.vertices = coordinates;
        mesh.triangles = new int[]
        {
            0, 1, 2, 0, 2, 1
        };
    }
}
