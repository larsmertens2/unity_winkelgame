using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class ItemsInCart : NetworkBehaviour
{
    public NetworkList<NetworkObjectReference> itemsInCart;

    public NetworkVariable<float> cartTotal = new NetworkVariable<float>(0f, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Server);

    private void Awake()
    {
        itemsInCart = new NetworkList<NetworkObjectReference>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsServer) return; // Only the server should track cart items

        if (other.TryGetComponent<ItemObject>(out var item))
        {
            NetworkObject netObj = item.GetComponent<NetworkObject>();

            NetworkObjectReference itemRef = new NetworkObjectReference(netObj);
            if (!itemsInCart.Contains(itemRef))
            {
                itemsInCart.Add(itemRef);
                
                UpdateCartTotal();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsServer) return;

        if (other.TryGetComponent<ItemObject>(out var item))
        {
            NetworkObject netObj = item.GetComponent<NetworkObject>();
            NetworkObjectReference itemRef = new NetworkObjectReference(netObj);

            if (itemsInCart.Contains(itemRef))
            {
                itemsInCart.Remove(itemRef);
                
                UpdateCartTotal();
            }
        }
    }

    private void UpdateCartTotal()
    {
        float total = 0f;

        foreach (var itemRef in itemsInCart)
        {
            if (itemRef.TryGet(out NetworkObject networkObject))
            {
                if (networkObject.TryGetComponent<ItemObject>(out var itemObj))
                {
                    total += itemObj.price;
                }
            }
        }

        cartTotal.Value = total;
    }

    public List<NetworkObjectReference> GetItems()
    {
        return new List<NetworkObjectReference>((IEnumerable<NetworkObjectReference>)itemsInCart);
    }
}