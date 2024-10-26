using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

/// <summary>
/// Character controller for the player. Can both get the movement input and 
/// move the player. Player character can instantly change directions when 
/// moving provided the previous movement has ended.
/// </summary>
public class PlayerController : MonoBehaviour
{
    /// <summary> Access to the game pause controller to prevent player from moving while paused. </summary>
    [SerializeField] GameStateManager gameManager;

    /// <summary> Cached transform micro-optimization. </summary>
    private Transform _transform;

    /// <summary> How much time the movement takes in seconds. </summary>
    [Header("Movement Settings")]
    [SerializeField]
    private float _movementTime = 0.5f;
    /// <summary> How much the player moves in Unity units. </summary>
    private float _movementAmount = 1f;
    [SerializeField]
    private Ease _movementEase = Ease.InOutSine;

    [Header("Interactible Tiles")]
    [SerializeField] LayerMask interactibleTileLayers;
    [SerializeField] Direction currentFacingDir;
    [SerializeField] bool canInteract = false; // Can the player interact with the tile directly in front of them?
    [SerializeField] GameObject currentFacingTile; // Which interactible tile is the player currently facing? TODO Eventually change this to puzzle class rather than gameobject?
    public UnityEngine.UI.Image interactButtonPrompt; // Visual aid to let players know when they can interact with a tile.

    /// <summary>
    /// After movement has happened how long to wait before playing footstep 
    /// sound. Negative number means that playing sound is started before 
    /// the movement is completed.
    /// </summary>
    [Header("Sound Settings")]
    [SerializeField] private float _footstepSoundDelayInSeconds = -0.1f;

    /// <summary> Is player allowed to move? Is set to false at the start of movement.</summary>
    [Header("State")]
    [SerializeField]
    private bool _canMove = true;

    /// <summary>
    /// A reference to the Animation Controller instance on the same object as this
    /// in order to send the required information across
    /// -Seb
    /// </summary>
    [Header("Animation")]
    [SerializeField] PlayerAnimationController animController;

    /// <summary> Movement directions the player can go. Not relative to player rotation. {made public for use in PlayerAnimationController.cs -Seb}</summary>
    public enum Direction { Left, Right, Up, Down }

    private void Awake()
    {
        _transform = transform;

        if(animController == null) { Debug.LogError("Animation Controller not found on Player!"); }
    }

    void Update()
    {
        if (!_canMove || gameManager.currentGameState != GameStateManager.GameStates.OVERWORLD)
        {
            return;
        }

        #region Input Check: Movement
        // catch movement input
        if (PressedUp())
        {
            animController.SetFacingDirection(Direction.Up); // Set Direction for the animation controller. One per direction. -Seb
            currentFacingDir = Direction.Up;

            if (IsEmptySpace(Direction.Up))
            {
                _canMove = false;
                animController.isMoving = true; // Change to dynamic sprite movement. Turned off in OnComplete() lambda. -Seb

                _transform.DOLocalMoveY(_transform.position.y + _movementAmount, _movementTime)
                    .SetEase(_movementEase)
                    .OnComplete(() => { _canMove = true; animController.isMoving = false; });
                // SoundManager.Instance.Invoke(nameof(SoundManager.Instance.Footsteps), time: _movementTime + _footstepSoundDelayInSeconds);
            }
        }
        else if (PressedDown())
        {
            animController.SetFacingDirection(Direction.Down);
            currentFacingDir = Direction.Down;

            if (IsEmptySpace(Direction.Down))
            {
                _canMove = false;
                animController.isMoving = true;

                _transform.DOLocalMoveY(_transform.position.y - _movementAmount, _movementTime)
                    .SetEase(_movementEase)
                    .OnComplete(() => { _canMove = true; animController.isMoving = false; });
            }
        }
        else if (PressedLeft())
        {
            animController.SetFacingDirection(Direction.Left);
            currentFacingDir = Direction.Left;

            if (IsEmptySpace(Direction.Left))
            {
                _canMove = false;
                animController.isMoving = true;

                _transform.DOLocalMoveX(_transform.position.x - _movementAmount, _movementTime)
                    .SetEase(_movementEase)
                    .OnComplete(() => { _canMove = true; animController.isMoving = false; });
            }
        }
        else if (PressedRight())
        {
            animController.SetFacingDirection(Direction.Right);
            currentFacingDir = Direction.Right;

            if (IsEmptySpace(Direction.Right))
            {
                _canMove = false;
                animController.isMoving = true;

                _transform.DOLocalMoveX(_transform.position.x + _movementAmount, _movementTime)
                    .SetEase(_movementEase)
                    .OnComplete(() => { _canMove = true; animController.isMoving = false; });
            }
        }
        #endregion

        #region Input Check: Puzzle Interaction
        canInteract = IsNextSpaceInteractible();
        // TODO Toggle on GUI stuff here: button prompt to tell players they can interact.
        if(canInteract && PressedInteract())
        {
            // Open up a puzzle screen here!
            Debug.Log("Interacting with " + currentFacingTile.name);
        }
        #endregion
    }

    private bool PressedUp() => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Keypad8) || Input.GetKey(KeyCode.UpArrow);

    private bool PressedDown() => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.DownArrow);

    private bool PressedLeft() => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.LeftArrow);

    private bool PressedRight() => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Keypad6) || Input.GetKey(KeyCode.RightArrow);

    private bool PressedInteract() => Input.GetKeyDown(KeyCode.E); // other hotkeys here if we think of some.

    /// <summary>
    /// Checks if there is empty space for the player to move to.
    /// </summary>
    /// <param name="direction">What direction to check.</param>
    /// <returns>True if player can move to desired direction.</returns>
    private bool IsEmptySpace(Direction direction)
    {
        var maxRayDistance = _movementAmount * 1.1f;
        var debugRayDuration = 0.4f;
        switch (direction)
        {
            case Direction.Up:
                Debug.DrawRay(transform.position, Vector3.up, Color.green, debugRayDuration);
                if (Physics2D.Raycast(_transform.position, Vector3.up, maxRayDistance)) // Had to change to 2D Raycasts for correct interaction with Tilemap Collider. -Seb
                {
                    return false;
                }
                break;
            case Direction.Right:
                Debug.DrawRay(transform.position, Vector3.right, Color.green, debugRayDuration);
                if (Physics2D.Raycast(_transform.position, Vector3.right, maxRayDistance))
                {
                    return false;
                }
                break;
            case Direction.Down:
                Debug.DrawRay(transform.position, Vector3.down, Color.green, debugRayDuration);
                if (Physics2D.Raycast(_transform.position, Vector3.down, maxRayDistance))
                {
                    return false;
                }
                break;
            case Direction.Left:
                Debug.DrawRay(transform.position, Vector3.left, Color.green, debugRayDuration);
                if (Physics2D.Raycast(_transform.position, Vector3.left, maxRayDistance))
                {
                    return false;
                }
                break;
        }
        return true;
    }

    /// <summary>
    /// Raycast check in the player's current facing direction to see if the tile
    /// immediately in front of them is interactible (Puzzle, Door, Etc) or not.
    /// </summary>
    /// <returns>True if next tile in player's facing direction is interactible. False otherwise.</returns>
    private bool IsNextSpaceInteractible()
    {
        var maxRayDistance = _movementAmount * 1.1f;

        Vector3 checkDir = Vector3.zero;
        switch (currentFacingDir)
        {
            case Direction.Left:
                checkDir = Vector3.left;
                break;
            case Direction.Right:
                checkDir = Vector3.right;
                break;
            case Direction.Up:
                checkDir = Vector3.up;
                break;
            case Direction.Down:
                checkDir = Vector3.down;
                break;
        }
        RaycastHit2D hitTile = Physics2D.Raycast(_transform.position, checkDir, maxRayDistance, interactibleTileLayers);

        Color debugRayColor = hitTile ? Color.green : Color.red;
        Debug.DrawRay(transform.position, checkDir, debugRayColor);

        if (hitTile)
        {
            currentFacingTile = hitTile.transform.gameObject;
            interactButtonPrompt.enabled = true;
            return true;
        }
        else
        {
            currentFacingTile = null;
            interactButtonPrompt.enabled = false;
            return false;
        }
    }
}
