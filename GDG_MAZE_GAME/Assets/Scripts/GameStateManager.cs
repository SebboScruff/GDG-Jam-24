using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateManager : MonoBehaviour
{
    // Input Checking
    private bool PressedPause() => Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab);

    [System.Serializable] public enum GameStates
    {
        OVERWORLD,  // When the Player is exploring the Maze.
        PUZZLE,     // When the Player opens a Puzzle Minigame
        PAUSED      // When the game is Paused.
    }
    public GameStates currentGameState { get; private set; }

    [SerializeField] GameObject overworldGUI, pauseGUI, doorGUI; // TODO Add other Puzzle Screens to manage GUI activation/deactivation.

    private void Awake()
    {
        currentGameState = GameStates.OVERWORLD;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check for pause toggling first before other inputs.
        if (PressedPause())
        {
            switch(currentGameState)
            {
                case GameStates.OVERWORLD:
                    SetGameState(GameStates.PAUSED);
                    break;
                case GameStates.PUZZLE:
                    SetGameState(GameStates.OVERWORLD);
                    break;
                case GameStates.PAUSED:
                    SetGameState(GameStates.OVERWORLD);
                    break;
                default:
                    break;
            }
        }
    }

    public void SetGameState(GameStates _newState)
    {
        currentGameState = _newState;

        switch (currentGameState)
        {
            case GameStates.OVERWORLD:
                overworldGUI.SetActive(true);
                pauseGUI.SetActive(false);
                doorGUI.SetActive(false);
                break;
            case GameStates.PUZZLE:
                break;
            case GameStates.PAUSED:
                overworldGUI.SetActive(false);
                pauseGUI.SetActive(true);
                doorGUI.SetActive(false);
                break;
            default:
                break;
        }
    }
}