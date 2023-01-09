using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugCell : MonoBehaviour
{
    class DebugCellData
    {
        public Cell cell;
        public SpriteRenderer upTri;
        public SpriteRenderer downTri;
        public SpriteRenderer leftTri;
        public SpriteRenderer rightTri;


        /// <summary>
        /// Spawn Debug triangles 
        /// Set position, rotation and color
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="debugTri"></param>
        /// <param name="cell"></param>
        public DebugCellData(Vector2 pos, GameObject debugTri, Cell cell)
        {
            this.cell = cell;

            upTri = initTri(debugTri, pos, 0, 0.66f, 0);
            downTri = initTri(debugTri, pos, 0, -0.66f, 180);
            leftTri = initTri(debugTri, pos, 0.66f, 0, 90);
            rightTri = initTri(debugTri, pos, -0.66f, 0, 270);
        }

        /// <summary>
        /// Creates triangle based on information
        /// </summary>
        /// <param name="debugTri">Object to Spawn</param>
        /// <param name="pos">Position to spawn triangle</param>
        /// <param name="xD">X Offset</param>
        /// <param name="yD">Y Offset</param>
        /// <param name="angle">Angle to rotate triangle</param>
        /// <returns></returns>
        SpriteRenderer initTri(GameObject debugTri, Vector2 pos, float xD, float yD, int angle)
        {
            GameObject tri = Instantiate(debugTri, pos, Quaternion.identity);
            tri.transform.position = new Vector3(tri.transform.position.x + xD, tri.transform.position.y + yD, 0);
            var triSpr = tri.GetComponent<SpriteRenderer>();
            tri.transform.eulerAngles = new Vector3(0, 0, 180);
            triSpr.color = Color.green;
            triSpr.sortingOrder = 200;
            return triSpr;
        }
    }
    // Start is called before the first frame update

    public GameObject debugTri;
    List<DebugCellData> debugCells = new List<DebugCellData>();
    Grid grid;

    /// <summary>
    /// Attaches debug update to onMove Event
    /// Initialises debug cells
    /// Calls first debug update
    /// </summary>
    public void Init()
    {
        grid = GameObject.FindObjectOfType<Grid>();
        grid.OnMove += DebugUpdate;
        InitDebug();
        DebugUpdate();
    }

    /// <summary>
    /// Initialises Debug data for each cell
    /// </summary>
    void InitDebug()
    {
        var grid = FindObjectOfType<Grid>();
        foreach (var cell in grid.grid)
        {
            Vector2 cellPos = cell.cellText.transform.parent.position;
            DebugCellData debugCell = new DebugCellData(cellPos, debugTri, cell);
            debugCells.Add(debugCell);
            cell.cellText.transform.parent.GetChild(1).gameObject.SetActive(true);
            cell.cellText.transform.parent.GetChild(1).GetComponent<TMPro.TMP_Text>().text = cell.gridPos.ToString();
        }
    }

    /// <summary>
    /// Sets triangle color to green or red
    /// Depending on if cell has a neighbor in that direction
    /// </summary>
    void DebugUpdate()
    {
        foreach (var cell in debugCells)
        {
            cell.upTri.color = cell.cell.neighbors.up ? Color.green : Color.red;
            cell.downTri.color = cell.cell.neighbors.down ? Color.green : Color.red;
            cell.leftTri.color = cell.cell.neighbors.left ? Color.green : Color.red;
            cell.rightTri.color = cell.cell.neighbors.right ? Color.green : Color.red;
        }
    }
}
