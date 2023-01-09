using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridColor : MonoBehaviour
{
    public Color[] colors;
    List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    // Start is called before the first frame update
    Grid grid;

    /// <summary>
    /// Initialises the list of sprite renderes used to update color of cell
    /// </summary>
    public void Init()
    {
        grid = FindObjectOfType<Grid>();
        grid.OnMove += UpdateColors;
        foreach (var cell in grid.grid)
        {
            var spr = cell.cellText.transform.parent.GetComponent<SpriteRenderer>();
            sprites.Add(spr);
            spr.color = colors[0];
        }
    }

    /// <summary>
    /// Updates color of each cell once move has been made
    /// </summary>
    void UpdateColors()
    {
        for (int i = 0; i < Grid.GRID_SIZE; i++)
        {
            for (int j = 0; j < Grid.GRID_SIZE; j++)
            {
                // Gets the cell and its sprite renderer
                var spr = sprites[i * Grid.GRID_SIZE + j];
                var cell = grid.grid[i, j];

                // Checks the number, if its 0 sets color to empty color
                // Otherwise gets log base 2 of the number and uses that as the index
                if (cell.num == 0)
                    spr.color = colors[0];
                else
                    spr.color = colors[Mathf.Min((int)Mathf.Log(cell.num, 2), colors.Length - 1)];
            }
        }
    }
}
