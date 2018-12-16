using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJumpPlayerState : PlayerState {

    private static readonly WallJumpPlayerState singleton = new WallJumpPlayerState();

    static WallJumpPlayerState() {
    }

    private WallJumpPlayerState() {
    }

    public static WallJumpPlayerState Instance {
        get {
            return singleton;
        }
    }

    public override void Enter(Player player, ref Vector3 velocity) {
        player.controller.wallJumpEndTime = Time.time + player.controller.wallJumpDuration;

        int wallDirectionX = player.controller.controller2D.collisionInfo.left ? -1 : 1;
        velocity.x = -wallDirectionX * player.controller.wallLeap.x;
        velocity.y = player.controller.wallLeap.y;
    }

    public override void Exit(Player player, ref Vector3 velocity) {  }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        inputs = inputs * player.controller.speedRatio;

        float targetVelocityX = inputs.x * player.controller.finalMoveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref player.controller.smoothingVelocityX,
                                      player.controller.controller2D.collisionInfo.below ? player.controller.accelerationTimeGrounded : player.controller.accelerationTimeAirborne);

        if (player.controller.controller2D.collisionInfo.below) {
            player.controller.SwitchState(PlayerController.States.IDLE);
        }
        else if (!player.isStunned) {
            if ((player.controller.controller2D.collisionInfo.left || player.controller.controller2D.collisionInfo.right)) {
                player.controller.SwitchState(PlayerController.States.SLIDING);
            }
        }

    }

}
