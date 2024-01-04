using System.Collections.Generic;
using UnityEngine;

public class TerritoriesRepresentation : IRepresentation
{
    //int mapWidth;
    //int mapHeight;
    //public int MapWidth { get => mapWidth; set => mapWidth = value; }
    //public int MapHeight { get => mapHeight; set => mapHeight = value; }
    //You were thinking about making the 2d array board your actual tile list, so that the game rules carry across.
    public int[,] board;
    List<Move> surroundingTiles = new List<Move>();
    public TerritoriesRepresentation()
    {
        //SetBoard();
    }

    List<Move> Find2DSurrondingTiles(Move move) //Gets the surrounding moves for this move.
    {
        List<Move> moves = new List<Move>();
        Move aboveIM = new Move(new Vector2Int(move.Position.x, move.Position.y + 1));
        if (aboveIM.Position.x < 0 || aboveIM.Position.x >= board.GetLength(0)) //not valid x coord
        {
            //Debug.LogError("Looking outside map!");
        }
        else if (aboveIM.Position.y < 0 || aboveIM.Position.y >= board.GetLength(1)) //not valid y coord
        {
            //Debug.LogError("Looking outside map!");
        }
        else
        {
            moves.Add(aboveIM);
        }


        Move belowIM = new Move(new Vector2Int(move.Position.x, move.Position.y - 1));
        if (belowIM.Position.x < 0 || belowIM.Position.x >= board.GetLength(0))
        {
            //Debug.LogError("Looking outside map!");
        }
        else if (belowIM.Position.y < 0 || belowIM.Position.y >= board.GetLength(1))
        {
            //Debug.LogError("Looking outside map!");
        }
        else
        {
            moves.Add(belowIM);
        }

        Move rightIM = new Move(new Vector2Int(move.Position.x + 1, move.Position.y));
        if (rightIM.Position.x < 0 || rightIM.Position.x >= board.GetLength(0))
        {
            //Debug.LogError("Looking outside map!");
        }
        else if (rightIM.Position.y < 0 || rightIM.Position.y >= board.GetLength(1))
        {
            //Debug.LogError("Looking outside map!");
        }
        else
        {
            moves.Add(rightIM);
        }

        Move leftIM = new Move(new Vector2Int(move.Position.x - 1, move.Position.y));
        if (leftIM.Position.x < 0 || leftIM.Position.x >= board.GetLength(0))
        {
            //Debug.LogError("Looking outside map!");
        }
        else if (leftIM.Position.y < 0 || leftIM.Position.y >= board.GetLength(1))
        {
            //Debug.LogError("Looking outside map!");
        }
        else
        {
            moves.Add(leftIM);
        }
        return moves;
    }


    void CheckSurroundingBoardTiles(Move move, List<Move> importMoves, int player) //Checks the tiles around this tile, but for the rep board.
    { //Look in my TileScript.cs for a cleaner, easier to understand version. Also uses Unity Libaries rather than bad coding below.
        int tempCount = 0;
        if (board[move.Position.x, move.Position.y] == 0)
        {
            int tempPlayer1Count = 0; //Start counting from 0 for Player 1
            int tempPlayer2Count = 0; //Start counting from 0 for Player 2
            foreach (Move moves in importMoves)
            {
                if (board[moves.Position.x, moves.Position.y] == 1) //if a neighbouring tile is Player 1 tiles increment tempPlayer1 
                {
                    tempPlayer1Count++;
                }
                else if (board[moves.Position.x, moves.Position.y] == -1) //if a neighbouring tile is Player 2 tiles increment tempPlayer2
                {
                    tempPlayer2Count++;
                }
            }
            if (tempPlayer1Count >= 3) //Once loop is finished, checks to see how many tiles surrounding itself is the opponents from count
            {
                MakeMove(move, 1, true); //If the count is greater than or equal to 2 then turns into opponent tile using ChangeTile as recheck
            }
            else if (tempPlayer2Count >= 3)
            {
                MakeMove(move, -1, true);
            }
        }
        else if (board[move.Position.x, move.Position.y] == 1)
        {
            foreach (Move moves in importMoves)
            {
                if (board[moves.Position.x, moves.Position.y] != 1 &&
                    board[moves.Position.x, moves.Position.y] != 0 &&
                    board[moves.Position.x, moves.Position.y] != 2)
                {
                    tempCount++;
                }
            }
            if (tempCount >= 2)
            {
                MakeMove(move, -1, true);
            }
        }
        else if (board[move.Position.x, move.Position.y] == -1)
        {
            foreach (Move moves in importMoves)
            {
                if (board[moves.Position.x, moves.Position.y] != -1 &&
                    board[moves.Position.x, moves.Position.y] != 0 &&
                    board[moves.Position.x, moves.Position.y] != 2)
                {
                    tempCount++;
                }
            }
            if (tempCount >= 2)
            {
                MakeMove(move, 1, true);
            }
        }
    }
    public void SetBoard(/*int width, int height*/ int[,] repBoard) //Simple setting the board to the same as the real one, this is pretty standard.
    {
        //width = mapWidth;
        //height = mapHeight;
        //board = new int[width, height];
        //for (int y = 0; y < board.GetLength(1); y++)
        //{
        //    for (int x = 0; x < board.GetLength(0); x++)
        //    {
        //        board[x, y] = 0;
        //    }
        //}
        board = repBoard;
    }

    public IRepresentation Duplicate() //Dupe a loop (Duping the board is ez pz thanks to henk's code!)
    {
        return new TerritoriesRepresentation(board.Clone() as int[,]);
    }

    public TerritoriesRepresentation(int[,] board) //Loop a dupe (this is a ryhme, not actually duping a loop)
    {
        this.board = board;
    }

    public List<Move> GetPossibleMoves(int player) //Gets all moves possible for the AI to choose from.
    {
        List<Move> moves = new List<Move>();

        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                if (board[x, y] == 0)
                {
                    moves.Add(new Move(new Vector2Int(x, y)));
                }
            }
        }

        return moves;
    }

    bool IsBoardFull() //Checks to see if the board is full
    {
        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                if (board[x, y] == 0)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public int PlayerTileCounter(int player) //Simple player tile counter, nothing else really.
    {
        int count = 0;
        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                if (board[x, y] == player)
                {
                    count++;
                }
            }
        }
        return count;
    }

    public int TotalScore()
    {
        int total = 0;
        int player1Tiles = 0;
        int player2Tiles = 0;
        player1Tiles = PlayerTileCounter(1);
        player2Tiles = PlayerTileCounter(-1);

        total = player2Tiles - player1Tiles;
        //Debug.LogError(total);
        return total;
    }

    int CheckWinner() //Who's winning in the current state of the board.
    {
        int player1Tiles = 0;
        int player2Tiles = 0;
        player1Tiles = PlayerTileCounter(1);
        player2Tiles = PlayerTileCounter(-1);
        if (player1Tiles > player2Tiles)
        {
            return 1;
        }
        else if (player1Tiles < player2Tiles)
        {
            return -1;
        }
        else
        {
            return 0;
        }
    }

    public bool MakeMove(Move move, int player, bool reCheck) //Pain, Agony even. Suffering if you will
    {
        board[move.Position.x, move.Position.y] = player; //Makes changes to representation board.
        if (!reCheck) //Checks to see if it's a recheck call
        {
            surroundingTiles = Find2DSurrondingTiles(move); //Finds the surrounding tiles for the move being called
            foreach (Move moves in surroundingTiles) //Loops through the new tiles.
            {
                List<Move> tempSurrTiles = new List<Move>(); //new list for tempTiles for the tiles around the tiles around our tile.
                tempSurrTiles = Find2DSurrondingTiles(moves); //Finding the tiles around the tiles next to our tile.
                CheckSurroundingBoardTiles(moves, tempSurrTiles, player); //Checks to see if they get captured from this move made.
                //Debug.Log(tempSurrTiles.Count);
            }
        }
        Debug.LogWarning(ToString());
        return true;
    }


    public GameState GetGameOutcome() //Loads of changes done here.
    {
        int winner = CheckWinner(); //Who's winning?
        bool isBoardFull = IsBoardFull(); //Is the board full?

        switch (winner) //Switch case rather than if elses, cuz it looks nice
        {
            case 0 when isBoardFull: //each of these have the condition for the board to be full, otherwise my AI stops playing after 3 turns.
                return GameState.DRAW;
            case 1 when isBoardFull:
                return GameState.PLAYER1;
            case -1 when isBoardFull:
                return GameState.PLAYER2;
        }

        return GameState.UNDETERMINED;
    }

    public double[] GetAs1DArray()
    {
        double[] board1D = new double[board.GetLength(0) * board.GetLength(1)];
        int index = 0;

        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                board1D[index] = board[x, y];
                index++;
            }
        }

        return board1D;
    }

    public override string ToString()
    {
        string s = "";
        for (int y = 0; y < board.GetLength(1); y++)
        {
            for (int x = 0; x < board.GetLength(0); x++)
            {
                Debug.Log("Y:"+board.GetLength(1) + " X:" + board.GetLength(0) + " | Attempted look: " + "y:" + y + " x:" + x);
                
                switch (board[x, y])
                {
                    case 0:
                        s += "[0]";
                        break;
                    case 1:
                        s += "[X]";
                        break;
                    case -1:
                        s += "[A]";
                        break;
                    case 2:
                        s += "[#]";
                        break;
                }
            }
            s += "\n";
        }

        return s;
    }

}