using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct LobbyPlayer
{
    public int id;

    public LobbyPlayer(int index)
    {
        id = index;
    }
}

public class MatchManager : MonoBehaviour
{
    public static MatchManager instance;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    private List<LobbyPlayer> lobby;
    private Map currentMap;

    public void setMap(Map map)
    {
        currentMap = map;
    }

    private void Awake()
    {
        lobby = new List<LobbyPlayer>();
        if (instance)
        {
            Destroy(instance.gameObject);
        }
        instance = this;
    }

    private void Start()
    {
        if (UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex == 2)
        {
            instantiatePlayers();
        }
    }

    private void instantiatePlayers()
    {
        List<Player> list = new List<Player>();

        foreach (Player player in players.Values)
        {
            list.Add(player);
        }
        currentMap.players = list;
        currentMap.spawnPlayers();
    }

    public void addLobbyPlayer(int index)
    {
        lobby.Add(new LobbyPlayer(index));
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
