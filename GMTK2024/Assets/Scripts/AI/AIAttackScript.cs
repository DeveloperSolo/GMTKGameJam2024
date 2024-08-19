using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BaseAITargetFinderScript))]
public class AIAttackScript : MonoBehaviour
{
    [SerializeField] private float minAttackRange;
    [SerializeField] private float maxAttackRange;
    [SerializeField] private int attackDamage;
    [SerializeField] private float attackSpeed;
    [SerializeField] private GameObject attackPrefab;

    private BaseAITargetFinderScript targetFinder;
    private float timeTillNextAttack = 0.0f;

    private ScaleMechanicComponent scalableOwner;

    private void Awake()
    {
        targetFinder = GetComponent<BaseAITargetFinderScript>();
        scalableOwner = GetComponentInChildren<ScaleMechanicComponent>();
    }

    private void OnEnable()
    {
        timeTillNextAttack = 0.0f;
    }

    private void Update()
    {
        if(HasTargetInRange())
        {
            TryAttack(Time.deltaTime);
        }
    }

    public bool HasTargetInRange()
    {
        GameObject target = targetFinder.GetTarget();
        if (target == null)
        {
            return false;
        }
        return (target.transform.position - transform.position).sqrMagnitude <= maxAttackRange * maxAttackRange;
    }

    public bool HasTargetTooClose()
    {
        GameObject target = targetFinder.GetTarget();
        if (target == null)
        {
            return false;
        }
        return (target.transform.position - transform.position).sqrMagnitude <= minAttackRange * minAttackRange;
    }

    private void TryAttack(float elapsed)
    {
        timeTillNextAttack -= elapsed;
        if(timeTillNextAttack > 0.0f)
        {
            return;
        }

        SpawnAttack();
        timeTillNextAttack += 1.0f / attackSpeed;
        Debug.Log("next attack: " + timeTillNextAttack);
    }

    private void SpawnAttack()
    {
        GameObject target = targetFinder.GetTarget();
        Vector2 dir = (target.transform.position - transform.position).normalized;

        GameObject instance = Instantiate(attackPrefab);
        instance.transform.position = transform.position;
        instance.GetComponent<BulletScript>().Initialize(dir, attackDamage, scalableOwner);
    }

    public void GetDamageValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = attackDamage.ToString();
    }

    public void GetSpeedValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = attackSpeed.ToString();
    }

    public void SetDamageFromScaling(float newDamage)
    {
        attackDamage = Mathf.FloorToInt(newDamage);
    }

    public void SetSpeedFromScaling(float newSpeed)
    {
        attackSpeed = newSpeed;
    }

    public void SetMinRangeFromScaling(float value)
    {
        minAttackRange = value;
    }

    public void SetMaxRangeFromScaling(float value)
    {
        maxAttackRange = value;
    }
}