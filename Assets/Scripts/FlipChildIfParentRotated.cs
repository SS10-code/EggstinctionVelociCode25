using UnityEngine;

public class FlipToFacePlayer : MonoBehaviour
{
    public bool facingRight = true; 

    private Transform player;

    void Start()
    {
        GameObject playerObj = GameObject.FindWithTag("Player");

        player = playerObj.transform;
    }

    void Update()
    {
        if (player == null) return;

        Vector3 scale = transform.localScale;

        bool playerIsLeft = player.position.x < transform.position.x;

        if (facingRight)
            scale.x = playerIsLeft ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);

        else
            scale.x = playerIsLeft ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);



        transform.localScale = scale;
    }
}
