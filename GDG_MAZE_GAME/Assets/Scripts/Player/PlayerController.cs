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

    /// <summary> Movement directions the player can go. Not relative to player rotation. </summary>
    private enum Direction { Left, Right, Up, Down }

    private void Awake()
    {
        _transform = transform;
    }

    void Update()
    {
        if (!_canMove)
        {
            return;
        }

        // catch movement input
        if (PressedUp())
        {
            if (IsEmptySpace(Direction.Up))
            {
                _canMove = false;
                _transform.DOLocalMoveY(_transform.position.y + _movementAmount, _movementTime)
                    .SetEase(_movementEase)
                    .OnComplete(() => { _canMove = true; });
                // SoundManager.Instance.Invoke(nameof(SoundManager.Instance.Footsteps), time: _movementTime + _footstepSoundDelayInSeconds);
            }
        }
        else if (PressedDown())
        {
            if (IsEmptySpace(Direction.Down))
            {
                _canMove = false;
                _transform.DOLocalMoveY(_transform.position.y - _movementAmount, _movementTime)
                    .SetEase(_movementEase)
                    .OnComplete(() => { _canMove = true; });
            }
        }
        else if (PressedLeft())
        {
            if (IsEmptySpace(Direction.Left))
            {
                _canMove = false;
                _transform.DOLocalMoveX(_transform.position.x - _movementAmount, _movementTime)
                    .SetEase(_movementEase)
                    .OnComplete(() => { _canMove = true; });
            }
        }
        else if (PressedRight())
        {
            if (IsEmptySpace(Direction.Right))
            {
                _canMove = false;
                _transform.DOLocalMoveX(_transform.position.x + _movementAmount, _movementTime)
                    .SetEase(_movementEase)
                    .OnComplete(() => { _canMove = true; });
            }
        }
    }

    private bool PressedUp() => Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Keypad8) || Input.GetKey(KeyCode.UpArrow);

    private bool PressedDown() => Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.Keypad2) || Input.GetKey(KeyCode.DownArrow);

    private bool PressedLeft() => Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.Keypad4) || Input.GetKey(KeyCode.LeftArrow);

    private bool PressedRight() => Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.Keypad6) || Input.GetKey(KeyCode.RightArrow);

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
                if (Physics.Raycast(_transform.position, Vector3.forward, maxRayDistance))
                {
                    return false;
                }
                break;
            case Direction.Right:
                Debug.DrawRay(transform.position, Vector3.right, Color.green, debugRayDuration);
                if (Physics.Raycast(_transform.position, Vector3.right, maxRayDistance))
                {
                    return false;
                }
                break;
            case Direction.Down:
                Debug.DrawRay(transform.position, Vector3.down, Color.green, debugRayDuration);
                if (Physics.Raycast(_transform.position, Vector3.back, maxRayDistance))
                {
                    return false;
                }
                break;
            case Direction.Left:
                Debug.DrawRay(transform.position, Vector3.left, Color.green, debugRayDuration);
                if (Physics.Raycast(_transform.position, Vector3.left, maxRayDistance))
                {
                    return false;
                }
                break;
        }
        return true;
    }
}
