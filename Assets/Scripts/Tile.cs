using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour
{
    public int x;
    public int y;
    public int colorID;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private BoardController boardController;

    public void OnPointerDown(PointerEventData eventData)
    {
        startTouchPosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        endTouchPosition = Input.mousePosition;
        CalculateSwape();
    }

    void CalculateSwape()
    {
        Vector2 distance = endTouchPosition - startTouchPosition;
        if (distance.magnitude > 50)
        {
            if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
            {
                if (distance.x > 0)
                {
                    //swipe right
                }
                else
                {
                    //swipe left
                }
            }
            else
            {
                if (distance.y > 0)
                {
                    //swipe up
                }
                else
                {
                    //swipe down
                }
            }
        }
        else
        {

        }
    }
}