using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AirborneState : PlayerState {

    private static readonly AirborneState singleton = new AirborneState();

    static AirborneState() {
    }

    private AirborneState() {
    }

    public static AirborneState Instance {
        get {
            return singleton;
        }
    }

    public override void Enter(Player player, ref Vector3 velocity) {   }

    public override void Exit(Player player) {  }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        float targetVelocityX = inputs.x * player.controller.finalMoveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref player.controller.smoothingVelocityX,
                                      player.controller.controller2D.collisionInfo.below ? player.controller.accelerationTimeGrounded : player.controller.accelerationTimeAirborne);

        if (!player.isStunned) {
            if (player.inputDevice.GetControl(PlayerActions.ACTION_1).WasReleased && player.controller.isDashAvailable) {
                player.controller.SwitchState(PlayerController.States.DASHING);
            }
            else if (player.controller.controller2D.collisionInfo.below) {
                player.controller.SwitchState(PlayerController.States.IDLE);
            }
            else if (!player.controller.controller2D.collisionInfo.below && (player.controller.controller2D.collisionInfo.left || player.controller.controller2D.collisionInfo.right)) {
                player.controller.SwitchState(PlayerController.States.SLIDING);
            }
        }
    }
}
