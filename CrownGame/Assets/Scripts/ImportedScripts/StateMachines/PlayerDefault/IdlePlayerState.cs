using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePlayerState : PlayerState {

    public IdlePlayerState(Player owner) : base(owner) {}

    protected override void StateUpdate(ref Vector2 inputs, ref Vector3 velocity) {

        float targetVelocityX = inputs.x * controller.finalMoveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref controller.smoothingVelocityX,
                                      controller.controller2D.collisionInfo.below ? controller.accelerationTimeGrounded : controller.accelerationTimeAirborne);

        if (!owner.isStunned) {
            if (owner.inputDevice.GetControl(PlayerActions.JUMP).WasPressed) {
                controller.SwitchState(PlayerController.States.JUMPING);
            }
            else if (!collisionInfo.below && (collisionInfo.left || collisionInfo.right)) {
                controller.SwitchState(PlayerController.States.SLIDING);
            }
        }
    }
}
