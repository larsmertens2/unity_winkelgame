using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class NetworkUI : NetworkBehaviour
{
    public TextMeshProUGUI allPlayersCartText;
    public TextMeshProUGUI countdownText; // 🆕 Assign in the Inspector

    private float updateInterval = 0.1f;
    private float updatetimer;

    private float countdownTime = 60f; // 🆕 Start at 60 seconds

    private void Update()
    {
        if (!IsServer) return;

        updatetimer += Time.deltaTime;

        if (updatetimer >= updateInterval)
        {
            updatetimer = 0f;
            UpdatePlayerScores();
            UpdateCountdown(); // 🆕 Handle countdown
        }
    }

    private void UpdatePlayerScores()
    {
        string allText = "";
        if (NetworkManager.Singleton == null) return;

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

    private void UpdateCountdown()
    {
        countdownTime -= updateInterval;
        if (countdownTime < 0f)
        {
            countdownTime = 0f;
            // Optionally: handle countdown end, like load a new scene or show message
        }

        string formattedTime = countdownTime.ToString("F1") + "s";
        countdownText.text = formattedTime;
        UpdateCountdownClientRpc(formattedTime);
    }

    [ClientRpc]
    private void UpdateCartTotalClientRpc(string totalText)
    {
        if (!IsServer)
        {
            allPlayersCartText.text = totalText;
        }
    }

    [ClientRpc]
    private void UpdateCountdownClientRpc(string countdownFormatted)
    {
        if (!IsServer)
        {
            countdownText.text = countdownFormatted;
        }
    }
}