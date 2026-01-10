using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(VisualizationManager))]
public class VisualizationEditor : Editor
{
    public override void OnInspectorGUI()
    {
        VisualizationManager m = (VisualizationManager)target;

        GUILayout.Space(10);
        GUILayout.Label("ANALYTICS DASHBOARD", EditorStyles.boldLabel);

        GUI.backgroundColor = m.isLoaded ? Color.white : Color.green;
        if (GUILayout.Button("1. Download & Parse Data", GUILayout.Height(30))) m.RefreshData();
        GUI.backgroundColor = Color.white;

        if (!m.isLoaded) return;

        GUILayout.Space(10);
        GUILayout.Label("VIEW SETTINGS", EditorStyles.boldLabel);
        GUILayout.BeginVertical("box");
        m.visualizationMode = (VisualizationManager.VisMode)EditorGUILayout.EnumPopup("Mode:", m.visualizationMode);

        if (m.visualizationMode == VisualizationManager.VisMode.Heatmap)
        {
            m.gridSize = EditorGUILayout.Slider("Grid Size:", m.gridSize, 1f, 20f);
            m.gridAlpha = EditorGUILayout.Slider("Density Opacity:", m.gridAlpha, 0.1f, 1f);
        }
        GUILayout.EndVertical();

        GUILayout.Space(10);
        GUILayout.Label("DATA FILTERS", EditorStyles.boldLabel);
        m.filterPlayerID = EditorGUILayout.IntField("Player ID (-1 All)", m.filterPlayerID);
        m.filterSessionID = EditorGUILayout.IntField("Session ID (-1 All)", m.filterSessionID);
        m.filterRunID = EditorGUILayout.IntField("Run ID (-1 All)", m.filterRunID);
        m.winFilter = (VisualizationManager.WinStateFilter)EditorGUILayout.EnumPopup("Win Status:", m.winFilter);
        m.restartFilter = (VisualizationManager.RestartFilter)EditorGUILayout.EnumPopup("Restarts:", m.restartFilter);

        GUILayout.Label("Exclusions", EditorStyles.miniBoldLabel);
        m.onlyRunsWithNoDeaths = EditorGUILayout.Toggle("Immortal (No Death)", m.onlyRunsWithNoDeaths);
        m.onlyRunsWithNoDamage = EditorGUILayout.Toggle("Perfect (No Hit)", m.onlyRunsWithNoDamage);
        m.onlyRunsWithNoKills = EditorGUILayout.Toggle("Pacifist (No Kills)", m.onlyRunsWithNoKills);

        GUILayout.Space(10);
        GUILayout.Label("EVENT LAYERS", EditorStyles.boldLabel);

        DrawToggleColor(ref m.showPath, ref m.pathColor, "Path / Movement");
        DrawToggleColor(ref m.showDeaths, ref m.deathColor, "Deaths");
        DrawToggleColor(ref m.showKills, ref m.killColor, "Enemy Kills");
        DrawToggleColor(ref m.showJumps, ref m.jumpColor, "Jumps");
        DrawToggleColor(ref m.showBoxes, ref m.boxColor, "Boxes Broken");
        DrawToggleColor(ref m.showButtons, ref m.buttonColor, "Buttons Pressed");
        DrawToggleColor(ref m.showKeys, ref m.keyColor, "Keys Collected");
        DrawToggleColor(ref m.showHeals, ref m.healColor, "Items / Heals");
        DrawToggleColor(ref m.showCheckpoints, ref m.checkpointColor, "Checkpoints");
        DrawToggleColor(ref m.showPlayerHits, ref m.hitColor, "Damage Taken");

        GUILayout.Space(15);
        if (GUILayout.Button("Apply Filters / Refresh View", GUILayout.Height(40)))
        {
            m.ApplyFilters();
            SceneView.RepaintAll();
        }

        GUILayout.Space(20);
        GUILayout.Label("SCENE CONTROL", EditorStyles.boldLabel);
        DrawLevelToggle();
    }

    void DrawToggleColor(ref bool toggle, ref Color col, string label)
    {
        GUILayout.BeginHorizontal();
        toggle = EditorGUILayout.Toggle(toggle, GUILayout.Width(20));
        col = EditorGUILayout.ColorField(col, GUILayout.Width(50));
        GUILayout.Label(label);
        GUILayout.EndHorizontal();
    }

    void DrawLevelToggle()
    {
        GameObject examples = FindGameObjectInScene("Examples");
        GameObject level = FindGameObjectInScene("ExampleLevel"); 
        GameObject canvas = FindGameObjectInScene("Canvas");


        if (examples == null && level == null && canvas == null) return;

        bool currentlyHidden = (examples != null && !examples.activeSelf) || (level != null && !level.activeSelf) || (canvas != null && !canvas.activeSelf);

        GUI.backgroundColor = currentlyHidden ? Color.cyan : Color.white;
        string btnText = currentlyHidden ? "Show Level Geometry (Un-Hide)" : "Hide Level Geometry (Focus Data)";

        if (GUILayout.Button(btnText, GUILayout.Height(25)))
        {
            if (examples) { Undo.RecordObject(examples, "Toggle Vis"); examples.SetActive(currentlyHidden); }
            if (level) { Undo.RecordObject(level, "Toggle Vis"); level.SetActive(currentlyHidden); }
            if (canvas) { Undo.RecordObject(canvas, "Toggle Vis"); canvas.SetActive(currentlyHidden); }
        }
        GUI.backgroundColor = Color.white;
    }

    GameObject FindGameObjectInScene(string name)
    {
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            if (go.hideFlags != HideFlags.None) continue;
            if (go.name == name) return go;
        }
        return null;
    }
}