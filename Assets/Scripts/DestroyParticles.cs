using UnityEngine;

public class DestroyParticles : MonoBehaviour
{
    private void Start()
    {
        Destroy(gameObject, 3f);
    }
}
