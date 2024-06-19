using System;
using UnityEngine;

public class baloonInflating : MonoBehaviour
{
    public Rigidbody rb;

    public float maxSize = 3;
    [Range(0, 1)] public float riseSize = .5f; // Determines at what percentage of the max size the balloon Gameobject will start to rise.

    public float timeToReachMaxSize = 1;
    public float timeBeforeDeflatingInSecs = 1;
    private float timeInflating;

    private float startDrag; //equals the 'Drag' setting of the Rigidbody component.
    public float maxDrag = 10; //has to be higher than the 'Drag' setting of the Rigidbody component.

    private float speed;
    public float maxSpeed;

    private Vector3 baseScale;
    private Vector3 targetScale;
    // private Vector3 sizeToRise; //the scale factor at which the balloon game object should start rising

    private bool inflating = false;
    private float timer;

    void Start()
    {
        // rb = GetComponent<Rigidbody>();
        baseScale = rb.transform.localScale;
        targetScale = baseScale * maxSize;
        // sizeToRise = baseScale * (riseSize * maxSize);
        startDrag = rb.drag;
    }

    void Update()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            print("inflating");
            timeInflating += Time.deltaTime;
            inflating = true;
            timer = 0;
        }
        else
        {
            if (inflating == true)
            {
                timer += Time.deltaTime;
                if (timer > timeBeforeDeflatingInSecs)
                {
                    inflating = false;
                }
            }
        }

        // if (transform.localScale.magnitude >= sizeToRise.magnitude)
        // {
        //     rb.useGravity = false;
        //     rb.velocity = new Vector3(0, speed, 0);
        // }
        // else
        // {
        //     rb.useGravity = true;
        // }

        if (!inflating)
        {
            timeInflating -= Time.deltaTime;
        }

        timeInflating = Mathf.Clamp(timeInflating, 0, timeToReachMaxSize);

        rb.transform.localScale = Vector3.Lerp(baseScale, targetScale, timeInflating / timeToReachMaxSize); // Inflate/Deflate over time.

        // rb.drag = Mathf.Lerp(startDrag, maxDrag, InverseLerp(baseScale, sizeToRise, transform.localScale)); //Determines the drag of the balloon. The bigger/the more inflated the balloon the faster it will rise and vice versa.

        // speed = Mathf.Lerp(0, maxSpeed, InverseLerp(sizeToRise, targetScale, transform.localScale)); //Determines the upwards velocity depending on the size of the balloon game object.
    }

    public static float InverseLerp(Vector3 a, Vector3 b, Vector3 value)
    {
        Vector3 AB = b - a;
        Vector3 AV = value - a;
        return Vector3.Dot(AV, AB) / Vector3.Dot(AB, AB);
    }
}