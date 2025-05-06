using System;
using Unity.Netcode;
using UnityEngine;

public class PlayerPickup : NetworkBehaviour
{
    private ItemObject holdingItemObject;
    private int pickupRange = 4;
    public LayerMask itemLayer;

    public Camera playercam;
    public GameObject HoldingPoint;

    private NetworkVariable<NetworkObjectReference> heldItemRef = new NetworkVariable<NetworkObjectReference>();
    public NetworkVariable<float> totalScorePlayer = new NetworkVariable<float>(
        0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server
    );

    void Update()
    {
        if (!IsOwner) return;

        Debug.Log(totalScorePlayer.Value);

        if (Input.GetMouseButtonDown(0))
        {
            if (holdingItemObject == null)
            {
                TryPickup();
            }
            else
            {
                DropItem();
            }
        }

        if (holdingItemObject != null && Input.GetKey(KeyCode.E))
        {
            StealItem();
        }
    }

    private void StealItem()
    {
        if (holdingItemObject != null)
        {
            StealItemServerRpc(holdingItemObject.NetworkObject);
            holdingItemObject = null;
        }
    }

    private void TryPickup()
    {
        if (Physics.Raycast(playercam.transform.position, playercam.transform.forward, out RaycastHit hit, pickupRange, itemLayer))
        {
            if (hit.transform.TryGetComponent<ItemObject>(out var item))
            {
                var itemNetObj = item.GetComponent<NetworkObject>();
                if (itemNetObj != null && itemNetObj.IsSpawned)  // Ensure the object is spawned
                {
                    RequestPickupServerRpc(itemNetObj);  // Pass the actual NetworkObject
                    holdingItemObject = item;
                }
                else
                {
                    Debug.LogWarning($"Item {item.name} is not spawned on the network.");
                }
            }
        }
    }

    private void DropItem()
    {
        if (holdingItemObject != null)
        {
            DropItemServerRpc(holdingItemObject.NetworkObject);
            holdingItemObject = null;
        }
    }

    [ServerRpc]
    private void RequestPickupServerRpc(NetworkObjectReference itemRef)
    {
        if (itemRef.TryGet(out var itemObj))
        {
            var item = itemObj.GetComponent<ItemObject>();
            if (item != null)
            {
                item.SetFollowTarget(HoldingPoint.transform);
                heldItemRef.Value = item.NetworkObject;  // Update reference after successful pickup
            }
        }
    }

    [ServerRpc]
    private void DropItemServerRpc(NetworkObjectReference itemRef)
    {
        if (itemRef.TryGet(out var itemObj))
        {
            var item = itemObj.GetComponent<ItemObject>();
            if (item != null)
            {
                item.SetFollowTarget(null);
                heldItemRef.Value = default;
            }
        }
    }

    [ServerRpc]
    private void StealItemServerRpc(NetworkObjectReference itemRef)
    {
        if (itemRef.TryGet(out var itemObj))
        {
            var item = itemObj.GetComponent<ItemObject>();
            if (item != null)
            {
                item.SetFollowTarget(transform);
                heldItemRef.Value = item.NetworkObject;
            }
        }
    }
}