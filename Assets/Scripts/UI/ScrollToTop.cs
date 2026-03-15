using UnityEngine;
using UnityEngine.UI;

public class ScrollToTop : MonoBehaviour
{
    private ScrollRect _scrollRect;

    void Start()
    {
        _scrollRect = GetComponent<ScrollRect>();
        _scrollRect.verticalNormalizedPosition = 1f;
    }
}