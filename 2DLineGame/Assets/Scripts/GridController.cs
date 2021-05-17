using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{

    public int height;
    private int width;
    private float length;
    public float itemDistance;

    public float startXTest, startYTest;
    public int timesUntilNextTurn = 2;
    public GameObject dotPrefab;
    public GameObject pointPrefab;
    private GameObject guide;

    private Player currentPlayer;
    private Player p1;
    private Player p2;
    public GameObject sqrp1Prefab, sqrp2Prefab, lostPrefab, previewPrefab;

    private GameObject grid;
    private GameObject squares;

    private bool isMovingInXDirection;
    private Vector2 pos, prevPos;
    
    GuideController controller;

    void Start()
    {
        width = height;
        length = 2 * width;
        InitGrid();
        
        isMovingInXDirection = true;

        p1 = new Player(1, GameObject.FindGameObjectWithTag("guidep1"), sqrp1Prefab, timesUntilNextTurn);
        p2 = new Player(2, GameObject.FindGameObjectWithTag("guidep2"), sqrp2Prefab, timesUntilNextTurn);
        
        currentPlayer = p1;

        squares = GameObject.FindGameObjectWithTag("squares");
    }

    private void InitGrid()
    {

        grid = GameObject.FindGameObjectWithTag("grid");

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GameObject obj = Instantiate(dotPrefab, new Vector2((x - width / 2) * itemDistance, (y - height / 2) * itemDistance), Quaternion.identity);
                obj.transform.parent = grid.transform;
            }
        }
    }

    void Update()
    {

        guide = currentPlayer.guidePrefab;
        controller = guide.GetComponent<GuideController>();

        if (controller.shouldTrackPlaces)
        {
            Instantiate(previewPrefab, new Vector2(controller.cursorX * itemDistance, controller.cursorY * itemDistance), Quaternion.identity);
        }

        if (prevPos.x == controller.cursorX && prevPos.y == controller.cursorY)
        {
            controller.tmpVisitedPlaces.Clear();
            DeletePreviews();
        }

        if (!currentPlayer.isStarted)
        {
            pos = new Vector2(startXTest, startYTest);
            controller.Move(pos.x, pos.y, length, 0, itemDistance, width, height);

            currentPlayer.isStarted = true;
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            currentPlayer.spaceCounter++;

            if (currentPlayer.spaceCounter % 2 == 1)
                controller.ToggleTrackingPlaceCondition(true);
            else
            {
                DrawPrefabs();
                controller.ToggleTrackingPlaceCondition(false);
                if (CheckLost())
                {
                    Time.timeScale = 0;
                    Debug.Log("PLAYER " + currentPlayer.index + " LOST!");
                }
            }
            
            pos = controller.StopMove();



            InstantiatePointPrefab();

            MoveGuideInSpace();
            prevPos = pos;

            currentPlayer.timesUntilNextTurn--;
            if (currentPlayer.timesUntilNextTurn == 0)
            {
                ChangePlayer();
            }
        }

        if (currentPlayer.spaceCounter % 2 == 0)
        {
            if (!isMovingInXDirection)
            {
                if (Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.RightArrow))
                {
                    controller.ShiftStart(new Vector2(1, 0));
                }
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    controller.ShiftStart(new Vector2(-1, 0));
                }
            }
            else
            {
                if (Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.UpArrow))
                {
                    controller.ShiftStart(new Vector2(0, 1));
                }
                if (Input.GetKeyDown(KeyCode.S) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    controller.ShiftStart(new Vector2(0, -1));
                }
            }
        }
    }

    private void ChangePlayer()
    {
        currentPlayer.isStarted = false;
        currentPlayer.guidePrefab.GetComponent<GuideController>().StopMove();
        currentPlayer.guidePrefab.transform.position = new Vector2(1000, 1000);

        currentPlayer.timesUntilNextTurn = timesUntilNextTurn;
        if (currentPlayer.index == 1)
            currentPlayer = p2;
        else
            currentPlayer = p1;

        currentPlayer.spaceCounter = 0;

        DeletePreviews();
    }

    void DrawPrefabs()
    {
        List<Vector2> places = controller.tmpVisitedPlaces;
        
        foreach(var item in places)
        {
            currentPlayer.placedblocks.Add(item);
            GameObject obj = Instantiate(currentPlayer.sqrPrefab, new Vector2(item.x * itemDistance, item.y * itemDistance), Quaternion.identity);
            obj.transform.parent = squares.transform;
        }
        controller.tmpVisitedPlaces.Clear();
    }

    private bool CheckLost()
    {
        foreach(var i in p1.placedblocks)
        {
            foreach (var j in p2.placedblocks)
            {
                if (i.x == j.x && i.y == j.y)
                {
                    Instantiate(lostPrefab, new Vector2(i.x * itemDistance, i.y * itemDistance), Quaternion.identity);
                    return true;
                }
            }
        }
        return false;
    }

    void MoveGuideInSpace()
    {
        if (currentPlayer.spaceCounter % 4 == 0 || currentPlayer.spaceCounter % 4 == 1)
        {
            controller.Move(pos.x, pos.y, length, 0, itemDistance, width, height);
            isMovingInXDirection = true;
        }
        else
        {
            controller.Move(pos.x, pos.y, 0, length, itemDistance, width, height);
            isMovingInXDirection = false;
        }

        
    }

    void InstantiatePointPrefab()
    {
        GameObject obj = Instantiate(currentPlayer.sqrPrefab, new Vector2(pos.x * itemDistance, pos.y * itemDistance), Quaternion.identity);
        obj.transform.parent = squares.transform;
    }

    void DeletePreviews()
    {
        foreach (var item in GameObject.FindGameObjectsWithTag("preview"))
        {
            Destroy(item);
        }
    }
}


public class Player
{
    public GameObject guidePrefab;
    public GameObject sqrPrefab;
    public int index;
    public int timesUntilNextTurn;
    public bool isStarted;
    public int spaceCounter;

    public List<Vector2> placedblocks;

    public Player(int index, GameObject guidePrefab, GameObject sqrPrefab, int timesUntilNextTurn)
    {
        this.index = index;
        this.guidePrefab = guidePrefab;
        this.sqrPrefab = sqrPrefab;
        this.timesUntilNextTurn = timesUntilNextTurn;
        isStarted = false;
        spaceCounter = 0;

        this.placedblocks = new List<Vector2>();

    }
}
