using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Globalization;
using System.Reflection;

public class VisualizationManager : MonoBehaviour
{
    private string getDataURL = "https://citmalumnes.upc.es/~marcac6/game_analytics/get_data.php";

    public AnalyticsData data;
    public bool isLoaded = false;

    public enum WinStateFilter { All, WonOnly, LostOnly }
    public enum RestartFilter { All, NormalRunOnly, RestartsOnly }
    public enum VisMode { ExactPoints, Heatmap }

    [Header("Visualization Mode")]
    public VisMode visualizationMode = VisMode.ExactPoints;
    [Range(1f, 10f)] public float gridSize = 2.0f;
    [Range(0.1f, 1f)] public float gridAlpha = 0.8f;

    [Header("Color Palette")]
    public Color pathColor = Color.green;
    public Color deathColor = Color.red;
    public Color killColor = Color.black;
    public Color jumpColor = Color.yellow;
    public Color boxColor = new Color(0.6f, 0.3f, 0f);
    public Color buttonColor = Color.magenta;
    public Color keyColor = new Color(1f, 0.5f, 0f);
    public Color healColor = Color.cyan;
    public Color checkpointColor = Color.blue;
    public Color hitColor = new Color(1f, 0f, 0.5f);

    [HideInInspector] public int filterPlayerID = -1;
    [HideInInspector] public int filterSessionID = -1;
    [HideInInspector] public int filterRunID = -1;
    [HideInInspector] public WinStateFilter winFilter = WinStateFilter.All;
    [HideInInspector] public RestartFilter restartFilter = RestartFilter.All;

    [HideInInspector] public bool onlyRunsWithNoDeaths = false;
    [HideInInspector] public bool onlyRunsWithNoDamage = false;
    [HideInInspector] public bool onlyRunsWithNoKills = false;

    [HideInInspector] public bool showPath = true;
    [HideInInspector] public bool showDeaths = true;
    [HideInInspector] public bool showKills = true;
    [HideInInspector] public bool showJumps = true;
    [HideInInspector] public bool showBoxes = true;
    [HideInInspector] public bool showButtons = true;
    [HideInInspector] public bool showKeys = true;
    [HideInInspector] public bool showHeals = true;
    [HideInInspector] public bool showCheckpoints = true;
    [HideInInspector] public bool showPlayerHits = true;

    private HashSet<int> validRunIDs = new HashSet<int>();
    private Dictionary<int, int> runToPlayerMap = new Dictionary<int, int>();
    private Dictionary<int, int> runToSessionMap = new Dictionary<int, int>();

    public void RefreshData() { StartCoroutine(DownloadRoutine()); }

    IEnumerator DownloadRoutine()
    {
        Debug.Log("Downloading...");
        using (UnityWebRequest www = UnityWebRequest.Get(getDataURL))
        {
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success)
            {
                try
                {
                    data = JsonUtility.FromJson<AnalyticsData>(www.downloadHandler.text);
                    isLoaded = true;
                    BuildMaps();
                    ApplyFilters();
                    Debug.Log($"Loaded. Runs: {data.gameRuns.Count}");
                }
                catch (System.Exception e) { Debug.LogError($"JSON Error: {e.Message}"); }
            }
            else Debug.LogError($"Net Error: {www.error}");
        }
    }

    void BuildMaps()
    {
        runToPlayerMap.Clear();
        runToSessionMap.Clear();
        Dictionary<int, int> sessionPlayer = new Dictionary<int, int>();

        foreach (var sess in data.gameSessions)
            if (int.TryParse(sess.Session_id, out int sID) && int.TryParse(sess.Player_id, out int pID))
                if (!sessionPlayer.ContainsKey(sID)) sessionPlayer.Add(sID, pID);

        foreach (var run in data.gameRuns)
            if (int.TryParse(run.GameRun_ID, out int rID) && int.TryParse(run.GameSession_ID, out int sID))
            {
                runToSessionMap[rID] = sID;
                if (sessionPlayer.ContainsKey(sID)) runToPlayerMap[rID] = sessionPlayer[sID];
                else runToPlayerMap[rID] = -1;
            }
    }

    public void ApplyFilters()
    {
        if (!isLoaded) return;
        validRunIDs.Clear();

        HashSet<int> runsWithDeaths = new HashSet<int>();
        foreach (var d in data.deaths) if (int.TryParse(d.GameRun_ID, out int id)) runsWithDeaths.Add(id);

        HashSet<int> runsWithDamage = new HashSet<int>();
        foreach (var h in data.playerHits) if (int.TryParse(h.GameRun_ID, out int id)) runsWithDamage.Add(id);

        HashSet<int> runsWithKills = new HashSet<int>();
        foreach (var k in data.kills) if (int.TryParse(k.GameRun_ID, out int id)) runsWithKills.Add(id);

        foreach (var run in data.gameRuns)
        {
            if (!int.TryParse(run.GameRun_ID, out int rID)) continue;

            int sID = runToSessionMap.ContainsKey(rID) ? runToSessionMap[rID] : -1;
            int pID = runToPlayerMap.ContainsKey(rID) ? runToPlayerMap[rID] : -1;

            if (filterPlayerID != -1 && pID != filterPlayerID) continue;
            if (filterSessionID != -1 && sID != filterSessionID) continue;
            if (filterRunID != -1 && rID != filterRunID) continue;

            bool isWin = (run.Win == "1");
            if (winFilter == WinStateFilter.WonOnly && !isWin) continue;
            if (winFilter == WinStateFilter.LostOnly && isWin) continue;

            bool isRestart = (run.IsRestart == "1");
            if (restartFilter == RestartFilter.NormalRunOnly && isRestart) continue;
            if (restartFilter == RestartFilter.RestartsOnly && !isRestart) continue;

            if (onlyRunsWithNoDeaths && runsWithDeaths.Contains(rID)) continue;
            if (onlyRunsWithNoDamage && runsWithDamage.Contains(rID)) continue;
            if (onlyRunsWithNoKills && runsWithKills.Contains(rID)) continue;

            validRunIDs.Add(rID);
        }
    }

    void OnDrawGizmos()
    {
        if (!isLoaded || data == null) return;

        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.2f);
        Gizmos.DrawWireCube(new Vector3(50, 0, 50), new Vector3(100, 0, 100));

        if (visualizationMode == VisMode.Heatmap)
        {
            if (showPath) DrawHeatmapLayer(data.positions, pathColor, 0.1f);
            if (showDeaths) DrawHeatmapLayer(data.deaths, deathColor, 0.6f);
            if (showKills) DrawHeatmapLayer(data.kills, killColor, 0.6f);
            if (showJumps) DrawHeatmapLayer(data.jumps, jumpColor, 0.6f);
            if (showBoxes) DrawHeatmapLayer(data.breakBoxes, boxColor, 0.6f);
            if (showButtons) DrawHeatmapLayer(data.buttons, buttonColor, 0.6f);
            if (showKeys) DrawHeatmapLayer(data.keys, keyColor, 0.6f);
            if (showHeals) DrawHeatmapLayer(data.heals, healColor, 0.6f);
            if (showCheckpoints) DrawHeatmapLayer(data.checkpoints, checkpointColor, 0.6f);
            if (showPlayerHits) DrawHeatmapLayer(data.playerHits, hitColor, 0.6f);
        }
        else
        {
            if (showPath)
            {
                Gizmos.color = pathColor;
                for (int i = 0; i < data.positions.Count; i += 3)
                    if (IsVisible(data.positions[i].GameRun_ID))
                        Gizmos.DrawSphere(ParseVec(data.positions[i].Pos_x, data.positions[i].Pos_y, data.positions[i].Pos_z), 0.15f);
            }

            DrawEventList(data.deaths, showDeaths, deathColor, true);
            DrawEventList(data.kills, showKills, killColor, true);
            DrawEventList(data.playerHits, showPlayerHits, hitColor, false);
            DrawEventList(data.jumps, showJumps, jumpColor, false);
            DrawEventList(data.breakBoxes, showBoxes, boxColor, true);
            DrawEventList(data.buttons, showButtons, buttonColor, true);
            DrawEventList(data.keys, showKeys, keyColor, true);
            DrawEventList(data.heals, showHeals, healColor, true);
            DrawEventList(data.checkpoints, showCheckpoints, checkpointColor, true);
        }
    }

    void DrawHeatmapLayer<T>(List<T> list, Color baseColor, float alphaMultiplier)
    {
        Dictionary<Vector3Int, int> localMap = new Dictionary<Vector3Int, int>();
        int maxDensity = 1;

        foreach (var item in list)
        {
            string rID = (string)item.GetType().GetField("GameRun_ID").GetValue(item);
            if (!IsVisible(rID)) continue;

            string sx = (string)item.GetType().GetField("Pos_x").GetValue(item);
            string sy = (string)item.GetType().GetField("Pos_y").GetValue(item);
            string sz = (string)item.GetType().GetField("Pos_z").GetValue(item);

            Vector3 pos = ParseVec(sx, sy, sz);

            int gx = Mathf.FloorToInt(pos.x / gridSize);
            int gy = Mathf.FloorToInt(pos.y / gridSize);
            int gz = Mathf.FloorToInt(pos.z / gridSize);
            Vector3Int key = new Vector3Int(gx, gy, gz);

            if (localMap.ContainsKey(key)) localMap[key]++;
            else localMap[key] = 1;

            if (localMap[key] > maxDensity) maxDensity = localMap[key];
        }

        foreach (var kvp in localMap)
        {
            float t = (float)kvp.Value / (float)maxDensity;
            float finalAlpha = Mathf.Clamp01(t * gridAlpha);
            if (finalAlpha < 0.1f) finalAlpha = 0.1f;

            Gizmos.color = new Color(baseColor.r, baseColor.g, baseColor.b, finalAlpha);

            Vector3 center = new Vector3(
                kvp.Key.x * gridSize + (gridSize * 0.5f),
                kvp.Key.y * gridSize + (gridSize * 0.5f),
                kvp.Key.z * gridSize + (gridSize * 0.5f)
            );

            Gizmos.DrawCube(center, Vector3.one * (gridSize * 0.9f));
        }
    }

    void DrawEventList<T>(List<T> list, bool toggle, Color col, bool solid)
    {
        if (!toggle) return;
        Gizmos.color = col;
        foreach (var item in list)
        {
            string rid = (string)item.GetType().GetField("GameRun_ID").GetValue(item);
            if (!IsVisible(rid)) continue;

            string x = (string)item.GetType().GetField("Pos_x").GetValue(item);
            string y = (string)item.GetType().GetField("Pos_y").GetValue(item);
            string z = (string)item.GetType().GetField("Pos_z").GetValue(item);

            if (solid) Gizmos.DrawSphere(ParseVec(x, y, z), 0.5f);
            else Gizmos.DrawWireSphere(ParseVec(x, y, z), 0.4f);
        }
    }

    bool IsVisible(string runIDStr)
    {
        if (string.IsNullOrEmpty(runIDStr)) return false;
        if (int.TryParse(runIDStr, out int rID)) return validRunIDs.Contains(rID);
        return false;
    }

    Vector3 ParseVec(string x, string y, string z)
    {
        if (string.IsNullOrEmpty(x)) return Vector3.zero;
        if (float.TryParse(x, NumberStyles.Float, CultureInfo.InvariantCulture, out float fx) &&
            float.TryParse(y, NumberStyles.Float, CultureInfo.InvariantCulture, out float fy) &&
            float.TryParse(z, NumberStyles.Float, CultureInfo.InvariantCulture, out float fz))
        {
            return new Vector3(fx, fy, fz);
        }
        return Vector3.zero;
    }
}