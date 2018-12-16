using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PlayerState {

    public abstract void Enter(Player player, ref Vector3 velocity);

    public abstract void Exit(Player player, ref Vector3 velocity);

    public virtual void HandleCollision(GameObject player, Collider2D collision, ref Vector3 velocity) {
        Player enemy = collision.GetComponent<Player>();

        if (enemy) {
            Player self = player.GetComponent<Player>();

            if(self.collisionInfo.playerLeft || self.collisionInfo.playerRight) {
                int bumpDirection = self.collisionInfo.playerRight ? -1 : 1;
                self.controller.Knockback(Vector2.right * self.controller.sideBumpSpeed * bumpDirection);
                enemy.controller.Knockback(Vector2.right * enemy.controller.sideBumpSpeed * -bumpDirection);

                self.controller.SwitchState(PlayerController.States.SIDE_BUMP);
                enemy.controller.SwitchState(PlayerController.States.SIDE_BUMP);
            }
            else {
                Debug.Log("Definitely not a sidebump");
                if (self.collisionInfo.playerBelow) {
                    // TODO: Head stomp
                }
            }
        }
    }

    public virtual void Hit(Player player, GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce) {
        hitRecord.hitObject = player;
        hitRecord.hitObjectID = player.playerID;
        hitRecord.reflected = false;

        player.Knockback(knockbackForce);
        player.Damage();
    }

    public virtual void Execute(Player player, ref Vector2 inputs, ref Vector3 velocity) {
        HandleCCs(player, ref inputs);
        Update(player, ref inputs, ref velocity);
    }

    protected virtual void HandleCCs(Player player, ref Vector2 inputs) {
        if (player.isStunned) {
            inputs = Vector2.zero;
        }
    }

    protected abstract void Update(Player player, ref Vector2 inputs, ref Vector3 velocity);

}
