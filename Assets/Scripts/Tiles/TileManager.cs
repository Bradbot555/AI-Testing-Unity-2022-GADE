using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManager : MonoBehaviour
{
    public GameObject tileObstacle;
    public GameObject tilePrefab;
    public GameObject mainCamera;

    [SerializeField]
    private int mapWidth;
    [SerializeField]
    private int mapHeight;
    [SerializeField] float tileOffset = 0.75f;
    Vector3 firstTilePlace;
    Vector3 lastTilePlace;

    [SerializeField]
    bool UsesRandomSeed;
    public String seed;

    bool ObstsacleSet = true;
    bool NeuralNetwork;

    public int[,] tempBoard;

    public List<GameObject> tileObjects = new List<GameObject>();

    public int MapWidth { get => mapWidth; set => mapWidth = value; }
    public int MapHeight { get => mapHeight; set => mapHeight = value; }

    void Start()
    {
        //CreateMap();
    }

    public void CreateMap() //Generates a tile map using Cubes, pretty simple once I did it. All credit goes to: https://www.youtube.com/watch?v=3NQnpaSNsKY
    {
        ClearMap();
        if (UsesRandomSeed) //Generates a seed if user has selected for a random seed.
        {
            seed = Time.time.ToString();
        }
        System.Random rando = new System.Random(seed.GetHashCode()); //Seed is then made into hashcode
        if (NeuralNetwork) //Neural network only works with max map;
        {
            mapHeight = 9;
            mapWidth = 9;
        }
        tempBoard = new int[mapWidth, mapHeight];
        for (int x = 0; x < mapWidth; x++)
        {
            for (int y = 0; y < mapHeight; y++)
            {
                if (rando.Next(0, 100) > 75 && ObstsacleSet)
                {
                    tempBoard[x, y] = 2;
                    GameObject temp1 = Instantiate(tileObstacle); //Takes the prefab obstacle and makes a new one
                    temp1.transform.position = new Vector3(x * tileOffset, y * tileOffset, 0); //Sets their location with offset so they don't overlap (you can make them overlap with a low enough offset)
                    temp1.transform.Rotate(new Vector3(0, 0, 90));
                    SetTileInfo(temp1, x, y); //Names the newly generated tile!
                }
                else
                {
                    tempBoard[x, y] = 0;
                    GameObject temp = Instantiate(tilePrefab); //Takes the prefab cube and makes new ones
                    temp.transform.position = new Vector3(x * tileOffset, y * tileOffset, 0); //Sets their location with offset so they don't overlap (you can make them overlap with a low enough offset)
                    SetTileInfo(temp, x, y); //Names the newly generated tile!
                }
            }
        }

        TileSurroundings();

        SetCamera();
    }


    public void TileSurroundings()
    {
        foreach (GameObject @object in tileObjects)
        {
            @object.GetComponent<TileScript>().TileCreation();
        }
    }

    private void ClearMap()
    {
        foreach (Transform child in transform)
        {
            child.GetComponent<TileScript>().ClearAroundMe();
            
            Destroy(child.gameObject);
        }

        tileObjects.Clear();
    }

    void SetTileInfo(GameObject @object, int x, int y) //Once the map is generated it names the tiles based on their location and generated order.
    {
        @object.transform.parent = this.transform;
        @object.name = "" + x + y;
        @object.GetComponent<TileScript>().SetInfo(x, y);
        Debug.Log(@object.name + "Created!");
        tileObjects.Add(@object); //Then adds to a GameObject List
    }

    void SetCamera() //Hardest I've ever thought in coding, I had to some math and even then it was useless because untiy has Lerp
    {// This simply zooms the camera in and out depending on the map size.
        firstTilePlace = tileObjects[0].transform.position;
        lastTilePlace = tileObjects[tileObjects.Count - 1].transform.position;
        mainCamera.transform.position = Vector3.Lerp(firstTilePlace, lastTilePlace, 0.5f) + new Vector3(0, 0, -20);
    }

    public int CheckNormalTiles() //Counts how many normal/base tiles are left in the game.
    {
        int tempCount = 0;
        for (int x = 0; x < tileObjects.Count; x++)
        {
            if (tileObjects[x].tag == "BaseTile")
            {
                tempCount++;
            }
        }
        return tempCount;
    }
    public int CheckPlayer1Tiles() //Counts how many tiles belong to Player 1
    {
        int tempCount = 0;
        for (int x = 0; x < tileObjects.Count; x++)
        {
            if (tileObjects[x].tag == "Player1Tile")
            {
                tempCount++;
            }
        }
        return tempCount;
    }//Just make this into one method! Comeon
    public int CheckPlayer2Tiles() //Counts how many tiles belong to Player 2
    {
        int tempCount = 0;
        for (int x = 0; x < tileObjects.Count; x++)
        {
            if (tileObjects[x].tag == "Player2Tile")
            {
                tempCount++;
            }
        }
        return tempCount;
    }

    public void SetObstacles()
    {
        if (ObstsacleSet)
        {
            ObstsacleSet = false;
        }
        else
        {
            ObstsacleSet = true;
        }
    }
    public void SetNeuralBool()
    {
        if (NeuralNetwork)
        {
            NeuralNetwork = false;
        }
        else
        {
            NeuralNetwork = true;
        }
    }
}
