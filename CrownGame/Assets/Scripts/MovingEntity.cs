using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Controller2D))]
public abstract class MovingEntity : MonoBehaviour {

    [Header("Controller settings")]
    [Range(0, 50)]
    public float moveSpeed;
    public float jumpMinHeight = 1.5f;
    public float jumpMaxHeight = 4;
    public float timeToJumpMaxHeight = 0.4f;
    protected float maxJumpVelocity {
        get {
            return GetMaxJumpVelocity();
        }
    }
    protected float minJumpVelocity {
        get {
            return GetMinJumpVelocity();
        }
    }

    protected float gravity;
    internal float smoothingVelocityX;

    internal float accelerationTimeAirborne = 0.2f;
    internal float accelerationTimeGrounded = 0.1f;

    internal Vector2 direction = Vector2.right;
    internal Controller2D controller;
    protected Vector2 velocity;

    public Controller2D.CollisionInfo collisionInfo {
        get {
            return controller.collisionInfo;
        }
    }

    protected virtual void Start() {
        controller = GetComponent<Controller2D>();
        gravity = -(2 * jumpMaxHeight) / Mathf.Pow(timeToJumpMaxHeight, 2);
    }

    /*Should be overriden to change maxJumpVelocity property return value*/
    protected virtual float GetMaxJumpVelocity() {
        return Mathf.Abs(gravity) * timeToJumpMaxHeight;
    }

    /*Should be overriden to change minJumpVelocity property return value*/
    protected virtual float GetMinJumpVelocity() {
        return Mathf.Sqrt(2 * Mathf.Abs(gravity) * jumpMinHeight);
    }

    protected virtual void Update() {
        float targetVelocityX = direction.x * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref smoothingVelocityX,
                                      controller.collisionInfo.below ? accelerationTimeGrounded : accelerationTimeAirborne);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime, false);

        if (controller.collisionInfo.above || controller.collisionInfo.below) {
            velocity.y = 0;
        }
    }

}
