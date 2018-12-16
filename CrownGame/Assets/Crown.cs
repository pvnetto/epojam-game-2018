using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crown : MovingEntity {

    private int currentHealth;
    private FlashController flashController;

    [Header("Dash parameters")]
    [Range(0.1f, 0.5f)]
    public float maxDashDuration = 0.1f;
    [HideInInspector]
    public float currentDashDuration;   // How much time the dash has travelled

    [Range(2, 100)]
    public float dashDistance = 4;

    internal float dashSpeed;

    [Range(2.0f, 5.0f)]
    public float dashCooldown = 0.5f;
    private float lastDashTime = 0.0f;

    // This is set to false when the dash is used, and set to true when the player is grounded again
    internal bool isDashBack = true;
    public bool isDashAvailable {
        get {
            return isDashBack && lastDashTime <= Time.time + dashCooldown;
        }
    }

    public Vector2 dashLeap {
        get {
            if (ownerController) {
                float dashX = ownerController.xAxis == 0 ? 0 : Mathf.RoundToInt(ownerController.xAxis);
                float dashY = ownerController.yAxis == 0 ? 0 : Mathf.RoundToInt(ownerController.yAxis);
                if (dashX == 0 && dashY == 0) {
                    dashX = ownerController.xHeading;
                }

                return new Vector2(dashX, dashY).normalized;
            }
            return Vector2.zero;
        }
    }

    public PlayerState dashState { get; private set; }
    private PlayerController ownerController;

    protected override void Start() {
        base.Start();

        dashState = BunnyCrownState.Instance;
        flashController = GetComponent<FlashController>();

        direction = Vector2.zero;
        dashSpeed = dashDistance / maxDashDuration;
    }

    public Crown Pick(GameObject player) {
        PlayerController playerController = player.GetComponent<PlayerController>();
        ownerController = playerController;

        playerController.player.AddCrown(this);

        gameObject.SetActive(false);

        return this;
    }

    public void AddKnockbackForce(Vector2 knockbackForce) {
        velocity += knockbackForce;
    }

    protected override void Update() {
        float targetVelocityX = direction.x * moveSpeed;

        velocity.x = Mathf.SmoothDamp(velocity.x,
                                      targetVelocityX,
                                      ref smoothingVelocityX,
                                      controller.collisionInfo.below ? accelerationTimeGrounded : accelerationTimeAirborne);

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime, false);

        if (controller.collisionInfo.above || controller.collisionInfo.below) {
            velocity.y = 0;
        }
    }

}
