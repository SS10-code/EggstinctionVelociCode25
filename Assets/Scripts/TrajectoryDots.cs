using System.Collections.Generic;
using UnityEngine;

public class TrajectoryDots : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject dotPrefab;
    [SerializeField] private int maxDots = 100;
    [SerializeField] private float timeStep = 0.01f; 

    [SerializeField] private LayerMask collisionMask;
    [SerializeField, Range(0f, 2f)] private float bounceDamping = 0.7f;
    [SerializeField, Range(0f, 10f)] private float linearDrag = 0.1f;
    [SerializeField] private float minSpeed = 0.01f;

    [SerializeField] private float maxSimTime = 10f;

    private List<GameObject> dots = new List<GameObject>();
    public Rigidbody2D eggRB;

    public void Initialize(Rigidbody2D rb)
    {
        eggRB = rb;

        if (dots.Count == 0)
        {
            for (int dotNum = 0; dotNum < maxDots; dotNum++)
            {
                GameObject dot = Instantiate(dotPrefab, transform);

                dot.SetActive(false);
                dots.Add(dot);
            }
        }
    }

    public void Show(Vector2 initialVelocity)
    {
        if (eggRB == null) return;

        Vector2 pos = eggRB.position;
        Vector2 vel = initialVelocity;

        float simTime = 0f;

        int activeDots = 0;
        bool bounced = false;

        for (int dotNum = 0; dotNum < maxDots; dotNum++)
        {
            if (vel.magnitude < minSpeed || simTime > maxSimTime)
                break;

            float dragAmount = 1f / (1f + linearDrag * timeStep);
            vel *= dragAmount;

            Vector2 nextPos = pos + vel * timeStep;

            RaycastHit2D hit = Physics2D.Linecast(pos, nextPos, collisionMask);
            if (hit.collider != null)
            {
                pos = hit.point;

                vel = Vector2.Reflect(vel, hit.normal) * bounceDamping;

                pos += vel.normalized * 0.01f;

                bounced = true;
            }
            else
            {
                pos = nextPos;
                bounced = false;

            }

            simTime += timeStep;

            GameObject dot = dots[dotNum];
            dot.transform.position = new Vector3(pos.x, pos.y, 0f);

            dot.SetActive(true);


            if (bounced)
                dot.GetComponent<SpriteRenderer>().color = Color.yellow;

            else
                dot.GetComponent<SpriteRenderer>().color = Color.white;


            dot.transform.localScale = Vector3.one * 0.2f;
            activeDots++;
        }

        if (activeDots > 0)
        {
            GameObject finalDot = dots[activeDots - 1];
            finalDot.transform.localScale = Vector3.one * 0.4f;
            finalDot.GetComponent<SpriteRenderer>().color = Color.red;
        }

        for (int dotNum = activeDots; dotNum < dots.Count; dotNum++)
            dots[dotNum].SetActive(false);
    }

    public void Hide()
    {
        foreach (var dot in dots)
            dot.SetActive(false);
    }
}
