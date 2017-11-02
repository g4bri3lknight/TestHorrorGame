using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInfoDB : ScriptableObject
{
    private Vector3 positionInScene;

    private Quaternion rotationInScene;

    private float currentHealth;

    private float totalHealth;

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
}
