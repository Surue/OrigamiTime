using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Spawn point")]
public class Spawnable : ScriptableObject {
    [SerializeField]
    public GameObject prefabToSpawn;

    [System.Serializable]
    public enum Type {
        ENNEMY,
        COLLTECTIBLE
    }

    [SerializeField]
    public Type type = Type.ENNEMY;

}
