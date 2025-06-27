using UnityEngine;
using UnityEngine.UI;

public class LimitScrolling : MonoBehaviour
{
    public ScrollRect scrollRect;

    void LateUpdate()
    {
        if (scrollRect.verticalNormalizedPosition < 0f)
            scrollRect.verticalNormalizedPosition = 0f;

        if (scrollRect.verticalNormalizedPosition > 1f)
            scrollRect.verticalNormalizedPosition = 1f;
    }
}
