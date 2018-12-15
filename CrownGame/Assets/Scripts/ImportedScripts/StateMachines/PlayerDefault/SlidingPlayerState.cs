using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPlayerState : PlayerState {

    public float maxWallSlideSpeed;
    public Vector2 wallLeap;
    public float wallStickTime;
    private float wallUnstickTime;

    public SlidingPlayerState(Player player, float maxWallSlideSpeed, Vector2 wallLeap, float wallStickTime,
        float wallUnstickTime) : base(player) {

        this.maxWallSlideSpeed = maxWallSlideSpeed;
        this.wallLeap = wallLeap;
        this.wallStickTime = wallStickTime;
        this.wallUnstickTime = wallUnstickTime;
    }

    protected override void StateUpdate(ref Vector2 inputs, ref Vector3 velocity) {

        int wallDirectionX = controller.controller2D.collisionInfo.left ? -1 : 1;

        float targetVelocityX = inputs.x * controller.finalMoveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref controller.smoothingVelocityX,
                                      controller.controller2D.collisionInfo.below ? controller.accelerationTimeGrounded : controller.accelerationTimeAirborne);

        if (controller.controller2D.collisionInfo.below || owner.isStunned) {
            controller.SwitchState(PlayerController.States.IDLE);
        }
        else if(!controller.controller2D.collisionInfo.right && !controller.controller2D.collisionInfo.left
            && !controller.controller2D.collisionInfo.oldRight && !controller.controller2D.collisionInfo.oldLeft) {
            controller.SwitchState(PlayerController.States.IDLE);
        }
        else {
            if (velocity.y < -maxWallSlideSpeed) {
                velocity.y = -maxWallSlideSpeed;
            }
            if (wallUnstickTime > 0) {
                controller.smoothingVelocityX = 0;
                velocity.x = 0;

                if (inputs.x != wallDirectionX && inputs.x != 0) {
                    wallUnstickTime -= Time.deltaTime;
                }
                else {
                    wallUnstickTime = wallStickTime;
                }
            }
            else {
                wallUnstickTime = wallStickTime;
            }

            if (owner.inputDevice.GetControl(PlayerActions.JUMP).WasPressed) {
                controller.SwitchState(PlayerController.States.WALL_JUMPING);
            }
        }
    }

}
