using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class NeighborStats
{
    // True -> Spot is free
    // False -> Spot is occupied
    public bool left;
    public bool up;
    public bool right;
    public bool down;

    public NeighborStats()
    {
        this.left = true;
        this.up = true;
        this.right = true;
        this.down = true;
    }

    // Constructor
    public NeighborStats(bool left, bool up, bool right, bool down)
    {
        this.left = left;
        this.up = up;
        this.right = right;
        this.down = down;
    }
}

[System.Serializable]
public class Cell
{
    public TMP_Text cellText;
    public NeighborStats neighbors;
    public uint num;
    public Vector2Int gridPos;
    public bool combined;

    public Cell()
    {
        this.cellText = null;
        this.neighbors = new NeighborStats();
        this.num = 0;
    }

    /// <summary>
    /// Constructor initialises variables
    /// Joins events to trigger functions on move
    /// </summary>
    /// <param name="pos"></param>
    /// <param name="cellText"></param>
    public Cell(Vector2Int pos, TMP_Text cellText)
    {
        this.gridPos = pos;
        this.cellText = cellText;
        this.neighbors = new NeighborStats();
        this.num = 0;
        this.cellText.text = "";

        GameObject.FindObjectOfType<Grid>().OnMove += ResetCombined;
        GameObject.FindObjectOfType<Grid>().OnMove += SetNum;
        GameObject.FindObjectOfType<Grid>().BeforeMove += ResetColor;
    }

    // Reset combined before turn so that cells can combine again
    void ResetCombined() => this.combined = false;

    // Set the number of the cell in the textbox to reflect the value of the cell
    void SetNum()
    {
        if (num == 0)
            cellText.text = "";
        else
            cellText.text = num.ToString();
    }

    // Reset the color of the cellText to black (for debugging purposes)
    void ResetColor() => cellText.color = Color.black;
}
