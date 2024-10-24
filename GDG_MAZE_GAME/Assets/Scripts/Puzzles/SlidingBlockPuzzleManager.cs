using UnityEngine;
using UnityEngine.UI;

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

    [Header("Component References")]
    [SerializeField]
    private Transform blocksParent;

    // Values needed for positioning elements when they move.
    private float blockSize;
    private float halfBlockSize;
    private float blockPadding;


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

        // Assign right graphics with shader.
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                var t = blocksParent.GetChild(gridSize*y +x);
                t.name = $"{gridSize * y + x}";
                // Material Property Blocks don't work on UI elements so doing
                // this the hacky way. Could be done outside of play mode.
                Material mat = Instantiate(t.GetComponent<Image>().material);
                mat.SetFloat("_Row", (gridSize - 1) - (float)y);
                mat.SetFloat("_Column", (float)x);
                t.GetComponent<Image>().material = mat;
            }
        }

        blocksParent.GetChild(15).gameObject.SetActive(false);
    }
}
