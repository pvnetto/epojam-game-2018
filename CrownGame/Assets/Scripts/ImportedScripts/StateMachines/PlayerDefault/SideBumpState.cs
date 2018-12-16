using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SideBumpState : PlayerState {

    private static readonly SideBumpState singleton = new SideBumpState();

    static SideBumpState() {
    }

    private SideBumpState() {
    }

    public static SideBumpState Instance {
        get {
            return singleton;
        }
    }

    public override void HandleCollision(GameObject player, Collider2D collision, ref Vector3 velocity) {
    }

    public override void Enter(Player player, ref Vector3 velocity) {
        player.controller.currentSideBumpTime = 0.0f;
    }

    public override void Exit(Player player, ref Vector3 velocity) {
        velocity = velocity / 5.0f;
    }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        player.controller.currentSideBumpTime += Time.deltaTime;

        if (player.controller.currentSideBumpTime >= player.controller.sideBumpDuration) {
            if (player.collisionInfo.below) {
                player.controller.SwitchState(PlayerController.States.IDLE);
            }
            else {
                player.controller.SwitchState(PlayerController.States.AIRBORNE);
            }
        }
    }
}
