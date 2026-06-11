using UnityEngine;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public int x;
    public int y;
    public int colorID;

    private Vector2 startTouchPosition;
    private Vector2 endTouchPosition;
    private BoardController boardController;

    public void OnPointerDown(PointerEventData eventData)
    {
        startTouchPosition = eventData.position;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        endTouchPosition = eventData.position;
        CalculateSwape();
    }

    void CalculateSwape()
    {
        Vector2 distance = endTouchPosition - startTouchPosition;
        if (distance.magnitude > 50)
        {
            if (boardController == null)
            {
                boardController = Object.FindAnyObjectByType<BoardController>();
            }

            if (boardController == null) return;

            if (Mathf.Abs(distance.x) > Mathf.Abs(distance.y))
            {
                if (distance.x > 0)
                {
                    boardController.MoveTitle(this, Vector2.right);
                }
                else
                {
                    boardController.MoveTitle(this, Vector2.left);
                }
            }
            else
            {
                if (distance.y > 0)
                {
                    boardController.MoveTitle(this, Vector2.up);
                }
                else
                {
                    boardController.MoveTitle(this, Vector2.down);
                }
            }
        }
        else
        {

        }
    }
}