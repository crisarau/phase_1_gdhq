using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class GameplayWaveData {
    public int playerKilled; //how many killed
    public int playerGoal; //how many to kill
    public int lastLogicalWaveIndex; // if we in this index..time to move to the next gameplay wave
    public bool bonusObtained; //did we get the bonus by hitting the goal?

    public GameplayWaveData(int goal, int boundaryToNextGameplayWave){ 
        playerGoal = goal;
        lastLogicalWaveIndex = boundaryToNextGameplayWave;
        playerKilled = 0;
        bonusObtained = false;
    }
}
public class LevelController : MonoBehaviour
{

    [SerializeField]
    private LevelSO level;
    [SerializeField]
    int currentWaveIndex;
    [SerializeField]
    int currentSpawnOrderedIndex;
    [SerializeField]
    int currentSpawnRandomDone;

    [SerializeField]
    static private List<GameplayWaveData> WaveScoreBoard;
    [SerializeField]
    int GameplayWaveIndex;

    EnemyManager enemyManager;
    WaveEntitySO currentWaveObjectToSpawn;

    [SerializeField]
    GameObject innactiveEnemyHolder; //innactive by default

    float scrollingSpeed;

    static float orderedDelayEndTime;
    static float randomDelayEndTime;

    [SerializeField]
    Transform playerPositionReference;

    [SerializeField]
    GameObject baseEnemyTemplate;


    // Start is called before the first frame update
    void Start()
    {
        //instantiate EnemyManager and spawn enemies as needed
        enemyManager = new EnemyManager(25, innactiveEnemyHolder.transform, playerPositionReference, baseEnemyTemplate);
        enemyManager.InitializeTable();

        //initialize wavescoreboard...we just did it through the editor lmao
        InitializeScoreboard();
        GameplayWaveIndex = 0;
        currentSpawnOrderedIndex = 0;
        currentSpawnRandomDone = 0;

        Debug.Log("STARTING WAVE: " + GameplayWaveIndex);

        AddToRandomDelay(level.waves[0].delayForRandomsAtStart);
;        
    }

    // Update is called once per frame
    void Update()
    {
        //is level still going?
        if(currentWaveIndex < level.waves.Count){
            //is current wave's ordered spawn list still going?
            if(currentSpawnOrderedIndex < level.waves[currentWaveIndex].orderedSpawns.Count){
                //are we done with delay?
                if(Time.time >= orderedDelayEndTime){
                    //SPAWN from ordred
                    currentWaveObjectToSpawn = level.waves[currentWaveIndex].orderedSpawns[currentSpawnOrderedIndex];
                    Spawn(false, GameplayWaveIndex);
                    currentSpawnOrderedIndex++;

                }//else do nothing
            }
            //is the current wave's random spawn quota met?
            if(currentSpawnRandomDone < level.waves[currentWaveIndex].randomSpawnTarget){
                if(Time.time >= randomDelayEndTime){
                    //SPAWN from random bank
                    currentWaveObjectToSpawn = level.waves[currentWaveIndex].randomSpawns[Random.Range(0, level.waves[currentWaveIndex].randomSpawns.Count)];
                    Spawn(true, GameplayWaveIndex);
                    currentSpawnRandomDone++;
                }
            }
            if(currentSpawnOrderedIndex >= level.waves[currentWaveIndex].orderedSpawns.Count && currentSpawnRandomDone >= level.waves[currentWaveIndex].randomSpawnTarget){
                //if we made it through both spawn lists...
                //NEXT Wave and reset quotas
                //was this the last of a logical wave? go to next gameplay wave, remember each enemy will do its own tracking on whether it will add to its corresponding wave
                if(WaveScoreBoard[GameplayWaveIndex].lastLogicalWaveIndex == currentWaveIndex){
                    GameplayWaveIndex++;
                    Debug.Log("STARTING WAVE: " + GameplayWaveIndex);
                    Debug.Log("Previuos WAVE Score: " + WaveScoreBoard[GameplayWaveIndex-1].playerKilled);
                }
                currentWaveIndex++;
                currentSpawnOrderedIndex = 0;
                currentSpawnRandomDone = 0;

            }
        }else{
            //once done with level do the logic to call for then end of level.
            Debug.Log("Level Complete!");
        }

        
        //gameObject.SetActive(false);
    }

    void Spawn(bool isRandom, int ofGameplayWave){
        //tells EnemyManager to Spawn an enemy for them and get it ready!
        Debug.Log("will spawn object from gameplay wave: "+ ofGameplayWave);
        enemyManager.Spawn(currentWaveObjectToSpawn, isRandom, ofGameplayWave);
    }

    void InitializeScoreboard(){
        //where do we get the data for Level1?... another SCRIPTABLE OBJECT???
        //hard writing it for now.

        //haven't validated the adding of a new boundary to be higher than the other.
        WaveScoreBoard = new List<GameplayWaveData>();

        //first test one enemy
        WaveScoreBoard.Add(new GameplayWaveData(3,0));
        //WaveScoreBoard.Add(new GameplayWaveData(4,0));
        //WaveScoreBoard.Add(new GameplayWaveData(3,1));

        //WaveScoreBoard.Add(new GameplayWaveData(3,0));
        //WaveScoreBoard.Add(new GameplayWaveData(3,1));
        //WaveScoreBoard.Add(new GameplayWaveData(1,2));
    }
    static public void ChangeScoreBoard(int gamePlayWave){
        WaveScoreBoard[gamePlayWave].playerKilled += 1;
        Debug.Log("KILLED ONE ENEMY FROM WAVE " + gamePlayWave + " total score : " + WaveScoreBoard[gamePlayWave].playerKilled + " out of " +  WaveScoreBoard[gamePlayWave].playerGoal);

        //should we trigger bonus spawn from here?
        if(WaveScoreBoard[gamePlayWave].playerKilled == WaveScoreBoard[gamePlayWave].playerGoal){
            Debug.Log("CONGRATS YOU DEFEATED ALL IN WAVE: " + gamePlayWave);
        }
    }

    public static void AddToOrderedDelay(float timeAdded){
        orderedDelayEndTime = Time.time + timeAdded;
    }
    public static void AddToRandomDelay(float timeAdded){
        randomDelayEndTime = Time.time + timeAdded;
    }
}
