using UnityEngine;

public class ShaderGameHandler : MonoBehaviour
{
    [SerializeField] private MagnetFieldMesh magnetFieldMesh = null;

    private void Start()
    {
        Vector3[] coordinates = new Vector3[]
        {
            new Vector3( 0.5f, 0.2f, 0.0f),
            new Vector3(-3.0f, 3.0f, 0.0f),
            new Vector3(-1.0f,-3.0f, 0.0f),
        };

        magnetFieldMesh.UpdateMesh(coordinates);
    }
}
