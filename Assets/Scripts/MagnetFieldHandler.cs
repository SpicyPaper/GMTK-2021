using UnityEngine;

public class MagnetFieldHandler : MonoBehaviour
{
    public GameObject magnetPointLeft = null;
    public GameObject magnetPointRight = null;

    public MagnetLauncherManager magnetLauncherLeft = null;
    public MagnetLauncherManager magnetLauncherRight = null;

    public GameObject characterController = null;

    [SerializeField] private MagnetFieldMesh fieldMesh = null;

    [SerializeField] private Color color = Color.red;

    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    private float fieldCastTime;
    private float timePassed;
    private bool casting;

    private Rigidbody2D rigidbody;
    private CharacterController2D characterController2D;

    private const float FLASH_TIME = 0.2f;
    private const float DAMAGE_OFFSET = 0.2f;

    private void Start()
    {
        casting = false;
        rigidbody = characterController.GetComponent<Rigidbody2D>();
        characterController2D = characterController.GetComponent<CharacterController2D>();

        fieldMesh.UpdateColor(color);
    }

    public void Attack()
    {
        if (AreMagnetsFixed())
        {
            if (!casting)
            {
                GetMagnetFieldCoordinates();

                casting = true;
                fieldCastTime = 1 + ComputeSurface() / 60;

                rigidbody.constraints = RigidbodyConstraints2D.FreezePosition;
            }
        }
    }

    private void Update()
    {

        if (casting)
        {
            timePassed += Time.deltaTime;

            if (timePassed > fieldCastTime - FLASH_TIME)
            {
                float flashProgress = 1 - 2 * Mathf.Abs(timePassed - (fieldCastTime - FLASH_TIME / 2)) / FLASH_TIME;
                if (flashProgress < 0)
                {
                    flashProgress = 0;
                }

                Color mixedColor = Color.Lerp(color, Color.white, flashProgress);
                fieldMesh.UpdateColor(mixedColor);
            }

            if (timePassed > fieldCastTime - DAMAGE_OFFSET)
            {
                fieldMesh.SetIsTrigger(true);
            }

            float progress = timePassed / fieldCastTime;
            if (progress >= 1)
            {
                progress = 0;
                timePassed = 0;
                casting = false;
                rigidbody.constraints = RigidbodyConstraints2D.None;
                fieldMesh.SetIsTrigger(false);
            }

            fieldMesh.UpdateProgress(progress);
        }
    }

    private bool AreMagnetsFixed()
    {
        return magnetLauncherLeft.magnetState == MagnetLauncherManager.MagnetState.FIXED
            && magnetLauncherRight.magnetState == MagnetLauncherManager.MagnetState.FIXED;
    }

    private void GetMagnetFieldCoordinates()
    {
        p1 = characterController.transform.position;
        p2 = magnetPointLeft.transform.position;
        p3 = magnetPointRight.transform.position;

        fieldMesh.UpdateMesh(new Vector3[] { p1, p2, p3 });
    }

    private float ComputeSurface()
    {
        return Mathf.Abs(p1.x * p2.y + p2.x * p3.y + p3.x * p1.y - p1.y * p2.x - p2.y * p3.x - p3.y * p1.x) / 2;
    }
}
