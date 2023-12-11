using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventInfoSO", menuName = "Exploration/EventInfoSO", order = 1)]
public class EventInfoSO : ScriptableObject
{
    [field: SerializeField] public string id { get; private set; }

    public string displayName;
    public GameObject[] eventStepPrefabs;

    private void OnValidate()
    {
        #if UNITY_EDITOR
        id = this.name;
        #endif
    }
}
