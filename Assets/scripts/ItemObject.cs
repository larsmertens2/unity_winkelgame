using Unity.Netcode;
using UnityEngine;

[RequireComponent(typeof(NetworkObject))]
public class ItemObject : NetworkBehaviour
{
    private FollowTransform followTransform;

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
}
