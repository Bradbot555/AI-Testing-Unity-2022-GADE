using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NN;

public class AdvancedAI : IAIPlayer
{
    IEvaluator evaluator;
    NeuralNetwork network;
    TileManager tileManager;
    TerritoriesRepresentation territoriesRepresentation;

    Move bestMove;

    int[] hidden = new int[] { 3, 6 };
    int output = 20;
    double learnRate = 0.01;
    double momentum = 0.01;


    public AdvancedAI(IEvaluator evaluator, int input = 81)
    {
        this.evaluator = evaluator;
        network = new NeuralNetwork(input, hidden, output, learnRate, momentum);
    }

    public Move GetMove(IRepresentation representation, int player)
    {
        territoriesRepresentation = representation as TerritoriesRepresentation;
        double[] outputs = network.Compute(territoriesRepresentation.GetAs1DArray());
        return ConvertToMove(outputs);
    }

    public void Train(TerritoriesRepresentation representation, Move thisMove, int player)
    {
        network.Compute(representation.GetAs1DArray());
        double[] doubleMove = ConvertToDouble(thisMove);
        Debug.Log("double move size: " + doubleMove);
        network.BackPropagate(doubleMove);
    }

    double[] ConvertToDouble(Move move)
    {
        double[] array = new double[20];
        int x = move.Position.x;
        int y = move.Position.y;

        for (int i = 0; i < array.Length; i++)
        {
            array[i] = 0;
        }
        array[x] = 1;
        array[y + 10] = 1;

        return array;
    }

    Move ConvertToMove(double[] doubles)
    {
        int Xcount = 0;
        int Ycount = 0;
        int x = 0;
        int y = 0;
        double highest = 0;

        for (int i = 0; i < 10; i++)
        {
            Xcount++;
            if (doubles[i] > highest)
            {
                highest = doubles[i];
                x = Xcount;
            }
        }
        highest = 0;
        for (int i = 10; i < 20; i++)
        {
            Ycount++;
            if (doubles[i] > highest)
            {
                highest = doubles[i];
                y = Ycount;
            }
        }

        return new Move(new Vector2Int(x, y));
    }

}
