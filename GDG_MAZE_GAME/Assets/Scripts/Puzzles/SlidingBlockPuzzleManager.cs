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
    [SerializeField] GameStateManager gameStateManager;

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
    [Range(0.5f, 1f)]
    private float tileMovementDuration = 0.4f;
    /// <summary>
    /// How many percentage of tiles need to be in order for the clue to be awarded.
    /// Each tile represents for example 1/16 = 6.25 percent.
    /// </summary>
    private float requiredCompletionPercentage = 0.8f;

    [Header("Component References")]
    [SerializeField]
    private Transform blocksParent;

    // Values needed for positioning elements when they move.
    private float blockSize;
    private float halfBlockSize;
    private float blockPadding;

    /// <summary> Is both the delay and duration of pieces that are moved during the initial shuffle. </summary>
    [Header("Starting Shuffle Options")]
    [SerializeField]
    private float shuffleMoveDelay = 0.2f;
    /// <summary> How many times to shuffle the puzzle when it starts. </summary>
    [SerializeField]
    private int shuffleMovesAmount = 20;
    /// <summary>
    /// What was the last grid position that was clicked programmatically 
    /// during starting shuffle. Prevents moving the same piece back and forth.
    /// </summary>
    [SerializeField]
    private int[] lastMovedShufflePosition = { -1, -1 };
    /// <summary>
    /// What was the last tile index that was clicked programmatically 
    /// during starting shuffle.Prevents moving the same piece back and forth.
    /// </summary>
    [SerializeField]
    private int lastMovedShuffleTileIndex = -1;


    // State for the empty block which makes it easier for other blocks to compare their position to it.
    /// <summary> On what row the empty block is in? Zero indexed. </summary>
    [Header("State")]
    private int emptyRow = 3;
    /// <summary> On what column the empty block is in? Zero indexed. </summary>
    private int emptyColumn = 3;
    /// <summary> Where the empty piece is in the grid. </summary>
    private int[] emptyXY = { -1, -1 };
    private int[] prevEmptyXY = { -1, -1 };
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

    [Header("Testing")]
    [SerializeField]
    private Sprite spriteSwapTestSprite;

    private void Start()
    {
        if(gameStateManager == null)
        {
            Debug.Log("No Game State Manager assigned to 15-Tile Puzzle! SlidingBlockPuzzle/Scripts/SlidingBlockPuzzleManager object.");
        }

        SetupBlocks();
        BeginGame();
    }

    public void BeginGame()
    {
        for (int i = 0; i < shuffleMovesAmount; i++)
        {
            Invoke(nameof(ShuffleNearestUnvisited), i * shuffleMoveDelay * 1.1f);
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
            randResult = validSlots[UnityEngine.Random.Range(0, validSlots.Count)];
            whileCount++;
            if (whileCount == 100)
            {
                print("infinite loop");
                break;
            }
        }
        while (randResult[0] == prevEmptyXY[0] && randResult[1] == prevEmptyXY[1]);

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

        // update grid representation
        blocks[emptyRow, emptyColumn] = index;
        blocks[tileGridPos[0], tileGridPos[1]] = 15;

        // move tiles, one instantly and one with delay
        blockTransforms[index].DOMove(emptyPos, speed);
        blockTransforms[15].position = tilePos;

        // update empty position to be used later for checks
        prevEmptyXY = emptyXY;
        emptyXY = tileGridPos;
        emptyRow = emptyXY[0];
        emptyColumn = emptyXY[1];
    }

    private void SetupBlocks()
    {
        // Assign correct image first -Seb
        
        GameStateManager.Symbol slidingBlockSymbol = gameStateManager.doorAnswer[0];
        int symbolIndex = (int)slidingBlockSymbol;
        Sprite answerSprite = gameStateManager.totalClueImages[symbolIndex];
        Debug.LogFormat("Setting {0} as Sliding Puzzle Sprite", answerSprite.name);

        // Then continue Setup
        blockPadding = blocksParent.GetComponent<GridLayoutGroup>().spacing.x;
        blocksParent.GetComponent<GridLayoutGroup>().enabled = false;

        blockSize = blocksParent.GetChild(0).GetComponent<RectTransform>().rect.width;
        halfBlockSize = blockSize * 0.5f;

        // Assign right graphics with shader and add blocks to collections.
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                var tileNum = gridSize * y + x;
                var t = blocksParent.GetChild(tileNum);
                blockTransforms.Add(t);
                t.name = $"{tileNum}";
                blocks[y, x] = tileNum;
                // Material Property Blocks don't work on UI elements so doing
                // this the hacky way. Could be done outside of play mode.

                t.GetComponent<Image>().sprite = answerSprite; // GET RID OF THIS TO RETURN TO DEFAULT IMAGE -Seb

                Material mat = Instantiate(t.GetComponent<Image>().material);
                mat.SetFloat("_Row", (gridSize - 1) - (float)y);
                mat.SetFloat("_Column", (float)x);
                t.GetComponent<Image>().material = mat;

                var b = t.GetComponent<Button>();
                b.onClick.AddListener(() => MoveBlockToNearbyEmpty(tileNum));
            }
        }

        blocksParent.GetChild(15).gameObject.SetActive(false);
    }

    /// <summary>
    /// Button command, basically OnTileInteracted, for gameplay tile moving..
    /// </summary>
    /// <param name="index">What block to move. Same as block name.</param>
    private void MoveBlockToNearbyEmpty(int index)
    {
        // todo early return if too far
        print("trying to move tile " + index);

        var clickedTilePos = new int[] { -1, -1 };
        // figure out which gridPos we're in
        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (blocks[y, x] == index)
                {
                    clickedTilePos[0] = y;
                    clickedTilePos[1] = x;
                    break;
                }
            }
        }

        // for readability
        var column = 1;
        var row = 0;

        bool shouldMoveTile = false;

        // find nearby empty to switch positions with
        // brute force to see if it's any of the 4 blocks
        if (clickedTilePos[row] - 1 == emptyRow && clickedTilePos[column] == emptyColumn) // Empty is above?
        {
            shouldMoveTile = true;
        }
        else if (clickedTilePos[row] + 1 == emptyRow && clickedTilePos[column] == emptyColumn) // empty is right below
        {
            shouldMoveTile = true;
        }
        else if (clickedTilePos[column] - 1 == emptyColumn && clickedTilePos[row] == emptyRow) // empty on left side
        {
            shouldMoveTile = true;
        }
        else if (clickedTilePos[column] + 1 == emptyColumn && clickedTilePos[row] == emptyRow) // empty on right side
        {
            shouldMoveTile = true;
        }

        if (shouldMoveTile)
        {
            MoveTile(index, clickedTilePos, tileMovementDuration);
            // since this is a player initiated move, check for completion so that the clue can be added to journal
            if (CompletionPercentage() >= requiredCompletionPercentage)
            {
                AwardClue();
            }
        }
    }

    /// <summary>
    /// Completion percentage is calculated based on tiles in order in blocks[,].
    /// </summary>
    /// <returns> In order tiles divided by total number of tiles. </returns>
    private float CompletionPercentage()
    {
        // how many tiles the puzzle has, 
        var totalNumbers = gridSize * gridSize;
        //
        var tilesInOrderCounter = 0;
        // previous number
        var prevNum = -1;

        for (int y = 0; y < gridSize; y++)
        {
            for (int x = 0; x < gridSize; x++)
            {
                if (blocks[y, x] - 1 == prevNum)
                {
                    tilesInOrderCounter++;
                }
                prevNum = blocks[y, x];
            }
        }

        print("Completion %: " + (tilesInOrderCounter / (float)totalNumbers) + " Required: " + requiredCompletionPercentage);
        return tilesInOrderCounter / (float)totalNumbers;
    }

    private void AwardClue()
    {
        print("TODO: CLUE AWARDED!");
        gameStateManager.AddClueToJournal(0);
    }

    [ContextMenu("Test puzzle sprite swap")]
    private void TestSpriteSwap()
    {
        ChangePuzzleImage(spriteSwapTestSprite);
    }

    /// <summary>
    /// Can be called during runtime to change the image of the puzzle.
    /// </summary>
    /// <param name="sprite"> Image to change the puzzle image to. </param>
    public void ChangePuzzleImage(Sprite sprite)
    {
        Debug.Log("Changing Sliding Block Image to " + sprite.name);

        for (int i = 0; i < blockTransforms.Count; i++)
        {
            blockTransforms[i].GetComponent<Image>().sprite = sprite;
            //blockTransforms[i].GetComponent<Material>().mainTexture = sprite.texture;
        }
    }
}
