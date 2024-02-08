using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour
{    
    [Range(0, 2)] public float amplitude = 1f;
    [Range(1, 100)] public float frequency = 1f;
    [Range(0, 5)] public float speed = 1f;
    
    private float offset = 0f;        
    private float planeExtent = 2f;

    public float MaxHeight {
        get { return amplitude * 2; }
    }
    
    public void Initialize(float pw) {
        this.planeExtent = pw / 2;
    }

    public void OnUpdate(float dt)
    {
        offset += Time.deltaTime * speed;
    }

    public float GetWaveHeight(float worldX)
    {
        var normalizedX = worldX / planeExtent;
        var height = amplitude * (Mathf.Sin((normalizedX * frequency) + (Time.time * speed)) + 1); 
        return height + transform.position.y;
    }
    
}
