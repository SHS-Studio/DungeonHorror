using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class FlashLight : MonoBehaviour
{
    public Light spotlight; // Assign the spotlight in the Inspector
    public float batteryLevel = 100f; // Initial battery level
    public float DecreaseBatteryChargePercentage; // Time in seconds to decrease battery by 1%
    private bool isFlickering = false;

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
        ActivateEnemy();
    }

    public void LightSettings()
    {
        batteryLevel = PickUpManager.instance.CurntBatteryLevel;
        if (batteryLevel <= 0)
        {
            // Turn off the spotlight when battery is 0
            spotlight.enabled = false;
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
                spotlight.intensity = 2.5f;
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

    IEnumerator FlickerLight()
    {
        isFlickering = true;
        while (batteryLevel > 0 && batteryLevel < 10)
        {
            spotlight.intensity = 1;
            spotlight.enabled = !spotlight.enabled;
            yield return new WaitForSeconds(Random.Range(0.1f, 0.2f));
        }
        // Ensure the light stays on or off based on battery level
        spotlight.enabled = batteryLevel > 0;
        isFlickering = false;
    }
    void ActivateEnemy()
    {
        if (spotlight != null && spotlight.enabled)
        {
            RaycastHit hit;
            
            if (Physics.Raycast(spotlight.transform.position, spotlight.transform.forward, out hit,Mathf.Infinity))
            {
                Debug.DrawRay(spotlight.transform.position, spotlight.transform.forward, Color.red, Mathf.Infinity);

                GameObject EnemyAIhitbyraycast = hit.transform.gameObject;
                if (EnemyAIhitbyraycast.GetComponent<EnemyAI>())
                {
                    EnemyAI AI = EnemyAIhitbyraycast.GetComponent<EnemyAI>();
                    AI.CheckSpotlight();
                }
                

            }
           
        }

    }
}


