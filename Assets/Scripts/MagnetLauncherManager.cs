using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public class MagnetLauncherManager : MonoBehaviour
{
    public enum LauncherSide
    {
        RIGHT,
        LEFT
    }

    public enum MagnetState
    {
        NONE,
        THROWN,
        FIXED
    }

    public Camera mainCamera = null;
    [SerializeField] private GameObject body = null;
    public GameObject magnetPoint = null;
    [SerializeField] private GameObject magnetLauncher = null;
    [SerializeField] private LauncherSide launcherSide = LauncherSide.RIGHT;

    public MagnetState magnetState;

    private PointEffector2D magnetPointEffector;
    private Rigidbody2D magnetRigidbody;
    private SpriteRenderer magnetSprite;

    private bool mouseDown;

    private const float MAGNET_SPEED = 20;
    private const float MAGNET_DISTANCE_FROM_BODY = 0.6f;
    private const float MAGNET_OFFSET = 0.05f;

    public void Init()
    {
        magnetPointEffector = magnetPoint.GetComponent<PointEffector2D>();
        magnetRigidbody = magnetPoint.GetComponent<Rigidbody2D>();
        magnetSprite = magnetPoint.GetComponentInChildren<SpriteRenderer>();
    }

    private void Start()
    {
        magnetPointEffector.enabled = false;
        magnetSprite.enabled = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((collision.tag == "MagnetRight" && launcherSide == LauncherSide.RIGHT) ||
            (collision.tag == "MagnetLeft" && launcherSide == LauncherSide.LEFT))
            &&
            magnetState == MagnetState.FIXED)
        {
            magnetPointEffector.enabled = false;
            magnetSprite.enabled = false;
            magnetState = MagnetState.NONE;
        }
    }

    private Vector2 RotateVector(Vector2 v, float degrees)
    {
        float sin = Mathf.Sin(degrees * Mathf.Deg2Rad);
        float cos = Mathf.Cos(degrees * Mathf.Deg2Rad);

        float tx = v.x;
        float ty = v.y;
        v.x = (cos * tx) - (sin * ty);
        v.y = (sin * tx) + (cos * ty);
        return v;
    }

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            Debug.LogError("No mouse connected.");
            return;
        }

        // Mouse move
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 magnetPos = magnetPoint.transform.position;
        Vector2 bodyPos = body.transform.position;

        Vector2 resVector;
        float y;
        float x;
        float offset;

        if (!magnetPointEffector.enabled)
        {
            resVector = mousePos - bodyPos;

            y = mousePos.y - bodyPos.y;
            x = mousePos.x - bodyPos.x;

            offset = (launcherSide == LauncherSide.RIGHT ? 1 : -1) * MAGNET_OFFSET;
        }
        else
        {
            resVector = magnetPos - bodyPos;

            y = magnetPos.y - bodyPos.y;
            x = magnetPos.x - bodyPos.x;

            offset = 0;
        }

        float radian = Mathf.Atan2(y, x);
        float degrees = radian * Mathf.Rad2Deg;
        float resDegrees = degrees - 90;

        magnetLauncher.transform.position = bodyPos + resVector.normalized * MAGNET_DISTANCE_FROM_BODY +
            RotateVector(resVector.normalized, 90) * offset;
        magnetLauncher.transform.rotation = Quaternion.Euler(0, 0, resDegrees);

        // Mouse click
        ButtonControl control;
        switch (launcherSide)
        {
            case LauncherSide.RIGHT:
                control = mouse.rightButton;
                break;
            case LauncherSide.LEFT:
                control = mouse.leftButton;
                break;
            default:
                Debug.Log("Case not treated.");
                return;
        }
        if (mouseDown && magnetState == MagnetState.THROWN && magnetRigidbody.velocity == Vector2.zero)
        {
            magnetState = MagnetState.FIXED;
            magnetPointEffector.enabled = true;
        }
        if (control.wasPressedThisFrame)
        {
            mouseDown = true;

            switch (magnetState)
            {
                case MagnetState.NONE:
                    magnetSprite.enabled = true;
                    magnetPoint.transform.position = body.transform.position;
                    magnetRigidbody.velocity = resVector.normalized * MAGNET_SPEED;
                    magnetState = MagnetState.THROWN;
                    break;
                case MagnetState.THROWN:
                    magnetRigidbody.velocity = Vector2.zero;
                    magnetPointEffector.enabled = true;
                    magnetState = MagnetState.FIXED;
                    break;
                case MagnetState.FIXED:
                    magnetPointEffector.enabled = true;
                    break;
                default:
                    break;
            }
        }
        if (control.wasReleasedThisFrame)
        {
            mouseDown = false;

            switch (magnetState)
            {
                case MagnetState.NONE:
                    break;
                case MagnetState.THROWN:
                    magnetPointEffector.enabled = false;
                    break;
                case MagnetState.FIXED:
                    magnetPointEffector.enabled = false;
                    break;
                default:
                    break;
            }
        }
    }
}
