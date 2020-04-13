using System.Collections.Generic;
using UnityEngine;

public class PrefabReplicator : MonoBehaviour
{
    public GameObject prefab;
    public int numberOfInstances = 1;
    public Vector3 offset;

    void Awake()
    {
        for (int i = 1; i < numberOfInstances; i++)
        {
            Instantiate(
                prefab,
                prefab.transform.position + offset * i,
                prefab.transform.rotation,
                prefab.transform.parent
            );
        }
    }
}
