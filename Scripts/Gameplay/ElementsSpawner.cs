using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElementsSpawner : MonoBehaviour, IInitializable
{
    [SerializeField] private List<GameObject> _levelPrefab;

    public void Initialize()
    {
        InitializeLevel(LevelManager.Instance.CurrentLevelId-1);
    }
    private void InitializeLevel(int levelId)
    {
        Instantiate(_levelPrefab[levelId]);
    }
}
