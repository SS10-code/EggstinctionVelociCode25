using UnityEngine;
using UnityEngine.UI;


public class ClickSound : MonoBehaviour
{
    public AudioClip clickSound;
    private AudioSource audioSource;
    public GameObject clickObject;

    void Start()
    {
        if(clickObject == null)
            audioSource = Camera.main.GetComponent<AudioSource>();
        else
            audioSource = clickObject.GetComponent<AudioSource>();
    }

    public void PlayClickSound()
    {

        audioSource = Camera.main.GetComponent<AudioSource>();
        audioSource.PlayOneShot(clickSound);
    }
}
