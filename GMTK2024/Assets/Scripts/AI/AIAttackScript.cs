using System;
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
    [SerializeField] private float attackAngle;
    [SerializeField] private SpawnManager bulletManager;

    private BaseAITargetFinderScript targetFinder;

    private float attackWaveDuration;
    private float attackWaveCount;
    private float timeTillNextAttack = 0.0f;
    private float attackCountRemainder;

    private ScaleMechanicComponent scalableOwner;

    private void Awake()
    {
        targetFinder = GetComponent<BaseAITargetFinderScript>();
        scalableOwner = GetComponentInChildren<ScaleMechanicComponent>();
        CalculateAttackRate();
    }

    private void OnEnable()
    {
        CalculateAttackRate();
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
        timeTillNextAttack += attackWaveDuration;
    }

    private void SpawnAttack()
    {
        GameObject target = targetFinder.GetTarget();
        Vector2 targetDir = (target.transform.position - transform.position).normalized;

        attackCountRemainder += attackWaveCount;
        int count = Mathf.FloorToInt(attackCountRemainder);
        attackCountRemainder -= count;

        float angle = -attackAngle * (((float)count / 2) - 0.5f);
        for (int i = 0; i < count; ++i)
        {
            float rad = Mathf.Deg2Rad * angle;
            Vector2 dir = new Vector2(
                targetDir.x * Mathf.Cos(rad) - targetDir.y * Mathf.Sin(rad),
                targetDir.x * Mathf.Sin(rad) + targetDir.y * Mathf.Cos(rad)
            );

            GameObject instance = bulletManager.SpawnInstance();
            instance.transform.position = transform.position;
            instance.GetComponent<BulletScript>().Initialize(dir, attackDamage, maxAttackRange, scalableOwner);

            angle += attackAngle;
        }
    }

    private void CalculateAttackRate()
    {
        if (attackSpeed <= 0)
        {
            attackWaveDuration = 0.0f;
            attackWaveCount = 0.0f;
            return;
        }

        attackWaveDuration = 1.0f / attackSpeed;
        attackWaveCount = Mathf.Ceil(attackSpeed);
        attackWaveDuration *= attackWaveCount;
    }

    public void GetDamageValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = attackDamage.ToString();
    }

    public void GetSpeedValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = attackSpeed.ToString("F2") + "\n(" + attackWaveCount + " in " + attackWaveDuration.ToString("F1") + "s)";
    }

    public void SetDamageFromScaling(float newDamage)
    {
        attackDamage = Mathf.FloorToInt(newDamage);
    }

    public void SetSpeedFromScaling(float newSpeed)
    {
        attackSpeed = newSpeed;
        CalculateAttackRate();
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