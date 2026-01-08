using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public struct PositionData
{
    public int gameRunID;
    public float x, y, z;
    public float rotY_Cam, rotY_Player;
    public string timestamp;
}

[System.Serializable]
public class PositionWrapper
{
    public List<PositionData> items;
}

public class AnalyticsManager : MonoBehaviour
{
    public static AnalyticsManager Instance;

    private string baseURL = "https://citmalumnes.upc.es/~marcac6/game_analytics/";

    public int playerID = 1;
    public int currentSessionID = 0;
    public int currentRunID = 0;

    private static bool hasSessionStarted = false;
    private bool hasWonRun = false;

    private List<PositionData> positionBuffer = new List<PositionData>();

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "ExampleScene")
        {
            if (currentSessionID == 0) return;

            if (currentRunID != 0) EndRun(false);

            Debug.Log("[Analytics] Scene Reloaded. Starting New Run.");
            StartRun();
        }
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        } else
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Start()
    {
        yield return StartCoroutine(StartSessionRoutine());

        if (currentSessionID != 0)
        {
            StartRun();
        }
        else
        {
            Debug.LogError("[Analytics] Failed to initialize Session. Run will not record.");
        }
    }

    IEnumerator StartSessionRoutine()
    {
        WWWForm form = new WWWForm();
        form.AddField("player_id", playerID);

        using (UnityWebRequest www = UnityWebRequest.Post(baseURL + "start_session.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Network Error: " + www.error);
            }
            else
            {
                string response = www.downloadHandler.text;

                if (int.TryParse(response, out int newID))
                {
                    currentSessionID = newID;
                    //Debug.Log("Session Established: " + currentSessionID);
                }
                else
                {
                    Debug.LogError("Failed to parse Session ID. Server said: " + response);
                }
            }
        }
    }

    public void StartRun()
    {
        hasWonRun = false;
        StartCoroutine(StartRunRoutine());
    }

    IEnumerator StartRunRoutine()
    {
        int isRestartVal = hasSessionStarted ? 1 : 0;
        hasSessionStarted = true;

        WWWForm form = new WWWForm();
        form.AddField("session_id", currentSessionID);
        form.AddField("isRestart", isRestartVal);

        using (UnityWebRequest www = UnityWebRequest.Post(baseURL + "start_run.php", form))
        {
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                if (int.TryParse(www.downloadHandler.text, out int newRunID))
                {
                    currentRunID = newRunID;
                    Debug.Log($"[Analytics] Run Started. ID: {currentRunID}");
                }
                else
                {
                    Debug.LogError($"[Analytics] PHP Logic Error: {www.downloadHandler.text}");
                }
            }
            else
            {
                Debug.LogError($"[Analytics] Network Error: {www.error} \nServer said: {www.downloadHandler.text}");
            }
        }
    }

    public void RecordPosition(Vector3 pos, float rotCam, float rotPlayer)
    {
        if (currentRunID == 0) return;

        PositionData data = new PositionData();
        data.gameRunID = currentRunID;
        data.x = pos.x;
        data.y = pos.y;
        data.z = pos.z;
        data.rotY_Cam = rotCam;
        data.rotY_Player = rotPlayer;
        data.timestamp = System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

        positionBuffer.Add(data);

        if (positionBuffer.Count >= 10) FlushPositions();
    }

    public void RecordJump(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("run_id", currentRunID);
        form.AddField("x", pos.x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("y", pos.y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("z", pos.z.ToString(System.Globalization.CultureInfo.InvariantCulture));
        StartCoroutine(PostData("upload_jump.php", form));
    }

    public void RecordHit(bool isPlayer, int? enemyID, string source, float health, Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("run_id", currentRunID);

        if (enemyID.HasValue) form.AddField("enemy_id", enemyID.Value);

        form.AddField("source", source);
        form.AddField("health", health.ToString("F1"));
        form.AddField("x", pos.x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("y", pos.y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("z", pos.z.ToString(System.Globalization.CultureInfo.InvariantCulture));

        string script = isPlayer ? "upload_playerhit.php" : "upload_enemyhit.php";
        StartCoroutine(PostData(script, form));
    }

    public void RecordDeath(bool isPlayer, int? killerID, Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("run_id", currentRunID);

        if (killerID.HasValue) form.AddField("enemy_id", killerID.Value);

        form.AddField("x", pos.x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("y", pos.y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("z", pos.z.ToString(System.Globalization.CultureInfo.InvariantCulture));

        string script = isPlayer ? "upload_death.php" : "upload_kill.php";
        StartCoroutine(PostData(script, form));
    }

    public void RecordHeal(int amount, string source, Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("run_id", currentRunID);
        form.AddField("amount", amount);
        form.AddField("source", source);
        form.AddField("x", pos.x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("y", pos.y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("z", pos.z.ToString(System.Globalization.CultureInfo.InvariantCulture));
        StartCoroutine(PostData("upload_heal.php", form));
    }

    public void RecordCheckpoint(string checkpointName, Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("run_id", currentRunID);
        form.AddField("name", checkpointName);
        form.AddField("x", pos.x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("y", pos.y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("z", pos.z.ToString(System.Globalization.CultureInfo.InvariantCulture));
        StartCoroutine(PostData("upload_checkpoint.php", form));
    }

    public void RecordKey(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("run_id", currentRunID);
        form.AddField("x", pos.x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("y", pos.y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("z", pos.z.ToString(System.Globalization.CultureInfo.InvariantCulture));
        StartCoroutine(PostData("upload_key.php", form));
    }

    public void RecordButton(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("run_id", currentRunID);
        form.AddField("x", pos.x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("y", pos.y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("z", pos.z.ToString(System.Globalization.CultureInfo.InvariantCulture));
        StartCoroutine(PostData("upload_button.php", form));
    }

    public void RecordBreakBox(Vector3 pos)
    {
        WWWForm form = new WWWForm();
        form.AddField("run_id", currentRunID);
        form.AddField("x", pos.x.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("y", pos.y.ToString(System.Globalization.CultureInfo.InvariantCulture));
        form.AddField("z", pos.z.ToString(System.Globalization.CultureInfo.InvariantCulture));
        StartCoroutine(PostData("upload_breakbox.php", form));
    }

    public void EndRun(bool playerWon)
    {
        if (currentRunID == 0) return;

        if (playerWon) hasWonRun = true;

        WWWForm form = new WWWForm();
        form.AddField("run_id", currentRunID);
        form.AddField("win", hasWonRun ? 1 : 0);

        StartCoroutine(PostData("end_run.php", form));
    }

    public void EndSession()
    {
        if (currentSessionID == 0) return;

        WWWForm form = new WWWForm();
        form.AddField("session_id", currentSessionID);

        StartCoroutine(PostData("end_session.php", form));
    }

    void OnApplicationQuit()
    {
        FlushPositions();
        if (currentRunID != 0) EndRun(hasWonRun);

        EndSession();
    }

    void FlushPositions()
    {
        if (positionBuffer.Count == 0) return;

        PositionWrapper wrapper = new PositionWrapper();
        wrapper.items = positionBuffer;

        string json = JsonUtility.ToJson(wrapper);

        WWWForm form = new WWWForm();
        form.AddField("data", json);
        form.AddField("session_id", currentSessionID);

        StartCoroutine(PostData("upload_positions.php", form));
        positionBuffer.Clear();
    }

    IEnumerator PostData(string phpScript, WWWForm form)
    {
        if (currentRunID == 0) yield break;

        using (UnityWebRequest www = UnityWebRequest.Post(baseURL + phpScript, form))
        {
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.LogError("Upload Failed: " + www.error);
            }
            else
            {
                Debug.Log("Server Response: " + www.downloadHandler.text);
            }
        }
    }
}
