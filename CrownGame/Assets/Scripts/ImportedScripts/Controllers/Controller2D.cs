using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class Controller2D : RaycastController2D {

    public float maxSlopeClimbAngle = 80.0f;
    public float maxSlopeDescendAngle = 75.0f;

    public CollisionInfo collisionInfo;

    private Vector2 playerInput;
    private float maxVelocity = 50;

    private Player owner;

    protected override void Start() {
        base.Start();
        collisionInfo.faceDirection = 1;
        owner = GetComponent<Player>();
    }

    public void Move(Vector3 velocity, bool standingOnPlatform) {
        Move(velocity, Vector2.zero, standingOnPlatform);
    }

    public void Move(Vector3 velocity, Vector2 inputs, bool standingOnPlatform = false) {
        UpdateRaycastOrigins();
        collisionInfo.Reset();
        collisionInfo.velocityOld = velocity;
        playerInput = inputs;

        if(velocity.x != 0) {
            collisionInfo.faceDirection = (int)Mathf.Sign(velocity.x);
        }

        if(velocity.y < 0) {
            DescendSlope(ref velocity);
        }
        HorizontalCollisions(ref velocity);
        if(velocity.y != 0) {
            VerticalCollisions(ref velocity);
        }

        velocity = Vector3.ClampMagnitude(velocity, maxVelocity);

        transform.Translate(velocity);

        if (standingOnPlatform) {
            collisionInfo.below = true;
        }
    }

    #region raycast_collision_handling
    private void HorizontalCollisions(ref Vector3 velocity) {
        float directionX = collisionInfo.faceDirection;
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if(Mathf.Abs(velocity.x) < skinWidth) {
            rayLength = 2 * skinWidth;
        }

        for (int i = 0; i < horizontalRayCount; i++) {
            Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
            rayOrigin += Vector2.up * (horizontalRaySpacing * i);

            RaycastHit2D hit = RaycastIgnoreSelf(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.right * directionX * rayLength, Color.red);

            /*If the ray hits something, handle collision*/
            if (hit) {
                if(hit.distance == 0) {
                    continue;
                }

                Player playerHit = hit.collider.GetComponent<Player>();
                if (playerHit) {
                    collisionInfo.playerLeft = directionX == -1;
                    collisionInfo.playerRight = directionX == 1;

                    collisionInfo.playerHit = playerHit;

                    if (!owner) {
                        continue;
                    }
                }

                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                /*i == 0 means we're checking the leftmost ray*/
                if(i == 0 && slopeAngle <= maxSlopeClimbAngle) {
                    StartSlopeClimbing(ref velocity, hit, directionX, slopeAngle);
                }

                if(!collisionInfo.climbingSlope || slopeAngle > maxSlopeClimbAngle) {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    rayLength = hit.distance;

                    /*Detects an obstable on a slope*/
                    if (collisionInfo.climbingSlope) {
                        velocity.y = Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);
                    }

                    collisionInfo.left = directionX == -1;
                    collisionInfo.right = directionX == 1;
                }
            }
        }
    }

    private void VerticalCollisions(ref Vector3 velocity) {
        float directionY = Mathf.Sign(velocity.y);
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < verticalRayCount; i++) {
            Vector2 rayOrigin = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
            rayOrigin += Vector2.right * (verticalRaySpacing * i + velocity.x);

            RaycastHit2D hit = RaycastIgnoreSelf(rayOrigin, Vector2.up * directionY, rayLength, collisionMask);

            Debug.DrawRay(rayOrigin, Vector2.up * directionY * rayLength, Color.red);

            if (hit) {

                // Checking if the player is stomping another player
                Player player = hit.collider.GetComponent<Player>();
                if (player && directionY == -1) {
                    collisionInfo.playerHit = player;
                    collisionInfo.playerBelow = true;

                    if (!owner) {
                        continue;
                    }
                }

                if (hit.collider.tag == "OneWayPlatform") {
                    if(directionY == 1 || hit.distance == 0) {
                        continue;
                    }
                    if (collisionInfo.fallingThroughPlatform) {
                        continue;
                    }
                    if(playerInput.y <= -0.9f) {
                        collisionInfo.fallingThroughPlatform = true;
                        Invoke("ResetFallingThroughPlatform", 0.5f);
                        continue;
                    }
                }

                velocity.y = (hit.distance - skinWidth) * directionY;
                rayLength = hit.distance;

                if (collisionInfo.climbingSlope) {
                    velocity.x = velocity.y / Mathf.Tan(collisionInfo.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);
                }

                collisionInfo.below = directionY == -1;
                collisionInfo.above = directionY == 1;
            }
        }

        if (collisionInfo.climbingSlope) {
            float directionX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((directionX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, collisionMask);

            if (hit) {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);

                /*Collided with a new slope*/
                if(slopeAngle != collisionInfo.slopeAngle) {
                    velocity.x = (hit.distance - skinWidth) * directionX;
                    collisionInfo.slopeAngle = slopeAngle;
                }
            }
        }
    }
    #endregion

    #region slope_walking
    private void StartSlopeClimbing(ref Vector3 velocity, RaycastHit2D hit, float directionX, float slopeAngle) {
        if (collisionInfo.descendingSlope) {
            collisionInfo.descendingSlope = false;
            velocity = collisionInfo.velocityOld;
        }

        /*Makes the controller get close to the slope before starting to climb it*/
        float distanceToSlopeStart = 0;
        if (collisionInfo.slopeAngle != collisionInfo.slopeAngleOld) {
            distanceToSlopeStart = hit.distance - skinWidth;
            velocity.x -= distanceToSlopeStart * directionX;
        }

        ClimbSlope(ref velocity, slopeAngle);
        velocity.x += distanceToSlopeStart;
    }

    private void ClimbSlope(ref Vector3 velocity, float slopeAngle) {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sign(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        /*Controller isn't jumping*/
        if(velocity.y <= climbVelocityY) {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);

            collisionInfo.below = true;
            collisionInfo.climbingSlope = true;
            collisionInfo.slopeAngle = slopeAngle;
        }
    }

    private void DescendSlope(ref Vector3 velocity) {
        float directionX = Mathf.Sign(velocity.x);
        Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;

        RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.down, Mathf.Infinity, collisionMask);

        if (hit) {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if(slopeAngle != 0 && slopeAngle <= maxSlopeDescendAngle) {
                if(Mathf.Sign(hit.normal.x) == directionX) {
                    if(hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Rad2Deg) * Mathf.Abs(velocity.x)) {
                        float moveDistance = Mathf.Abs(velocity.x);
                        float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

                        velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                        velocity.y -= descendVelocityY;

                        collisionInfo.slopeAngle = slopeAngle;
                        collisionInfo.descendingSlope = true;
                        collisionInfo.below = true;
                    }
                }
            }
        }
    }
    #endregion

    private RaycastHit2D RaycastIgnoreSelf(Vector2 origin, Vector2 direction, float distance, int layerMask) {
        RaycastHit2D[] hits = Physics2D.RaycastAll(origin, direction, distance, layerMask);
        hits.OrderBy(h => h.distance).ToArray();

        RaycastHit2D hit = new RaycastHit2D();

        foreach (RaycastHit2D hitObj in hits) {
            if (hitObj.collider) {
                //TODO Handle this in a better way
                if (hitObj.transform.root.gameObject == transform.root.gameObject) {
                    continue;
                }
                else {
                    hit = hitObj;
                    break;
                }
            }
        }

        return hit;
    }

    private void ResetFallingThroughPlatform() {
        collisionInfo.fallingThroughPlatform = false;
    }

    public struct CollisionInfo {
        public bool above, below;

        public bool left, right;
        public bool oldLeft, oldRight;
        public int leftFrameCount, rightFrameCount;

        public bool climbingSlope, descendingSlope;
        public float slopeAngle, slopeAngleOld;
        public Vector3 velocityOld;

        public int faceDirection;

        public bool fallingThroughPlatform;

        public bool playerBelow, playerLeft, playerRight;

        public Player playerHit;

        public void Reset() {
            above = below = playerBelow = playerLeft = playerRight = false;
            playerHit = null;

            int sideCollisionFrameDuration = 4;

            leftFrameCount = left ? sideCollisionFrameDuration : Mathf.Max(leftFrameCount - 1, 0);
            oldLeft = leftFrameCount > 0;
            rightFrameCount = right ? sideCollisionFrameDuration : Mathf.Max(rightFrameCount - 1, 0);
            oldRight = rightFrameCount > 0;

            left = right = false;

            climbingSlope = descendingSlope = false;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;
        }
    }

}
