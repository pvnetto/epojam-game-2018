using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TrailGhost : MonoBehaviour {

    public SpriteRenderer spriteRenderer;
    [Range(0.05f, 0.3f)]
    public float trailDuration = 0.1f;
    private float currentTrailTime = 0.0f;
    private float maxAlpha = 0.1f;
    private float minAlpha = 0.02f;

    public void Setup(Sprite sprite, Vector3 position, Vector3 scale) {
        transform.position = position;
        transform.localScale = scale;
        spriteRenderer.sprite = sprite;
    }

    void Update() {
        currentTrailTime += Time.deltaTime;

        Color currentColor = spriteRenderer.color;
        float currentAlpha = Mathf.Lerp(maxAlpha, minAlpha, currentTrailTime / trailDuration);

        spriteRenderer.color = new Color(currentColor.r, currentColor.g, currentColor.b, currentAlpha);

        if (currentTrailTime >= trailDuration) {
            Destroy(gameObject);
        }
    }

}
