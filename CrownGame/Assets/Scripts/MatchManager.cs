using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
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
    public System.Action<int, GameObject> spawnPlayers;

    public static MatchManager instance;
    private Dictionary<int, Player> players = new Dictionary<int, Player>();
    public List<LobbyPlayer> lobby;
    [SerializeField] private GameObject prefab;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            SceneManager.sceneLoaded += SceneManager_sceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        if (instance.lobby == null || instance.lobby.Count == 0)
        {
            lobby = new List<LobbyPlayer>();
        }
    }

    private void SceneManager_sceneLoaded(Scene arg0, LoadSceneMode arg1)
    {
        if (arg0.name == "Game")
        {
            instantiatePlayers();
        }
    }

    private void instantiatePlayers()
    {
        List<Player> list = new List<Player>();

        spawnPlayers.Invoke(lobby.Count, prefab);
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
