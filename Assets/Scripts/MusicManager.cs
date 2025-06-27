using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager self;
    private void Awake()
    {
        self = this;
        DontDestroyOnLoad(gameObject);
    }
}
