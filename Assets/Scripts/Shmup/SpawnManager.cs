using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private bool _stopSpawning = false;
    [SerializeField]
    private GameObject _enemyPrefab;
    [SerializeField]
    private GameObject _enemyContainer;

    private void Start() {
        StartSpawning();
    }
    public void StartSpawning(){
        StartCoroutine(SpawnEnemyRoutine());
    }

    //spawn a thing every 5 seconds
    //Ienumerator type allows for yield keyword...pauses and returns execution order back to the caller.
    IEnumerator SpawnEnemyRoutine(){
        yield return new WaitForSeconds(3.0f);
        while(!_stopSpawning){
            GameObject newEnemy = Instantiate(_enemyPrefab, new Vector3(Random.Range(-5f,5f),7.0f), Quaternion.identity);
            //setting it to child of container.
            newEnemy.transform.parent = _enemyContainer.transform;
            yield return new WaitForSeconds(3.5f);
        }
    }
    public void OnPlayerDeath(){
        _stopSpawning = true;
    }
}
