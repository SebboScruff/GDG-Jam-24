using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The goal of this puzzle is to align the discs. It is made more challenging by 
/// the fact that when player spins a disc it has it's own spin amount so the player
/// needs to learn the spin amounts to have a chance of aligning them.
/// The algorithm follows basic multiplication trick with common denominator (in this case 3).
/// </summary>
public class SpinningDiscsPuzzleManager : MonoBehaviour
{
    [SerializeField] GameStateManager gameStateManager;

    [SerializeField] Transform circleInner, circleMiddle, circleOuter;

    /// <summary>
    /// From inner to outer.
    /// </summary>
    [SerializeField] int[] rotations = { 0, 0, 0 };
    [SerializeField] int[] rotationAmountsPerSpin = { 6, 9, 12 };

    private enum Disc { Inner, Middle, Outer };

    void Start()
    {
        SetupMinigame();
    }

    private void SetupMinigame()
    {
        // randomize how much the discs have already spun.
        for (int i = 0; i < rotations.Length; i++)
        {
            rotations[i] = Random.Range(0, 15);
        }

        SpinInnerCircle();
        SpinMiddleCircle();
        SpinOuterCircle();
    }

    /// <summary> Button command which spins the inner circle by its spin amount.</summary>
    public void SpinInnerCircle()
    {
        rotations[(int)Disc.Inner]++;
        circleInner.rotation = Quaternion.Euler((Vector3.forward * rotationAmountsPerSpin[(int)Disc.Inner]) * rotations[(int)Disc.Inner]);
        CheckForWin();
    }

    /// <summary> Button command which spins the middle circle by its spin amount.</summary>
    public void SpinMiddleCircle()
    {
        rotations[(int)Disc.Middle]++;
        circleMiddle.rotation = Quaternion.Euler((Vector3.forward * rotationAmountsPerSpin[(int)Disc.Middle]) * rotations[(int)Disc.Middle]);
        CheckForWin();
    }

    /// <summary> Button command which spins the outer circle by its spin amount.</summary>
    public void SpinOuterCircle()
    {
        rotations[(int)Disc.Outer]++;
        circleOuter.rotation = Quaternion.Euler((Vector3.forward * rotationAmountsPerSpin[(int)Disc.Outer]) * rotations[(int)Disc.Outer]);
        CheckForWin();
    }

    /// <summary>
    /// Checks if the picture is in a upright position. Other matching rotations are not yet supported.
    /// </summary>
    private void CheckForWin()
    {
        if (rotations[(int)Disc.Inner] * rotationAmountsPerSpin[(int)Disc.Inner] % 360 == 0
            && rotations[(int)Disc.Middle] * rotationAmountsPerSpin[(int)Disc.Middle] % 360 == 0
            && rotations[(int)Disc.Outer] * rotationAmountsPerSpin[(int)Disc.Outer] % 360 == 0)
        {
            print("You won the spinning disc puzzle.");
            AwardClue();
        }
    }

    private void AwardClue()
    {
        gameStateManager?.AddClueToJournal(0);
        gameStateManager?.SetGameState(GameStateManager.GameStates.OVERWORLD);
    }
}
