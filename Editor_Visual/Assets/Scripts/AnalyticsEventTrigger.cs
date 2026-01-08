using UnityEngine;

public class AnalyticsEventTrigger : MonoBehaviour
{
    public enum AnalyticsType
    {
        Key,
        Button,
        BreakBox,
        Checkpoint,
        Heal,
    }

    public AnalyticsType eventType;

    public int healAmount = 1;

    private bool hasTriggered = false;

    void Start()
    {
        Debug.Log($"[System Check] WinZone Script is ACTIVE on object: {gameObject.name}");
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log($"[AnalyticsTrigger] Something entered: {other.name} (Tag: {other.tag})");

        if (other.CompareTag("Player"))
        {
            if (hasTriggered && (eventType == AnalyticsType.Checkpoint || eventType == AnalyticsType.Heal || eventType == AnalyticsType.Key))
                return;

            SendEvent();

            if (eventType == AnalyticsType.Checkpoint || eventType == AnalyticsType.Heal || eventType == AnalyticsType.Key)
                hasTriggered = true;
        }
    }

    public void ManualTrigger()
    {
        SendEvent();
    }

    private void SendEvent()
    {
        if (AnalyticsManager.Instance == null) return;

        switch (eventType)
        {
            case AnalyticsType.Key:
                AnalyticsManager.Instance.RecordKey(transform.position);
                Destroy(this);
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
                Destroy(this);
                break;
        }
    }
}