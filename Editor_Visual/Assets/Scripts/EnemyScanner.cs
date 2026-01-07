using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;


public class EnemyScanner : MonoBehaviour
{
    private string baseURL = "https://citmalumnes.upc.es/~marcac6/game_analytics/";

    [ContextMenu("Scan Enemies")]
    public void ScanAndUpload()
    {
        StartCoroutine(UploadEnemies());
    }

    IEnumerator UploadEnemies()
    {
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        Transform[] allObjects = FindObjectsOfType<Transform>();
        List<GameObject> realEnemies = new List<GameObject>();

        if (enemyLayer == -1)
        {
            yield break;
        }

        foreach (Transform t in allObjects)
        {
            if (t.gameObject.layer == enemyLayer)
            {
                if (t.GetComponent<UnityEngine.AI.NavMeshAgent>() != null)
                {
                    realEnemies.Add(t.gameObject);
                }
            }
        }

        Debug.Log($"Found {realEnemies.Count} enemies on Layer 'Enemy'. Uploading...");

        foreach (var enemy in realEnemies)
        {
            WWWForm form = new WWWForm();

            string type = enemy.name.Contains("Chomper") ? "Chomper" : "Spitter";

            form.AddField("type", type);
            form.AddField("x", enemy.transform.position.x.ToString(System.Globalization.CultureInfo.InvariantCulture));
            form.AddField("y", enemy.transform.position.y.ToString(System.Globalization.CultureInfo.InvariantCulture));
            form.AddField("z", enemy.transform.position.z.ToString(System.Globalization.CultureInfo.InvariantCulture));

            using (UnityWebRequest www = UnityWebRequest.Post(baseURL + "upload_enemy_static.php", form))
            {
                yield return www.SendWebRequest();
                if (www.result == UnityWebRequest.Result.Success)
                {
                    string response = www.downloadHandler.text;
                    Debug.Log($"Uploaded: {enemy.name}. Server respone: {response}");
                } else
                {
                    Debug.LogError($"Failed {enemy.name}: {www.error}");
                }
            }
        }
    }
}
