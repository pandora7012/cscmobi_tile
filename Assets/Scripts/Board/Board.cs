using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;


public class Board : MonoBehaviour
{
    [InfoBox("Change the board size and cell size to fit your needs")]
    public static int boardSize = 36;

    public static int cellSize = 8;

    [SerializeField] Transform cellRoot;

    private int remainTile = boardSize;


    private List<Cell> _cells;
    private Dictionary<int, List<Tile>> _tileMap = new Dictionary<int, List<Tile>>();

    [Button("Setup")]
    public void Setup()
    {
        Utils.Setup(boardSize);
        SetupTile();
        GenerateCell();
    }

    private void SetupTile()
    {
        var resource = Resources.Load(Constants.TILE_PREFAB);

        int tileLayer = 0;

        while (true)
        {
            if (Utils.CanGetRandomType == false)
                break;
            var tiles = new List<Tile>();

            for (float j = -tileLayer / 2f; j <= tileLayer / 2f; j++)
            {
                if (Utils.CanGetRandomType == false)
                    break;
                for (float k = -tileLayer / 2f; k <= tileLayer / 2f; k++)
                {
                    if (Utils.CanGetRandomType == false)
                        break;
                    var tile = Instantiate(resource, transform).GetComponent<Tile>();
                    tiles.Add(tile);
                    tile.SetPosition(new PositionData(j, k, tileLayer));
                    tile.SetType(Utils.GetRandomType());
                    tile.SetCoveringTiles(GetCoveringTiles(tile));
                    tile.UpdateView();
                }
            }

            _tileMap.Add(tileLayer, tiles);
            tileLayer++;
        }
    }

    private void GenerateCell()
    {
        var prefab = Resources.Load(Constants.CELL_PREFAB) as GameObject;
        _cells = new List<Cell>();
        for (int i = 0; i < cellSize; i++)
        {
            Cell cell = new Cell(prefab, cellRoot, new Vector3(i * 1.2f - cellSize / 2f * 1.2f, 0, 0));
            _cells.Add(cell);
        }
    }


    private List<Tile> GetCoveringTiles(Tile tile)
    {
        var result = new List<Tile>();
        var position = tile.position;
        var layer = (int)position.z;


        if (_tileMap.ContainsKey(layer - 1))
        {
            foreach (var t in _tileMap[layer - 1])
            {
                if (Mathf.Abs(t.position.x - position.x) <= 1 && Mathf.Abs(t.position.y - position.y) <= 1)
                {
                    result.Add(t);
                    t.AddCoveredTile(tile);
                }

                if (result.Count == 4)
                    return result;
            }
        }

        return result;
    }

    public int PushTileToCell(Tile tile)
    {
        if (tile.CanInteract is false)
            return -1;

        for (int i = 0; i < _cells.Count; i++)
        {
            if (_cells[i].IsFree)
            {
                _cells[i].Assign(tile);
                tile.MoveTo(_cells[i] , CheckLose);
                return -1;
            }
            else if (_cells[i].IsTheSameTile(tile))
            {
                for (int j = _cells.Count - 1; j >= i + 1; j--)
                {
                    if (_cells[j - 1].IsTheSameTile(tile))
                    {
                        _cells[j].Assign(tile);
                        tile.MoveTo(_cells[j], () => CheckDestroy(j));
                        return j;
                    }

                    var tl = _cells[j - 1].Free();
                    if (tl is null)
                        continue;
                    _cells[j].Assign(tl);
                    tl.MoveTo(_cells[j]);
                }
            }
        }

        return -1;
    }

    private void CheckDestroy(int j)
    {
        if (j > 1 && _cells[j].IsTheSameTile(_cells[j - 1].Tile) && _cells[j].IsTheSameTile(_cells[j - 2].Tile))
        {
            _cells[j].DeleteTile();
            _cells[j - 1].DeleteTile();
            _cells[j - 2].DeleteTile();
            FillUpCell();
            remainTile -= 3;
        }

        CheckWin();
        CheckLose();
        
    }

    private void FillUpCell()
    {
        for (int i = 0; i < _cells.Count - 3; i++)
        {
            if (_cells[i].IsFree)
            {
                var tile = _cells[i + 3].Free();
                if (tile is null)
                    continue;
                _cells[i].Assign(tile);
                tile.MoveTo(_cells[i]);
            }
        }
    }

    private void CheckWin()
    {
        if (remainTile == 0)
        {
            GameManager.Instance.WinGame();
            
        }
    }

    private void CheckLose()
    {
        if (_cells[^1].IsFree is false)
        {
            GameManager.Instance.LoseGame();
        }
    }
}

public class PositionData
{
    public float x;
    public float y;
    public float z;

    public PositionData(float x, float y, float z)
    {
        this.x = x;
        this.y = y;
        this.z = z;
    }
}