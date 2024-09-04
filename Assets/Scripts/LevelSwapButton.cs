using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelSwapButton : MonoBehaviour {

    [SerializeField]
    LevelManager levelManager;

    [SerializeField]
    int toCheck;

    private void Awake()
    {
        LevelMechanics.OnLevelLoaded += OnLevelLoaded;
    }

    private void OnDestroy()
    {
        LevelMechanics.OnLevelLoaded -= OnLevelLoaded;
    }

    private void OnLevelLoaded(int index, string difficulty)
    {
        gameObject.SetActive(true);
        if (index + toCheck < 0 || index + toCheck >= levelManager.GetLevelsCount())
            gameObject.SetActive(false);

    }
}
