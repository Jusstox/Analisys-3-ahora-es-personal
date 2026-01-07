using UnityEngine;

public class AnalyticsEventTrigger : MonoBehaviour
{
    public enum AnalyticsType
    {
        Key,
        Button,
        BreakBox,
        Win
    }

    public AnalyticsType eventType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SendEvent();
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
                break;
            case AnalyticsType.Button:
                AnalyticsManager.Instance.RecordButton(transform.position);
                break;
            case AnalyticsType.BreakBox:
                AnalyticsManager.Instance.RecordBreakBox(transform.position);
                break;
            case AnalyticsType.Win:
                AnalyticsManager.Instance.EndRun(true);
                break;
        }

        if (eventType == AnalyticsType.Key) Destroy(this);
    }
}