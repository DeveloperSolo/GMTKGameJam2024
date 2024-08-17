using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class AIControllerScript : MonoBehaviour
{
    private enum AIState
    {
        None,
        Wander,
        Pursue,
    }

    private static Dictionary<AIState, AIBaseStateBehaviour> GenerateBehaviours()
    {
        Dictionary<AIState, AIBaseStateBehaviour> behaviours = new Dictionary<AIState, AIBaseStateBehaviour>();
        GenerateBehaviourToDictionary<AIWanderStateBehaviour>(behaviours);
        GenerateBehaviourToDictionary<AIPursueStateBehaviour>(behaviours);
        return behaviours;
    }

    private class AIPursueStateBehaviour : AIBaseStateBehaviour
    {
        public override AIState State => AIState.Pursue;

        public override void OnEnter(AIControllerScript controller)
        {
            base.OnEnter(controller);
        }

        public override void OnExit(AIControllerScript controller)
        {
            base.OnExit(controller);
        }

        public override void OnUpdate(AIControllerScript controller, float elapsed)
        {
            base.OnUpdate(controller, elapsed);

            GameObject target = controller.GetTarget();

            if (target == null)
            {
                controller.SetState(AIState.Wander);
                return;
            }

            controller.MoveToPosition(target.transform.position);
        }
    }

    private class AIWanderStateBehaviour : AIBaseStateBehaviour
    {
        public override AIState State => AIState.Wander;

        private float wait;

        public override void OnEnter(AIControllerScript controller)
        {
            base.OnEnter(controller);

            controller.MoveToRandomPosition();
            wait = 1.0f;
        }

        public override void OnExit(AIControllerScript controller)
        {
            base.OnExit(controller);
        }

        public override void OnUpdate(AIControllerScript controller, float elapsed)
        {
            base.OnUpdate(controller, elapsed);

            if(controller.HasTarget())
            {
                controller.SetState(AIState.Pursue);
                return;
            }

            if(controller.IsMoving())
            {
                return;
            }

            if(wait > 0.0f)
            {
                wait -= elapsed;
                return;
            }

            controller.SetState(AIState.Wander);
        }
    }

    private class AIBaseStateBehaviour
    {
        public virtual AIState State => AIState.None;

        public virtual void OnEnter(AIControllerScript controller) { }
        public virtual void OnExit(AIControllerScript controller) { }
        public virtual void OnUpdate(AIControllerScript controller, float elapsed) { }
    }

    private static void GenerateBehaviourToDictionary<BehaviourType>(Dictionary<AIState, AIBaseStateBehaviour> dictionary) where BehaviourType : AIBaseStateBehaviour, new()
    {
            BehaviourType behaviour = new BehaviourType();
            dictionary.Add(behaviour.State, behaviour);
    }
}