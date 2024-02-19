using System.Collections;
using System.Threading;
using UnityEngine;

public class FrameRateManager : MonoBehaviour
{
    [Header("Frame Settings")]
    int MaxRate = 9999;
    public float TargetFrameRate = 60.0f;
    float currentFrameTime;

    void Awake()
    {
        // Sets the vertical sync off
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = MaxRate;
        currentFrameTime = Time.realtimeSinceStartup;
        StartCoroutine(WaitForNextFrame());
    }

    IEnumerator WaitForNextFrame()
    {
        while (true)
        {
           
            yield return new WaitForEndOfFrame();
            
            // Calculating the next expected frame time
            currentFrameTime += 1.0f / TargetFrameRate;
            
            // This calculate the sleep time until the next frame
            float sleepTime = currentFrameTime - Time.realtimeSinceStartup - 0.01f;
            
            // If we need to use sleep, do so
            if (sleepTime > 0)
                Thread.Sleep((int)(sleepTime * 1000));

            // Checking that the next frame starts at the expected time
            while (Time.realtimeSinceStartup < currentFrameTime)
                yield return null;
        }
    }
}
   
   

