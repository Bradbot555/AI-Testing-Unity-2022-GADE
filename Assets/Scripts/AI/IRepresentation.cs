using System.Collections.Generic;

public interface IRepresentation 
{

    IRepresentation Duplicate();

    List<Move> GetPossibleMoves(int player);
    bool MakeMove(Move move, int player, bool reCheck);
    public int PlayerTileCounter(int player);
    public int TotalScore();

    GameState GetGameOutcome();
}

public enum GameState{
    PLAYER1,
    PLAYER2,
    DRAW,
    UNDETERMINED
}
