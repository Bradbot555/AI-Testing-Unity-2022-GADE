using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class GameManager : MonoBehaviour
{
    public TerritoriesRepresentation representation;
    TileManager tileManager;

    static int PLAYER1 = 1;
    static int PLAYER2 = -1;

    int breakPoint = 0;
    public int AIDifficultySet;
    #region UI Stuff
    [SerializeField]
    GameObject PlayerUI, InGameUI, Settings, WinScreen;
    [SerializeField]
    Text playerTurn, HeightText, WidthText;
    [SerializeField]
    Slider HeightCount, WidthCount, AISlider;
    [SerializeField]
    TextMeshProUGUI WinningTitle, Player1Score, Player2Score, Player1ScoreWin, Player2ScoreWin, AISetting;
    [SerializeField]
    Button Player1Surr, Player2Surr;

    #endregion

    int PlayerTurn;

    bool AI;
    bool NeuralAI= false;
    [SerializeField]
    bool trainingAI;

    [SerializeField]
    GameObject clicking;

    IAIPlayer AIPlayer;
    AdvancedAI NNPlayer;

    void Start()
    {
        tileManager = new TileManager();
        //Settings.SetActive(true);
        //PlayerUI.SetActive(false);
        // Player1Turn = true;
        // Player2Turn = false;
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("Game");
    }

    public void SliderTextUpdate()
    {
        HeightText.text = HeightCount.value.ToString();
        tileManager.MapHeight = (int)HeightCount.value;
        //representation.MapHeight = (int)HeightCount.value;
        WidthText.text = WidthCount.value.ToString();
        tileManager.MapWidth = (int)WidthCount.value;
        //representation.MapWidth = (int)WidthCount.value;
    }

    public void GameStart() //Is called when the game starts
    {
        representation = new TerritoriesRepresentation();
        tileManager = GameObject.Find("TileManager").GetComponent<TileManager>();

        InGameUI.SetActive(true); //Turns off the UI
        WinScreen.SetActive(false); //Makes sure the winscreen is off
        tileManager.CreateMap(); //Creates the map

        NNPlayer = new AdvancedAI(new TerritoriesEvaluator(), 9 * 9);
        AIPlayer = new MinimaxAIPlayer2(new TerritoriesEvaluator());//Sets everything for the Neural Netowrk
        representation.SetBoard(tileManager.tempBoard); //Sets the board for the AI to see

        PlayerTurn = UnityEngine.Random.Range(0f, 1f) > 0.5 ? 1 : -1; //Randomly chooses a player to start with

        //PlayerTurn = PLAYER1;
        Debug.Log("Starting player is: " + PlayerTurn); //Logs the starting player
        if (PlayerTurn == PLAYER1) //Sets the player as first
        {
            if (trainingAI)
            {
                if (AI) //if AI is enabled, then AI is first
                {
                    StartCoroutine(AITurnCoroutine());
                    clicking.GetComponent<ClickScript>().enabled = false;
                }
            }
            playerTurn.text = "Player 1 Turn!";
            playerTurn.color = Color.cyan;
            clicking.GetComponent<ClickScript>().enabled = true;
        }
        else if (PlayerTurn == PLAYER2) //Sets player 2 as first
        {
            if (AI) //if AI is enabled, then AI is first
            {
                StartCoroutine(AITurnCoroutine());
                clicking.GetComponent<ClickScript>().enabled = false;
            }
            if (NeuralAI)
            {
                StartCoroutine(NNTurnCoroutine());
                clicking.GetComponent<ClickScript>().enabled = false;
            }
            playerTurn.text = "Player 2 Turn!";
            playerTurn.color = Color.red;
        }
        Settings.SetActive(false);
        PlayerUI.SetActive(true);
    }

    public void TileClick(GameObject tile) //When a tile is 'clicked' on
    {
        if (PlayerTurn == PLAYER1)
        {
            if (tile.GetComponent<TileScript>().CheckTile()) //Is the tile avaiable
            {
                PlayerTextCheck(PLAYER1);
                tile.GetComponent<TileScript>().ChangeTile(PLAYER1, false); //Changes the tile to the player tile
                Debug.Log("Player: " + PLAYER1 + " clicked on : " + tile.name);
                PlayerTurn *= -1;
                Player1Surr.interactable = false;
                Player2Surr.interactable = true;
                switch (AI)
                {
                    case true:
                        StartCoroutine(AITurnCoroutine()); //If AI is on, starts the AI's turn
                        representation.MakeMove(tile.GetComponent<TileScript>().move, PLAYER1, false);
                        clicking.GetComponent<ClickScript>().enabled = false;
                        break;
                    default:
                        clicking.GetComponent<ClickScript>().enabled = true;
                        break;
                }
                switch (NeuralAI)
                {
                    case true:
                        StartCoroutine(NNTurnCoroutine());
                        representation.MakeMove(tile.GetComponent<TileScript>().move, PLAYER1, false);
                        clicking.GetComponent<ClickScript>().enabled = false;
                        break;
                    default:
                        clicking.GetComponent<ClickScript>().enabled = true;
                        break;
                }
            }
        }
        else if (PlayerTurn == PLAYER2)
        {
            if (tile.GetComponent<TileScript>().CheckTile())//Same verse as the first
            {
                PlayerTextCheck(PLAYER2);
                tile.GetComponent<TileScript>().ChangeTile(PLAYER2, false);

                Debug.Log("Player: " + PLAYER2 + " clicked on : " + tile.name);
                PlayerTurn *= -1;
                Player1Surr.interactable = true;
                Player2Surr.interactable = false;
                switch (AI)
                {
                    case true:
                        if (trainingAI)
                        {
                            StartCoroutine(AITurnCoroutine()); //If AI is on, starts the AI's turn
                        }
                        representation.MakeMove(tile.GetComponent<TileScript>().move, PLAYER2, false);
                        break;
                    default:
                        clicking.GetComponent<ClickScript>().enabled = true;
                        break;
                }
                switch (NeuralAI)
                {
                    case true:
                        StartCoroutine(NNTurnCoroutine());
                        representation.MakeMove(tile.GetComponent<TileScript>().move, PLAYER1, false);
                        clicking.GetComponent<ClickScript>().enabled = false;
                        break;
                    default:
                        clicking.GetComponent<ClickScript>().enabled = true;
                        break;
                }
            }
        }
        Player1Score.text = tileManager.CheckPlayer1Tiles().ToString(); //Outputs the score for the players to see.
        Player2Score.text = tileManager.CheckPlayer2Tiles().ToString();
        CheckWin(); //Checks to see if anyone has won
    }

    void PlayerTextCheck(int player) //Checks to see if the player's color text is the right color
    {
        switch (player)
        {
            case 1:
                playerTurn.text = "Player 2 Turn!";
                playerTurn.color = Color.red;
                break;
            case -1:
                playerTurn.text = "Player 1 Turn!";
                playerTurn.color = Color.cyan;
                break;
        }
    }

    public int CheckWin() // Has anyone won the game yet?
    {
        int returnValue = 0;
        if (tileManager.CheckNormalTiles() > 0) //If there are any tiles open, the game isn't over
        {
            return 0;
        }
        else if (tileManager.CheckNormalTiles() < 1) //if there are no tiles, check to see who owns the tiles
        {
            if (tileManager.CheckPlayer1Tiles() > tileManager.CheckPlayer2Tiles()) //If player 1 has more tiles than player 2, Player 1 wins!
            {
                returnValue = 1;
                Victory(returnValue);
                Debug.Log("Player 1 Won!");
            }
            else if (tileManager.CheckPlayer1Tiles() < tileManager.CheckPlayer2Tiles()) //If player 2 has more tiles than player 1, Player 2 wins!
            {
                returnValue = -1;
                Victory(returnValue);
                Debug.Log("Player 2 Won!");
            }
            else //If neither have won, but there are no tiles; it must be a draw!
            {
                returnValue = 0;
                Victory(returnValue);
                Debug.Log("Either a bug or actual draw, either way take a look at it");
            }
        }
        return returnValue;
    }

    public void AICheck() //Is this thing on?
    {
        AI = ToggleBool(AI);
        NeuralAI = ToggleBool(NeuralAI, true, false);
    }

    public void NeuralAICheck()
    {
        NeuralAI = ToggleBool(NeuralAI);
        AI = ToggleBool(AI, true, false);
    }

    public void AIDifficultySetting() //How hard do you want to get hurt by the AI
    {
        switch (AISlider.value)
        {
            case 0:
                AISetting.color = Color.green;
                AISetting.text = "VERY EASY";
                AIDifficultySet = 0;
                break;
            case 1:
                AISetting.color = Color.white;
                AISetting.text = "EASY";
                AIDifficultySet = 1;
                break;
            case 2:
                AISetting.color = Color.cyan;
                AISetting.text = "STILL EASY";
                AIDifficultySet = 2;
                break;
            case 3:
                AISetting.color = Color.red;
                AISetting.text = "DIE";
                AIDifficultySet = 3;
                break;
            default:
                AIDifficultySet = 2;
                break;
        }
    }

    IEnumerator AITurnCoroutine() //How the AI moves
    {
        yield return new WaitForSeconds(0.5f); //Wait for a second to make the player quake in fear!
        Move move = AIPlayer.GetMove(representation, PlayerTurn); //Get the best move from MiniMax 90% of the hardlifting right here
        if (move != null) //If the getMove doesn't return null, do this stuff!
        {
            GameObject tempTile = GameObject.Find(move.Position.x.ToString() + move.Position.y.ToString()); //Get the tile the AI wants to go to
            Debug.Log("AI trying to move here: " + move.Position.ToString());
            if (tempTile.GetComponent<TileScript>().CheckTile()) //See if the AI got the right tile
            {
                TileClick(tempTile); //'click' the tile
                NNPlayer.Train(representation, move, PlayerTurn);
            }
            else
            {
                if (breakPoint == 5)
                {
                    Debug.LogError("AI is unable to find an opening! Either the game has ended or the AI isn't able to see the open tile!");
                }
                else
                {
                    breakPoint++;
                    Debug.LogError("AI is attempting to pick an already taken slot! Retrying!" + move.Position.x.ToString() + move.Position.y.ToString());
                    StartCoroutine(AITurnCoroutine());
                }
            }
        }
        else //I've seen this popup too many times to count please end me, I beg you
        {
            Debug.LogError("MOVE IS NULL! MAJOR ERROR!"); // Ha ha error go brrr
        }
    }
    IEnumerator NNTurnCoroutine() //How the AI moves
    {
        yield return new WaitForSeconds(0.5f); //Wait for a second to make the player quake in fear!
        Move move = NNPlayer.GetMove(representation, PlayerTurn); //Get the best move from MiniMax 90% of the hardlifting right here
        if (move != null) //If the getMove doesn't return null, do this stuff!
        {
            GameObject tempTile = GameObject.Find(move.Position.x.ToString() + move.Position.y.ToString()); //Get the tile the AI wants to go to
            Debug.Log("Neural Network AI trying to move here: " + move.Position.ToString());
            if (tempTile.GetComponent<TileScript>().CheckTile()) //See if the AI got the right tile
            {
                TileClick(tempTile); //'click' the tile
            }
            else
            {
                if (breakPoint == 5)
                {
                    Debug.LogError("Neural Network AI is unable to find an opening! Either the game has ended or the AI isn't able to see the open tile!");
                }
                else
                {
                    breakPoint++;
                    Debug.LogError("Neural Network AI is attempting to pick an already taken slot! Retrying!" + move.Position.x.ToString() + move.Position.y.ToString());
                    StartCoroutine(NNTurnCoroutine());
                }
            }
        }
        else //I've seen this popup too many times to count please end me, I beg you
        {
            Debug.LogError("MOVE IS NULL! MAJOR ERROR!"); // Ha ha error go brrr
        }
    }
    public void Victory(int winner) //Victory Screen summoner!
    {
        switch (trainingAI)
        {
            case false:
                StopAllCoroutines();
                GameStart();
                break;
            default:
                WinScreen.SetActive(true);
                if (winner == PLAYER1)
                {
                    WinningTitle.text = "Player 1 Wins!";
                    WinningTitle.color = Color.cyan;
                }
                else if (winner == PLAYER2)
                {
                    WinningTitle.text = "Player 2 Wins!";
                    WinningTitle.color = Color.red;
                }
                else
                {
                    WinningTitle.text = "It's a draw!";
                    WinningTitle.color = Color.yellow;
                }
                Player1ScoreWin.text = tileManager.CheckPlayer1Tiles().ToString();
                Player2ScoreWin.text = tileManager.CheckPlayer2Tiles().ToString();
                break;
        }
    }

    private bool ToggleBool(bool toggle, bool ForceBool = false, bool ForcedBool = true)
    {
        if (ForceBool)
        {
            return ForcedBool;
        }
        switch (toggle)
        {
            case true:
                return false;
            case false:
                return true;
        }
    }
}
