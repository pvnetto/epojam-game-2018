using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlayerState : PlayerState {

    private float minJumpVelocity;

    public JumpPlayerState(Player player, float minJumpVelocity, float maxJumpVelocity, ref Vector3 velocity) : base(player) {
        this.minJumpVelocity = minJumpVelocity;
        velocity.y = maxJumpVelocity;
    }

    protected override void StateUpdate(ref Vector2 inputs, ref Vector3 velocity) {
        float targetVelocityX = inputs.x * controller.finalMoveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref controller.smoothingVelocityX,
                                      controller.controller2D.collisionInfo.below ? controller.accelerationTimeGrounded : controller.accelerationTimeAirborne);

        if (controller.controller2D.collisionInfo.below) {
            controller.SwitchState(PlayerController.States.IDLE);
            return;
        }
        else if (!owner.isStunned)  {
            if ((controller.controller2D.collisionInfo.left || controller.controller2D.collisionInfo.right)
               || (controller.controller2D.collisionInfo.oldLeft || controller.controller2D.collisionInfo.oldRight)) {
                controller.SwitchState(PlayerController.States.SLIDING);
                return;
            }
            if (owner.inputDevice.GetControl(PlayerActions.JUMP).WasReleased) {
                if (velocity.y > minJumpVelocity) {
                    velocity.y = minJumpVelocity;
                }
            }
        }
    }

}
