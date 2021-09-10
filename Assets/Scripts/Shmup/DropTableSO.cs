using System.Collections;
using System.Collections.Generic;
using UnityEngine;



[CreateAssetMenu(fileName = "DropTableSO", menuName = "GoGalactic/DropTableSO", order = 0)]
public class DropTableSO : ScriptableObject {
    public List<Drop> drops;

    //does this provide a shallow copy? to be used only on reset and initialization of the live one
    public Drop GetDrop(int index){
        Drop temp = new Drop();
        temp.weight = drops[index].weight; 
        temp.prefab = drops[index].prefab;
        return temp;
    }
}

[System.Serializable]
public class Drop{
    public float weight;
    public GameObject prefab;
}
