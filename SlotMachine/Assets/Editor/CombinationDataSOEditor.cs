using UnityEngine;
using UnityEditor;
using Data.Combination; 

[CustomEditor(typeof(CombinationDataSO))]
public class CombinationDataSOEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        CombinationDataSO data = (CombinationDataSO)target;

        int rows = data.Rows;
        int cols = data.Columns;

        EditorGUILayout.LabelField("Combination Matrix (Toggle by Clicking)");
        for (int row = 0; row < rows; row++)
        {
            EditorGUILayout.BeginHorizontal();
            for (int col = 0; col < cols; col++)
            {
                bool currentValue = data.GetCombination(row, col);
                Color originalColor = GUI.backgroundColor;
                GUI.backgroundColor = currentValue ? Color.green : Color.red;
                if (GUILayout.Button("", GUILayout.Width(40), GUILayout.Height(25)))
                {
                    data.SetCombination(row, col, !currentValue);
                    EditorUtility.SetDirty(data);
                }
                GUI.backgroundColor = originalColor;
            }
            EditorGUILayout.EndHorizontal();
        }
    }
}