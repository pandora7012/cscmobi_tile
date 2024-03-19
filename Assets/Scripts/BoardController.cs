using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public Board m_board;
    public Camera m_cam;


    public bool isBusy;

    public void Setup()
    {
        m_cam = Camera.main;
        m_board.Setup();
        GameManager.Instance.StateChangedAction += OnGameStateChange;
    }

    private void OnGameStateChange(GameManager.GameState state)
    {
        isBusy = state switch
        {
            GameManager.GameState.ON_BUSY => true,
            GameManager.GameState.ON_WAIT => false,
            GameManager.GameState.ON_END => true,
            _ => isBusy
        };
    }


    void Update()
    {
        if (isBusy)
            return;


        var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
        if (Input.GetMouseButtonDown(0))
        {
            if (hit.collider != null)
            {
                Tile tile = hit.collider.GetComponent<Tile>();
                var pos = m_board.PushTileToCell(tile);
            }
        }
    }
}