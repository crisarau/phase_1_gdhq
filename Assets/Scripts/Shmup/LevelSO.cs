using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName = "Game", menuName = "Game/Level")]
public class LevelSO : ScriptableObject
{
    public List<WaveSO> waves;
}
