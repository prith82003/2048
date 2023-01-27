using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchControls : MonoBehaviour
{
    public static System.Action<direction> OnSwipe;

    public float minPercentForSwipe = 0.20f;

    Vector2 startPos;

    /// <summary>
    /// Checks for any touches
    /// </summary>
    void Update()
    {
        // If there are any touches on the screen
        if (Input.touchCount > 0)
        {
            // If the touch has just began
            if (Input.GetTouch(0).phase == TouchPhase.Began)
            {
                StartCoroutine(StartTouch());
            }
        }
    }

    /// <summary>
    /// Checks if a swipe has been made
    /// </summary>
    /// <returns>null</returns>
    IEnumerator StartTouch()
    {
        startPos = Input.GetTouch(0).position;

        while (Input.GetTouch(0).phase != TouchPhase.Ended)
        {
            Vector2 currentpos = Input.GetTouch(0).position;

            // Check if the finger has moved across minimum portion of screen
            if (Mathf.Abs(currentpos.x - startPos.x) / Screen.currentResolution.width > minPercentForSwipe)
            {
                // Tell other scripts that swipe has happened and its direction
                if (currentpos.x > startPos.x)
                    OnSwipe?.Invoke(direction.right);
                else
                    OnSwipe?.Invoke(direction.left);

                yield break;
            }

            if (Mathf.Abs(currentpos.y - startPos.y) / Screen.currentResolution.height > minPercentForSwipe)
            {
                if (currentpos.y > startPos.y)
                    OnSwipe?.Invoke(direction.up);
                else
                    OnSwipe?.Invoke(direction.down);

                yield break;
            }

            yield return null;
        }
    }

}
