using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour {

    public static MatchManager instance;

    private void Awake() {
        if (instance) {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    public bool isPaused = false;

    public void PauseGame(int playerID) {
        isPaused = true;
    }

    public int GetAvailableID() {
        return 1;
    }

}
