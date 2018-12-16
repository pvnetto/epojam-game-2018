using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using InControl;

[RequireComponent(typeof(PlayerController))]
[RequireComponent(typeof(FlashController))]
public class Player : MonoBehaviour, IHittable, IStunnable {

    public bool isStunned {
        get {
            return ccManager.stunned;
        }
    }

    public int playerID { get; protected set; }
    protected List<int> alliedPlayersNumbers;

    // Crown parameters
    internal List<Crown> crowns = new List<Crown>();
    public Crown equippedCrown { get; private set; }
    private int equippedCrownIndex;

    private bool dead = false;  /*Stops the damage function from calling Die multiple times*/

    internal CCManager ccManager = new CCManager();

    public InputDevice inputDevice;
    private FlashController flashController;
    internal SpriteRenderer spriteRenderer;
    internal PlayerController controller;

    public delegate void OnCrownPick();
    public event OnCrownPick onCrownPick;

    public delegate void OnCrownDrop();
    public event OnCrownDrop onCrownDrop;

    private void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flashController = GetComponent<FlashController>();
        controller = GetComponent<PlayerController>();
    }

    protected virtual void Start () {
        //TODO Remove this check
        // This is used for editor testing purposes. It automatically assigns an available ID to the player.
        /*
        if (playerID == 0) {
            if (MatchManager.instance) {
                SetPlayerID(MatchManager.instance.SubscribePlayer(this));
            }
        }
        /**/
        Debug.Log(playerID);
        Debug.Log(InputManager.Devices.Count);
        inputDevice = InputManager.Devices[playerID];

        SetAllyList();

        // Setting the callbacks for crown picking and dropping
        onCrownPick += controller.SetDashChargeTime;
        onCrownPick += controller.SetPlayerSpeed;

        onCrownDrop += controller.SetDashChargeTime;
        onCrownDrop += controller.SetPlayerSpeed;
    }

    public void AddCrown(Crown crown) {
        crowns.Add(crown);

        if (!equippedCrown) {
            equippedCrown = crown;
        }

        onCrownPick.Invoke();
    }

    public void DropRandomCrown() {

    }

    protected void SetAllyList() {
        alliedPlayersNumbers = new List<int>();
        alliedPlayersNumbers.Add(playerID);
    }

    public void SetPlayerID(int playerID) {
        this.playerID = playerID;
    }

    public void Knockback(Vector2 force) {
        controller.Knockback(force);
    }

    public bool isAlly(int num) {
        return alliedPlayersNumbers.Contains(num);
    }

    public void Damage() {
        BroadcastMessage("Flash");
    }

    public void Hit(GameObject attacker, ref HitRecord hitRecord, Vector2 knockbackForce) {
        controller.Hit(attacker, ref hitRecord, knockbackForce);
    }

    public void Stun(GameObject source, float duration) {
        ccManager.Stun(duration);
    }

    public void AddSpeedModifier(GameObject source, float multiplier, float duration, int maxStacks = 1) {
        ccManager.AddSpeedModifier(source, multiplier, duration, maxStacks);
    }
    
    public void RemoveSpeedModifier(GameObject source) {
        ccManager.RemoveSpeedModifier(source);
    }

    public void AddGravityModifier(GameObject source, float multiplier, float duration, int maxStacks = 1) {
        ccManager.AddGravityModifier(source, multiplier, duration, maxStacks);
    }

    public void RemoveGravityModifier(GameObject source) {
        ccManager.RemoveGravityModifier(source);
    }

    private void Update() {
        if(inputDevice.GetControl(PlayerActions.PAUSE).WasPressed) {
            MatchManager.instance.PauseGame(playerID);
        }
        ccManager.Update();
    }
}
