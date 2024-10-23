using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

/// <summary>
/// Controller for adjusting FMOD parameters for the
/// main level background music during runtime. 
/// TODO: adjust the Time_Elapsed parameter based on something TBD.
/// This parameter should scale up from 0.0f to 1.0f as the game progresses.
/// </summary>
public class MazeMusicController : MonoBehaviour
{
    ///<summary> Reference the Event Emitter for later adjustment of parameters </summary>
    [SerializeField] StudioEventEmitter musicEmitter;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
