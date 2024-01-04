using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    [SerializeField]
    //private LayerMask layerMask = default;
    const int PLAYER1 = 1;
    const int PLAYER2 = -1;
    TileManager tileManager;
    TerritoriesRepresentation representation;

    public Vector2Int myPos;
    public Move move;
    public enum TileType { NORMAL, PLAYER1, PLAYER2, OBSTACLE }
    TileType type;

    public Material baseMaterial;
    public Material Player1Material;
    public Material Player2Material;
    public Material obstacleMaterial;
    [SerializeField]
    private GameObject obstacleDefault;
    [SerializeField]
    private GameObject aboveMe;
    [SerializeField]
    private GameObject belowMe;
    [SerializeField]
    private GameObject rightMe;
    [SerializeField]
    private GameObject leftMe;

    public List<GameObject> surroundingTiles = new List<GameObject>(4);


    void Start()
    {
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();
        //TileCreation();
    }

    public void SetInfo(int x, int y)
    {
        this.myPos = new Vector2Int(x,y);
        this.move = new Move(myPos); //Turning my position into a Move for the AI to see.
    }

    public void TileCreation()
    {
        if (this.tag == "ObstacleTile") //Checking to see if this tile is starting as an obstacle
        {
            type = TileType.OBSTACLE;
            this.gameObject.GetComponent<MeshRenderer>().material = obstacleMaterial;
            return;
        }
        else //otherwise it's a normal tile
        {
            AddSurrounding();
            this.gameObject.GetComponent<MeshRenderer>().material = baseMaterial;
            type = TileType.NORMAL;
        }
    }

    private void AddSurrounding()
    {
        obstacleDefault = GameObject.Find("Obstacle"); //Default stopper tile, sets the boundries of the map
        #region OldSurrounding
        ////Casts a ray to hit and return the gameobject. Used to gather info on surrounding tiles.
        //RaycastHit hit;
        //Vector3 up = transform.TransformDirection(Vector3.up);
        //if (Physics.Raycast(transform.position, up, out hit))
        //{
        //    Debug.DrawRay(transform.position, up, Color.green, Mathf.Infinity);//Shows a line showing the ray was able to find another tile
        //    aboveMe = hit.collider.gameObject;
        //    Debug.Log("I hit something! " + hit.collider.gameObject.name);
        //}
        //else //if the ray hits nothing, rather than returning null it returns a obstacle prefab
        //{
        //    aboveMe = obstacleDefault;
        //    Debug.DrawRay(transform.position, up, Color.red, Mathf.Infinity);
        //}

        //Vector3 down = transform.TransformDirection(Vector3.down);
        //if (Physics.Raycast(transform.position, down, out hit))
        //{
        //    Debug.DrawRay(transform.position, down, Color.green, Mathf.Infinity);
        //    belowMe = hit.collider.gameObject;
        //    Debug.Log("I hit something! " + hit.collider.gameObject.name);
        //}
        //else
        //{
        //    belowMe = obstacleDefault;
        //    Debug.DrawRay(transform.position, down, Color.red, Mathf.Infinity);
        //}

        //Vector3 right = transform.TransformDirection(Vector3.right);
        //if (Physics.Raycast(transform.position, right, out hit))
        //{
        //    Debug.DrawRay(transform.position, right, Color.green, Mathf.Infinity);
        //    rightMe = hit.collider.gameObject;
        //    Debug.Log("I hit something! " + hit.collider.gameObject.name);
        //}
        //else
        //{
        //    rightMe = obstacleDefault;
        //    Debug.DrawRay(transform.position, right, Color.red, Mathf.Infinity);
        //}

        //Vector3 left = transform.TransformDirection(Vector3.left);
        //if (Physics.Raycast(transform.position, left, out hit))
        //{
        //    Debug.DrawRay(transform.position, left, Color.green, Mathf.Infinity);
        //    leftMe = hit.collider.gameObject;
        //    Debug.Log("I hit something! " + hit.collider.gameObject.name);
        //}
        //else
        //{
        //    leftMe = obstacleDefault;
        //    Debug.DrawRay(transform.position, left, Color.red, Mathf.Infinity);
        //}

        //surroundingTiles.Add(aboveMe); //Once all the surrounding tiles are collected, adds them to a list
        //surroundingTiles.Add(belowMe);
        //surroundingTiles.Add(rightMe);
        //surroundingTiles.Add(leftMe);
        #endregion
        this.aboveMe = GameObject.Find((myPos.x + "") + (myPos.y + 1 + "")); //Looks for the surrounding tiles using myPos and then math to figure out the rest
        this.belowMe = GameObject.Find((myPos.x + "") + (myPos.y - 1 + ""));
        this.rightMe = GameObject.Find((myPos.x + 1 +"") + (myPos.y + ""));
        this.leftMe = GameObject.Find((myPos.x - 1 + "") + (myPos.y + ""));
        if (this.aboveMe == null) //if it can't find the tile, it must not exist, therefore either bug or out of map
        {
            this.aboveMe = obstacleDefault;
        }
        if (this.belowMe == null)
        {
            this.belowMe = obstacleDefault;
        }
        if (this.rightMe == null)
        {
            this.rightMe = obstacleDefault;
        }
        if (this.leftMe == null)
        {
            this.leftMe = obstacleDefault;
        }
        this.surroundingTiles.Add(aboveMe); //Once all the surrounding tiles are collected, adds them to a list
        this.surroundingTiles.Add(belowMe);
        this.surroundingTiles.Add(rightMe);
        this.surroundingTiles.Add(leftMe);
    }

    public bool CheckTile() //Checks the tile being clicked on to see if it is avaiable to be taken.
    {
        switch (type)
        {
            case TileType.OBSTACLE:
                Debug.LogWarning("This is an obstacle tile! No one can place here! |" + name);
                return false;
            case TileType.NORMAL:
                Debug.Log("Picking tile: " + name);
                return true;
            case TileType.PLAYER1:
                Debug.LogWarning("A Player has already chosen this spot! Returning False flag. |" + name);
                return false;
            case TileType.PLAYER2:
                Debug.LogWarning("A Player has already chosen this spot! Returning False flag. |" + name);
                return false;
            default:
                Debug.LogError("No case type was found for tile! Please investigate! |" + name);
                return false;
        }
    }

    void LookAroundMe(TileType origin, TileType target, int returnPlayer) //Loops through the surrounding tiles to make them check their surrounding tiles
    {
        for (int x = 0; x < surroundingTiles.Count; x++)
        {
            if (surroundingTiles[x].GetComponent<TileScript>().type == TileType.NORMAL)//Checks to see if the tile is an uncaptured tile
            {
                surroundingTiles[x].GetComponent<TileScript>().NormalTileCapture();
            }
            else if (surroundingTiles[x].GetComponent<TileScript>().type != TileType.OBSTACLE)//Checks to see if the tile is not an obstacle before continue
            {
                //surroundingTiles[x].GetComponent<TileScript>().EnemyCheck(target, returnPlayer);
                surroundingTiles[x].GetComponent<TileScript>().CaptureAround();
            }
        }
        
    }

    private void NormalTileCapture()
    {
        int tempPlayer1Count = 0; //Start counting from 0 for Player 1
        int tempPlayer2Count = 0; //Start counting from 0 for Player 2
        for (int i = 0; i < surroundingTiles.Count; i++) //Loop through the surrounding tiles
        {
            if (surroundingTiles[i].GetComponent<TileScript>().type == TileType.PLAYER1) //if a neighbouring tile is Player 1 tiles increment tempPlayer1 
            {
                tempPlayer1Count++;
            }
            else if (surroundingTiles[i].GetComponent<TileScript>().type == TileType.PLAYER2) //if a neighbouring tile is Player 2 tiles increment tempPlayer2
            {
                tempPlayer2Count++;
            }
        }
        if (tempPlayer1Count >= 3) //Once loop is finished, checks to see how many tiles surrounding itself is the opponents from count
        {
            ChangeTile(PLAYER1, true); //If the count is greater than or equal to 2 then turns into opponent tile using ChangeTile as recheck
        }
        else if (tempPlayer2Count >= 3)
        {
            ChangeTile(PLAYER2, true);
        }
    }

    private void CaptureAround()
    {
        int tempCount = 0;
        for (int x = 0; x < surroundingTiles.Count; x++)
        {
            if (surroundingTiles[x].GetComponent<TileScript>().type != this.type && 
                surroundingTiles[x].GetComponent<TileScript>().type != TileType.NORMAL && 
                surroundingTiles[x].GetComponent<TileScript>().type != TileType.OBSTACLE)
            {
                tempCount++;
            }
        }
        if (tempCount >= 2 && type == TileType.PLAYER1)
        {
            ChangeTile(PLAYER2, true);
        }
        else if (tempCount >= 2 && type == TileType.PLAYER2)
        {
            ChangeTile(PLAYER1, true);
        }
    }

    //private void EnemyCheck(TileType target, int returnPlayer) //Looks around the tile to see if it's surronded by the opposite player
    //{
    //    int tempCount = 0; //Start counting from 0
    //    for (int i = 0; i < surroundingTiles.Count; i++) //Loop through the surrounding tiles
    //    {
    //        if (surroundingTiles[i].GetComponent<TileScript>().type == target) //if a neighbouring tile is of an opposing player adds to count
    //        {
    //            tempCount++;
    //        }
    //    }
    //    if (tempCount >= 2) //Once loop is finished, checks to see how many tiles surrounding itself is the opponents from count
    //    {
    //        ChangeTile(returnPlayer, true); //If the count is greater than or equal to 2 then turns into opponent tile using ChangeTile as recheck
    //    }
    //}

    public void ChangeTile(int player, bool recheck) //Changes the tile to the current player, with color change and tag with Enum (even tho the enum is somewhat useless rn)
    {
        representation = GameObject.Find("GameManager").GetComponent<GameManager>().representation;
        if (player == PLAYER1)
        {
            type = TileType.PLAYER1; //Set enum to Player1
            tag = "Player1Tile"; //Sets tag to Player1
            this.gameObject.GetComponent<MeshRenderer>().material = Player1Material; //Sets matt to player 1 mat
            representation.MakeMove(move, PLAYER1,true); //Updates the tile change onto the 2D array for the AI to see
            Debug.Log(move.Position+"|"+PLAYER1);
            if (!recheck) //Checks to see if this method is being called with the recheck value, anytime this is false it won't check surrounding tiles.
            {
                LookAroundMe(type, TileType.PLAYER1, PLAYER2);
            }
        }
        else if (player == PLAYER2)
        {
            type = TileType.PLAYER2;
            tag = "Player2Tile";
            this.gameObject.GetComponent<MeshRenderer>().material = Player2Material;//Sets matt to player 2 mat
            representation.MakeMove(move, PLAYER2,true);//Updates the tile change onto the 2D array for the AI to see
            Debug.Log(move.Position + "|" + PLAYER2);
            if (!recheck)
            {
                LookAroundMe(type, TileType.PLAYER2, PLAYER1);
            }
        }
    }

    public void ClearAroundMe()
    {
        surroundingTiles.Clear();
    }

}
