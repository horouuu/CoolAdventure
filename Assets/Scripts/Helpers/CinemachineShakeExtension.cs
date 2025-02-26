using UnityEngine;
using Unity.Cinemachine;
using System.Collections;

// Created with reference from: https://discussions.unity.com/t/how-to-shake-camera-with-cinemachine/672561/12

/// <summary>
/// An advanced camera shake extension for Cinemachine that allows for
/// trauma-based, dampened shaking with precise control
/// </summary>
[ExecuteInEditMode]
[SaveDuringPlay]
[AddComponentMenu("")] // Hide in menu
public class CinemachineShakeExtension : CinemachineExtension
{
    [Tooltip("Maximum distance in each dimension that the camera can shake")]
    public Vector3 MaxShakeOffset = new Vector3(1f, 1f, 0f);
    
    [Tooltip("Maximum angle in degrees that the camera can rotate during shake")]
    public Vector3 MaxShakeRotation = new Vector3(0f, 0f, 2f);
    
    [Tooltip("How quickly the shake effect will fade out (higher = faster fade)")]
    public float DecaySpeed = 2.0f;
    
    [Tooltip("Noise frequency/roughness of the shake")]
    public float Frequency = 25.0f;

    // Trauma is the current intensity of the shake between 0 and 1
    private float _trauma = 0f;
    private float _timeCounter = 0f;
    
    // Seed values for the noise functions to make them unique
    private float _seedX, _seedY, _seedZ;
    private float _seedRotX, _seedRotY, _seedRotZ;

    private void Start()
    {
        // Initialize random seeds to prevent all instances from shaking identically
        _seedX = Random.value * 100f;
        _seedY = Random.value * 100f;
        _seedZ = Random.value * 100f;
        _seedRotX = Random.value * 100f;
        _seedRotY = Random.value * 100f;
        _seedRotZ = Random.value * 100f;
    }

    protected override void PostPipelineStageCallback(
        CinemachineVirtualCameraBase vcam,
        CinemachineCore.Stage stage,
        ref CameraState state,
        float deltaTime)
    {
        // Only apply shake to the Body stage in the pipeline
        if (stage == CinemachineCore.Stage.Aim)
        {
            // If there's no trauma, don't do any processing
            if (_trauma <= 0)
                return;
                
            // Increment our noise time counter
            _timeCounter += deltaTime * Frequency;
            
            // Calculate shake amount based on trauma (squared for more natural fade out)
            float shake = _trauma * _trauma;
            
            // Apply positional shake
            state.PositionCorrection += new Vector3(
                MaxShakeOffset.x * shake * Mathf.PerlinNoise(_seedX, _timeCounter),
                MaxShakeOffset.y * shake * Mathf.PerlinNoise(_seedY, _timeCounter),
                MaxShakeOffset.z * shake * Mathf.PerlinNoise(_seedZ, _timeCounter)
            );
            
            // Apply rotational shake
            state.OrientationCorrection *= Quaternion.Euler(
                MaxShakeRotation.x * shake * Mathf.PerlinNoise(_seedRotX, _timeCounter),
                MaxShakeRotation.y * shake * Mathf.PerlinNoise(_seedRotY, _timeCounter),
                MaxShakeRotation.z * shake * Mathf.PerlinNoise(_seedRotZ, _timeCounter)
            );
            
            // Decay trauma over time
            _trauma = Mathf.Max(0f, _trauma - DecaySpeed * deltaTime);
        }
    }

    /// <summary>
    /// Adds shake trauma to the camera (trauma controls intensity)
    /// </summary>
    /// <param name="amount">Amount of trauma to add (0-1)</param>
    public void AddShake(float amount)
    {
        _trauma = Mathf.Clamp01(_trauma + amount);
    }
    
    /// <summary>
    /// Instantly sets the shake trauma value
    /// </summary>
    /// <param name="amount">Trauma amount (0-1)</param>
    public void SetShake(float amount)
    {
        _trauma = Mathf.Clamp01(amount);
    }
    
    /// <summary>
    /// Stops any current camera shake
    /// </summary>
    public void StopShake()
    {
        _trauma = 0f;
    }
}