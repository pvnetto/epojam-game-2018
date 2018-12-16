using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpPlayerState : PlayerState {

    private static readonly JumpPlayerState singleton = new JumpPlayerState();

    static JumpPlayerState() {
    }

    private JumpPlayerState() {
    }

    public static JumpPlayerState Instance {
        get {
            return singleton;
        }
    }

    public override void Enter(Player player, ref Vector3 velocity) {
        velocity.y = player.controller.maxJumpVelocity;
    }

    public override void Exit(Player player) {  }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {
        float targetVelocityX = inputs.x * player.controller.finalMoveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref player.controller.smoothingVelocityX,
                                      player.controller.controller2D.collisionInfo.below ? player.controller.accelerationTimeGrounded : player.controller.accelerationTimeAirborne);

        if (player.inputDevice.GetControl(PlayerActions.ACTION_1).WasReleased && player.controller.isDashAvailable) {
            player.controller.SwitchState(PlayerController.States.DASHING);
        }
        else if (player.controller.controller2D.collisionInfo.below) {
            player.controller.SwitchState(PlayerController.States.IDLE);
        }
        else if (!player.isStunned)  {
            if ((player.controller.controller2D.collisionInfo.left || player.controller.controller2D.collisionInfo.right)
               || (player.controller.controller2D.collisionInfo.oldLeft || player.controller.controller2D.collisionInfo.oldRight)) {

                player.controller.SwitchState(PlayerController.States.SLIDING);
            }
            else if (player.inputDevice.GetControl(PlayerActions.JUMP).WasReleased) {
                if (velocity.y > player.controller.minJumpVelocity) {
                    velocity.y = player.controller.minJumpVelocity;
                }
            }
        }
    }

}
