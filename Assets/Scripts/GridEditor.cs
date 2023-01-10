#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;


// Modifies the inspector for the Grid class
// Adds a button to reset the high score
[CustomEditor(typeof(Grid))]
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
#endif