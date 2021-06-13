using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
public class MagnetFieldMesh : MonoBehaviour
{
    private Mesh mesh;
    private Material mat;

    private Vector3[] coordinates;

    private const float MAX_RADIUS_RATIO = 0.51f;
    private const float ALPHA_RATIO = 0.5f;

    private PolygonCollider2D polyCollider;

    private void Awake()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        mat = GetComponent<MeshRenderer>().material;
        tag = "Field";
        polyCollider = GetComponent<PolygonCollider2D>();
        polyCollider.pathCount = 1;
    }

    public void UpdateMesh(Vector3[] coordinates)
    {
        this.coordinates = coordinates;

        mesh.Clear();

        mesh.vertices = coordinates;
        mesh.triangles = new int[]
        {
            0, 1, 2, 0, 2, 1
        };

        Vector2[] uvs = new Vector2[coordinates.Length];

        for (int i = 0; i < uvs.Length; i++)
        {
            uvs[i] = new Vector2(coordinates[i].x, coordinates[i].y);
        }
        mesh.uv = uvs;

        // Update Shader points
        mat.SetVector("Vector3_point1", coordinates[0]);
        mat.SetVector("Vector3_point2", coordinates[1]);
        mat.SetVector("Vector3_point3", coordinates[2]);

        // Update Collider
        Vector2[] points = new Vector2[coordinates.Length];
        for (int i = 0; i < coordinates.Length; i++)
        {
            points[i] = new Vector2(coordinates[i].x, coordinates[i].y);
        }
        polyCollider.SetPath(0, points);
    }

    public void SetIsTrigger(bool isTrigger)
    {
        polyCollider.enabled = isTrigger;
    }

    public void UpdateProgress(float progress)
    {
        if (progress > 1)
        {
            progress = 1;
        }

        if (progress < 0)
        {
            progress = 0;
        }

        float radius = progress * ComputeRadius();
        float alpha = 1 - progress * ALPHA_RATIO;

        if (alpha < 0)
        {
            alpha = 0;
        }

        mat.SetFloat("Vector1_radius", radius);
        mat.SetFloat("Vector1_alpha", alpha);
    }

    public void UpdateColor(Color color)
    {
        mat.SetColor("Color_color", color);
    }

    private float ComputeRadius()
    {
        float d1 = (coordinates[0] - coordinates[1]).magnitude;
        float d2 = (coordinates[1] - coordinates[2]).magnitude;
        float d3 = (coordinates[2] - coordinates[0]).magnitude;

        float dMax = Mathf.Max(d1, Mathf.Max(d2, d3));

        return MAX_RADIUS_RATIO * dMax;
    }
}
