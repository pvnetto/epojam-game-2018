using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PlayerController))]
public class PartsAnimator : MonoBehaviour {

    // TODO: Move to another script
    [Header("King sprites")]
    public SpriteRenderer sclera;
    public SpriteRenderer iris;
    public SpriteRenderer pupil;

    [Header("Eye movement parameters")]
    [Range(0.0f, 1.0f)]
    public float maxIrisOffset = 0.1f;
    [Range(0.0f, 1.0f)]
    public float maxPupilOffset = 0.1f;

    public Vector2 targetIrisOffset = Vector2.zero;
    public Vector2 targetPupilOffset = Vector2.zero;

    private Vector2 irisSmoothing;
    private Vector2 pupilSmoothing;

    private PlayerController playerController;

    private TrailEffect[] trails;
    private bool areTrailsEnabled = false;

    private void Start() {
        playerController = GetComponent<PlayerController>();
        trails = GetComponentsInChildren<TrailEffect>();
    }

    public void AnimatePupil() {
        float maxX = 1.0f;
        float minX = 0.4f;
        float xScale = Mathf.Lerp(maxX, minX, playerController.dashCharge);
        pupil.transform.localScale = new Vector3(xScale, 1.0f, 1.0f);
    }

    public void AnimateEyeMovement() {
        targetIrisOffset = new Vector2(playerController.xAxis * maxIrisOffset * transform.localScale.x, playerController.yAxis * maxIrisOffset);
        targetPupilOffset = new Vector2(playerController.xAxis* maxPupilOffset *transform.localScale.x, playerController.yAxis* maxPupilOffset);

        Vector2 currentIrisOffset = iris.transform.localPosition;
        Vector2 currentPupilOffset = pupil.transform.localPosition;

        currentPupilOffset = Vector2.SmoothDamp(currentPupilOffset, targetPupilOffset, ref pupilSmoothing, 0.1f);
        currentIrisOffset = Vector2.SmoothDamp(currentIrisOffset, targetIrisOffset, ref irisSmoothing, 0.1f);


        pupil.transform.localPosition = currentPupilOffset;
        iris.transform.localPosition = currentIrisOffset;
    }

    public void EnableTrails() {
        areTrailsEnabled = true;
    }

    public void DisableTrails() {
        areTrailsEnabled = false;
    }

    private void AnimateTrail() {
        if (areTrailsEnabled) {
            foreach (TrailEffect trailEffect in trails) {
                trailEffect.SpawnTrail();
            }
        }
    }
	
	void Update () {
        AnimatePupil();
        AnimateEyeMovement();
        AnimateTrail();
	}
}
