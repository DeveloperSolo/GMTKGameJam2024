using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AIMovementScript))]
public partial class AIControllerScript : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private ScaleMechanicComponent scaleMechanic;

    private AIMovementScript movement;
    private AIAttackScript attack;
    private BaseAITargetFinderScript targetFinder;
    private SpawnerScript bulletSpawner;

    private AIBaseStateBehaviour currentBehaviour = null; 
    Dictionary<AIState, AIBaseStateBehaviour> allBehaviours = new Dictionary<AIState, AIBaseStateBehaviour>();

    private void Awake()
    {
        movement = GetComponent<AIMovementScript>();
        attack = GetComponent<AIAttackScript>();
        bulletSpawner = GetComponent<SpawnerScript>();
        targetFinder = GetComponent<BaseAITargetFinderScript>();
        allBehaviours = GenerateBehaviours();

        if(attack != null)
        {
            attack.enabled = false;
        }
    }

    private void OnEnable()
    {
        ResetState();
    }

    private void Update()
    {
        if (currentBehaviour != null)
        {
            currentBehaviour.OnUpdate(this, Time.deltaTime);
        }
    }

    public void IsDamaged()
    {
        SetState(AIState.Damaged);
    }

    private void ResetState()
    {
        SetState(AIState.Wander);
    }

    private void SetState(AIState newState)
    {
        if(currentBehaviour != null)
        {
            currentBehaviour.OnExit(this);
        }
        movement.Interrupt();
        if (allBehaviours.TryGetValue(newState, out currentBehaviour))
        {
            currentBehaviour.OnEnter(this);
        }
    }

    private AIState GetState()
    {
        return currentBehaviour.State;
    }

    #region Movement

    private void MoveToRandomPosition()
    {
        Vector2 targetPos = Vector2.zero;
        targetPos += new Vector2(Random.Range(-9.0f, 9.0f), Random.Range(-9.0f, 9.0f));
        targetPos = GameController.Instance.ClampToGameArea(targetPos, scaleMechanic.CurrentSize);
        movement.TargetPosition = (Vector2)transform.position + targetPos;
    }

    private void MoveToPosition(Vector2 position)
    {
        movement.TargetPosition = position;
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