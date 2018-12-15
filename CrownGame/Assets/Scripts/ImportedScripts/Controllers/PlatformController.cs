using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformController : RaycastController2D {

    public float speed;
    public bool isCyclic;
    public float waitTime;
    [Range(0, 20)]
    public float easeAmount;
    public LayerMask passengerMask;

    public Vector3[] localWaypoints;
    private Vector3[] globalWaypoints;

    private float nextMoveTime = 0;
    private int fromWaypointIndex = 0;
    private float percentageBetweenWaypoints = 0;

    List<PassengerMovement> passengerMovement;
    Dictionary<Transform, Controller2D> passengerDictionary = new Dictionary<Transform, Controller2D>();

    private void Awake() {
        globalWaypoints = new Vector3[localWaypoints.Length];
        for (int i = 0; i < localWaypoints.Length; i++) {
            globalWaypoints[i] = localWaypoints[i] + transform.position;
        }
    }

    private float Ease(float x) {
        float a = easeAmount + 1;

        return Mathf.Pow(x, a) / Mathf.Max(0.00001f, (Mathf.Pow(x, a) + Mathf.Pow(1 - x, a)));
    }

    private Vector3 CalculatePlatformMovement() {
        if(Time.time < nextMoveTime) {
            return Vector3.zero;
        }

        fromWaypointIndex %= globalWaypoints.Length;
        int toWaypointIndex = (fromWaypointIndex + 1) % globalWaypoints.Length;

        float distanceBetweenWaypoints = Vector3.Distance(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex]);
        percentageBetweenWaypoints += Time.deltaTime * speed / distanceBetweenWaypoints;
        Mathf.Clamp(percentageBetweenWaypoints, 0.0001f, 1);

        float easedPercentBetweenWaypoints = Ease(percentageBetweenWaypoints);

        Vector3 newPos = Vector3.Lerp(globalWaypoints[fromWaypointIndex], globalWaypoints[toWaypointIndex], easedPercentBetweenWaypoints);

        if(percentageBetweenWaypoints >= 1) {
            percentageBetweenWaypoints = 0;
            fromWaypointIndex++;

            if (!isCyclic) {
                if (fromWaypointIndex >= globalWaypoints.Length - 1) {
                    fromWaypointIndex = 0;
                    System.Array.Reverse(globalWaypoints);
                }
            }
            nextMoveTime = Time.time + waitTime;
        }

        return newPos - transform.position;
    }

    private void CalculatePassengerMovement(Vector3 velocity) {
        HashSet<Transform> movedPassengers = new HashSet<Transform>();
        passengerMovement = new List<PassengerMovement>();

        float directionX = Mathf.Sign(velocity.x);
        float directionY = Mathf.Sign(velocity.y);

        /*Vertically moving platform*/
        if (velocity.y != 0) {
            float rayLength = Mathf.Abs(velocity.y) + skinWidth;

            for (int i = 0; i < verticalRayCount; i++) {
                Vector2 rayOrigin = directionY == -1 ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;
                rayOrigin += Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up * directionY, rayLength, passengerMask);
                /*hit.distance checks if the player is inside the platform*/
                if (hit && hit.distance != 0) {
                    if (!movedPassengers.Contains(hit.transform)) {
                        float pushX = directionY == 1 ? velocity.x : 0;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), directionY == 1, true));

                        movedPassengers.Add(hit.transform);
                    }
                }
            }
        }

        /*Horizontally moving platform*/
        if (velocity.x != 0) {
            float rayLength = Mathf.Abs(velocity.x) + skinWidth;

            for (int i = 0; i < horizontalRayCount; i++) {
                Vector2 rayOrigin = directionX == -1 ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;
                rayOrigin += Vector2.up * (horizontalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.right * directionX, rayLength, passengerMask);

                if (hit && hit.distance != 0) {
                    if (!movedPassengers.Contains(hit.transform)) {
                        float pushX = velocity.x - (hit.distance - skinWidth) * directionX;
                        float pushY = -skinWidth;
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), false, true));

                        movedPassengers.Add(hit.transform);
                    }
                }
            }
        }

        /*Passenger is on top of horizontally moving platform*/
        if(directionY == -1 || velocity.y == 0 && velocity.x != 0) {
            float rayLength = skinWidth * 2;

            for (int i = 0; i < verticalRayCount; i++) {
                Vector2 rayOrigin = raycastOrigins.topLeft + Vector2.right * (verticalRaySpacing * i);

                RaycastHit2D hit = Physics2D.Raycast(rayOrigin, Vector2.up, rayLength, passengerMask);
                if (hit && hit.distance != 0) {
                    if (!movedPassengers.Contains(hit.transform)) {
                        float pushX = velocity.x;
                        float pushY = velocity.y - (hit.distance - skinWidth) * directionY;
                        passengerMovement.Add(new PassengerMovement(hit.transform, new Vector3(pushX, pushY), true, false));

                        movedPassengers.Add(hit.transform);
                    }
                }
            }
        }

    }

    private void MovePassengers(bool beforeMovePlaform) {
        foreach(PassengerMovement passenger in passengerMovement) {
            if (!passengerDictionary.ContainsKey(passenger.transform)) {
                passengerDictionary.Add(passenger.transform, passenger.transform.GetComponent<Controller2D>());
            }
            if(passenger.movedBeforePlatform == beforeMovePlaform) {
                passengerDictionary[passenger.transform].Move(passenger.velocity, passenger.standingOnPlatform);
            }
        }
    }
	
	void Update () {
        UpdateRaycastOrigins();

        Vector3 velocity = CalculatePlatformMovement();

        CalculatePassengerMovement(velocity);

        MovePassengers(true);

        if(velocity != Vector3.zero) {
            transform.Translate(velocity);
        }
        MovePassengers(false);
	}

    private void OnDrawGizmos() {
        if (localWaypoints != null) {
            Gizmos.color = Color.red;
            float size = 0.3f;

            for(int i = 0; i < localWaypoints.Length; i++) {
                Vector3 globalWaypoint = Application.isPlaying ? globalWaypoints[i] : localWaypoints[i] + transform.position;
                Gizmos.DrawLine(globalWaypoint - Vector3.up * size, globalWaypoint + Vector3.up * size);
                Gizmos.DrawLine(globalWaypoint - Vector3.left * size, globalWaypoint + Vector3.left * size);
            }
        }
    }

    struct PassengerMovement {
        public Transform transform;
        public Vector3 velocity;
        public bool standingOnPlatform;
        public bool movedBeforePlatform;

        public PassengerMovement(Transform transform, Vector3 velocity, bool standingOnPlatform, bool movedBeforePlatform) {
            this.transform = transform;
            this.velocity = velocity;
            this.standingOnPlatform = standingOnPlatform;
            this.movedBeforePlatform = movedBeforePlatform;
        }
    }
}
