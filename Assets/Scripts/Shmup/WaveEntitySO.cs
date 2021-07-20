using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Game", menuName = "Game/WaveEntity")]
public class WaveEntitySO : ScriptableObject
{
    public GameObject entity;
    public Vector3 creationPosition;
    public bool driveBy; // is it meant to stay to fight it out?

    public int WeaponType;
    public int AbilityType;
    public int EvasiveManeuverType;
    public int MovementOptionType;
    public List<EnemyMovementInputs> inputs;

    public float delay;



}
