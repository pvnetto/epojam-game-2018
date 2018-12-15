using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState {

    public Vector2 dashLeap;
    public float dashDuration;
    private float currentDuration = 0.0f;

    private float maxSpeed;

    public DashState(Player owner, Vector2 dashLeap, float dashDuration, float maxSpeed, ref Vector3 velocity) : base(owner) {
        this.dashLeap = dashLeap;
        this.dashDuration = dashDuration;
        this.maxSpeed = maxSpeed;
    }

    protected void StateEnter(ref Vector3 velocity) {
        int wallDirectionX = controller.controller2D.collisionInfo.left ? -1 : 1;
        velocity.x = -wallDirectionX * dashLeap.x;
        velocity.y = dashLeap.y;
    }

    protected override void StateUpdate(ref Vector2 inputs, ref Vector3 velocity) {

        currentDuration += Time.deltaTime;

        //inputs = inputs * maxSpeed;

        float targetVelocityX = dashLeap.x * maxSpeed;

        velocity.x = targetVelocityX;
        //velocity.x = Mathf.SmoothDamp(velocity.x,
        //                              targetVelocityX,
        //                              ref controller.smoothingVelocityX,
        //                              controller.controller2D.collisionInfo.below ? controller.accelerationTimeGrounded : controller.accelerationTimeAirborne);

        velocity.y = dashLeap.y * maxSpeed;

        if (currentDuration >= dashDuration) {
            if (collisionInfo.below) {
                controller.SwitchState(PlayerController.States.IDLE);
            }
            else {
                // TODO: Switch to Airborne
                controller.SwitchState(PlayerController.States.IDLE);
            }
        }

    }

}
