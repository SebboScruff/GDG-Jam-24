using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;
using UnityEngine.SceneManagement;

/// <summary>
/// Game State Manager Class, used for storing and transferring important game data
/// such as current game state (paused vs in puzzle vs in overworld etc), 
/// </summary>
public class GameStateManager : MonoBehaviour
{
    const int GAME_TIMER_SECONDS = 300; // How long the in-game timer will last before the player loses

    // Input Checking
    private bool PressedPause() => Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab);

    /// <summary>
    /// Available Game States. May need to break into individual puzzles but hopefully not.
    /// </summary>
    [System.Serializable] public enum GameStates
    {
        OVERWORLD,  // When the Player is exploring the Maze.
        PUZZLE,     // When the Player opens a Puzzle Minigame
        PAUSED      // When the game is Paused.
    }
    public GameStates currentGameState { get; private set; }

    // GUI Parent Objects
    [SerializeField] GameObject overworldGUI, pauseGUI, doorGUI; // TODO Add other Puzzle Screens to manage GUI activation/deactivation.

    [Header("Music")]
    // Make sure these are on different objects - things tend to go wrong if one gameobject has more than 1 FMOD event emitter
    [SerializeField] StudioEventEmitter backgroundMusicEmitter;
    [SerializeField] StudioEventEmitter pauseSnapshot;

    // Timer Settings
    private float gameTimerRemaining;

    [Header("Puzzle Setup")]


    private void Awake()
    {
        currentGameState = GameStates.OVERWORLD;
    }

    // Start is called before the first frame update
    void Start()
    {
        gameTimerRemaining = 0; // start at 0 and increment for easier FMOD use
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

        // At the end of Update, decrease the game timer unless paused.
        if(currentGameState != GameStates.PAUSED)
        {
            gameTimerRemaining += Time.deltaTime;
            if (gameTimerRemaining >= GAME_TIMER_SECONDS)
            {
                // Player has lost. Go to Game Over Screen here.
            }
        }

        // FMOD PARAMETER UPDATES - Make sure String Parameters are exactly correct. 
        backgroundMusicEmitter.SetParameter("Time_Elapsed", gameTimerRemaining / GAME_TIMER_SECONDS); // Increments by the inverse of the max timer so that the BGM layers are added correctly.
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
                pauseSnapshot.Stop();
                break;
            case GameStates.PUZZLE:
                pauseSnapshot.Stop();
                break;
            case GameStates.PAUSED:
                overworldGUI.SetActive(false);
                pauseGUI.SetActive(true);
                doorGUI.SetActive(false);
                pauseSnapshot.Play();
                break;
            default:
                break;
        }
    }
}