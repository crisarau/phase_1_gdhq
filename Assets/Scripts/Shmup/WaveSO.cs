using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Game", menuName = "Game/Wave")]
public class WaveSO : ScriptableObject
{

    public List<WaveEntitySO> orderedSpawns;
    public List<WaveEntitySO> randomSpawns;

    public enum WaveBehavior{
        KeepGoing, Stop, ChangeSpeed
    }
    public WaveBehavior StartBehavior;
    public WaveBehavior EndBehavior;
    public float endDelay;
    public float StartSpeedChange;
    public float EndSpeedChange;
}
