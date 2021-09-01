using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Game", menuName = "Game/WaveEntity")]
public class WaveEntitySO : ScriptableObject
{
    [Header("Spawn Settings")]
    public float delay;
    public GameObject entity;
    public Vector3 creationPosition;
    public bool driveBy; // is it meant to stay to fight it out?
    
    [Header("Despawn Settings")]
    public float lifeTime;
    public int exitType;
    


    [Header("Shoot Behavior")]
    //shoot behavior settings
    public Enemy.ShootBehavior ShootBehavior;
    public int consecutiveShotsBank;
    public float MinShotFrequency;
    public float MaxShotFrequency;

    [Header("Shoot Controller")]
    //shotcontroller settings
    public int bulletsAtOnce;
    public float minAngle;
    public float maxAngle;
    public bool RandomSpreadShot;

    [Header("Instance Types")]
    public int EnemyType;
    public int WeaponType;
    public int PowerUpType;
    public int AbilityType;
    public float minAbilityLaunchTimeCycle;
    public float maxAbilityLaunchTimeCycle;
    public int EvasiveManeuverType;
    public float colliderTimeAlive;
    public float minEvasiveColliderActivateTime;
    public float maxEvasiveColliderActivateTime;
    
    [Header("Movement Settings")]
    public int MovementOptionType;
    public bool rotationTowardsTarget;
    public List<EnemyMovementInputs> inputs;
    public GameObject path;
    public Vector3 pathPlacement;

    [Header("AutoAim Settings")]
    public bool autoAim;
}
