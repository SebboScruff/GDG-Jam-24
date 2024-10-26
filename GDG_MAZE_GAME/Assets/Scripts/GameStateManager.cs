using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
using UnityEngine.SceneManagement;

/// <summary>
/// Game State Manager Class, used for storing and transferring important game data
/// such as current game state (paused vs in puzzle vs in overworld etc), 
/// </summary>
public class GameStateManager : MonoBehaviour
{
    const int GAME_TIMER_SECONDS = 300; // How long the in-game timer will last before the player loses
    public const int DOOR_ANSWER_LENGTH = 3;   // How long is the code at the end of the game.

    // Input Checking
    private bool PressedPause() => Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab);

    /// <summary> Available Game States. May need to break into individual puzzles but hopefully not. </summary>
    [System.Serializable] public enum GameStates
    {
        OVERWORLD,      // When the Player is exploring the Maze.
        PUZZLE,         // When the Player opens a Puzzle Minigame
        PAUSED,         // When the game is Paused.
        GAMEOVER_WIN,   // Successfully solving the Door
        GAMEOVER_LOSE   // Running out of Time.
    }
    public GameStates currentGameState { get; private set; }

    // ----
    // GUI Parent Objects
    // ----
    [SerializeField] GameObject overworldGUI, pauseGUI, doorGUI; // TODO Add other Puzzle Screens to manage GUI activation/deactivation.
    [SerializeField] Sprite[] totalClueImages = new Sprite[6];    // Need to be in the same order as the Symbols array. Index 0 doesn't matter since that represents Symbol.NONE
    [SerializeField] Image[] journalClueImages = new Image[DOOR_ANSWER_LENGTH];

    [Header("Music")]
    // Make sure these are on different objects - things tend to go wrong if one gameobject has more than 1 FMOD event emitter
    [SerializeField] StudioEventEmitter backgroundMusicEmitter;
    [SerializeField] StudioEventEmitter pauseSnapshot;

    // ----
    // Timer Settings
    // ----
    private float gameTimerRemaining;

    // ----
    // PUZZLE SETUP
    // ----
    /// <summary> This enum is organised by the visual order of the buttons in the door puzzle GUI </summary>
    [System.Serializable] public enum Symbol
    {
        // Organised top-to-bottom as per Door Puzzle GUI layout.
        NONE,       // 0    DEFAULT VALUE, USED TO CLEAR PLAYER'S INPUT WHEN EXITING DOOR PUZZLE.
        CRYSTAL,    // 1    Corresponding Image File: GUI/MiscIcons_235
        SWORD,      // 2    Corresponding Image File: GUI/MiscIcons_10
        SUN,        // 3    Corresponding Image File: GUI/MiscIcons_132
        GAUNTLET,   // 4    Corresponding Image File: GUI/MiscIcons_231
        BULLIONS    // 5    Corresponding Image File: GUI/MiscIcons_264
    }
    /// <summary> Door code is generated randomly at start of runtime. 
    /// All classes or objects that need access to this level's answer
    /// will be able to get readonly access by referencing the GSM and using gameStateManager.doorAnswer
    /// </summary>
    public Symbol[] doorAnswer { get; private set; } = new Symbol[DOOR_ANSWER_LENGTH];

    private void Awake()
    {
        currentGameState = GameStates.OVERWORLD;

        // Generate this run's Door Puzzle. Done in Awake to ensure it's available for puzzle setup in Start()
        System.Array symbolValues = System.Enum.GetValues(typeof(Symbol)); // extract values from enum format
        for (int i = 0; i < DOOR_ANSWER_LENGTH; i++)
        {
            Symbol answer = (Symbol)symbolValues.GetValue(Random.Range(1, 6)); // get a random Symbol answer and assign to relevant index. Ignores the NONE symbol.
            doorAnswer[i] = answer;
        }
        // TODO Get rid of this before building.
        Debug.Log("Door Code is " + doorAnswer[0] + " " + doorAnswer[1] + " " + doorAnswer[2]);
    }

    // Start is called before the first frame update
    void Start()
    {
        gameTimerRemaining = 0; // start at 0 and increment for easier FMOD use

        // TESTING JOURNAL
        //AddClueToJournal(0);
        //AddClueToJournal(1);
        //AddClueToJournal(2);
    }

    // Update is called once per frame
    void Update()
    {
        // Check for pause toggling first before other inputs.
        if (PressedPause())
        {
            switch (currentGameState)
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
                case GameStates.GAMEOVER_WIN:
                    break;
                case GameStates.GAMEOVER_LOSE:
                    break;
                default:
                    break;
            }
        }

        // At the end of Update, decrease the game timer unless paused.
        if (currentGameState != GameStates.PAUSED)
        {
            gameTimerRemaining += Time.deltaTime;
            // TODO Some kind of visual indicator for the timer?

            if (gameTimerRemaining >= GAME_TIMER_SECONDS)
            {
                // Timer ran out, player has lost.
                SetGameState(GameStates.GAMEOVER_LOSE);
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
            case GameStates.GAMEOVER_WIN:
                backgroundMusicEmitter.SetParameter("Game_Won", 1);
                // TODO Wait for a bit then change scenes.
                break;
            case GameStates.GAMEOVER_LOSE:
                // Gotta do something else here
                break;
            default:
                break;
        }
    }

    /// <summary>
    /// Update the player's journal GUI with the clue gained from solving a puzzle. 
    /// </summary>
    /// <param name="_clueIndex">Target Index in clueImages[]</param>
    public void AddClueToJournal(int _clueIndex)
    {
        // Interrogate doorAnswer[clueIndex] to extract Symbol as Integer value
        // Extract corresponding sprite out of totalClueImages array
        // Assign that sprite into the Pause Menu GUI
        Symbol targetSymbol = doorAnswer[_clueIndex];
        int symbolIndex = (int)targetSymbol;
        journalClueImages[_clueIndex].sprite = totalClueImages[symbolIndex];
        journalClueImages[_clueIndex].color = new Color(1, 1, 1, 1);
    }

    public void StartDoorPuzzle()
    {
        SetGameState(GameStates.PUZZLE);
        doorGUI.SetActive(true);
    }
}