using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagnetManager : MonoBehaviour
{
    private new Rigidbody2D rigidbody2D;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Wall")
        {
            var collisionPoint = collision.ClosestPoint(transform.position);
            Debug.Log(transform.position);
            transform.position = collisionPoint;
            Debug.Log(transform.position);
            rigidbody2D.velocity = Vector2.zero;
        }
    }
}
