using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.UI;


public class TestRelay : MonoBehaviour
{
    [Header("UI Elements")]
    public TMP_InputField joinCodeInputField;
    public TMP_Text joinCodeDisplayText;

    public TMP_Text PlayersinLobbyText;
    public Button createRelayButton;
    public Button joinRelayButton;
    public Button leaveRelayButton;
    public Button startGameButton;

    private int maxPlayers = 4;

    private void Awake()
    {
        createRelayButton.onClick.AddListener(() => CreateRelay());
        joinRelayButton.onClick.AddListener(() => JoinRelay(joinCodeInputField.text));
        startGameButton.onClick.AddListener(() => StartGame());
        leaveRelayButton.onClick.AddListener(() => LeaveRelay());
        startGameButton.gameObject.SetActive(false); 
    }

    private async void Start()
    {
        Debug.Log("Initializing Unity Services...");
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in as: " + AuthenticationService.Instance.PlayerId);
        };
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
    }

    private void Update()
    {


        int playerCount = NetworkManager.Singleton.ConnectedClientsList.Count;

        if (playerCount == 0)
        {
            PlayersinLobbyText.text = "";
            joinCodeDisplayText.text = "";
            startGameButton.gameObject.SetActive(false);
        }
        else
        {
            PlayersinLobbyText.text = "Players in lobby: " + playerCount.ToString();
        }
    }

    private async void CreateRelay()
    {
        try
        {
            Debug.Log("Creating Relay allocation...");
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(maxPlayers - 1);
            Debug.Log("Allocation created successfully.");

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            Debug.Log("Join code obtained: " + joinCode);

            joinCodeDisplayText.text = "Join Code: " + joinCode;

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData);

            bool started = NetworkManager.Singleton.StartHost();
            Debug.Log("Host started: " + started);

            startGameButton.gameObject.SetActive(started);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("RelayServiceException during CreateRelay: " + e.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("General exception during CreateRelay: " + ex.Message);
        }
    }

    private async void JoinRelay(string joinCode)
    {
        if (string.IsNullOrWhiteSpace(joinCode))
        {
            Debug.LogError("Join code is empty.");
            return;
        }

        joinCode = joinCode.Trim().ToUpper();  // Relay join codes are uppercase, 6 letters

        if (joinCode.Length != 6)
        {
            Debug.LogError("Join code format is invalid: " + joinCode);
            return;
        }

        try
        {
            Debug.Log("Attempting to join relay with code: " + joinCode);

            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                Debug.LogWarning("Unity Services not initialized. Initializing now...");
                await UnityServices.InitializeAsync();
            }

            if (!AuthenticationService.Instance.IsSignedIn)
            {
                Debug.LogWarning("User not signed in. Signing in anonymously...");
                await AuthenticationService.Instance.SignInAnonymouslyAsync();
            }

            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);
            Debug.Log("Successfully joined relay allocation.");

            var transport = NetworkManager.Singleton.GetComponent<UnityTransport>();
            transport.SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData);

            bool started = NetworkManager.Singleton.StartClient();
            Debug.Log("Client started: " + started);
        }
        catch (RelayServiceException e)
        {
            Debug.LogError("RelayServiceException during JoinRelay: " + e.Message);
        }
        catch (Exception ex)
        {
            Debug.LogError("General exception during JoinRelay: " + ex.Message);
        }
    }

    private void LeaveRelay()
    {
        Debug.Log("Shutting down network manager...");
        NetworkManager.Singleton.Shutdown();
    }

    private void StartGame()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host is starting the game...");
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}