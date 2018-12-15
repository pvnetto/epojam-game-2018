using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastController2D : MonoBehaviour {

    protected const float skinWidth = 0.015f;

    public LayerMask collisionMask;

    public int horizontalRayCount = 4, verticalRayCount = 4;
    protected float horizontalRaySpacing, verticalRaySpacing;

    protected BoxCollider2D controllerCollider;
    protected RaycastOrigins raycastOrigins;

    protected virtual void Start () {
        controllerCollider = GetComponent<BoxCollider2D>();
        CalculateRaySpacing();
    }

    protected void UpdateRaycastOrigins() {
        /*GetComponent is necessary for Crystal Shield, since it changes the player's collider*/
        controllerCollider = GetComponent<BoxCollider2D>();

        CalculateRaySpacing();
        Bounds bounds = controllerCollider.bounds;
        bounds.Expand(skinWidth * -2);

        raycastOrigins.bottomLeft = new Vector2(bounds.min.x, bounds.min.y);
        raycastOrigins.bottomRight = new Vector2(bounds.max.x, bounds.min.y);
        raycastOrigins.topLeft = new Vector2(bounds.min.x, bounds.max.y);
        raycastOrigins.topRight = new Vector2(bounds.max.x, bounds.max.y);
    }

    protected void CalculateRaySpacing() {
        Bounds bounds = controllerCollider.bounds;
        bounds.Expand(skinWidth * -2);

        horizontalRayCount = Mathf.Clamp(horizontalRayCount, 2, int.MaxValue);
        verticalRayCount = Mathf.Clamp(verticalRayCount, 2, int.MaxValue);

        horizontalRaySpacing = bounds.size.y / (horizontalRayCount - 1);
        verticalRaySpacing = bounds.size.x / (verticalRayCount - 1);
    }

    public struct RaycastOrigins {
        public Vector2 topLeft, topRight;
        public Vector2 bottomLeft, bottomRight;
    }
}
