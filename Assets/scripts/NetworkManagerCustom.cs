using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkManagerCustom : MonoBehaviour
{
    [Tooltip("Name of the game scene where player spawning occurs")]
    public string gameSceneName = "GameScene";
    public GameObject playerPrefab;

    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        if (NetworkManager.Singleton != null)
            NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnClientConnected(ulong clientId)
    {
        // If we’re already in the game scene, spawn immediately
        if (!NetworkManager.Singleton.IsServer) return;
        if (SceneManager.GetActiveScene().name == gameSceneName)
            SpawnPlayer(clientId);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When the game scene loads, spawn all connected clients
        if (!NetworkManager.Singleton.IsServer) return;
        if (scene.name != gameSceneName) return;

        foreach (var id in NetworkManager.Singleton.ConnectedClientsIds)
            SpawnPlayer(id);
    }

    public static Color GetColorForClient(ulong clientId)
    {
        Color[] colorPalette = new Color[]
        {
            Color.red, Color.blue, Color.green, Color.yellow,
            Color.cyan, Color.magenta, Color.gray, Color.white
        };
        return colorPalette[clientId % (ulong)colorPalette.Length];
    }
    private void SpawnPlayer(ulong clientId)
    {
        var spawnPoint = SpawnPointManager.Instance?.GetRandomSpawnPoint();
        if (spawnPoint == null)
        {
            Debug.LogError($"[{nameof(NetworkManagerCustom)}] No spawn point available for client {clientId}!");
            return;
        }

        var go = Instantiate(playerPrefab, spawnPoint.position, spawnPoint.rotation);
        go.GetComponent<NetworkObject>()?.SpawnAsPlayerObject(clientId);

        // Assign a unique color
        var renderer = go.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            var uniqueColor = GetColorForClient(clientId);
            var newMaterial = new Material(renderer.material); // Create a copy to avoid affecting others
            newMaterial.color = uniqueColor;
            renderer.material = newMaterial;
        }
    }
}