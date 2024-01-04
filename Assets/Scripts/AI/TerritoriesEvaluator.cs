using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerritoriesEvaluator : IEvaluator
{
    public int GetEvaluation(IRepresentation representation) //Checks to see who is the winner
    {
        GameState outcome = representation.GetGameOutcome();

        if (outcome == GameState.PLAYER1)
        {
            return 1;
        }
        if (outcome == GameState.PLAYER2)
        {
            return -1;
        }
        return 0;
    }

    //Due to game breaking bug, this method was moved to representation
    //public int TotalScore()
    //{
    //    int total = 0;
    //    int player1Tiles = 0;
    //    int player2Tiles = 0;
    //    player1Tiles = PlayerTileCounter(1);
    //    player2Tiles = PlayerTileCounter(-1);

    //    total = player2Tiles - player1Tiles;
    //    //Debug.LogError(total);
    //    return total;
    //}
}
