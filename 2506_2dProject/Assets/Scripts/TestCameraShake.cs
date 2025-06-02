using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCameraShake : MonoBehaviour
{
    [SerializeField] Cinemachine.CinemachineVirtualCamera vCam;
    [SerializeField] Cinemachine.NoiseSettings noiseProfile;

    [ContextMenu(nameof(StartShakeCamera))]
    public void StartShakeCamera()
    {
        var noise = vCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        noise.m_NoiseProfile = noiseProfile;
    }

    [ContextMenu(nameof(StopShakeCamera))]
    public void StopShakeCamera()
    {
        var noise = vCam.GetCinemachineComponent<Cinemachine.CinemachineBasicMultiChannelPerlin>();
        noise.m_NoiseProfile = null;
    }
}
