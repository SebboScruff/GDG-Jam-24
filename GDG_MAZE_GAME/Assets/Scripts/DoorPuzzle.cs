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

    /// <summary>
    /// Descriptors for the different symbols.
    /// These eventually need to be linked to the puzzle generation somehow,
    /// to ensure the door's answer is consistent with the puzzles' solutions.
    /// </summary>
    [System.Serializable]public enum Symbols
    {
        // Organised top-to-bottom as per Door Puzzle GUI layout.
        NONE,       // 0    DEFAULT VALUE, USED TO CLEAR PLAYER'S INPUT WHEN EXITING.
        CRYSTAL,    // 1
        SWORD,      // 2
        SUN,        // 3
        GAUNTLET,   // 4
        BULLIONS    // 5
    }

    /// <summary> The Answer to the Door Puzzle. Randomly Generate at start of runtime and do not change afterwards. </summary>
    private Symbols[] doorAnswer = new Symbols[ANSWER_LENGTH]; 
    /// <summary> The player's currently inputted answer. Dynamically changes and gets cleared upon exiting the Puzzle. </summary>
    [SerializeField] private Symbols[] currentInput = new Symbols[ANSWER_LENGTH];

    ///<summary>
    /// References to each of the on-screen buttons for controlling player input. 0-4 are column I on-screen; 5-9 are column II; 10-14 are column III
    /// This could theoretically be a 2D array (Button[3,5]) for easier delegation, but for ease if use in the unity inspector it's a 1x15
    ///</summary> 
    [SerializeField] Button[] guiInputButtons = new Button[15];
    [SerializeField] Button exitButton, submitButton;

    private void Awake()
    {
        // Generate this run's Door Puzzle. Done in Awake to ensure it's available for puzzle setup in Start()
        System.Array symbolValues = System.Enum.GetValues(typeof(Symbols)); // extract values from enum format outside of loop
        for (int i = 0; i < ANSWER_LENGTH; i++)
        {
            Symbols answer = (Symbols)symbolValues.GetValue(Random.Range(1, 6)); // get a random Symbol answer and assign to relevant index. Ignores the NONE symbol.
            doorAnswer[i] = answer;

            currentInput[i] = Symbols.NONE;
        }
        // TODO Get rid of this before building.
        Debug.Log("Door Code is " + doorAnswer[0] + " " + doorAnswer[1] + " " + doorAnswer[2]);
    }

    // Start is called before the first frame update
    void Start()
    {
        #region BUTTON DELEGATION
        // Button Delegation, to alter the player's answer based on button input.
        // Requires very specific setup in Inspector and is definitely not the optimal approach but it's a game jam

        // Column 1---
        guiInputButtons[0].onClick.AddListener(delegate { EditAnswer(0, Symbols.CRYSTAL); });
        guiInputButtons[1].onClick.AddListener(delegate { EditAnswer(0, Symbols.SWORD); });
        guiInputButtons[2].onClick.AddListener(delegate { EditAnswer(0, Symbols.SUN); });
        guiInputButtons[3].onClick.AddListener(delegate { EditAnswer(0, Symbols.GAUNTLET); });
        guiInputButtons[4].onClick.AddListener(delegate { EditAnswer(0, Symbols.BULLIONS); });
        // Column 2---
        guiInputButtons[5].onClick.AddListener(delegate { EditAnswer(1, Symbols.CRYSTAL); });
        guiInputButtons[6].onClick.AddListener(delegate { EditAnswer(1, Symbols.SWORD); });
        guiInputButtons[7].onClick.AddListener(delegate { EditAnswer(1, Symbols.SUN); });
        guiInputButtons[8].onClick.AddListener(delegate { EditAnswer(1, Symbols.GAUNTLET); });
        guiInputButtons[9].onClick.AddListener(delegate { EditAnswer(1, Symbols.BULLIONS); });
        // Column 3---
        guiInputButtons[10].onClick.AddListener(delegate { EditAnswer(2, Symbols.CRYSTAL); });
        guiInputButtons[11].onClick.AddListener(delegate { EditAnswer(2, Symbols.SWORD); });
        guiInputButtons[12].onClick.AddListener(delegate { EditAnswer(2, Symbols.SUN); });
        guiInputButtons[13].onClick.AddListener(delegate { EditAnswer(2, Symbols.GAUNTLET); });
        guiInputButtons[14].onClick.AddListener(delegate { EditAnswer(2, Symbols.BULLIONS); });

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
    public void EditAnswer(int _index, Symbols _newInput)
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

        for(int i = 0; i < ANSWER_LENGTH; i++)
        {
            if(currentInput[i] == Symbols.NONE)
            {
                // TODO Some kind of message to the player to let them know their answer is incomplete.
                Debug.Log("Answer is Incomplete!");
                return; // if the answer is incomplete, exit out of the Submit method but DO NOT exit the minigame.
            }
            if(currentInput[i] != doorAnswer[i])
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
            ExitPuzzle();
        }
    }

    private void ClearAnswer()
    {
        for(int i = 0; i < ANSWER_LENGTH; i++)
        {
            currentInput[i] = Symbols.NONE;
        }
    }

    private void ExitPuzzle()
    {
        ClearAnswer();
        // Return to regular Player Controls somehow
    }
}
