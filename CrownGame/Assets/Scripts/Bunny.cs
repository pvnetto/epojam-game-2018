using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bunny : MovingEntity, IHittable
{

    [Header("Bunny movement parameters")]
    public float speed = 8.0f;
    public Vector2 wallBumpLeap;

    private AnimalState currentState;
    private FlashController flashController;

    protected override void Start() {
        base.Start();
        flashController = GetComponent<FlashController>();

        currentState = BunnyIdleState.Instance;
    }

    // TODO: Move Hit to AnimalState
    public void Hit(GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce)
    {
        Destroy(gameObject);
    }

    public bool isAlly(int id)
    {
        return false;
    }

    protected override void Update() {
        float targetVelocityX = direction.x * moveSpeed;

        currentState.Update(this, ref velocity);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime, false);

        if (controller.collisionInfo.above || controller.collisionInfo.below) {
            velocity.y = 0;
        }
    }
}
