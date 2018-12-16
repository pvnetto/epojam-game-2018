using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(LevelEditor))]
public class Map : MonoBehaviour
{
    [SerializeField] private Transform[] playersSpawnPoints;
    [SerializeField] private Transform[] npcsSpawnPoints;
    public List<Player> players;
    public Vector2 offset;

    private void Awake()
    {
        MatchManager.instance.spawnPlayers += spawnPlayers;

        var mapSize = GetComponent<LevelEditor>().loadSource();
        Camera cam = Camera.main;
        cam.orthographicSize = mapSize.x > mapSize.y ? mapSize.y / 2 : mapSize.x / 2;
        cam.transform.position = new Vector3(mapSize.x / 2, mapSize.y / 2, cam.transform.position.z);
    }

    public void spawnPlayers(int amount, GameObject prefab)
    {
        for (int i = 0; i < amount; i++)
        {
            GameObject go = Instantiate(prefab);
            Player player = go.GetComponent<Player>();
            player.SetPlayerID(i);
            go.transform.position = playersSpawnPoints[player.playerID].position;
        }
    }

    public void spawnNpcs(GameObject[] npcs)
    {
        List<Transform> availablePositions = new List<Transform>(npcsSpawnPoints);

        foreach (GameObject npc in npcs)
        {
            int randomIndex = Random.Range(0, availablePositions.Count);
            Transform transform = availablePositions[randomIndex];
            npc.transform.position = transform.position;
            availablePositions.Remove(transform);
        }

        //TODO importar players pela lista de npcs
    }

}
