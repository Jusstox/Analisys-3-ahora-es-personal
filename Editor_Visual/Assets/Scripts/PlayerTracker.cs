using UnityEngine;

public class PlayerTracker : MonoBehaviour
{
    private float timer = 0f;
    private float sampleRate = 0.5f;
    private Vector3 lastPos;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= sampleRate)
        {
            if (Vector3.Distance(transform.position, lastPos) > 0.1f)
            {
                float camRot = Camera.main ? Camera.main.transform.eulerAngles.y : 0;
                AnalyticsManager.Instance.RecordPosition(transform.position, camRot, transform.eulerAngles.y);
                lastPos = transform.position;
            }
            timer = 0f;
        }

        if (Input.GetButtonDown("Jump"))
        {
            AnalyticsManager.Instance.RecordJump(transform.position);
        }
    }
}
