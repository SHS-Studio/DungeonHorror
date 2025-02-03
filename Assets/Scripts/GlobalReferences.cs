using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GlobalReferences : MonoBehaviour
{
    public static GlobalReferences instance { get; set; }
    public TMP_Text countdownText; // Reference to the UI text element
    public float countdownTime = 3f; // Countdown duration
    public GameObject bulletimpactprefab;
    public void Awake()
    {
        
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            instance = this;
        }
    }

    void Start()
    {
        StartCoroutine(CountdownAndResume());
    }

    IEnumerator CountdownAndResume()
    {
        Time.timeScale = 0f; // Pause the game
        float timer = countdownTime;

        while (timer > 0)
        {
            countdownText.text = timer.ToString("0"); // Display countdown
            yield return new WaitForSecondsRealtime(1f); // Wait for real-time second
            timer--;
        }

        countdownText.text = "Start!";
        yield return new WaitForSecondsRealtime(1f); // Display "Start!" before resuming

        countdownText.gameObject.SetActive(false); // Hide text after countdown
        Time.timeScale = 1f; // Resume the game
    }
}
