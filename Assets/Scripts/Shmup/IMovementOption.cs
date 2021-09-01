using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IMovementOption 
{
    void Move();
    void ResetForReuse(EnemyController temporaryHost);
    void SetResourcesBasedOnType(EnemyMovementResources resources);
}
