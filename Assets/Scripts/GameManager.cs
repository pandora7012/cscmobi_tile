using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public BoardController boardController;
    public Constants constants;
    
    public enum GameState
    {
        ON_WAIT,
        ON_BUSY,
        ON_END
    }
    
    public GameState State
    {
        get { return m_state; }
        private set
        {
            m_state = value;
            StateChangedAction(m_state);
        }
    }

    private GameState m_state;


    public event Action<GameState> StateChangedAction = delegate { };
    
    
    
    private void Awake()
    {
        Instance = this;
        StartGame();
    }

    private void StartGame()
    {
       boardController.Setup();
    }
    
    public void SetState(GameState state)
    {
        if (state is GameState.ON_END)
        {
            StartCoroutine(DelayedAction(0.2f, ()=>
            {
                State = state;
            })); 
            return ; 
        }
        State = state;
    }
    
    IEnumerator DelayedAction(float delay, Action action)
    {
        yield return new WaitForSeconds(delay);
        action();
    }
    
    public void WinGame()
    {
        SetState(GameState.ON_END);
        Debug.LogWarning("Win Game");
    }
    
    public void LoseGame()
    {
        SetState(GameState.ON_END);
        Debug.LogWarning("Lose Game");
    }
}
