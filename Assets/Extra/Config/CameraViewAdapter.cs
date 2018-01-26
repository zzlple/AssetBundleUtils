using UnityEngine;
using System.Collections;
[RequireComponent(typeof(Camera))]
[ExecuteInEditMode]
public class CameraViewAdapter : MonoBehaviour
{
    public Transform target;

    private Camera theCamera;
    [Range(0,1000)]
    public float distance = 8.5f;


    private Transform tx;


    void Awake()
    {
        if (!theCamera)
        {
            theCamera = GetComponent<Camera>();
        }
        tx = theCamera.transform;




    }

     void Start()
    {

        target.position = getCenterCenter();

        StartCoroutine(AutoScale(target));
    }


    void Update()
    {

        //   Debug.Log(target.forward);

        //distance = Vector3.Distance(theCamera.transform.position, target.position);
        findCorners();

    }


    private bool IsTagBeIntersects(Transform target)
    {

        MeshRenderer[] mfs = target.GetComponentsInChildren<MeshRenderer>();

        Bounds[] bounds = getBounds();

        for (int i = 0; i < mfs.Length; i++)
        {

            if (mfs[i].bounds.Intersects(bounds[0]) ||
                mfs[i].bounds.Intersects(bounds[1]) ||
                mfs[i].bounds.Intersects(bounds[2]) ||
                mfs[i].bounds.Intersects(bounds[3])||
                mfs[i].bounds.Intersects(bounds[4])||
                mfs[i].bounds.Intersects(bounds[5])
               
               )
            {
                return true;
            }
        }

        return false;

    }




   public  IEnumerator AutoScale(Transform src)
    {


        if (IsTagBeIntersects(src))
        {

            while (IsTagBeIntersects(src))
            {


                src.localScale /= 1.2f;

                Debug.Log(src.localScale);

                yield return new WaitForEndOfFrame();
            }


            src.localScale *= 1.2f;
        }
        else
        {

            while (!IsTagBeIntersects(src))
            {


                src.localScale *= 1.2f;

                Debug.Log(src.localScale);

                yield return new WaitForEndOfFrame();
            }



        }




        yield return 0;

    }





    private float distanceWidth
    {
        get
        {
            Vector3[] corners = getCorners(distance);
            return Vector3.Distance(corners[0], corners[1]);
        }
    }
    private float distanceHeight
    {
        get
        {
            Vector3[] corners = getCorners(distance);
            return Vector3.Distance(corners[2], corners[0]);
        }
    }

    void findCorners()
    {
        Vector3[] corners = getCorners(distance);

        // for debugging
        Debug.DrawLine(corners[0], corners[1], Color.yellow); // UpperLeft -> UpperRight
        Debug.DrawLine(corners[1], corners[3], Color.yellow); // UpperRight -> LowerRight
        Debug.DrawLine(corners[3], corners[2], Color.yellow); // LowerRight -> LowerLeft
        Debug.DrawLine(corners[2], corners[0], Color.yellow); // LowerLeft -> UpperLeft



        Debug.DrawLine(corners[0], corners[0] + Vector3.forward * distanceWidth, Color.yellow);
        Debug.DrawLine(corners[1], corners[1] + Vector3.forward * distanceWidth, Color.yellow);
        Debug.DrawLine(corners[2], corners[2] + Vector3.forward * distanceWidth, Color.yellow);
        Debug.DrawLine(corners[3], corners[3] + Vector3.forward * distanceWidth, Color.yellow);


        Debug.DrawLine(corners[0] + Vector3.forward * distanceWidth, corners[1] + Vector3.forward * distanceWidth, Color.yellow);
        Debug.DrawLine(corners[1] + Vector3.forward * distanceWidth, corners[3] + Vector3.forward * distanceWidth, Color.yellow);
        Debug.DrawLine(corners[3] + Vector3.forward * distanceWidth, corners[2] + Vector3.forward * distanceWidth, Color.yellow);
        Debug.DrawLine(corners[2] + Vector3.forward * distanceWidth, corners[0] + Vector3.forward * distanceWidth, Color.yellow);
        Debug.DrawLine(theCamera.transform.position, getCenterCenter(), Color.blue);




        Debug.DrawLine(getTopCenter(), getLeftCenter(), Color.green);

        Debug.DrawLine(getTopCenter(), getRightCenter(), Color.green);


        Debug.DrawLine(getTopCenter(), getForwardCenter(), Color.green);

        Debug.DrawLine(getTopCenter(), getBackCenter(), Color.green);

        Debug.DrawLine(getTopCenter(), getBottomCenter(), Color.green);

    }


    public Vector3 getTopCenter()
    {
        Vector3[] corners = getCorners(distance);
        Vector3 nearCenter = corners[0] + Vector3.right * (distanceWidth / 2);
        Vector3 topCenter = nearCenter + Vector3.forward * (distanceWidth / 2);
        return topCenter;
    }

    public Vector3 getBottomCenter()
    {
        return getTopCenter() - Vector3.up * distanceHeight;
    }

    public Vector3 getLeftCenter()
    {
        return getTopCenter() + Vector3.left * (distanceWidth / 2) - Vector3.up * (distanceHeight / 2);
    }


    public Vector3 getRightCenter()
    {
        return getTopCenter() + Vector3.right * (distanceWidth / 2) - Vector3.up * (distanceHeight / 2);
    }



    public Vector3 getForwardCenter()
    {
        return getTopCenter() + Vector3.forward * (distanceWidth / 2) - Vector3.up * (distanceHeight / 2);
    }





    public Vector3 getBackCenter()
    {
        return getTopCenter() + Vector3.back * (distanceWidth / 2) - Vector3.up * (distanceHeight / 2);
    }






    public Vector3 getCenterCenter()
    {
        Vector3[] corners = getCorners(distance);

        Vector3 centerCenter = getTopCenter() - Vector3.up * (distanceHeight / 2);

        return centerCenter;
    }



    /// <summary>
    /// Gets the bounds.left front right back top bottom 
    /// </summary>
    /// <returns>The bounds.</returns>
    public Bounds[] getBounds()
    {

        Vector3[] corners = getCorners(distance);
        Bounds[] bounds = new Bounds[6];

        bounds[0] = new Bounds(getLeftCenter(), new Vector3(0,distanceHeight,distanceWidth));
        bounds[1] = new Bounds(getForwardCenter(), new Vector3(distanceWidth, distanceHeight, 0));
        bounds[2] = new Bounds(getRightCenter(), new Vector3(0, distanceHeight, distanceWidth));
        bounds[3] = new Bounds(getBackCenter(), new Vector3(distanceWidth, distanceHeight, 0));
        bounds[4] = new Bounds(getTopCenter(), new Vector3(distanceWidth, 0, distanceWidth));
        bounds[5] = new Bounds(getBottomCenter(), new Vector3(distanceWidth, 0, distanceWidth));


        return bounds;
    }



    public Vector3[] getCorners(float distance)
    {
        Vector3[] corners = new Vector3[4];

        float halfFOV = (theCamera.fieldOfView * 0.5f) * Mathf.Deg2Rad;
        float aspect = theCamera.aspect;

        float height = distance * Mathf.Tan(halfFOV);
        float width = height * aspect;

        // UpperLeft
        corners[0] = tx.position - (tx.right * width);
        corners[0] += tx.up * height;
        corners[0] += tx.forward * distance;

        // UpperRight
        corners[1] = tx.position + (tx.right * width);
        corners[1] += tx.up * height;
        corners[1] += tx.forward * distance;

        // LowerLeft
        corners[2] = tx.position - (tx.right * width);
        corners[2] -= tx.up * height;
        corners[2] += tx.forward * distance;

        // LowerRight
        corners[3] = tx.position + (tx.right * width);
        corners[3] -= tx.up * height;
        corners[3] += tx.forward * distance;

        return corners;
    }
}