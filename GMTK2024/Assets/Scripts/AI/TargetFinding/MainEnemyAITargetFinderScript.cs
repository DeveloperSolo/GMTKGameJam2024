using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainEnemyAITargetFinderScript : BaseAITargetFinderScript
{
    private void Start()
    {
        target = GameController.Instance.MainEnemy;
    }
}
