using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Animation controller for the player. Retrieves some properties from the  
/// Player Controller to rotate and animate the player's sprite, as well 
/// as the flashlight used for navigation around the level.
/// </summary>

public class PlayerAnimationController : MonoBehaviour
{
    /// <summary>
    /// Reference to the sprite animator component. There are 3 parameters within it:
    /// IsMoving (bool) to determine whether to use the moving or stationary animations
    /// XMove and YMove (ints) to determine which set of animations to use: up, down, left, or right
    /// </summary>
    private Animator _animator;

    /// <summary> A reference to the light object attached to the player. Assign in Inspector. </summary>
    [SerializeField] Light2D flashlight; 

    /// <summary> Utilised to assign the correct animation parameters. </summary>
    private PlayerController.Direction facingDirection;
    public bool isMoving;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        if(_animator == null) { Debug.LogError("No Animator component on this object!"); }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        _animator.SetBool("IsMoving", isMoving);
    }

    /// <summary> Uses PlayerController movement directions to assign animation parameters. </summary>
    /// <param name="_newDir">The most recently attempted move direction.</param>
    public void SetFacingDirection(PlayerController.Direction _newDir)
    {
        switch (_newDir)
        {
            case PlayerController.Direction.Left:
                _animator.SetInteger("XMove", -1);
                _animator.SetInteger("YMove", 0);
                flashlight.transform.eulerAngles = new Vector3(0, 0, 90); // flashlight faces left
                break;
            case PlayerController.Direction.Right:
                _animator.SetInteger("XMove", 1);
                _animator.SetInteger("YMove", 0);
                flashlight.transform.eulerAngles = new Vector3(0, 0, 270);
                break;
            case PlayerController.Direction.Up:
                _animator.SetInteger("XMove", 0);
                _animator.SetInteger("YMove", 1);
                flashlight.transform.eulerAngles = new Vector3(0, 0, 0);
                break;
            case PlayerController.Direction.Down:
                _animator.SetInteger("XMove", 0);
                _animator.SetInteger("YMove", -1);
                flashlight.transform.eulerAngles = new Vector3(0, 0, 180);
                break;
            default:
                Debug.LogWarning("Attempting to move in a strange way...");
                break;
        }
    }
}