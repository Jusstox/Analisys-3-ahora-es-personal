using UnityEngine;
using Gamekit3D;

public class PlayerEnvironmentListener : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.name.Contains("InfoZone") || other.name.Contains("Checkpoint")) return;

        DeathVolume deathZone = other.GetComponent<DeathVolume>();

        if (deathZone != null || other.name.Contains("KillZone"))
        {
            Debug.Log($"[Analytics] Environmental Death Detected: {other.name}");
            AnalyticsManager.Instance.RecordDeath(true, null, transform.position);
        }
    }
}