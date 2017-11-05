using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoDB : ScriptableObject
{
    private string currentScene;

    private Vector3 positionInScene;

    private Quaternion rotationInScene;

    private float currentHealth;

    private float totalHealth;

    private bool hasFlashLight;

    private float flashLightHealth;

    private float flashLightTotalHealth;



    public string CurrentScene
    {
        get
        {
            return this.currentScene;
        }
        set
        {
            currentScene = value;
        }
    }

    public Vector3 PositionInScene
    {
        get
        {
            return this.positionInScene;
        }
        set
        {
            positionInScene = value;
        }
    }

    public Quaternion RotationInScene
    {
        get
        {
            return this.rotationInScene;
        }
        set
        {
            rotationInScene = value;
        }
    }

    public float CurrentHealth
    {
        get
        {
            return this.currentHealth;
        }
        set
        {
            currentHealth = value;
        }
    }

    public float TotalHealth
    {
        get
        {
            return this.totalHealth;
        }
        set
        {
            totalHealth = value;
        }
    }

    public bool HasFlashLight
    {
        get
        {
            return this.hasFlashLight;
        }
        set
        {
            hasFlashLight = value;
        }
    }

    public float FlashLightHealth
    {
        get
        {
            return this.flashLightHealth;
        }
        set
        {
            flashLightHealth = value;
        }
    }

    public float FlashLightTotalHealth
    {
        get
        {
            return this.flashLightTotalHealth;
        }
        set
        {
            flashLightTotalHealth = value;
        }
    }
}
