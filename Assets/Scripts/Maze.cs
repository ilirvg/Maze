using System.Collections.Generic;
using UnityEngine;

public class Maze : MonoBehaviour {

    [System.Serializable]
    public class Cell {
        public bool visited;
        public GameObject north;//1
        public GameObject east; //2
        public GameObject west; //3
        public GameObject south;//4
        public GameObject center; //5
    }

    public GameObject wallObj;
    public GameObject finishObj;
    public GameObject floorObj;
    public GameObject playerObj;
    public int xSize;
    public int ySize;
    public float wallLength;

    private Cell[] cells;
    private List<int> lastCells;
    private GameObject wallHolder;
    private GameObject floorHolder;
    private int currentCell;
    private int totalCells;
    private int visitedCells;
    private int currentNeighbour;
    private bool startedBuilding = false;
    private int backingUp;
    private int wallToBrake;

    void Start () {
        CreateWalls();	
	}

    void CreateWalls() {
        Vector3 initialPos = new Vector3((-xSize / 2) + wallLength / 2, 0.0f, (-ySize / 2) + wallLength / 2);
        wallHolder = new GameObject();
        wallHolder.name = "Maze";
        Vector3 myPos = initialPos;
        GameObject tempWall;

        //forX Axis
        for (int i = 0; i < ySize; i++) {
            for (int j = 0; j <= xSize; j++) {
                myPos = new Vector3(initialPos.x + (j * wallLength) - wallLength / 2, 0.3f, initialPos.z +(i * wallLength) - wallLength / 2);
                tempWall = Instantiate(wallObj, myPos, Quaternion.identity) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        //for Y Axis
        for (int i = 0; i <= ySize; i++) {
            for (int j = 0; j < xSize; j++) {
                myPos = new Vector3(initialPos.x + (j * wallLength), 0.3f, initialPos.z + (i * wallLength) - wallLength);
                tempWall = Instantiate(wallObj, myPos, Quaternion.Euler(0.0f, 90.0f, 0.0f)) as GameObject;
                tempWall.transform.parent = wallHolder.transform;
            }
        }

        CreateFloor();
    }

    void CreateFloor() {
        Vector3 initialEndPointPos = new Vector3((-xSize / 2) + wallLength / 2, 0.0f, -ySize / 2);
        GameObject tempFloor;
        floorHolder = new GameObject();
        floorHolder.name = "Floor";

        for (int l = 0; l < ySize; l++) {
            for (int m = 0; m < xSize; m++) {
                Vector3 florPos = new Vector3(initialEndPointPos.x + (m * wallLength), 0.0f, initialEndPointPos.z + (l * wallLength));
                tempFloor = Instantiate(floorObj, florPos, Quaternion.identity) as GameObject;
                tempFloor.transform.parent = floorHolder.transform;
            }
        }
        CreateCells();
    }

    void CreateCells() {
        lastCells = new List<int>();
        lastCells.Clear();
        totalCells = xSize * ySize;
        int children = wallHolder.transform.childCount;
        GameObject[] allWalls = new GameObject[children];
        cells = new Cell[xSize * ySize];
        int easWest = 0;
        int childCount = 0;
        int xCount = 0;

        //Add walls to array
        for (int i = 0; i < children; i++) {
            allWalls[i] = wallHolder.transform.GetChild(i).gameObject;
        }

        //Assign walls to cells
        for (int i = 0; i < cells.Length; i++) {
            if (xCount == xSize) {
                easWest++;
                xCount = 0;
            }
            cells[i] = new Cell();

            cells[i].east = allWalls[easWest];
            cells[i].south = allWalls[childCount + (xSize + 1) * ySize];

            easWest++;

            xCount++;
            childCount++;

            cells[i].west = allWalls[easWest];
            cells[i].north = allWalls[(childCount + (xSize + 1) * ySize) + xSize-1];
        }
        CreateMaze();
        AddPlayerAndFinish();
    }

    void CreateMaze() {
        while (visitedCells < totalCells) {
            if (startedBuilding) {
                FindNeighbour();
                if (cells[currentNeighbour].visited == false && cells[currentCell].visited == true) {
                    BrakeWall();
                    cells[currentNeighbour].visited = true;
                    visitedCells++;
                    lastCells.Add(currentCell);
                    currentCell = currentNeighbour;
                    if (lastCells.Count > 0) {
                        backingUp = lastCells.Count - 1;
                    }
                }
            }
            else {
                currentCell = Random.Range(0, totalCells);
                cells[currentCell].visited = true;
                visitedCells++;
                startedBuilding = true;
            }

        }
    }

    void FindNeighbour() {
        int length = 0;
        int[] neighbours = new int[4];
        int[] connectingWall = new int[4];
        int check = 0;

        check = ((currentCell + 1) / xSize);
        check -= 1;
        check *= xSize;
        check += xSize;

        //west
        if (currentCell + 1 < totalCells && (currentCell + 1) != check) {
            if (cells[currentCell + 1].visited == false) {
                neighbours[length] = currentCell + 1;
                connectingWall[length] = 3;
                length++;
            }
        }

        //east
        if (currentCell - 1 >= 0 && currentCell != check) {
            if (cells[currentCell - 1].visited == false) {
                neighbours[length] = currentCell - 1;
                connectingWall[length] = 2;
                length++;
            }
        }

        //north
        if (currentCell + xSize < totalCells) {
            if (cells[currentCell + xSize].visited == false) {
                neighbours[length] = currentCell + xSize;
                connectingWall[length] = 1;
                length++;
            }
        }

        //south
        if (currentCell - xSize >= 0) {
            if (cells[currentCell - xSize].visited == false) {
                neighbours[length] = currentCell - xSize;
                connectingWall[length] = 4;
                length++;
            }
        }

        if (length != 0) {
            int chosenNeighbour = Random.Range(0, length);
            currentNeighbour = neighbours[chosenNeighbour];
            wallToBrake = connectingWall[chosenNeighbour];
        }
        else {
            if (backingUp > 0) {
                currentCell = lastCells[backingUp];
                backingUp--;
            }
        }
    }

    void BrakeWall() {
        switch (wallToBrake) {
            case 1: Destroy(cells[currentCell].north); break;
            case 2: Destroy(cells[currentCell].east); break;
            case 3: Destroy(cells[currentCell].west); break;
            case 4: Destroy(cells[currentCell].south); break;
        }

        
    }

    void AddPlayerAndFinish() {
        int floorCount = floorHolder.transform.childCount;
        GameObject[] allFloors = new GameObject[floorCount];

        //Add floors to array
        for (int i = 0; i < floorCount; i++) {
            allFloors[i] = floorHolder.transform.GetChild(i).gameObject;
        }

        //Add finish point randomly in one floor
        int chosenFloor = Random.Range(floorCount - (floorCount / 4), floorCount);

        Vector3 finishPointPos = new Vector3 (allFloors[chosenFloor].transform.position.x, 0.25f, allFloors[chosenFloor].transform.position.z);
        Instantiate(finishObj, finishPointPos, Quaternion.identity);

        //Add player infirst floor line
        int initialPlayerFloor = Random.Range(0, xSize);
        Vector3 initialPlayerPos = new Vector3(allFloors[initialPlayerFloor].transform.position.x, 0.25f, allFloors[initialPlayerFloor].transform.position.z);
        Instantiate(playerObj, initialPlayerPos, Quaternion.identity);
    }
}
