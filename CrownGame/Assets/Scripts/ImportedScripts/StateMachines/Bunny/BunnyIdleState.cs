using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BunnyIdleState : AnimalState {

    private static readonly BunnyIdleState singleton = new BunnyIdleState();

    static BunnyIdleState() {
    }

    private BunnyIdleState() {
    }

    public static BunnyIdleState Instance {
        get {
            return singleton;
        }
    }

    public override void Enter(Bunny player, ref Vector3 velocity) {   }

    public override void Exit(Bunny player) {  }

    private void InvertBunnyDirection(Bunny animal, ref Vector2 velocity) {
        animal.direction = -animal.direction.normalized;
        int reverseWallHeading = animal.collisionInfo.right ? -1 : 1;
        Vector2 leap = animal.wallBumpLeap * reverseWallHeading;
        velocity += leap;
    }

    public override void Update(Bunny animal, ref Vector2 velocity) {

        float targetVelocityX = animal.direction.x * animal.speed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref animal.smoothingVelocityX,
                                      animal.controller.collisionInfo.below ? animal.accelerationTimeGrounded : animal.accelerationTimeAirborne);


        if(animal.collisionInfo.right || animal.collisionInfo.left) {
            InvertBunnyDirection(animal, ref velocity);
        }

    }
}
