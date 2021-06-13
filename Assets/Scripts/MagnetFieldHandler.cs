using UnityEngine;

public class MagnetFieldHandler : MonoBehaviour
{
    public GameObject magnetPointLeft = null;
    public GameObject magnetPointRight = null;

    public MagnetLauncherManager magnetLaucherLeft = null;
    public MagnetLauncherManager magnetRightLeft = null;

    public GameObject characterController = null;

    [SerializeField] private MagnetFieldMesh fieldMesh = null;

    private Vector3 p1;
    private Vector3 p2;
    private Vector3 p3;

    private float fieldCastTime;
    private float timePassed;
    private bool casting;

    private void Start()
    {
        casting = false;
    }

    private void Update()
    {
        if (Input.GetKey("e"))
        {
            if (!casting)
            {
                casting = true;
                fieldCastTime = 1 + ComputeSurface() / 10;

                GetMagnetFieldCoordinates();
            }
        }

        if (casting)
        {
            timePassed += Time.deltaTime;
            float progress = timePassed / fieldCastTime;

            if (progress >= 1)
            {
                progress = 0;
                timePassed = 0;
                casting = false;
            }

            fieldMesh.UpdateProgress(progress);
        }
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
