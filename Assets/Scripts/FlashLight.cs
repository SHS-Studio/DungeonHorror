using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{
    public Light spotlight; // Assign the spotlight in the Inspector
    public float maxIntensity; // Assign the Intensity in the Inspector
    public float lowIntensity; // Assign the Intensity in the Inspector
    public float batteryLevel = 100f; // Initial battery level
    public float DecreaseBatteryChargePercentage; // Time in seconds to decrease battery by 1%
    private bool isFlickering = false;
    public float RayMaxdistance = 300;
    public int raysCount = 60;
    GameObject EnemyAIhitbyraycast;
    void Start()
    {
        PickUpManager.instance.CurntBatteryLevel = batteryLevel;
        spotlight = GetComponent<Light>();

        if (spotlight == null)
        {
            Debug.LogError("Spotlight is not assigned!");
            return;
        }


    }

    void Update()
    {
        LightSettings();
        DrainBattery();
        //StartCoroutine(ActivateEnemy());
        EnemySpotter();
    }

    public void LightSettings()
    {
        batteryLevel = PickUpManager.instance.CurntBatteryLevel;
        if (batteryLevel <= 0)
        {
            // Turn off the spotlight when battery is 0
            spotlight.enabled = false;
            FollowRailTrack  followRailTrack = GameObject.FindObjectOfType<FollowRailTrack>();
            followRailTrack.speed = 0;
            EnemyAI ai = GameObject.FindObjectOfType<EnemyAI>();
            ai.ChaseTarget();

        }
        else if (batteryLevel < 10 && !isFlickering)
        {
            // Start flickering if battery is below 10%
            StartCoroutine(FlickerLight());
        }
        else
        {
            spotlight.enabled = true;
            if (!isFlickering)
            {
                spotlight.intensity = maxIntensity;
            }

        }
    }
    public void DrainBattery()
    {
        if (batteryLevel > 0)
        {
            PickUpManager.instance.CurntBatteryLevel -= DecreaseBatteryChargePercentage * Time.deltaTime;
            PickUpManager.instance.CurntBatteryLevel = Mathf.Clamp(PickUpManager.instance.CurntBatteryLevel, 0f, 100f);
        }
    }

    public IEnumerator FlickerLight()
    {
        isFlickering = true;
        while (batteryLevel > 0 && batteryLevel < 10)
        {
            spotlight.intensity = lowIntensity;
            spotlight.enabled = !spotlight.enabled;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        }
        // Ensure the light stays on or off based on battery level
        spotlight.enabled = batteryLevel > 0;
        isFlickering = false;

    }
    IEnumerator ActivateEnemy()
    {
        if (spotlight != null && spotlight.enabled)
        {
            Collider[] hitColliders = Physics.OverlapSphere(spotlight.transform.position, RayMaxdistance);
            Vector3 spotlightDirection = spotlight.transform.forward;
            float halfAngle = spotlight.spotAngle /*/ 2f*/;

            foreach (Collider col in hitColliders)
            {
                Vector3 toTarget = (col.transform.position - spotlight.transform.position).normalized;
                float angleToTarget = Vector3.Angle(spotlightDirection, toTarget);

                // Check if the object is within the cone
                if (angleToTarget <= halfAngle)
                {
                    Debug.DrawRay(spotlight.transform.position, toTarget * RayMaxdistance, Color.red, 0.1f);

                    EnemyAIhitbyraycast = col.gameObject;
                    if (EnemyAIhitbyraycast.GetComponent<EnemyAI>())
                    {
                     //   EnemyManager.instance.IsSpoted = true;
                        EnemyAI AI = EnemyAIhitbyraycast.GetComponent<EnemyAI>();
                        Debug.Log(EnemyAIhitbyraycast.name);
                        AI.IsSpoted = true;
                        AI.CheckSpotlight();
                    }
                    else
                    {
                        EnemyAI AI = GameObject.FindObjectOfType<EnemyAI>();
                        AI.IsSpoted = false;
                       // EnemyManager.instance.IsSpoted = false;
                    }
                }
            }

            yield return null;
        }
    }

    private void EnemySpotter()
    {
        if (spotlight == null || !spotlight.enabled)
            return;

        float spotLightAngle = spotlight.spotAngle * 0.5f;
        Vector3 spotLightDirection = spotlight.transform.forward;
        Collider[] colliders = Physics.OverlapSphere(spotlight.transform.position, RayMaxdistance);

        foreach (Collider collider in colliders)
        {
            if (!collider.TryGetComponent(out EnemyAI enemyAI))
                continue;

            Vector3 toTarget = (collider.transform.position - spotlight.transform.position).normalized;
            float angleToTarget = Vector3.Angle(spotLightDirection, toTarget);
            bool isSpotted = angleToTarget <= spotLightAngle;

            Debug.DrawRay(spotlight.transform.position, toTarget * RayMaxdistance, isSpotted ? Color.green : Color.red, 0.1f);
            EnemyAIhitbyraycast = collider.gameObject;

            enemyAI.IsSpoted = isSpotted;
            if (isSpotted)
            {
                enemyAI.CheckSpotlight();
                Debug.Log(EnemyAIhitbyraycast.name);
            }
          
        }
    }

    private void OnDrawGizmos()
    {
        if (spotlight != null)
        {
#if UNITY_EDITOR
            float angle = spotlight.spotAngle;
            Vector3 forward = spotlight.transform.forward;
            Vector3 up = spotlight.transform.up;

            Vector3 left = Quaternion.AngleAxis(-angle * 0.5f, up) * forward;
            Vector3 right = Quaternion.AngleAxis(angle * 0.5f, up) * forward;

            Vector3 center = spotlight.transform.position;

            UnityEditor.Handles.color = new(1.0f, 1.0f, 0.0f, 0.1f);
            UnityEditor.Handles.DrawWireArc(center, up, left, angle, RayMaxdistance);

            UnityEditor.Handles.DrawLine(center, center + left * RayMaxdistance);
            UnityEditor.Handles.DrawLine(center, center + right * RayMaxdistance);
#endif
        }
    }
}


