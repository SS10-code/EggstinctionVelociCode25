using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class EggController : MonoBehaviour
{
    [SerializeField] private TrajectoryDots trajectory;

    public AudioClip clickSound;
    private AudioSource audioSource;

    [Header("Movement")]
    [SerializeField] private float minSpeed = 0.05f;
    [SerializeField] private float maxPullDistance = 3f;

    [Header("Spin")]
    [SerializeField] private float rotationDuration = 0.3f;

    private Queue<float> rotationQueue = new Queue<float>();
    private bool isRotating = false;

    public float launchPower = 10f;

    private Rigidbody2D rb;
    private Vector2 dragStartPos;
    private bool dragging = false;

    void Start()
    {
        audioSource = Camera.main.GetComponent<AudioSource>();

        rb = GetComponent<Rigidbody2D>();

        trajectory.Initialize(rb);
    }

    void Update()
    {
        if (IsStill())
        {
            if (Input.GetMouseButtonDown(0))
            {
                dragStartPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
                dragging = true;
            }

            if (Input.GetMouseButton(0) && dragging)
            {

                Vector2 currentPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 direction = dragStartPos - currentPos;

                if (direction.magnitude > maxPullDistance)
                {
                    direction = direction.normalized * maxPullDistance;
                    dragStartPos = currentPos + direction;
                }

                trajectory.Show(direction * launchPower);
            }

            if (Input.GetMouseButtonUp(0) && dragging)
            {
                Vector2 dragEndPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                Vector2 direction = dragStartPos - dragEndPos;


                if (direction.magnitude > maxPullDistance)
                    direction = direction.normalized * maxPullDistance;

                Launch(direction);
                dragging = false;
                trajectory.Show(direction * launchPower);

            }
        }
        else
        {
            dragging = false;

            trajectory.Hide();
        }
    }

    void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction * launchPower;
    }

    bool IsStill()
    {

        return rb.linearVelocity.magnitude < minSpeed;
    }


    void OnCollisionEnter2D(Collision2D collision)
    {

        if(!(collision.gameObject.layer == 8 || collision.gameObject.tag == "Enemy"))
        {
            audioSource.PlayOneShot(clickSound);
            AddRotToQueue(90f);

        }

        if(collision.gameObject.layer != 8)
            CameraShake.Instance.Shake(0.1f, 0.1f);
    }

    public void AddRotToQueue(float degrees)
    {

        rotationQueue.Enqueue(degrees);
        if (!isRotating)
            StartCoroutine(RotQueue());
    }

    private IEnumerator RotQueue()
    {
        isRotating = true;

        while (rotationQueue.Count > 0)
        {
            float degrees = rotationQueue.Dequeue();
            float elapsed = 0f;
            float startZ = transform.eulerAngles.z;

            float endZ = startZ + degrees;

            while (elapsed < rotationDuration)
            {

                float z = Mathf.Lerp(startZ, endZ, elapsed / rotationDuration);
                transform.rotation = Quaternion.Euler(0f, 0f, z);

                elapsed += Time.deltaTime;
                yield return null;
            }

            transform.rotation = Quaternion.Euler(0f, 0f, endZ);
        }
        isRotating = false;
    }
}

