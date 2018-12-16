using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitState : PlayerState {

    private static readonly HitState singleton = new HitState();

    static HitState() {
    }

    private HitState() {
    }

    public static HitState Instance {
        get {
            return singleton;
        }
    }

    public override void Enter(Player player, ref Vector3 velocity) {
        player.controller.currentHitRecoilTime = 0.0f;
    }

    public override void Exit(Player player, ref Vector3 velocity) {    }

    protected override void Update(Player player, ref Vector2 inputs, ref Vector3 velocity) {

        player.controller.currentHitRecoilTime += Time.deltaTime;

        if(player.controller.currentHitRecoilTime >= player.controller.hitRecoilDuration) {
            if (player.collisionInfo.below) {
                player.controller.SwitchState(PlayerController.States.IDLE);
            }
            else {
                player.controller.SwitchState(PlayerController.States.AIRBORNE);
            }
        }
    }

}
