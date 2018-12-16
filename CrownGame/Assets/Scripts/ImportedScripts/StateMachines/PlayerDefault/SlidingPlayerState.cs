using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlidingPlayerState : PlayerState {

    private static readonly SlidingPlayerState singleton = new SlidingPlayerState();

    static SlidingPlayerState() {
    }

    private SlidingPlayerState() {
    }

    public static SlidingPlayerState Instance {
        get {
            return singleton;
        }
    }

    public override void Enter(Player player, ref Vector3 velocity) {
    }

    public override void Exit(Player player, ref Vector3 velocity) {
    }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        int wallDirectionX = player.controller.controller2D.collisionInfo.left ? -1 : 1;

        float targetVelocityX = inputs.x * player.controller.finalMoveSpeed;
        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref player.controller.smoothingVelocityX,
                                      player.controller.controller2D.collisionInfo.below ? player.controller.accelerationTimeGrounded : player.controller.accelerationTimeAirborne);

        if (player.controller.controller2D.collisionInfo.below || player.isStunned) {
            player.controller.SwitchState(PlayerController.States.IDLE);
        }
        else if(!player.controller.controller2D.collisionInfo.right && !player.controller.controller2D.collisionInfo.left
            && !player.controller.controller2D.collisionInfo.oldRight && !player.controller.controller2D.collisionInfo.oldLeft) {
            player.controller.SwitchState(PlayerController.States.IDLE);
        }
        else {
            if (velocity.y < -player.controller.maxWallSlideSpeed) {
                velocity.y = -player.controller.maxWallSlideSpeed;
            }
            if (player.controller.wallUnstickTime > 0) {
                player.controller.smoothingVelocityX = 0;
                velocity.x = 0;

                if (inputs.x != wallDirectionX && inputs.x != 0) {
                    player.controller.wallUnstickTime -= Time.deltaTime;
                }
                else {
                    player.controller.wallUnstickTime = player.controller.wallStickTime;
                }
            }
            else {
                player.controller.wallUnstickTime = player.controller.wallStickTime;
            }

            if (player.inputDevice.GetControl(PlayerActions.JUMP).WasPressed) {
                player.controller.SwitchState(PlayerController.States.WALL_JUMPING);
            }
        }
    }

}
