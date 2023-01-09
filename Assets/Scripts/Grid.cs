using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    // Text Fields
    [Header("Text Fields")]
    [SerializeField] TMPro.TMP_Text[] cellTexts;
    [SerializeField] TMPro.TMP_Text scoreText;
    [SerializeField] TMPro.TMP_Text highScoreText;

    // Score Fields
    static uint score;
    static uint highScore;

    // Top left is 0, 0
    public Cell[,] grid;

    // Events
    public System.Action OnMove;
    public System.Action BeforeMove;

    // Constants
    public const float COL_CHANGE_FAC = 125f;
    public const int GRID_SIZE = 4;

    // Debug
    public bool debugDisplay;
    public Vector2Int debugCellPos;

    // Game Vars
    static int movesMade = 0;
    enum direction { left, up, right, down };

    // Game Over
    bool gameOver = false;
    public GameObject GameOverScreen;

    void Start() => RestartGame();

    /// <summary>
    /// Resets all variables and Grid
    /// </summary>
    public void RestartGame()
    {
        // Reset Variables
        movesMade = 0;
        gameOver = false;
        score = 0;
        highScore = uint.Parse(HighScoreIO.ReadHighScore());
        GameOverScreen.SetActive(false);
        grid = new Cell[GRID_SIZE, GRID_SIZE];

        // Reset Text
        scoreText.text = score.ToString();
        highScoreText.text = HighScoreIO.ReadHighScore();

        // Reset Grid
        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                grid[i, j] = new Cell(new Vector2Int(j, i), cellTexts[i * GRID_SIZE + j]);

                if (i == 0)
                    grid[i, j].neighbors.up = false;
                if (i == GRID_SIZE - 1)
                    grid[i, j].neighbors.down = false;
                if (j == 0)
                    grid[i, j].neighbors.left = false;
                if (j == GRID_SIZE - 1)
                    grid[i, j].neighbors.right = false;
            }
        }

        // Spawn a number
        CreateNum();

        // Debug
        if (debugDisplay)
            FindObjectOfType<DebugCell>().Init();

        // Init Grid Color
        FindObjectOfType<GridColor>().Init();
    }

    /// <summary>
    /// Creates a new number in a random cell
    /// Uses Recursion to find empty cell
    /// Set corresponding neighbor to false
    /// </summary>
    void CreateNum()
    {
        // Randomly selects 2 or 4 and a random cell
        uint num = (uint)Random.Range(1, 3) * 2;
        int randRow = Random.Range(0, GRID_SIZE);
        int randCol = Random.Range(0, GRID_SIZE);

        // If cell is occupied, try again
        if (grid[randRow, randCol].num != 0)
        {
            CreateNum();
            return;
        }

        // Sets that random cell's num and text color (to debug)
        grid[randRow, randCol].num = num;
        grid[randRow, randCol].cellText.color = Color.cyan;

        // Set neighbors corresponding neighbor to false
        if (randRow > 0)
            grid[randRow - 1, randCol].neighbors.down = false;
        if (randRow < 3)
            grid[randRow + 1, randCol].neighbors.up = false;
        if (randCol > 0)
            grid[randRow, randCol - 1].neighbors.right = false;
        if (randCol < 3)
            grid[randRow, randCol + 1].neighbors.left = false;

        // Triggers OnMove event
        OnMove();
    }

    /// <summary>
    /// Moves the num from cell a to b
    /// Set corresponding neighbor to true
    /// Set old neigbors to false
    /// Uses recursion to slide num to the end
    /// </summary>
    /// <param name="a">Old Cell</param>
    /// <param name="b">New Cell</param>
    Vector2Int SwapNum(Cell a, Cell b, direction dir)
    {
        // Move has been made
        movesMade++;

        // Set b's num to a's num
        Vector2Int pos = b.gridPos;
        b.num = a.num;

        // Clear a's num
        a.num = 0;

        // Set a neighbors corresponding neighbor to true
        // for each check first check if in bounds

        if (grid[a.gridPos.y, a.gridPos.x].gridPos.y > 0)
            grid[a.gridPos.y - 1, a.gridPos.x].neighbors.down = true;
        if (grid[a.gridPos.y, a.gridPos.x].gridPos.y < 3)
            grid[a.gridPos.y + 1, a.gridPos.x].neighbors.up = true;
        if (grid[a.gridPos.y, a.gridPos.x].gridPos.x > 0)
            grid[a.gridPos.y, a.gridPos.x - 1].neighbors.right = true;
        if (grid[a.gridPos.y, a.gridPos.x].gridPos.x < 3)
            grid[a.gridPos.y, a.gridPos.x + 1].neighbors.left = true;

        // Set b neighbors corresponding neighbor to false
        if (grid[b.gridPos.y, b.gridPos.x].gridPos.y > 0)
            grid[b.gridPos.y - 1, b.gridPos.x].neighbors.down = false;
        if (grid[b.gridPos.y, b.gridPos.x].gridPos.y < 3)
            grid[b.gridPos.y + 1, b.gridPos.x].neighbors.up = false;
        if (grid[b.gridPos.y, b.gridPos.x].gridPos.x > 0)
            grid[b.gridPos.y, b.gridPos.x - 1].neighbors.right = false;
        if (grid[b.gridPos.y, b.gridPos.x].gridPos.x < 3)
            grid[b.gridPos.y, b.gridPos.x + 1].neighbors.left = false;

        // Call function again if b's neighbor is empty
        // Recursively slides num to the end
        if (dir == direction.up && b.neighbors.up)
            return SwapNum(b, grid[b.gridPos.y - 1, b.gridPos.x], direction.up);

        if (dir == direction.down && b.neighbors.down)
            return SwapNum(b, grid[b.gridPos.y + 1, b.gridPos.x], direction.down);

        if (dir == direction.left && b.neighbors.left)
            return SwapNum(b, grid[b.gridPos.y, b.gridPos.x - 1], direction.left);

        if (dir == direction.right && b.neighbors.right)
            return SwapNum(b, grid[b.gridPos.y, b.gridPos.x + 1], direction.right);

        // Returns the new position of the num
        return pos;
    }

    /// <summary>
    /// Erases old num
    /// Combines num to new Cell
    /// Set old neighbors to false
    /// </summary>
    /// <param name="a">Old Cell</param>
    /// <param name="b">New Cell</param>
    void CombineNum(Cell a, Cell b)
    {
        // Move has been made
        movesMade++;

        // Combine num by doubling
        b.num *= 2;

        // Set combined to true so num isn't combined again
        b.combined = true;
        a.num = 0;

        // Add to score
        score += b.num;

        // Set a neighbors corresponding neighbor to false
        if (grid[a.gridPos.y, a.gridPos.x].gridPos.y > 0)
            grid[a.gridPos.y - 1, a.gridPos.x].neighbors.down = true;
        if (grid[a.gridPos.y, a.gridPos.x].gridPos.y < 3)
            grid[a.gridPos.y + 1, a.gridPos.x].neighbors.up = true;
        if (grid[a.gridPos.y, a.gridPos.x].gridPos.x > 0)
            grid[a.gridPos.y, a.gridPos.x - 1].neighbors.right = true;
        if (grid[a.gridPos.y, a.gridPos.x].gridPos.x < 3)
            grid[a.gridPos.y, a.gridPos.x + 1].neighbors.left = true;
    }

    /// <summary>
    /// Checks if the given V2 coordinates are in bounds
    /// </summary>
    /// <param name="pos">Co-ordinate to be checked</param>
    /// <returns>Returns true if in bounds</returns>
    bool CheckInBounds(Vector2Int pos)
    {
        if (pos.x < 0 || pos.x > 3 || pos.y < 0 || pos.y > 3)
            return false;
        return true;
    }

    /// <summary>
    /// Debugs information of cell at given coordinates
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    void DebugC(int i, int j)
    {
        SpriteRenderer sr = grid[i, j].cellText.transform.parent.GetComponent<SpriteRenderer>();
        // sr.color = Color.gray;

        if (new Vector2Int(i, j) == debugCellPos)
        {
            Debug.Log("//////////////");
            Debug.Log("Selected Cell");
            Debug.Log("Up: " + grid[i, j].neighbors.up);
            Debug.Log("Down: " + grid[i, j].neighbors.down);
            Debug.Log("Left: " + grid[i, j].neighbors.left);
            Debug.Log("Right: " + grid[i, j].neighbors.right);
            Debug.Log("Num: " + grid[i, j].num);
            Debug.Log("Pos: (" + grid[i, j].gridPos.y + ", " + grid[i, j].gridPos.x + ")");
            Debug.Log("//////////////");
        }
    }

    /// <summary>
    /// Checks if cell is valid
    /// Attempts to move cell
    /// Attempts to combine cell
    /// </summary>
    /// <param name="i"></param>
    /// <param name="j"></param>
    /// <param name="iD"></param>
    /// <param name="jD"></param>
    /// <param name="dir"></param>
    void CheckCell(int i, int j, int iD, int jD, direction dir)
    {
        // If checking empty cell skip check
        if (grid[i, j].num == 0)
            return;

        // Debug information
        // DebugC(i, j);

        // Check if next cell is in bounds
        if (!CheckInBounds(new Vector2Int(i + iD, j + jD)))
            return;

        // Get new position of num
        Vector2Int newPos = new Vector2Int(j, i);
        uint curr_value = grid[i, j].num;

        // Check if next cell is empty and move num
        if (grid[i + iD, j + jD].num == 0)
            newPos = SwapNum(grid[i, j], grid[i + iD, j + jD], dir);

        if (!CheckInBounds(new Vector2Int(newPos.y + iD, newPos.x + jD)))
            return;

        // Store the combined bool value for both cells in temp var
        bool aCombined = grid[newPos.y, newPos.x].combined;
        bool bCombined = grid[newPos.y + iD, newPos.x + jD].combined;

        // Check if next cell is the same value and combine if both haven't been combined
        if (grid[newPos.y + iD, newPos.x + jD].num == curr_value && !aCombined && !bCombined)
            CombineNum(grid[newPos.y, newPos.x], grid[newPos.y + iD, newPos.x + jD]);
    }

    /// <summary>
    /// Loops thought entire grid
    /// Checks if there are any empty cells
    /// Checks if any valid move is left
    /// </summary>
    void CheckGameOver()
    {
        for (int i = 0; i < 4; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                // If there is an empty num, early return since game is not over
                if (grid[i, j].num == 0)
                    return;

                // Checks cell above to see if identical
                // If identical, early return since game is not over (valid move can be made)
                if (i > 0)
                {
                    if (grid[i, j].num == grid[i - 1, j].num)
                        return;
                }

                // Same but below
                if (i < 3)
                {
                    if (grid[i, j].num == grid[i + 1, j].num)
                        return;
                }

                // Same but left
                if (j > 0)
                {
                    if (grid[i, j].num == grid[i, j - 1].num)
                        return;
                }

                // Same but right
                if (j < 3)
                {
                    if (grid[i, j].num == grid[i, j + 1].num)
                        return;
                }
            }
        }

        // If no valid moves are left, game is over
        this.gameOver = true;
    }

    /// <summary>
    /// Main Game Loop
    /// Checks for input
    /// Loops through Grid in particular direction
    /// </summary>
    void Update()
    {
        // If Game over, reveal screen skip rest of game loop
        if (gameOver)
        {
            GameOverScreen.SetActive(true);
            return;
        }

        // If current score is higher than high score, update high score
        if (score > highScore)
        {
            highScore = score;
            HighScoreIO.SaveHighScore(score);
        }

        // Reset moves made as cells have not been moved yet
        movesMade = 0;

        // Update text
        scoreText.text = score.ToString();
        highScoreText.text = highScore.ToString();

        // Check game over
        CheckGameOver();

        // Order that game loops through grid is important
        // Defines which cells are updated first 
        // (when moving up, topmost cell should move first so that lower cells have more space to move)
        // Order also determines which cells are combined first (when moving up, topmost cell should combine first)

        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // Reset combined bools
            BeforeMove();
            Debug.Log("Up");
            // loop through grid from top left and push cell up if up neighbor is true
            for (int i = 0; i < GRID_SIZE; i++)
            {
                for (int j = 0; j < GRID_SIZE; j++)
                {
                    CheckCell(i, j, -1, 0, direction.up);
                }
            }

            // Only creates new number if cells have been moved or combined in this turn
            if (movesMade > 0)
                CreateNum();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            BeforeMove();
            Debug.Log("Down");
            // loop through grid from bottom left and push cell down if down neighbor is true
            for (int i = 3; i >= 0; i--)
            {
                for (int j = 0; j < 4; j++)
                {
                    CheckCell(i, j, 1, 0, direction.down);
                }
            }

            if (movesMade > 0)
                CreateNum();
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            BeforeMove();
            Debug.Log("Left");

            // loop through grid from top left and push cell left if left neighbor is true
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    CheckCell(i, j, 0, -1, direction.left);
                }
            }

            if (movesMade > 0)
                CreateNum();
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            BeforeMove();
            Debug.Log("Right");

            // loop through grid from top right and push cell right if right neighbor is true
            for (int i = 0; i < 4; i++)
            {
                for (int j = 3; j >= 0; j--)
                {
                    CheckCell(i, j, 0, 1, direction.right);
                }
            }

            if (movesMade > 0)
                CreateNum();
        }
    }

    // Just for me, Resets high score to 0 before build
    public void ResetHighScore() => HighScoreIO.SaveHighScore(0);
}

