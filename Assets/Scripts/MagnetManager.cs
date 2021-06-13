using UnityEngine;

public class MagnetManager : MonoBehaviour
{
    [SerializeField] private MagnetLauncherManager manager = null;

    private new Rigidbody2D rigidbody2D;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            rigidbody2D.velocity = Vector2.zero;
            manager.SetMagnetToFixed();
        }
    }
}
