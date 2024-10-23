using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A UI Controller for the Pause Menu. Controls toggling the menu on and off, 
/// as well as switching between various screens within the menu itself.
/// </summary>

public class PauseMenuController : MonoBehaviour
{
    // Input Checks.
    private bool PressedPause() => Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab);
    private bool PressedLeft() => Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.Keypad4) || Input.GetKeyDown(KeyCode.LeftArrow); // If people don't want to use the buttons
    private bool PressedRight() => Input.GetKeyDown(KeyCode.D) || Input.GetKeyDown(KeyCode.Keypad6) || Input.GetKeyDown(KeyCode.RightArrow); // If people don't want to use the buttons

    /// <summary> 
    /// If any other characters or agents in the game need to check whether we're paused, they
    /// can get a reference to the Pause Controller. 
    /// </summary>
    public bool isGamePaused { get; private set; }

    /// <summary>
    /// Different Game Objects to ensure menu toggling is all done correctly. Ensure to assign values in Inspector correctly.
    /// </summary>
    [Header("Pause Screen Game Objects")]
    [SerializeField] GameObject pauseMenuObject;
    [SerializeField] List<GameObject> pauseScreenParentObjects = new List<GameObject>(); // Assign in Inspector. Ensure List Indices are the same as in Enum PauseMenuScreens

    private void Update()
    {
        // Check for pause toggling first before other inputs.
        // May need to adjust timescale here as well if we ever have any projectiles.
        if (PressedPause())
        {
            isGamePaused = !isGamePaused;
            if (isGamePaused)
            {
                Debug.Log("Game is Paused.");
                pauseMenuObject.SetActive(true);
            }
            else
            {
                Debug.Log("Game is Unpaused.");
                pauseMenuObject.SetActive(false);
            }
            Debug.Log("Pause Pressed.");
        }

        // Additional controls without needing on-screen buttons.
        else if (PressedLeft())
        {
            PreviousPage();
        }
        else if (PressedRight())
        {
            NextPage();
        }
    }

    private enum PauseMenuScreens
    {
        MINIMAP,    // 0
        CLUES,      // 1
        SETTINGS    // 2
    }
    private PauseMenuScreens currentScreen;

    /// <summary> Assigned to The Right-facing arrow button on the menu. Iterates through menu screens. </summary>
    public void NextPage()
    {
        currentScreen++;
        if(currentScreen > PauseMenuScreens.SETTINGS) // Will need changing if new screens are added. Final element in enum used as condition - TODO change to be generic?
        {
            currentScreen = PauseMenuScreens.MINIMAP;
        }
        Debug.Log("Current Screen is " + currentScreen);

        switch (currentScreen)
        {
            // Disable all screens, then re-enable relevant screen.
            case PauseMenuScreens.MINIMAP:
                DeactivateGUIs();
                pauseScreenParentObjects[0].SetActive(true);
                break;
            case PauseMenuScreens.CLUES:
                DeactivateGUIs();
                pauseScreenParentObjects[1].SetActive(true);
                break;
            case PauseMenuScreens.SETTINGS:
                DeactivateGUIs();
                pauseScreenParentObjects[2].SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary> Assigned to The Left-facing arrow button on the menu. Iterates through menu screens. </summary>
    public void PreviousPage()
    {
        currentScreen--;
        if (currentScreen < 0)
        {
            currentScreen = PauseMenuScreens.SETTINGS; // Will need changing if new screens are added
        }
        Debug.Log("Current Screen is " + currentScreen);

        switch (currentScreen)
        {
            // Disable all screens, then re-enable relevant screen.
            case PauseMenuScreens.MINIMAP:
                DeactivateGUIs();
                pauseScreenParentObjects[0].SetActive(true);
                break;
            case PauseMenuScreens.CLUES:
                DeactivateGUIs();
                pauseScreenParentObjects[1].SetActive(true);
                break;
            case PauseMenuScreens.SETTINGS:
                DeactivateGUIs();
                pauseScreenParentObjects[2].SetActive(true);
                break;
            default:
                break;
        }
    }

    /// <summary> 
    /// Helper function to quickly deactive all screens. When changing screens,
    /// All screens are disabled, then the relevant one is re-enabled.
    /// </summary>
    private void DeactivateGUIs()
    {
        foreach(GameObject screen in pauseScreenParentObjects)
        {
            screen.SetActive(false);
        }
    }
}