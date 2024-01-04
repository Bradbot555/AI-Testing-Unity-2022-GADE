using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimaxAIPlayer2 : IAIPlayer
{
    IEvaluator evaluator; //Utility of the current state of the game (evaluates the representation)
    int maxDepth;
    Move bestMove;
    int diff = 0;

    public MinimaxAIPlayer2(IEvaluator evaluator, int maxDepth = 2) //MaxDepth is optional param
    {
        this.evaluator = evaluator;
        this.maxDepth = maxDepth;
    }

    public Move GetMove(IRepresentation representation, int player) //Returns the best move the AI can make due to Minimax
    {
        bool playerTurn = true;
        if (diff == 0)
        {
            maxDepth = 1;
        }
        if (diff == 3)
        {
            maxDepth = 2;
        }
        switch (player)
        {
            case 1:
                playerTurn = false;
                break;
            case -1:
                playerTurn = false;
                break;
            default:
                playerTurn = false;
                break;
        }
        Minimax(representation, maxDepth, int.MinValue, int.MaxValue, playerTurn);
        return bestMove;
    }

    private int Minimax(IRepresentation representation, int depth, int alpha, int beta, bool maximizingPlayer)
    {
        if (depth == 0 || representation.GetGameOutcome() != GameState.UNDETERMINED) //Loops through until game end, or depth 0
        {
            return evaluator.GetEvaluation(representation);
        }

        List<Move> possibleMoves = representation.GetPossibleMoves(maximizingPlayer ? 1 : -1); //Makes a list of possible moves for the AI to make
        List<IRepresentation> tempBoards = new List<IRepresentation>(); //Creates a list of tempBoards that copy the representation

        List<int> tempBoardScore = new List<int>(possibleMoves.Count);

        int bestEval = maximizingPlayer ? int.MaxValue : int.MinValue;

        diff = GameObject.Find("GameManager").GetComponent<GameManager>().AIDifficultySet;

        switch (diff) //The different difficulties for the AI
        {
            case 0:
                {
                    int randomChance = Random.Range(0, 2); //50/50 chance, do ya feel lucky? Punk.
                    if (randomChance == 1)
                    {
                        int decide = Random.Range(0, possibleMoves.Count); //Randomly choose a place
                        IRepresentation dupeBoard = representation.Duplicate();
                        dupeBoard.MakeMove(possibleMoves[decide], maximizingPlayer ? 1 : -1, false);
                        tempBoards.Add(dupeBoard);
                        tempBoardScore.Add(dupeBoard.TotalScore());
                    }
                    else
                    {
                        foreach (Move possibleMove in possibleMoves) //Is smart now, no random.
                        {
                            IRepresentation dupeBoard = representation.Duplicate();
                            dupeBoard.MakeMove(possibleMove, maximizingPlayer ? 1 : -1, false);
                            tempBoards.Add(dupeBoard);
                            tempBoardScore.Add(dupeBoard.TotalScore());
                        }
                    }

                    break;
                }

            case 1:
                {
                    int randomChance = Random.Range(0, 2); //50/50 chance, do ya feel lucky? Punk.
                    if (randomChance == 1)
                    {
                        int decide = Random.Range(0, possibleMoves.Count);
                        IRepresentation dupeBoard = representation.Duplicate();
                        dupeBoard.MakeMove(possibleMoves[decide], maximizingPlayer ? 1 : -1, false);
                        tempBoards.Add(dupeBoard);
                        tempBoardScore.Add(dupeBoard.TotalScore());
                    }
                    else
                    {
                        foreach (Move possibleMove in possibleMoves)
                        {
                            IRepresentation dupeBoard = representation.Duplicate();
                            dupeBoard.MakeMove(possibleMove, maximizingPlayer ? 1 : -1, false);
                            tempBoards.Add(dupeBoard);
                            tempBoardScore.Add(dupeBoard.TotalScore());
                        }
                    }

                    break;
                }

            case 2:
                {
                    foreach (Move possibleMove in possibleMoves)
                    {
                        IRepresentation dupeBoard = representation.Duplicate();
                        dupeBoard.MakeMove(possibleMove, maximizingPlayer ? 1 : -1, false);
                        tempBoards.Add(dupeBoard);
                        tempBoardScore.Add(dupeBoard.TotalScore());
                    }

                    break;
                }

            case 3:
                {
                    foreach (Move possibleMove in possibleMoves)
                    {
                        IRepresentation dupeBoard = representation.Duplicate();
                        dupeBoard.MakeMove(possibleMove, maximizingPlayer ? 1 : -1, false);
                        tempBoards.Add(dupeBoard);
                        tempBoardScore.Add(dupeBoard.TotalScore());
                    }

                    break;
                }
        }

        int index = 0;
        foreach (IRepresentation tempBoard in tempBoards) //Goes through all the tempBoards created to look at both Player and AI moves.
        {   //Repeats the minimax until depth is reached
            int eval = tempBoardScore[index];
            Minimax(tempBoard, depth - 1, alpha, beta, maximizingPlayer);
            if (maximizingPlayer) //If it is the AI's turn
            {
                if (eval > bestEval)
                {
                    bestEval = eval;
                    if (depth == maxDepth /*|| possibleMoves.Count == 0*/)
                    {
                        bestMove = possibleMoves[index]; //Sets this as the best possible move for it to make currently.
                    }
                }

                alpha = Mathf.Max(alpha, bestEval); 

                if (alpha >= beta)
                {
                    break;
                }
            }
            else //Player turn
            {

                if (eval > bestEval)
                {
                    bestEval = eval;
                    if (depth == maxDepth /*|| possibleMoves.Count == 0*/)
                    {
                        bestMove = possibleMoves[index];
                    }
                }


                beta = Mathf.Min(beta, bestEval); 

                if (beta <= alpha)
                {
                    break;
                }
            }
            index++;
        }
        return bestEval;
    }

    //This is all possible thanks to Henk who helped me with Minimax and my task 1 in class and made this all possible.
    //90% of this minimax code was done in class and corrected by Henk so this is technically Henk's code
}
