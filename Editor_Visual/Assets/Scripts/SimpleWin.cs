using UnityEngine;

public class SimpleWin : MonoBehaviour
{
    private BoxCollider zone;
    private Transform player;
    private bool won = false;

    void Start()
    {
        zone = GetComponent<BoxCollider>();
        GameObject p = GameObject.FindGameObjectWithTag("Player");
        if (p != null) player = p.transform;
    }

    void Update()
    {
        if (won || player == null) return;

        if (zone.bounds.Contains(player.position))
        {
            Debug.Log("WIN TRIGGERED via Math Check!");
            won = true;
            AnalyticsManager.Instance.EndRun(won);
        }
    }
}