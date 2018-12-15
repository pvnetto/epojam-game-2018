using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashState : PlayerState {

    private static readonly DashState singleton = new DashState();

    static DashState() {
    }

    private DashState() {
    }

    public static DashState Instance {
        get {
            return singleton;
        }
    }

    //public Vector2 dashLeap;
    //public float dashDuration;
    //private float currentDuration = 0.0f;

    //private float maxSpeed;

    //public DashState(Player owner, Vector2 dashLeap, float dashDuration, float maxSpeed, ref Vector3 velocity) : base(owner) {
    //    this.dashLeap = dashLeap;
    //    this.dashDuration = dashDuration;
    //    this.maxSpeed = maxSpeed;
    //}

    public override void Enter(Player player, ref Vector3 velocity) {
        int wallDirectionX = player.controller.controller2D.collisionInfo.left ? -1 : 1;
        velocity.x = -wallDirectionX * player.controller.dashLeap.x;
        velocity.y = player.controller.dashLeap.y;
    }

    public override void Exit(Player player) {
    }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        player.controller.currentDashCooldown += Time.deltaTime;

        //inputs = inputs * maxSpeed;

        float targetVelocityX = player.controller.dashLeap.x * player.controller.dashSpeed;

        velocity.x = targetVelocityX;
        //velocity.x = Mathf.SmoothDamp(velocity.x,
        //                              targetVelocityX,
        //                              ref controller.smoothingVelocityX,
        //                              controller.controller2D.collisionInfo.below ? controller.accelerationTimeGrounded : controller.accelerationTimeAirborne);

        velocity.y = player.controller.dashLeap.y * player.controller.dashSpeed;

        if (player.controller.currentDashCooldown >= player.controller.dashDuration) {
            if (player.controller.controller2D.collisionInfo.below) {
                player.controller.SwitchState(PlayerController.States.IDLE);
            }
            else {
                // TODO: Switch to Airborne
                player.controller.SwitchState(PlayerController.States.IDLE);
            }
        }

    }

}
