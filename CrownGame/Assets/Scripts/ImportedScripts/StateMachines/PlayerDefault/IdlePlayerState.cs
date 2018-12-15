using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdlePlayerState : PlayerState {

    private static readonly IdlePlayerState singleton = new IdlePlayerState();

    static IdlePlayerState() {
    }

    private IdlePlayerState() {
    }

    public static IdlePlayerState Instance {
        get {
            return singleton;
        }
    }

    // If the player is airborne, it has 'coyote time' seconds to jump before entering jumping state
    //private float coyoteTime = 0.1f;
    //private float timeAirborne = 0.0f;

    public override void Enter(Player player, ref Vector3 velocity) {
        player.controller.timeAirborne = 0.0f;
    }

    public override void Exit(Player player) {
        player.controller.timeAirborne = 0.0f;
    }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        float targetVelocityX = inputs.x * player.controller.finalMoveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref player.controller.smoothingVelocityX,
                                      player.controller.controller2D.collisionInfo.below ? player.controller.accelerationTimeGrounded : player.controller.accelerationTimeAirborne);

        if (!player.isStunned) {
            if (!player.controller.controller2D.collisionInfo.below) {
                player.controller.timeAirborne += Time.deltaTime;
            }
            if (player.inputDevice.GetControl(PlayerActions.ACTION_1).WasPressed) {
                player.controller.SwitchState(PlayerController.States.DASHING);
            }
            else if (player.inputDevice.GetControl(PlayerActions.JUMP).WasPressed) {
                player.controller.SwitchState(PlayerController.States.JUMPING);
            }
            else if (player.controller.timeAirborne > player.controller.coyoteTime) {
                // TODO: Enter Airborne state
                //controller.SwitchState(PlayerController.States.JUMPING);
            }
            else if (!player.controller.controller2D.collisionInfo.below && (player.controller.controller2D.collisionInfo.left || player.controller.controller2D.collisionInfo.right)) {
                player.controller.SwitchState(PlayerController.States.SLIDING);
            }
        }
    }
}
