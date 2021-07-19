using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public interface IEnemyEvasionManeuver : IEnemyAbility
{
    void SetCollisionInfo(Collider2D info);
    
}