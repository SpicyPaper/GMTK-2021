using UnityEngine;

public class MagnetManager : MonoBehaviour
{
    public MagnetLauncherManager manager = null;

    private new Rigidbody2D rigidbody2D;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (manager == null)
        {
            return;
        }

        if (collision.tag == "Wall")
        {
            rigidbody2D.velocity = Vector2.zero;
            manager.SetMagnetToFixed();
        }
    }
}
