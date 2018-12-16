using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(FlashController))]
public class Bunny : MovingEntity, IHittable {

    [Header("Crown parameters")]
    public GameObject crownPrefab;
    [Range(0.0f, 30.0f)]
    public float crownDropSpeed = 20.0f;

    [Header("Bunny movement parameters")]
    public float speed = 8.0f;
    public Vector2 wallBumpLeap;

    private SpriteRenderer spriteRenderer;
    private AnimalState currentState;
    private FlashController flashController;

    protected override void Start() {
        base.Start();
        spriteRenderer = GetComponent<SpriteRenderer>();
        flashController = GetComponent<FlashController>();

        currentState = BunnyIdleState.Instance;
    }

    // TODO: Move Hit to AnimalState. Ex: Tortoise can be hit 3 times
    public void Hit(GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce) {
        if (attacker.GetComponent<Player>()) {
            DropCrown();
            Destroy(gameObject);
        }
    }

    private void DropCrown() {
        GameObject crownGO = Instantiate(crownPrefab, transform.position, Quaternion.identity);
        Crown crown = crownGO.GetComponent<Crown>();

        Vector2 crownDirection = new Vector2(Random.Range(-1.0f, 1.0f) * crownDropSpeed, crownDropSpeed);
        crown.AddKnockbackForce(crownDirection);
    }

    public bool isAlly(int id)
    {
        return false;
    }

    private void FlipSpriteDirection() {

        // Flips the bunny according to the direction
        bool isFlipped = direction.x < 0;

        spriteRenderer.flipX = isFlipped;
    }

    protected override void Update() {

        float targetVelocityX = direction.x * moveSpeed;

        currentState.Update(this, ref velocity);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime, false);

        if (controller.collisionInfo.above || controller.collisionInfo.below) {
            velocity.y = 0;
        }

        FlipSpriteDirection();
    }
}
