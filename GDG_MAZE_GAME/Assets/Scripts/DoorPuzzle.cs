using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Behaviours and interactions with the final puzzle of the game.
/// Using on-screen GUI buttons, players will enter a code comprised of 3 symbols,
/// then submit their answer. Depending on the answer that is submitted, players either
/// win the game or die.
/// </summary>
public class DoorPuzzle : MonoBehaviour
{
    const int ANSWER_LENGTH = 3; // Number of columns in the Door Puzzle GUI.

    [SerializeField] GameStateManager gameStateManager;
 
    /// <summary> The player's currently inputted answer. Dynamically changes and gets cleared upon exiting the Puzzle. </summary>
    [SerializeField] private GameStateManager.Symbol[] currentInput = new GameStateManager.Symbol[GameStateManager.DOOR_ANSWER_LENGTH];

    ///<summary>
    /// References to each of the on-screen buttons for controlling player input. 0-4 are column I on-screen; 5-9 are column II; 10-14 are column III
    /// This could theoretically be a 2D array (Button[3,5]) for easier delegation, but for ease if use in the unity inspector it's a 1x15
    ///</summary> 
    [SerializeField] Button[] guiInputButtons = new Button[15];
    [SerializeField] Button exitButton, submitButton;

    private void Awake()
    {
        if(gameStateManager == null)
        {
            Debug.Log("NO GAME STATE MANAGER ASSIGNED TO DOOR PUZZLE. MasterGUI/DoorPuzzle/Behaviours needs object the GameStateManager assigned!");
        }
        // make sure the player doesnt have any default answer values.
        for (int i = 0; i < ANSWER_LENGTH; i++)
        {
            currentInput[i] = GameStateManager.Symbol.NONE;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        #region BUTTON DELEGATION
        // Button Delegation, to alter the player's answer based on button input.
        // Requires very specific setup in Inspector and is definitely not the optimal approach but it's a game jam

        // Column 1---
        guiInputButtons[0].onClick.AddListener(delegate { EditAnswer(0, GameStateManager.Symbol.CRYSTAL); });
        guiInputButtons[1].onClick.AddListener(delegate { EditAnswer(0, GameStateManager.Symbol.SWORD); });
        guiInputButtons[2].onClick.AddListener(delegate { EditAnswer(0, GameStateManager.Symbol.SUN); });
        guiInputButtons[3].onClick.AddListener(delegate { EditAnswer(0, GameStateManager.Symbol.GAUNTLET); });
        guiInputButtons[4].onClick.AddListener(delegate { EditAnswer(0, GameStateManager.Symbol.BULLIONS); });
        // Column 2---
        guiInputButtons[5].onClick.AddListener(delegate { EditAnswer(1, GameStateManager.Symbol.CRYSTAL); });
        guiInputButtons[6].onClick.AddListener(delegate { EditAnswer(1, GameStateManager.Symbol.SWORD); });
        guiInputButtons[7].onClick.AddListener(delegate { EditAnswer(1, GameStateManager.Symbol.SUN); });
        guiInputButtons[8].onClick.AddListener(delegate { EditAnswer(1, GameStateManager.Symbol.GAUNTLET); });
        guiInputButtons[9].onClick.AddListener(delegate { EditAnswer(1, GameStateManager.Symbol.BULLIONS); });
        // Column 3---
        guiInputButtons[10].onClick.AddListener(delegate { EditAnswer(2, GameStateManager.Symbol.CRYSTAL); });
        guiInputButtons[11].onClick.AddListener(delegate { EditAnswer(2, GameStateManager.Symbol.SWORD); });
        guiInputButtons[12].onClick.AddListener(delegate { EditAnswer(2, GameStateManager.Symbol.SUN); });
        guiInputButtons[13].onClick.AddListener(delegate { EditAnswer(2, GameStateManager.Symbol.GAUNTLET); });
        guiInputButtons[14].onClick.AddListener(delegate { EditAnswer(2, GameStateManager.Symbol.BULLIONS); });

        // Others---
        exitButton.onClick.AddListener(delegate { ExitPuzzle(); });
        submitButton.onClick.AddListener(delegate { SubmitAnswer(); });
        #endregion
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    /// <summary>
    /// Function for altering individual values in the currentInput array. Ties to on-screen buttons.
    /// </summary>
    /// <param name="_index">The index within the currentInput array where the value will change.</param>
    /// <param name="_newInput">The Symbol value that will replace the existing value at currentInput[_index]</param>
    public void EditAnswer(int _index, GameStateManager.Symbol _newInput)
    {
        // TODO Force certain buttons to maintain their visual 'pressed' state, as a visual aid for the player.
        currentInput[_index] = _newInput;
    }

    /// <summary>
    /// Activated when the player presses the on-screen 'Unlock' button. Various effects based on values of currentAnswer:
    /// - If incomplete (i.e. any values are NONE): do not accept solution. Play some error message and remain in Puzzle Screen.
    /// - If incorrect (i.e. currentInput != doorAnswer at any index): kill player or otherwise do failure consequence
    /// - If correct i.e. currentInput != doorAnswer at any index): player wins!
    /// </summary>
    private void SubmitAnswer()
    {
        int correctAnswers = 0; // track number of correct answers in the player's input array

        // First need to check that no answers are Empty: (needs to be done as a separate For loop so that players aren't killed if inputting e.g. [wrong] [wrong] [empty]
        for(int i = 0; i < GameStateManager.DOOR_ANSWER_LENGTH; i++)
        {
            if(currentInput[i] == GameStateManager.Symbol.NONE)
            {
                // TODO Some kind of message to the player to let them know their answer is incomplete.
                Debug.Log("Answer is Incomplete!");
                return; // if the answer is incomplete, exit out of the Submit method but DO NOT exit the minigame.
            }
        }
        // Then check if all submitted answers are correct:
        for(int i = 0; i < GameStateManager.DOOR_ANSWER_LENGTH; i++)
        {
            if (currentInput[i] != gameStateManager.doorAnswer[i])
            {
                // TODO Some kind of message to the player to let them know they died a horrible death deep in the maze
                Debug.Log("Wrong Answer!");
                //ExitPuzzle();
                return; // if the answer is incorrect, exit out of the Submit method AND the puzzle (and probably kill the player)
            }
            correctAnswers++;
        }

        // then check if all submitted answers are correct at the end.
        // this is done here rather than inside the for() above to ensure players must have all answers correct.
        if(correctAnswers == ANSWER_LENGTH)
        {
            // TODO Some kind of message to the player to let them know they are very cool and smart
            Debug.Log("Right Answer!");
            gameStateManager.SetGameState(GameStateManager.GameStates.GAMEOVER_WIN);
        }
    }

    private void ClearAnswer()
    {
        for(int i = 0; i < ANSWER_LENGTH; i++)
        {
            currentInput[i] = GameStateManager.Symbol.NONE;
        }
    }

    private void ExitPuzzle()
    {
        ClearAnswer();
        // Return to regular Player Controls somehow
        gameStateManager.SetGameState(GameStateManager.GameStates.OVERWORLD);
    }
}
