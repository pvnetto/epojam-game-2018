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

    public override void Hit(Player player, GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce) {
        hitRecord.hitObject = player;
        hitRecord.hitObjectID = player.playerID;
        hitRecord.reflected = true;

        player.Knockback(knockbackForce);
    }

    public override void HandleCollision(GameObject player, Collider2D collision) {
        Debug.Log("Handling collision");
        Bunny bunny = collision.GetComponent<Bunny>();

        if (bunny) {
            HitRecord hitRecord = new HitRecord();
            bunny.Hit(player, ref hitRecord, Vector2.zero);
        }
    }

    public override void Enter(Player player, ref Vector3 velocity) {
        player.controller.isDashBack = false;
        player.controller.currentDashDuration = 0.0f;
    }

    public override void Exit(Player player) {
        player.controller.currentDashChargeTime = 0.0f;
    }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        player.controller.currentDashDuration += Time.deltaTime;

        //inputs = inputs * maxSpeed;

        float targetVelocityX = player.controller.dashLeap.x * player.controller.dashSpeed * player.controller.dashCharge;

        velocity.x = targetVelocityX;
        //velocity.x = Mathf.SmoothDamp(velocity.x,
        //                              targetVelocityX,
        //                              ref player.controller.smoothingVelocityX,
        //                              player.controller.controller2D.collisionInfo.below ? player.controller.accelerationTimeGrounded : player.controller.accelerationTimeAirborne);
        
        velocity.y = player.controller.dashLeap.y * player.controller.dashSpeed * player.controller.dashCharge;

        if (player.controller.currentDashDuration >= player.controller.maxDashDuration) {
            if (player.controller.controller2D.collisionInfo.below) {
                player.controller.SwitchState(PlayerController.States.IDLE);
            }
            else {
                player.controller.SwitchState(PlayerController.States.AIRBORNE);
            }
        }

    }

}
