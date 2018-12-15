using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CCManager {

    public bool stunned { get; private set; }
    private float stunEndTimestamp;

    private List<Modifier> speedModifiers = new List<Modifier>();
    private List<Modifier> gravityModifiers = new List<Modifier>();

    //TODO Create list of damage over time objects, execute them every frame

    public void Stun(float duration) {
        float tmpEnd = Time.time + duration;
        if (!stunned || (stunned && tmpEnd > stunEndTimestamp)) {
            stunned = true;
            stunEndTimestamp = tmpEnd;
        }
    }

    private void Unstun() {
        stunned = false;
    }

    private void HandleStun() {
        if (stunned) {
            if (Time.time >= stunEndTimestamp) {
                Unstun();
            }
        }
    }

    #region speed_modifier_methods
    public void AddSpeedModifier(GameObject source, float multiplier, float duration, int maxStacks) {
        //If the source already put a modifier, it's refreshed
        foreach(Modifier modifier in speedModifiers) {
            if(modifier.source != null && modifier.source == source) {
                modifier.Refresh(multiplier, duration);
                return;
            }
        }

        //If the source hasn't put a modifier, it's added
        speedModifiers.Add(new Modifier(source, multiplier, duration, maxStacks));
    }

    public void RemoveSpeedModifier(GameObject source) {
        int i = 0;
        while (i < speedModifiers.Count) {
            if(speedModifiers[i].source == source) {
                speedModifiers.RemoveAt(i);
                return;
            }
            else {
                i++;
            }
        }
    }

    public List<Modifier> GetSpeedModifiers() {
        return speedModifiers;
    }
    #endregion

    #region gravity_modifier_methods
    public void AddGravityModifier(GameObject source, float multiplier, float duration, int maxStacks) {
        //If the source already put a modifier, it's refreshed
        foreach (Modifier modifier in gravityModifiers) {
            if (modifier.source == source) {
                modifier.Refresh(multiplier, duration);
                return;
            }
        }

        //If the source hasn't put a modifier, it's added
        gravityModifiers.Add(new Modifier(source, multiplier, duration, maxStacks));
    }

    public void RemoveGravityModifier(GameObject source) {
        int i = 0;
        while (i < gravityModifiers.Count) {
            if (gravityModifiers[i].source == source) {
                gravityModifiers.RemoveAt(i);
                return;
            }
            else {
                i++;
            }
        }
    }

    public List<Modifier> GetGravityModifiers() {
        return gravityModifiers;
    }

    #endregion

    private void HandleModifiers(List<Modifier> modifiers) {
        int i = 0;
        while (i < modifiers.Count) {
            Modifier modifier = modifiers[i];
            if (Time.time >= modifier.endTimestamp) {
                modifiers.RemoveAt(i);
            }
            else {
                i++;
            }
        }
    }

    public void Update() {
        HandleStun();
        HandleModifiers(speedModifiers);
        HandleModifiers(gravityModifiers);
    }

}
