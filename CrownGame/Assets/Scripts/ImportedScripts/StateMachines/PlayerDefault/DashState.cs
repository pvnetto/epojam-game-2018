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

        // Only reflects if at least 30% of the dash was charged
        hitRecord.reflected = player.controller.dashCharge >= 0.3f;
    }

    // TODO: Merge with BunnyCrownState
    public override void HandleCollision(GameObject player, Collider2D collision, ref Vector3 velocity) {
        Bunny bunny = collision.GetComponent<Bunny>();
        Player enemy = collision.GetComponent<Player>();

        Vector3 knockbackForce = velocity;
        if (bunny) {
            HitRecord hitRecord = new HitRecord();
            bunny.Hit(player, ref hitRecord, knockbackForce);
        }
        else if (enemy) {
            // Trying to hit the enemy player
            HitRecord hitRecord = new HitRecord();
            enemy.Hit(player, ref hitRecord, knockbackForce);

            // If an enemy is hit, the dash stops
            Player self = player.GetComponent<Player>();
            if (self.collisionInfo.below) {
                self.controller.SwitchState(PlayerController.States.IDLE);
            }
            else {
                self.controller.SwitchState(PlayerController.States.AIRBORNE);
            }

            // If the dash was reflected because of a collision with a dashing player, both players are repelled in the opposite facing direction
            if (hitRecord.reflected) {
                Player hitPlayer = hitRecord.hitObject as Player;
                Vector2 reflectDirection = hitPlayer.transform.position - player.transform.position;
                reflectDirection.Normalize();

                velocity = reflectDirection * 10.0f;

                Debug.Log("Reflected!");
            }
        }
    }

    public override void Enter(Player player, ref Vector3 velocity) {
        player.controller.isDashBack = false;
        player.controller.currentDashDuration = 0.0f;

        velocity += (Vector3)(player.controller.dashLeap * player.controller.dashSpeed); //* player.controller.dashCharge;

        player.partsAnimator.EnableTrails();
    }

    public override void Exit(Player player, ref Vector3 velocity) {
        player.controller.currentDashChargeTime = 0.0f;
        velocity = velocity / 3.0f;

        player.partsAnimator.DisableTrails();
    }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        player.controller.currentDashDuration += Time.deltaTime;

        //inputs = inputs * maxSpeed;

        if (player.controller.currentDashDuration >= player.controller.chargedDuration) {
            if (player.controller.controller2D.collisionInfo.below) {
                player.controller.SwitchState(PlayerController.States.IDLE);
            }
            else {
                player.controller.SwitchState(PlayerController.States.AIRBORNE);
            }
        }

    }

}
