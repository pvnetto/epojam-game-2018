using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashController : MonoBehaviour {

    [Range(0.1f, 0.8f)]
    public float duration = 0.45f;
    private SpriteRenderer spriteRenderer;
    private Material flashMaterial;
    private float unflashTime;
    private bool flashing = false;

    private float currentFlashValue = 0.0f;
    private float timeMultiplier;

    void Start() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        flashMaterial = spriteRenderer.material;

        timeMultiplier = 1.0f / duration;
    }

    public void Flash() {
        unflashTime = Time.time + duration;
        flashing = true;
        currentFlashValue = 1.0f;
    }

    void Update() {
        currentFlashValue = Mathf.Max(currentFlashValue - Time.deltaTime * timeMultiplier, 0);
        flashMaterial.SetFloat("_FlashAmount", currentFlashValue);
    }
}
