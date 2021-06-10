using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStats", menuName = "Galactic/Enemy")]
public class EnemyStats : ScriptableObject
{
    public Sprite sprite;
    public float baseSpeed;
    public int baseShield;
}
