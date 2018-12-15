using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

[RequireComponent(typeof(Controller2D))]
public class PlayerController : MonoBehaviour {

    public float moveSpeed = 12;
    [Header("Jump parameters")]
    public float jumpMinHeight = 1.5f;
    public float jumpMaxHeight = 4;
    public float timeToJumpMaxHeight = 0.4f;

    public float finalMoveSpeed {
        get {
            float finalSpeed = moveSpeed;
            float greatestDebuff = 0.0f;    //Debuffs don't stack, so only the strongest is applied,
                                            //but buffs DO stack.

            foreach (Modifier modifier in player.ccManager.GetSpeedModifiers()) {
                if(modifier.totalValue < 0) {
                    if (modifier.totalValue < greatestDebuff) {
                        greatestDebuff = modifier.totalValue;
                    }
                }
                else {
                    finalSpeed += (modifier.totalValue * moveSpeed);
                }
            }

            finalSpeed += (greatestDebuff * moveSpeed);
            return finalSpeed;
        }
    }

    [Range(0.0f, 0.3f)]
    public float coyoteTime = 0.1f;
    public float timeAirborne = 0.0f;

    [Header("Wall jumping parameters")]
    public float maxWallSlideSpeed = 3;
    public Vector2 wallLeap;
    [Range(0.0f, 0.5f)]
    public float wallJumpDuration = 0.3f;
    public float wallStickTime = 0.4f;

    public float wallJumpEndTime;

    public float speedRatio {
        get {
            if (Time.time >= wallJumpEndTime || wallJumpDuration == 0) {
                return 1.0f;
            }
            else {
                return 1.0f - ((wallJumpEndTime - Time.time) / wallJumpDuration);
            }
        }
    }

    [HideInInspector]
    public float wallUnstickTime;

    // TODO: Move to crown
    [Header("Dash parameters")]
    [Range(0.1f, 0.5f)]
    public float dashDuration = 0.1f;
    [Range(20, 120)]
    public int dashSpeed = 60;
    [Range(2.0f, 5.0f)]
    public float dashCooldown = 0.5f;
    public float currentDashCooldown;
   
    public Vector2 dashLeap {
        get {
            float dashX = xAxis == 0 ? 0 : Mathf.RoundToInt(xAxis);
            float dashY = yAxis == 0 ? 0 : Mathf.RoundToInt(yAxis);
            if (dashX == 0 && dashY == 0) {
                dashX = xHeading;
            }

            return new Vector2(dashX, dashY);
        }
    }

    private float lastDashTime = 0.0f;

    public bool isDashAvailable {
        get {
            return lastDashTime <= Time.time + dashCooldown;
        }
    }


    internal float accelerationTimeAirborne = 0.2f;
    internal float accelerationTimeGrounded = 0.1f;

    private float gravity;
    public float finalGravity {
        get {
            float finalGravity = gravity;
            float greatestDebuff = 0.0f;

            foreach(Modifier modifier in player.ccManager.GetGravityModifiers()) {
                if(modifier.totalValue > 0) {  //Greater than 0 means it's INCREASING GRAVITY
                    if(modifier.totalValue > greatestDebuff) {
                        Debug.Log("Debuffing = " + modifier.totalValue);
                        greatestDebuff = modifier.totalValue;
                    }
                }
                else {
                    Debug.Log("Buffing!!! = " + modifier.totalValue);
                    finalGravity += (modifier.totalValue * gravity);
                }
            }

            finalGravity += (greatestDebuff * gravity);

            return finalGravity;
        }
    }

    private float jumpGravity {
        get {
            float diff = Mathf.Abs(gravity) - Mathf.Abs(finalGravity);
            return Mathf.Abs(gravity - diff);
        }
    }

    public float maxJumpVelocity {
        get {
            return jumpGravity * timeToJumpMaxHeight;
        }
    }
    public float minJumpVelocity {
        get {
            return Mathf.Sqrt(2 * jumpGravity * jumpMinHeight);
        }
    }

    private Vector3 velocity;

    internal float smoothingVelocityX;

    internal Controller2D controller2D;
    internal Animator playerAnimator;
    internal SpriteRenderer playerSpriteRenderer;

    internal float xHeading;
    internal Collider2D playerCollider;

    internal Player player;
    private PlayerState currentState;

    public enum States { IDLE, JUMPING, AIRBORNE, SLIDING, WALL_JUMPING, DASHING}
    public States currentEnumState;
    internal bool isLocked;

    /*Temporary variables*/
    private float initialXScale;
    private Vector2 knockbackForce = Vector2.zero;

    // Input variables
    float xAxis = 0, yAxis = 0;

    private void Awake() {
        controller2D = GetComponent<Controller2D>();
        playerAnimator = GetComponent<Animator>();
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
        playerCollider = GetComponent<Collider2D>();
        player = GetComponent<Player>();

        initialXScale = transform.localScale.x;

        xHeading = 1;
    }

    private void Start () {
        gravity = -(2 * jumpMaxHeight) / Mathf.Pow(timeToJumpMaxHeight, 2);

        currentState = IdlePlayerState.Instance;
	}

    public void Hit(GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce) {
        currentState.Hit(player, attacker, ref hitRecord, knockbackForce);
    }

    public void Knockback(Vector2 force) {
        knockbackForce += force;
    }

    public void SwitchState(States state) {
        currentEnumState = state;
        switch (state) {
            case States.IDLE:
                SwitchState(IdlePlayerState.Instance);
                break;
            case States.JUMPING:
                SwitchState(JumpPlayerState.Instance);
                break;
            case States.SLIDING:
                SwitchState(SlidingPlayerState.Instance);
                break;
            case States.WALL_JUMPING:
                SwitchState(WallJumpPlayerState.Instance);
                break;
            case States.DASHING:
                float dashX = xAxis == 0 ? 0 : Mathf.RoundToInt(xAxis);
                float dashY = yAxis == 0 ? 0 : Mathf.RoundToInt(yAxis);
                if (dashX == 0 && dashY == 0) {
                    dashX = xHeading;
                }
                SwitchState(DashState.Instance);
                break;
        }
    }

    public void SwitchState(PlayerState newState) {
        if(currentState != null) {
            currentState.Exit(player);
        }
        currentState = newState;
        currentState.Enter(player, ref velocity);
    }

    private void SetPlayerFlip() {
        float dPadX = player.inputDevice.DPadX, stickX = player.inputDevice.LeftStick.X;
        float dPadY = player.inputDevice.DPadY, stickY = player.inputDevice.LeftStick.Y;

        xAxis = Mathf.Abs(dPadX) > Mathf.Abs(stickX) ? dPadX : stickX;
        yAxis = Mathf.Abs(dPadY) > Mathf.Abs(stickY) ? dPadY : stickY;

        if(xAxis != 0 || yAxis != 0) {
            Vector3 joystickPos = new Vector3(transform.position.x + xAxis,
                                              transform.position.y + yAxis,
                                              transform.position.z);
            bool isFlipped = joystickPos.x < transform.position.x;
        }
    }
	
	void Update () {

        if (MatchManager.instance.isPaused) {
            return;
        }

        Vector2 inputs = new Vector2(xAxis, yAxis);

        isLocked = player.inputDevice.GetControl(PlayerActions.ACTION_1);

        if (isLocked) {
            inputs = Vector2.zero;
        }

        if(knockbackForce != Vector2.zero) {
            velocity += (Vector3)knockbackForce;
            knockbackForce = Vector2.zero;
        }

        currentState.Execute(player, ref inputs, ref velocity);

        //velocity.y += (finalGravity * Time.deltaTime);
        if (!(currentState is DashState)) {
            velocity.y += (finalGravity * Time.deltaTime);
        }

        controller2D.Move(velocity * Time.deltaTime, inputs);

        xHeading = inputs.x != 0 ? Mathf.Sign(inputs.x) : xHeading;

        SetPlayerFlip();

        if (controller2D.collisionInfo.above || controller2D.collisionInfo.below) {
            velocity.y = 0;
        }

        //playerAnimator.SetBool("walk", controller2D.collisionInfo.below && inputs.x != 0);
        //playerAnimator.SetBool("air", !controller2D.collisionInfo.below);
        //playerAnimator.SetFloat("velocityY", Mathf.Sign(velocity.y));
        //playerAnimator.SetFloat("speedRatio", velocity.magnitude / moveSpeed);
    }
}
