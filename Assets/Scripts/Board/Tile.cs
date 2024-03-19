using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using DG.Tweening;
using Sirenix.OdinInspector;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using Debug = UnityEngine.Debug;

public class Tile : MonoBehaviour
{
    private Cell _cell;

    [SerializeField] private SpriteRenderer sp;

    public PositionData position { get; private set; }

    [ShowInInspector, ReadOnly] private List<Tile> coveringTiles; // tiles that are covering this tile
    [ShowInInspector, ReadOnly] private List<Tile> coveredTiles; // tiles that this tile is covering]

    public bool CanInteract => _cell is null && coveringTiles.Count == 0;

    public enum EType
    {
        TYPE_ONE,
        TYPE_TWO,
        TYPE_THREE,
        TYPE_FOUR,
        TYPE_FIVE,
        TYPE_SIX,
        TYPE_SEVEN,
        TYPE_EIGHT,
        TYPE_NINE,
    }


    public bool IsOnCell => _cell != null;

    public EType type;

    public void SetType(EType newType)
    {
        coveringTiles = new List<Tile>();
        coveredTiles = new List<Tile>();
        type = newType;
        sp.color = GameManager.Instance.constants.GetColor(newType);
    }

    public void SetPosition(PositionData position)
    {
        this.position = position;
        transform.localPosition = new Vector3(position.x, position.y, position.z);
        SetLayer((int)position.z);
    }

    public void SetCell(Cell cell)
    {
        this._cell = cell;
    }

    public void Delete()
    {
        Destroy(this.gameObject);
        // _viewTransform = null;
    }

    public void MoveTo(Cell cell, Action task = null)
    {
        if (_cell != null)
        {
            _cell.Free();
        }

        cell.Assign(this);
        GameManager.Instance.SetState(GameManager.GameState.ON_BUSY);
        ;
        transform.DOMove(cell.view.position, 0.2f).OnComplete(() =>
        {
            task?.Invoke();
            GameManager.Instance.SetState(GameManager.GameState.ON_WAIT);
        });
        UpdateAllCoveredTiles();
    }

    private void UpdateAllCoveredTiles()
    {
        foreach (var tile in coveredTiles)
        {
            tile.RemoveTileFromCoveringTiles(this);
            tile.UpdateView();
        }
    }

    private void RemoveTileFromCoveringTiles(Tile tile)
    {
        coveringTiles.Remove(tile);
    }

    public void SetCoveringTiles(List<Tile> tiles)
    {
        coveringTiles = tiles;
    }

    public void AddCoveredTile(Tile tile)
    {
        coveredTiles ??= new List<Tile>();
        coveredTiles.Add(tile);
    }

    public void UpdateView()
    {
        sp.DOFade(CanInteract || _cell != null ? 1 : 0.5f, 0.1f);
    }

    private void SetLayer(int layer)
    {
        sp.sortingOrder = 0 - layer;
    }

    public bool IsTheSameType(Tile tile)
    {
        return type == tile.type;
    }
}