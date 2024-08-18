using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class AIMovementScript : MonoBehaviour
{
    [Header("Stats")]
    [SerializeField] private float moveSpeed;
    [SerializeField] private float maxReachDistance;

    private bool isMoving = false;
    public bool IsMoving { get { return isMoving; } }

    private Vector2 targetPosition;
    public Vector2 TargetPosition { get { return targetPosition; } set { targetPosition = value; isMoving = true; } }

    [Header("Components")]

    private Rigidbody2D rbody;

    private void Awake()
    {
        rbody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if(!isMoving)
        {
            return;
        }

        Vector2 posDiff = targetPosition - (Vector2)transform.position;
        float distanceLeft = posDiff.magnitude;
        float maxDistanceLeft = Mathf.Max(maxReachDistance, 0.0f);
        if(distanceLeft < maxDistanceLeft)
        {
            rbody.velocity = Vector2.zero;
            isMoving = false;
            return;
        }
        rbody.velocity = posDiff / distanceLeft * moveSpeed;
    }

    public void Interrupt()
    {
        isMoving = false;
        if(rbody != null)
        {
            rbody.velocity = Vector2.zero;
        }
    }

    public void GetValueForInfoDisplay(EntityInfoScript.Info info)
    {
        info.InfoValue = moveSpeed.ToString("F1");
    }
}