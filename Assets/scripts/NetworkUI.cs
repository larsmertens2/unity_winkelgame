using Unity.Netcode;
using UnityEngine;
using TMPro;

public class NetworkUI : NetworkBehaviour
{
    public TextMeshProUGUI allPlayersCartText;

    private float updateInterval = 0.5f; // UI update every 0.5 seconds
    private float timer;

    private void Update()
    {
        if (!IsServer) return;

        timer += Time.deltaTime;
        if (timer >= updateInterval)
        {
            timer = 0f;
            UpdatePlayerScores();
        }
    }

    private void UpdatePlayerScores()
    {
        string allText = "";

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