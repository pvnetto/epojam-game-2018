using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Npc : MonoBehaviour, IHittable
{
    private Vector3 initialPosition;
    private Vector3 targetPosition;
    private float direction;

    [SerializeField] private float movementRange;
    [SerializeField] private float speed;

    private void Start()
    {
        initialPosition = transform.position;
        targetPosition = new Vector3(initialPosition.x + movementRange, initialPosition.y, initialPosition.z);
        direction = 1;
    }

    private void FixedUpdate()
    {
        if (transform.position == targetPosition)
        {
            direction *= -1;
            targetPosition = new Vector3(initialPosition.x + (direction * movementRange), initialPosition.y, initialPosition.z);
        }
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
    }

    public void Hit(GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce)
    {
        Destroy(gameObject);
    }

    public bool isAlly(int id)
    {
        return false;
    }
}
