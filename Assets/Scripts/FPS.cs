using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class FPS : MonoBehaviour
{

    [SerializeField] TMP_Text fps;
    private int fpsCounter = 0;
    private float _timer;
    private float _hudRefreshRate = 1f;

    void Update()
    {
        if (Time.unscaledTime > _timer)
        {
            int fpsCount = (int)(1f / Time.unscaledDeltaTime);
            fps.text = "FPS: " + fpsCount;
            _timer = Time.unscaledTime + _hudRefreshRate;
        }
    }
}
