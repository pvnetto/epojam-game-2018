using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LockedState : PlayerState {

    public LockedState(Player player) : base(player) {}

    protected override void StateUpdate(ref Vector2 inputs, ref Vector3 velocity) {
        bool isLocked = owner.inputDevice.GetControl(PlayerActions.LOCK);

        if (!isLocked) {
            controller.SwitchState(PlayerController.States.IDLE);
        }

        inputs = Vector2.zero;

        float targetVelocityX = inputs.x * controller.finalMoveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref controller.smoothingVelocityX,
                                      controller.controller2D.collisionInfo.below ? controller.accelerationTimeGrounded : controller.accelerationTimeAirborne);

    }
}
