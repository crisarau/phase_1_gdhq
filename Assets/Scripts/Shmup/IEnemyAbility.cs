using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IEnemyAbility
{
    //use the actual ability
    void UseEnemyAbility();
    //lets the ability reset and have a new host to control
    void ResetForReuse(EnemyController temporaryHost);
    void SetResourcesBasedOnType(EnemyAbilityResources resources);
}
