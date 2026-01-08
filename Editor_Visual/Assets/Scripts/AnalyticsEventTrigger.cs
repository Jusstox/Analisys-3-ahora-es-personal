using UnityEngine;
using System.Collections;

public class AnalyticsEventTrigger : MonoBehaviour
{
    public enum AnalyticsType
    {
        Key,
        Button,
        BreakBox,
        Checkpoint,
        Heal
    }

    public AnalyticsType eventType;
    public int healAmount = 1;

    private bool isLocked = false;
    private float startupTime;

    void Start()
    {
        startupTime = Time.time;
    }

    public void ManualTrigger()
    {
        if (isLocked) return;

        Debug.Log($"[Analytics] Manual Trigger Activated on {gameObject.name}");
        isLocked = true;
        SendEvent();
        HandleCleanup();
    }

    void OnCollisionEnter(Collision collision) { ProcessHit(collision.gameObject); }
    void OnTriggerEnter(Collider other) { ProcessHit(other.gameObject); }

    private void ProcessHit(GameObject obj)
    {
        if (Time.timeSinceLevelLoad < 0.5f) return;

        if (isLocked) return;

        string name = obj.name.ToLower();
        if (name.Contains("floor") || name.Contains("ground") || name.Contains("tile") || name.Contains("terrain"))
        {
            return;
        }

        if (!obj.isStatic && (obj.CompareTag("Player") || obj.CompareTag("Untagged")))
        {
            Debug.Log($"[Analytics] Hit Detected by: {obj.name}");

            isLocked = true;
            SendEvent();
            HandleCleanup();
        }
    }

    private void HandleCleanup()
    {
        switch (eventType)
        {
            case AnalyticsType.Key:
            case AnalyticsType.Heal:
            case AnalyticsType.Button:
            case AnalyticsType.BreakBox:
                Destroy(this);
                break;
            case AnalyticsType.Checkpoint:
                StartCoroutine(ResetCooldown(5.0f));
                break;
        }
    }

    private IEnumerator ResetCooldown(float duration)
    {
        yield return new WaitForSeconds(duration);
        isLocked = false;
    }

    private void SendEvent()
    {
        if (AnalyticsManager.Instance == null) return;

        switch (eventType)
        {
            case AnalyticsType.Key:
                AnalyticsManager.Instance.RecordKey(transform.position);
                break;
            case AnalyticsType.Button:
                AnalyticsManager.Instance.RecordButton(transform.position);
                break;
            case AnalyticsType.BreakBox:
                AnalyticsManager.Instance.RecordBreakBox(transform.position);
                break;
            case AnalyticsType.Checkpoint:
                AnalyticsManager.Instance.RecordCheckpoint(gameObject.name, transform.position);
                break;
            case AnalyticsType.Heal:
                AnalyticsManager.Instance.RecordHeal(healAmount, "HealthPickup", transform.position);
                break;
        }
    }
}