using System.Collections;
using System.Collections.Generic;
using UnityEngine;

partial class AIControllerScript : MonoBehaviour
{
    public enum AIState
    {
        None,
        Scaling,
        Wander,
        Pursue,
        Attack,
        Retreat,
        Damaged,
    }

    private static Dictionary<AIState, AIBaseStateBehaviour> GenerateBehaviours()
    {
        Dictionary<AIState, AIBaseStateBehaviour> behaviours = new Dictionary<AIState, AIBaseStateBehaviour>();
        GenerateBehaviourToDictionary<AIScalingStateBehaviour>(behaviours);
        GenerateBehaviourToDictionary<AIWanderStateBehaviour>(behaviours);
        GenerateBehaviourToDictionary<AIPursueStateBehaviour>(behaviours);
        GenerateBehaviourToDictionary<AIAttackStateBehaviour>(behaviours);
        GenerateBehaviourToDictionary<AIRetreatStateBehaviour>(behaviours);
        GenerateBehaviourToDictionary<AIDamagedStateBehaviour>(behaviours);
        return behaviours;
    }

    private class AIDamagedStateBehaviour : AIBaseStateBehaviour
    {
        public override AIState State => AIState.Damaged;
        private float wait;

        public override void OnEnter(AIControllerScript controller)
        {
            base.OnEnter(controller);
            controller.scaleMechanic.DisableDraggingGizmo();
            wait = 0.5f;
        }

        public override void OnExit(AIControllerScript controller)
        {
            base.OnExit(controller);
            controller.scaleMechanic.EnableDraggingGizmo();
        }

        public override void OnUpdate(AIControllerScript controller, float elapsed)
        {
            base.OnUpdate(controller, elapsed);
            if(wait > 0.0f)
            {
                wait -= elapsed;
                return;
            }

            controller.ResetState();
        }
    }

    private class AIRetreatStateBehaviour : AIBaseStateBehaviour
    {
        public override AIState State => AIState.Retreat;

        public override void OnEnter(AIControllerScript controller)
        {
            base.OnEnter(controller);
            controller.attack.enabled = true;
        }

        public override void OnExit(AIControllerScript controller)
        {
            base.OnExit(controller);
            controller.attack.enabled = false;
        }

        public override void OnUpdate(AIControllerScript controller, float elapsed)
        {
            base.OnUpdate(controller, elapsed);

            if (controller.scaleMechanic.IsDraggingGizmo())
            {
                controller.SetState(AIState.Scaling);
                return;
            }

            if (controller.attack != null)
            {
                if (!controller.attack.HasTargetTooClose())
                {
                    if(controller.attack.HasTargetInRange())
                    {
                        controller.SetState(AIState.Attack);
                    }
                    else
                    {
                        controller.SetState(AIState.Pursue);
                    }
                    return;
                }
            }

            GameObject target = controller.GetTarget();
            if (target == null)
            {
                controller.SetState(AIState.Wander);
                return;
            }

            Vector3 targetPos = controller.transform.position - (target.transform.position - controller.transform.position);
            controller.MoveToPosition(targetPos);
        }
    }

    private class AIAttackStateBehaviour : AIBaseStateBehaviour
    {
        public override AIState State => AIState.Attack;

        public override void OnEnter(AIControllerScript controller)
        {
            base.OnEnter(controller);
            controller.attack.enabled = true;
        }

        public override void OnExit(AIControllerScript controller)
        {
            base.OnExit(controller);
            controller.attack.enabled = false;
        }

        public override void OnUpdate(AIControllerScript controller, float elapsed)
        {
            base.OnUpdate(controller, elapsed);

            if (controller.scaleMechanic.IsDraggingGizmo())
            {
                controller.SetState(AIState.Scaling);
                return;
            }

            if (controller.attack != null)
            {
                if(controller.attack.HasTargetTooClose())
                {
                    controller.SetState(AIState.Retreat);
                    return;
                }
                else if (!controller.attack.HasTargetInRange())
                {
                    controller.SetState(AIState.Pursue);
                    return;
                }
            }

            GameObject target = controller.GetTarget();
            if (target == null)
            {
                controller.SetState(AIState.Wander);
                return;
            }

            //controller.MoveToPosition(target.transform.position);
        }
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

            if(controller.scaleMechanic.IsDraggingGizmo())
            {
                controller.SetState(AIState.Scaling);
                return;
            }

            if(controller.attack != null)
            {
                if(controller.attack.HasTargetInRange())
                {
                    controller.SetState(AIState.Attack);
                    return;
                }
            }

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
        private float maxWanderDuration;

        public override void OnEnter(AIControllerScript controller)
        {
            base.OnEnter(controller);

            controller.MoveToRandomPosition();
            wait = Random.Range(0.5f, 3.0f);
            maxWanderDuration = 5.0f;
        }

        public override void OnExit(AIControllerScript controller)
        {
            base.OnExit(controller);
        }

        public override void OnUpdate(AIControllerScript controller, float elapsed)
        {
            base.OnUpdate(controller, elapsed);

            if(controller.scaleMechanic.IsDraggingGizmo())
            {
                controller.SetState(AIState.Scaling);
                return;
            }
            if(controller.HasTarget())
            {
                controller.SetState(AIState.Pursue);
                return;
            }

            if (maxWanderDuration > 0.0f)
            {
                maxWanderDuration -= elapsed;
                if(maxWanderDuration <= 0.0f)
                {
                    controller.SetState(AIState.Wander);
                    return;
                }
            }
            if (controller.movement.IsMoving)
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

    private class AIScalingStateBehaviour : AIBaseStateBehaviour
    {
        public override AIState State => AIState.Scaling;

        public override void OnUpdate(AIControllerScript controller, float elapsed)
        {
            base.OnUpdate(controller, elapsed);
            if(!controller.scaleMechanic.IsDraggingGizmo())
            {
                controller.ResetState();
                return;
            }
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