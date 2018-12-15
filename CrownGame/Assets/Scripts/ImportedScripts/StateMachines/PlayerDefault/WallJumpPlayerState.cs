using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpPlayerState : PlayerState {

    public Vector2 wallLeap;
    public float leapDuration;

    private float wallJumpMomentumEnd;
    private float speedRatio {
        get {
            if(Time.time >= wallJumpMomentumEnd || leapDuration == 0) {
                return 1.0f;
            }
            else {
                return 1.0f - ((wallJumpMomentumEnd - Time.time) / leapDuration);
            }
        }
    }

    public WallJumpPlayerState(Player owner, Vector2 wallLeap, float duration, ref Vector3 velocity) : base(owner) {
        this.wallLeap = wallLeap;
        this.leapDuration = duration;
        this.wallJumpMomentumEnd = Time.time + leapDuration;

        StateEnter(ref velocity);
    }

    protected void StateEnter(ref Vector3 velocity) {
        int wallDirectionX = controller.controller2D.collisionInfo.left ? -1 : 1;
        velocity.x = -wallDirectionX * wallLeap.x;
        velocity.y = wallLeap.y;
    }

    protected override void StateUpdate(ref Vector2 inputs, ref Vector3 velocity) {

        inputs = inputs * speedRatio;

        float targetVelocityX = inputs.x * controller.finalMoveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref controller.smoothingVelocityX,
                                      controller.controller2D.collisionInfo.below ? controller.accelerationTimeGrounded : controller.accelerationTimeAirborne);

        if (controller.controller2D.collisionInfo.below) {
            controller.SwitchState(PlayerController.States.IDLE);
            return;
        }
        else if (!owner.isStunned) {
            if ((controller.controller2D.collisionInfo.left || controller.controller2D.collisionInfo.right)) {
                controller.SwitchState(PlayerController.States.SLIDING);
                return;
            }
        }

    }

}
