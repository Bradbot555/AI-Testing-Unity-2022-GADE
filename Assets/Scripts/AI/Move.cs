using UnityEngine;

public class Move { 
   
    public Vector2Int Position{ get; set;} = Vector2Int.zero;
    
    
    public Move(Vector2Int position){
        Position = position;
    }

}