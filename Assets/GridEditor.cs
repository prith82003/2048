using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Grid))]

// Modifies the inspector for the Grid class
// Adds a button to reset the high score
public class GridEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        Grid grid = (Grid)target;

        if (GUILayout.Button("Reset High Score"))
        {
            grid.ResetHighScore();
        }
    }
}
