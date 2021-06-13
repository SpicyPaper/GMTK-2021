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
    public ParticleSystem launcherParticleSystem = null;
    public bool isPlayerRed;
    public bool isControlledByGamepad;

    public MagnetState magnetState;

    private PointEffector2D magnetPointEffector;
    private ParticleSystem particleSystem;
    private Rigidbody2D magnetRigidbody;
    private SpriteRenderer magnetSprite;

    private Vector2 aimDirection;
    private bool isPressed;
    private bool isReleased;

    private bool isStayedPress;

    private const float MAGNET_SPEED = 10;
    private const float MAGNET_DISTANCE_FROM_BODY = 0.6f;
    private const float MAGNET_OFFSET = 0.05f;

    public void SetMagnetToFixed()
    {
        if (magnetState == MagnetState.THROWN)
        {
            magnetState = MagnetState.FIXED;
        }
    }

    public void Init()
    {
        magnetPointEffector = magnetPoint.GetComponent<PointEffector2D>();
        magnetRigidbody = magnetPoint.GetComponent<Rigidbody2D>();
        magnetSprite = magnetPoint.GetComponentInChildren<SpriteRenderer>();
        particleSystem = magnetPoint.GetComponentInChildren<ParticleSystem>();
    }

    private void Start()
    {
        aimDirection = Vector2.up;
        magnetSprite.enabled = false;
        magnetPointEffector.enabled = false;
        particleSystem.Stop();
        launcherParticleSystem.Stop();
        AudioManager.instance.StopMagnetSound(isPlayerRed, launcherSide);
    }

    public void Reset()
    {
        if (magnetPointEffector != null)
        {
            aimDirection = Vector2.up;
            magnetSprite.enabled = false;
            magnetPointEffector.enabled = false;

            magnetState = MagnetState.NONE;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (((collision.tag == "MagnetRight" && launcherSide == LauncherSide.RIGHT) ||
            (collision.tag == "MagnetLeft" && launcherSide == LauncherSide.LEFT))
            &&
            ((collision.transform.parent.tag == "PlayerRed" && isPlayerRed) ||
            (collision.transform.parent.tag == "PlayerGreen" && !isPlayerRed))
            &&
            magnetState == MagnetState.FIXED)
        {
            magnetPointEffector.enabled = false;
            magnetSprite.enabled = false;
            particleSystem.Stop();
            launcherParticleSystem.Stop();
            AudioManager.instance.StopMagnetSound(isPlayerRed, launcherSide);
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

    public void Aim(Vector2 aimDirection)
    {
        if (!isControlledByGamepad)
        {
            return;
        }

        aimDirection = aimDirection.normalized;

        if (aimDirection.magnitude <= 0.1f)
        {
            return;
        }

        this.aimDirection = aimDirection;
    }

    private void ApplyAim(Vector2 vector)
    {
        Vector2 bodyPos = body.transform.position;

        float radian = Mathf.Atan2(vector.y, vector.x);
        float degrees = radian * Mathf.Rad2Deg;
        float resDegrees = degrees - 90;
        float offset;

        if (!magnetPointEffector.enabled)
        {
            offset = (launcherSide == LauncherSide.LEFT ? 1 : -1) * MAGNET_OFFSET;
        }
        else
        {
            offset = 0;
        }

        magnetLauncher.transform.position = bodyPos + vector * MAGNET_DISTANCE_FROM_BODY +
            RotateVector(vector, 90) * offset;
        magnetLauncher.transform.rotation = Quaternion.Euler(0, 0, resDegrees);
    }

    public void Pressed(bool action)
    {
        this.isPressed = action;
    }

    public void Released(bool action)
    {
        this.isReleased = action;
    }

    private void ApplyShoot()
    {
        if (isStayedPress && magnetState == MagnetState.FIXED)
        {
            magnetPointEffector.enabled = true;
        }
        if (isPressed)
        {
            isPressed = false;
            isStayedPress = true;

            switch (magnetState)
            {
                case MagnetState.NONE:
                    magnetSprite.enabled = true;
                    magnetPoint.transform.position = body.transform.position;
                    magnetRigidbody.velocity = aimDirection * MAGNET_SPEED;
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
        if (isReleased)
        {
            isReleased = false;
            isStayedPress = false;

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

    // Update is called once per frame
    void Update()
    {
        Mouse mouse = Mouse.current;

        if (mouse == null)
        {
            Debug.LogError("No mouse connected.");
            return;
        }
        Vector2 mousePos = mainCamera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        Vector2 magnetPos = magnetPoint.transform.position;
        Vector2 bodyPos = body.transform.position;

        Vector2 resVector;

        if (magnetPointEffector.enabled)
        {
            resVector = magnetPos - bodyPos;
            particleSystem.Play();
            launcherParticleSystem.Play();
            AudioManager.instance.PlayMagnetSound(isPlayerRed, launcherSide);

            this.aimDirection = resVector.normalized;
        }
        else
        {
            particleSystem.Stop();
            launcherParticleSystem.Stop();
            AudioManager.instance.StopMagnetSound(isPlayerRed, launcherSide);
        }

        // Mouse move
        if (!isControlledByGamepad)
        {
            if (!magnetPointEffector.enabled)
            {
                resVector = mousePos - bodyPos;

                this.aimDirection = resVector.normalized;
            }

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

            isPressed = control.wasPressedThisFrame;
            isReleased = control.wasReleasedThisFrame;
        }

        ApplyAim(this.aimDirection);
        ApplyShoot();
    }
}
