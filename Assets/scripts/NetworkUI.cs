using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkUI : NetworkBehaviour
{
    public TextMeshProUGUI allPlayersCartText;
    public TextMeshProUGUI TimerText;

    private float updateInterval = 0.1f; // UI update every 0.5 seconds
    private float updatetimer;
    private float gametimercounter = 0;

    public float GameTimer = 60;

private void Update()
{
    if (!IsServer) return;

    updatetimer += Time.deltaTime;
    gametimercounter += Time.deltaTime;

    if (updatetimer >= updateInterval)
    {
        updatetimer = 0f;
        UpdatePlayerScores();
    }

    float remainingTime = GameTimer - gametimercounter;

    if (remainingTime <= 0f)
    {
        TimerText.text = "0.00";
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host is starting the game...");
            NetworkManager.Singleton.SceneManager.LoadScene("ScoreScene", LoadSceneMode.Single);
        }
    }
    else
    {
        TimerText.text = remainingTime.ToString("F2");
    }
}
    private void UpdatePlayerScores()
    {
        string allText = "";
        if(NetworkManager.Singleton == null) return;
        foreach (var client in NetworkManager.Singleton.ConnectedClientsList)
        {
            var clientObject = client.PlayerObject;
            if (clientObject != null)
            {
                var player = clientObject.GetComponentInChildren<PlayerPickup>();
                if (player != null)
                {
                    allText += $"Speler {client.ClientId}: €{player.totalScorePlayer.Value:F2}\n";
                }
                else
                {
                    Debug.LogWarning($"PlayerPickup not found in children of client {client.ClientId}'s PlayerObject.");
                }
            }
            else
            {
                Debug.LogWarning($"PlayerObject is null for client {client.ClientId}.");
            }
        }

        if (string.IsNullOrWhiteSpace(allText))
        {
            allText = "Geen scores beschikbaar.";
        }

        allPlayersCartText.text = allText;
        UpdateCartTotalClientRpc(allText);
    }

    [ClientRpc]
    private void UpdateCartTotalClientRpc(string totalText)
    {
        if (!IsServer)
        {
            allPlayersCartText.text = totalText;
        }
    }
}