using UnityEngine;

public class PickSprite : MonoBehaviour
{
    public GameObject[] sprites;
    private void Start()
    {
        int index = Random.Range(0, sprites.Length);
        for(int i = 0; i < sprites.Length; i++)
        {
            if (i == index)
            {
                sprites[i].SetActive(true);
            }
            else
            {
                sprites[i].SetActive(false);
            }
        }
    }
}
