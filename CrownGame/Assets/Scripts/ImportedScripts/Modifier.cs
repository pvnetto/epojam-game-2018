using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Modifier {
    
    public GameObject source { get; private set; }

    protected float stackValue = 0.0f;
    public float totalValue {
        get {
            return stackCount * stackValue;
        }
    }

    public float duration { get; private set; }
    public float endTimestamp { get; private set; }

    private int stackCount = 1;
    private int maxStacks;

    public Modifier(GameObject source, float stackValue, float duration, int maxStacks) {
        this.source = source;
        this.stackValue = stackValue;
        this.duration = duration;
        endTimestamp = Time.time + duration;
        this.maxStacks = maxStacks;
    }

    public void Refresh(float multiplier, float duration) {
        float tmpEnd = Time.time + duration;
        if(tmpEnd > endTimestamp) {
            this.duration = duration;
            endTimestamp = tmpEnd;

            if(stackCount < maxStacks) {
                stackValue = multiplier;
                stackCount++;
            }
        }
    }

}
