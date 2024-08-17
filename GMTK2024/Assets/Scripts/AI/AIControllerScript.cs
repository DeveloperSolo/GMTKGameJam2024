using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIMovementScript))]
public partial class AIControllerScript : MonoBehaviour
{
    private AIMovementScript movement;
    private BaseAITargetFinderScript targetFinder;

    private AIBaseStateBehaviour currentBehaviour = null; 
    Dictionary<AIState, AIBaseStateBehaviour> allBehaviours = new Dictionary<AIState, AIBaseStateBehaviour>();

    private void Awake()
    {
        movement = GetComponent<AIMovementScript>();
        targetFinder = GetComponent<BaseAITargetFinderScript>();
        allBehaviours = GenerateBehaviours();
    }

    private void OnEnable()
    {
        SetState(AIState.Wander);
    }

    private void Update()
    {
        if (currentBehaviour != null)
        {
            currentBehaviour.OnUpdate(this, Time.deltaTime);
        }
    }

    private void SetState(AIState newState)
    {
        if(currentBehaviour != null)
        {
            currentBehaviour.OnExit(this);
        }
        if(allBehaviours.TryGetValue(newState, out currentBehaviour))
        {
            currentBehaviour.OnEnter(this);
        }
    }

    #region Movement

    private void MoveToRandomPosition()
    {
        movement.TargetPosition = new Vector2(Random.Range(-5.0f, 5.0f), Random.Range(-5.0f, 5.0f));
    }

    private void MoveToPosition(Vector2 position)
    {
        movement.TargetPosition = position;
    }

    private bool IsMoving()
    {
        return movement.IsMoving;
    }

    #endregion Movement

    #region TargetFinder

    private bool HasTarget()
    {
        return targetFinder != null && targetFinder.GetTarget() != null;
    }

    private GameObject GetTarget()
    {
        return (targetFinder != null) ? targetFinder.GetTarget() : null;
    }

    #endregion
}