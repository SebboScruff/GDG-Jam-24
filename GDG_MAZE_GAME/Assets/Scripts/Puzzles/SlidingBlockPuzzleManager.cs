using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

/// <summary>
/// Contains algorithm for the 15 puzzle game and view code for moving 
/// the pieces around as well assigning their image. Also alerts other 
/// game systems when the player has completed enough of the puzzle.
/// 
/// Player can click blocks in the puzzle to slide them to the one 
/// available space next to them.
/// 
/// Game is finished if all 15 blocks are in order.
/// </summary>
public class SlidingBlockPuzzleManager : MonoBehaviour
{
    /// <summary>
    /// Represents the puzzle in an 2D array form. Final block is not shown.
    /// </summary>
    private int[,] blocks = new int[4, 4];

    private int gridSize = 4;
    /// <summary> Provides access to block transform positions when blocks are swapped. </summary>
    private List<Transform> blockTransforms = new();
    /// <summary> Provides access to block positions in the 2D array. </summary>
    private Dictionary<int, int[]> blockPositions;

    [SerializeField]
    private float tileMovementDuration = 0.4f;

    [Header("Component References")]
    [SerializeField]
    private Transform blocksParent;

    // Values needed for positioning elements when they move.
    private float blockSize;
    private float halfBlockSize;
    private float blockPadding;

    /// <summary> Is both the delay and duration of pieces that are moved during the initial shuffle. </summary>
    [Header("Starting Shuffle Options")]
    private float shuffleMoveDelay = 0.1f;
    /// <summary> How many times to shuffle the puzzle when it starts. </summary>
    private int shuffleMovesAmount = 20;
    /// <summary>
    /// What was the last grid position that was clicked programmatically 
    /// during starting shuffle. Prevents moving the same piece back and forth.
    /// </summary>
    private int[] lastMovedShufflePosition = { -1, -1 };
    /// <summary>
    /// What was the last tile index that was clicked programmatically 
    /// during starting shuffle.Prevents moving the same piece back and forth.
    /// </summary>
    private int lastMovedShuffleTileIndex = -1;


    // State for the empty block which makes it easier for other blocks to compare their position to it.
    /// <summary> On what row the empty block is in? Zero indexed. </summary>
    [Header("State")]
    private int emptyRow = 3;
    /// <summary> On what column the empty block is in? Zero indexed. </summary>
    private int emptyColumn = 3;
    /// <summary> Where the empty piece is in the grid. </summary>
    private int[] emptyXY = { -1, -1 };
    /// <summary>
    /// If isMovingAllowed is false the tiles are unresponsive when clicked. 
    /// This is false during starting shuffle and when pieces are moving.
    /// </summary>
    private bool isMovingAllowed = false;
    // Shuffling
    /// <summary> What row was last shuffled. </summary>
    private int lastShuffledRow = -1;
    /// <summary> What column was last shuffled. </summary>
    private int lastShuffledColumn = -1;

    private void Start()
    {
        SetupBlocks();
        BeginGame();
    }

    public void BeginGame()
    {
        for (int i = 0; i < shuffleMovesAmount; i++)
        {
            Invoke(nameof(ShuffleNearestUnvisited), i * shuffleMoveDelay);
        }
    }

    private void ShuffleNearestUnvisited()
    {
        var validSlots = new List<int[]>();

        // look all directions and add to list if possible
        if (emptyColumn != 0) // Add left side slots
        {
            validSlots.Add(new int[] { emptyRow, emptyColumn - 1 });
        }
        if (emptyColumn != gridSize - 1)
        {
            validSlots.Add(new int[] { emptyRow, emptyColumn + 1 });
        }
        if (emptyRow != 0)
        {
            validSlots.Add(new int[] { emptyRow - 1, emptyColumn });
        }
        if (emptyRow != gridSize - 1)
        {
            validSlots.Add(new int[] { emptyRow + 1, emptyColumn });
        }

        var whileCount = 0;
        var randResult = new int[] { };
        do
        {
            randResult = validSlots[Random.Range(0, validSlots.Count)];
            whileCount++;
            if (whileCount == 100)
            {
                break;
            }
        }
        while (randResult.Equals(emptyXY));

        // Update new empty tile position values
        //emptyXY = (int[])randResult.Clone();
        //emptyRow = emptyXY[0];
        //emptyColumn = emptyXY[1];
        lastShuffledRow = randResult.First();
        lastShuffledColumn = randResult.Last();

        MoveTile(blocks[randResult.First(), randResult.Last()], randResult, shuffleMoveDelay);
    }

    private void MoveTile(int index, int[] tileGridPos, float speed)
    {
        Vector3 emptyPos = blockTransforms[15].position;
        Vector3 tilePos = blockTransforms[index].position;

        blockTransforms[index].DOMove(emptyPos, speed);
        blockTransforms[15].position = tilePos;

        emptyXY = tileGridPos;
        emptyRow = emptyXY[0];
        emptyColumn = emptyXY[1];
    }

    private void SetupBlocks()
    {
        blockPadding = blocksParent.GetComponent<GridLayoutGroup>().spacing.x;
        blocksParent.GetComponent<GridLayoutGroup>().enabled = false;

        blockSize = blocksParent.GetChild(0).GetComponent<RectTransform>().rect.width;
        halfBlockSize = blockSize * 0.5f;

        // Assign right graphics with shader and add blocks to collections.
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                var t = blocksParent.GetChild(gridSize * y + x);
                blockTransforms.Add(t);
                t.name = $"{gridSize * y + x}";
                blocks[y, x] = gridSize * y + x;
                // Material Property Blocks don't work on UI elements so doing
                // this the hacky way. Could be done outside of play mode.
                Material mat = Instantiate(t.GetComponent<Image>().material);
                mat.SetFloat("_Row", (gridSize - 1) - (float)y);
                mat.SetFloat("_Column", (float)x);
                t.GetComponent<Image>().material = mat;
            }
        }

        blocksParent.GetChild(15).gameObject.SetActive(false);

        //print(string.Join(" ", blocks.Cast<int>()));
        // random shuffle, doesn't work
        //var shuffleAmt = 10;
        //for (int i = 0; i < shuffleAmt; i++)
        //{

        //    var target = new Vector2Int(r(), r()); //blocks[r(), r()];
        //    var destination = new Vector2Int(r(), r());
        //    if (target.Equals(destination))
        //    {
        //        continue;
        //    }

        //    // position change in the model representation of the game state
        //    var targetCopy = blocks[target.x, target.y];
        //    var initialTargetPosition = blockTransforms[blocks[target.x, target.y]].position;
        //    blocks[target.x, target.y] = blocks[destination.x, destination.y];
        //    blocks[destination.x, destination.y] = targetCopy;

        //    // real position change between blocks
        //    // target -> destination, destination -> targetCopy
        //    blockTransforms[blocks[target.x, target.y]].position = blockTransforms[blocks[destination.x, destination.y]].position;
        //    blockTransforms[blocks[destination.x, destination.y]].position = initialTargetPosition;
        //}
        //// Generates random number inside grid
        //int r() => Random.Range(0, gridSize);

        //print(string.Join(" ", blocks.Cast<int>()));
    }

    /// <summary>
    /// Button command, basically OnTileInteracted, for gameplay shuffling.
    /// </summary>
    /// <param name="index">What block to move.</param>
    private void MoveBlockToNearbyEmpty(int index)
    {
        // todo early return if too far
    }
}
