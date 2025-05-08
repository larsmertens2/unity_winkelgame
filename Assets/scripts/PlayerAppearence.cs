using Unity.Netcode;
using UnityEngine;

public class PlayerAppearance : NetworkBehaviour
{
    public Renderer targetRenderer; // Assign via Inspector or GetComponentInChildren
    private NetworkVariable<Color> playerColor = new NetworkVariable<Color>(
        writePerm: NetworkVariableWritePermission.Server
    );

    public override void OnNetworkSpawn()
    {
        // Apply the color when it changes
        playerColor.OnValueChanged += OnColorChanged;

        // Only the server sets the color
        if (IsServer)
        {
            Color newColor = NetworkManagerCustom.GetColorForClient(OwnerClientId);
            playerColor.Value = newColor;
        }
        else
        {
            // Apply immediately on join
            ApplyColor(playerColor.Value);
        }
    }

    private void OnColorChanged(Color oldColor, Color newColor)
    {
        ApplyColor(newColor);
    }

    private void ApplyColor(Color color)
    {
        if (targetRenderer != null)
        {
            var newMat = new Material(targetRenderer.material);
            newMat.color = color;
            targetRenderer.material = newMat;
        }
    }
}