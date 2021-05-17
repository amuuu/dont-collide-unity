using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuideController : MonoBehaviour
{

    public float cursorX, cursorY;
    private bool isMoving = false;
    private float startX;
    private float startY;
    private float lengthX;
    private float lengthY;

    private int dirX = 1;
    private int dirY = 1;
    private float scaleFactor;

    private float timeToGo;

    private float width;
    private float height;
    
    public List<Vector2> tmpVisitedPlaces;
    public bool shouldTrackPlaces = false;

    private void Start()
    {
        cursorX = 0;
        cursorY = 0;
        timeToGo = Time.fixedTime + 0.16f;

    }

    void FixedUpdate()
    {
        if (isMoving)
        {
            if (Time.fixedTime >= timeToGo)
            {
                updateCoordinates();
                timeToGo = Time.fixedTime + 0.16f;
            }
        }
        
    }

    public void Move(float startX, float startY, float lengthX, float lengthY, float scaleFactor, int width, int height)
    {
        //Debug.Log(" lx " + lengthX + " ly " + lengthY + " sx " + startX + " sy " + startY);

        this.startX = startX;
        this.startY = startY;
        this.lengthX = lengthX;
        this.lengthY = lengthY;
        this.scaleFactor = scaleFactor;

        this.width = width;
        this.height = height;

        isMoving = true;

        cursorX = this.startX;
        cursorY = this.startY;
    }

    public Vector2 StopMove()
    {
        isMoving = false;
        return new Vector2(cursorX, cursorY);
    }

    public void ShiftStart(Vector2 newPos)
    {
        cursorX += newPos.x;
        cursorY += newPos.y;
    }
    public void ToggleTrackingPlaceCondition(bool input)
    {
        shouldTrackPlaces = input;
        Debug.Log("SASDASD " + shouldTrackPlaces);
    }

    void updateCoordinates()
    {

        if (lengthX != 0)
        {
            //cursorX += 1 * dirX;
            //Debug.Log("x " + cursorX);
            cursorX = UpdatePosition(cursorX, dirX, width);
            dirX *= CheckBounds(startX, lengthX, cursorX, width);
        }
        
        if (lengthY != 0)
        {
            //cursorY += 1 * dirY;
            //Debug.Log("y " + cursorY);

            cursorY = UpdatePosition(cursorY, dirY, height);

            dirY *= CheckBounds(startY, lengthY, cursorY, height);
        }

        if (shouldTrackPlaces) { tmpVisitedPlaces.Add(new Vector2(cursorX, cursorY)); Debug.Log("HERE BOR"); }
        transform.position = new Vector2(cursorX * scaleFactor, cursorY * scaleFactor);


    }

    int CheckBounds(float start, float length, float cursor, float bound)
    {
        if ((cursor >= start + length) || ((cursor < bound/2) && (cursor > bound / 2 - 0.5f)))
        {
            //Debug.Log("JERE");
            return -1;
        }
        else
            return 1;
    }

    float UpdatePosition(float cursor, int dir, float bound)
    {
        cursor += 1 * dir;

        if (cursor >= bound / 2)
            cursor = -1 * bound / 2;
        
        if (cursor < -1 * bound / 2)
            cursor = bound /2 - 1;

        return cursor;
    }
}
