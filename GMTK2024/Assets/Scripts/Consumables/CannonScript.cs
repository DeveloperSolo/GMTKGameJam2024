using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    private float timeTillNextAttack = 1.0f;
    [SerializeField]
    private float attackSpeed = 1.0f;
    [SerializeField] 
    private SpawnManager cannonBallSpawner;
    [SerializeField]
    private GameObject cannon;
    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float range = 1000.0f;
    private ScaleMechanicComponent scalableOwner;

    // Start is called before the first frame update
    void Start()
    {
        scalableOwner = GetComponentInChildren<ScaleMechanicComponent>();
        timeTillNextAttack = 1.0f / attackSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        TryAttack(Time.deltaTime);
    }

    private Vector3 GetTargetPos()
    {
        return Vector3.zero;
    }

    private void TryAttack(float elapsed)
    {
        timeTillNextAttack -= elapsed;
        if (timeTillNextAttack > 0.0f)
        {
            return;
        }

        SpawnAttack();
        timeTillNextAttack = 1.0f/attackSpeed;
    }

    private void SpawnAttack()
    {
        Vector3 target = GetTargetPos();
        Vector2 targetDir = (target - transform.position).normalized;

        GameObject instance = cannonBallSpawner.SpawnInstance();
        instance.transform.position = transform.position;
        instance.GetComponent<BulletScript>().Initialize(cannon.transform.up, damage, range, scalableOwner);

    }
}
