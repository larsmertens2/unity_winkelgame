using System;
using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class ItemObject : NetworkBehaviour
{
    private FollowTransform followTransform;
    public NetworkVariable<bool> isStolen = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);

    public float price = 10f;

    private void Awake()
    {
        followTransform = GetComponent<FollowTransform>();
    }

    public void SetFollowTarget(Transform target)
    {
        followTransform.SetTargetTransform(target);
    }

    public Transform GetFollowTarget()
    {
        return followTransform.transform;
    }

    public NetworkVariable<bool> GetisStolen()
    {
        return isStolen;
    }

    public void SetisStolen(bool isStolen)
    {
        this.isStolen.Value =isStolen;
    }
}
