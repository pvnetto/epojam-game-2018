using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePlayerState : PlayerState {

    public IdlePlayerState(Player owner) : base(owner) {}

    // If the player is airborne, it has 'coyote time' seconds to jump before entering jumping state
    private float coyoteTime = 0.1f;
    private float timeAirborne = 0.0f;

    protected override void StateUpdate(ref Vector2 inputs, ref Vector3 velocity) {

        float targetVelocityX = inputs.x * controller.finalMoveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref controller.smoothingVelocityX,
                                      controller.controller2D.collisionInfo.below ? controller.accelerationTimeGrounded : controller.accelerationTimeAirborne);

        if (!owner.isStunned) {
            if (!collisionInfo.below) {
                timeAirborne += Time.deltaTime;
            }
            if (owner.inputDevice.GetControl(PlayerActions.ACTION_1).WasPressed) {
                controller.SwitchState(PlayerController.States.DASHING);
            }
            else if (owner.inputDevice.GetControl(PlayerActions.JUMP).WasPressed) {
                controller.SwitchState(PlayerController.States.JUMPING);
            }
            else if (timeAirborne > coyoteTime) {
                // TODO: Enter Airborne state
                //controller.SwitchState(PlayerController.States.JUMPING);
            }
            else if (!collisionInfo.below && (collisionInfo.left || collisionInfo.right)) {
                controller.SwitchState(PlayerController.States.SLIDING);
            }
        }
    }
}
