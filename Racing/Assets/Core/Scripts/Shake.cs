using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public static Shake Instance { get; private set; }

    private CinemachineVirtualCamera _camera;
    private float shakeTimer;

    private void Awake()
    {
       Instance = this; 
      _camera = GetComponent<CinemachineVirtualCamera>();
    }

    public void shake(float intensity, float time)
    {
        CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

        cinemachineBasicMultiChannelPerlin.m_AmplitudeGain= intensity;
        shakeTimer= time;
    }

    private void Update()
    {
       if (shakeTimer>0)
        {
            shakeTimer-= Time.deltaTime;
            if (shakeTimer<= 0f) 
            {
                //Time over
                CinemachineBasicMultiChannelPerlin cinemachineBasicMultiChannelPerlin = _camera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

                cinemachineBasicMultiChannelPerlin.m_AmplitudeGain= 0f;
            }
        }
    }
}
