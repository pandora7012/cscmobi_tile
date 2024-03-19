using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell 
{
    public Tile Tile { get; set; }

    public Transform view { get; private set; }
    
    public bool IsFree => Tile == null;
    
    public Cell(GameObject prefab, Transform parent, Vector3 position)
    {
        view = GameObject.Instantiate(prefab, parent).transform;
        view.localPosition = position; 

    }
    
    public void Assign(Tile tile)
    {
        if (tile is null)
            return; 
        Tile = tile; 
        tile.SetCell(this);
        
    }
    
    public Tile Free()
    {
        var tile = Tile; 
        Tile = null;
        return tile;
    }
    
    public bool IsTheSameTile(Tile tile)
    {
        if (Tile is null)
            return false; 
        return Tile.IsTheSameType(tile);
    }
    
    public void ApplyTileToPosition(){
        if(Tile != null){
            Tile.MoveTo(this);
        }
    }

    public void DeleteTile()
    {
        if (Tile is null)
            return; 
        Tile.Delete();
        Tile = null; 
    }

}
