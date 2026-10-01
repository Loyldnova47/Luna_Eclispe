using UnityEngine;

public class MoveImageAuto : MonoBehaviour
{
    // the two points I want my image to move between
    public RectTransform startPoint;
    public RectTransform endPoint;

    // how long it takes to go from start to end (in seconds)
    public float moveTime = 5f;

    // keeps track of how far along we are, 0 = start, 1 = end
    private float timer = 0f;

    void Start()
    {
        // make sure it starts right at the beginning position
        timer = 0f;
    }

    void Update()
    {
        // keep adding time until we reach the end
        if (timer < 1f)
        {
            timer += Time.deltaTime / moveTime;

            RectTransform myRect = GetComponent<RectTransform>();
            myRect.anchoredPosition = Vector2.Lerp(startPoint.anchoredPosition, endPoint.anchoredPosition, timer);
        }
    }
}