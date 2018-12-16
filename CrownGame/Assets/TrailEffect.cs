using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailEffect : MonoBehaviour {

    private float timer = 0.2f;
    private SpriteRenderer playerSpriteRenderer;

    public GameObject ghostPrefab;

    private void Awake() {
        playerSpriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void SpawnTrail() {
        GameObject ghost = Instantiate(ghostPrefab);
        TrailGhost trailGhost = ghost.GetComponent<TrailGhost>();

        trailGhost.Setup(playerSpriteRenderer.sprite, transform.position, transform.localScale);
    }

}
