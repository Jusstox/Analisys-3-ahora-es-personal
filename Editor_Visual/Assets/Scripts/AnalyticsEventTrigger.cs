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
        Heal,
    }

    public AnalyticsType eventType;

    public int healAmount = 1;

    private bool isLocked = false;

    void OnTriggerEnter(Collider other)
    {
        if (isLocked) return;

        if (other.CompareTag("Player"))
        {
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
                StartCoroutine(ResetCooldown(10.0f));
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