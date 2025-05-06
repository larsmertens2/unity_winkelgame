using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class EndZone : NetworkBehaviour
{
    public NetworkVariable<float> pointsTotal = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);


    private void OnTriggerStay(Collider other)
    {
        if (!IsServer) return; // Only the server should track cart items

        // Check if the other object has the ItemsInCart component
        if (other.TryGetComponent(out ItemsInCart items))
        {
            if (items != null)
            {
                float cartTotalValue = items.cartTotal.Value;

                pointsTotal.Value += cartTotalValue;

                foreach (var itemRef in items.itemsInCart)
                {
                    if (itemRef.TryGet(out NetworkObject item))
                    {
                        item.Despawn(true);
                    }
                }

                items.itemsInCart.Clear();
                items.cartTotal.Value = 0;

                if (NetworkManager.Singleton.ConnectedClients.TryGetValue(items.OwnerClientId, out var client))
                {
                    var clientObject = client.PlayerObject;
                    if (clientObject != null)
                    {
                        var player = clientObject.GetComponentInChildren<PlayerPickup>();
                        if (player != null)
                        {
                            player.totalScorePlayer.Value += cartTotalValue; 
                        }
                    }
                }
            }
            else
            {
                Debug.LogWarning("ItemsInCart component is null.");
            }
        }
        else
        {
            Debug.LogWarning("No ItemsInCart component found on the collider.");
        }
    }
}