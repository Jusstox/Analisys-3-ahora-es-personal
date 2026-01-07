using UnityEngine;
using System.Collections;
using Gamekit3D;
using Gamekit3D.Message;

public class AnalyticsDamageListener : MonoBehaviour, IMessageReceiver
{
    [Header("Settings")]
    public bool isPlayer = false;

    public int enemyID_FromDatabase = 0;

    public void OnReceiveMessage(MessageType type, object sender, object msg)
    {
        if (type == MessageType.DAMAGED)
        {
            var data = (Damageable.DamageMessage)msg;
            float hp = 0f;
            var damageable = GetComponent<Damageable>();
            if (damageable) hp = damageable.currentHitPoints;

            ProcessHit(data, hp);
        }

        if (type == MessageType.DEAD)
        {
            var data = (Damageable.DamageMessage)msg;

            ProcessHit(data, 0f);

            int? idToSend = null;


            if (isPlayer)
            {
                if (data.damager != null)
                {
                    var killer = data.damager.GetComponentInParent<AnalyticsDamageListener>();
                    if (killer != null) idToSend = killer.enemyID_FromDatabase;
                }

                AnalyticsManager.Instance.RecordDeath(true, idToSend, transform.position);
                AnalyticsManager.Instance.EndRun(false);
            }
            else
            {
                idToSend = this.enemyID_FromDatabase;
                AnalyticsManager.Instance.RecordDeath(false, idToSend, transform.position);
            }
        }
    }

    void ProcessHit(Damageable.DamageMessage data, float hp)
    {
        string sourceName = "Environment";
        int? idToSend = null;

        if (isPlayer)
        {
            if (data.damager != null)
            {
                sourceName = data.damager.name;
                var attacker = data.damager.GetComponentInParent<AnalyticsDamageListener>();
                if (attacker != null) idToSend = attacker.enemyID_FromDatabase;
            }
            AnalyticsManager.Instance.RecordHit(true, idToSend, sourceName, hp, transform.position);
        }
        else
        {
            idToSend = this.enemyID_FromDatabase;

            if (data.damager != null) sourceName = data.damager.name;

            AnalyticsManager.Instance.RecordHit(false, idToSend, sourceName, hp, transform.position);
        }
    }
}