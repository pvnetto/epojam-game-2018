using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyCrownState : PlayerState {

    private static readonly BunnyCrownState singleton = new BunnyCrownState();

    static BunnyCrownState() {
    }

    private BunnyCrownState() {
    }

    public static BunnyCrownState Instance {
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
        Bunny bunny = collision.GetComponent<Bunny>();

        if (bunny) {
            HitRecord hitRecord = new HitRecord();
            bunny.Hit(player, ref hitRecord, Vector2.zero);
        }
    }

    public override void Enter(Player player, ref Vector3 velocity) {
        player.equippedCrown.isDashBack = false;
        player.equippedCrown.currentDashDuration = 0.0f;
    }

    public override void Exit(Player player) {
        player.controller.currentDashChargeTime = 0.0f;
    }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        player.equippedCrown.currentDashDuration += Time.deltaTime;

        //inputs = inputs * maxSpeed;
        Vector2 dashLeap = player.equippedCrown.dashLeap;
        velocity = dashLeap * player.equippedCrown.dashSpeed * player.controller.dashCharge;

        if (player.equippedCrown.currentDashDuration >= player.equippedCrown.maxDashDuration) {
            if (player.controller.controller2D.collisionInfo.below) {
                player.controller.SwitchState(PlayerController.States.IDLE);
            }
            else {
                player.controller.SwitchState(PlayerController.States.AIRBORNE);
            }
        }

    }

}
