using Unity.Netcode;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class ScoreboardUI : NetworkBehaviour
{
    public Button PlayAgainButton;

    private void Awake() {
        PlayAgainButton.onClick.AddListener(() => PlayAgain());
        Cursor.lockState = CursorLockMode.None; 
        Cursor.visible = true;  
    }

    private void PlayAgain()
    {
        if (NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Host is starting the game...");
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene", LoadSceneMode.Single);
        }
    }
}