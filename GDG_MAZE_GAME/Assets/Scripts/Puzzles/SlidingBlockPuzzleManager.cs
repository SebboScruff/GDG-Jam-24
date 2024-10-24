using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

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

    [Header("Component References")]
    [SerializeField]
    private Transform blocksParent;

    // Values needed for positioning elements when they move.
    private float blockSize;
    private float halfBlockSize;
    private float blockPadding;


    // State for the empty block which makes it easier for other blocks to compare their position to it.
    /// <summary> On what row the empty block is in? Zero indexed. </summary>
    [Header("State")]
    private int emptyRow = 3;
    /// <summary> On what column the empty block is in? Zero indexed. </summary>
    private int emptyColumn = 3;

    void Start()
    {
        SetupBlocks();
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
    /// Button command for gameplay or simulates gameplay when shuffling.
    /// </summary>
    /// <param name="index">What block to move.</param>
    /// <param name="instantly">Should the movement be instant or animated.</param>
    private void MoveBlockToNearbyEmpty(int index, bool instantly)
    {
        // todo early return if too far
    }
}
