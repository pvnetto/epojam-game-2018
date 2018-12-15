using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MatchManager : MonoBehaviour {

    public static MatchManager instance;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();

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

    public int SubscribePlayer(Player player) {
        int availableID = GetAvailableID();

        players[availableID] = player;

        return availableID;
    }

    public void ResetPlayers() {
        players = new Dictionary<int, Player>();
    }

    public int GetAvailableID() {
        for(int i = 1; i <= 4; i++) {
            if (!players.ContainsKey(i)) {
                return i;
            }
        }

        return 0;
    }

}
