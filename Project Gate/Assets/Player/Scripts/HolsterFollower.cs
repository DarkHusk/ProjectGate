using UnityEngine;

public class HolsterFollower : MonoBehaviour
{
    [SerializeField] Transform xrOrigin;
    [SerializeField] Vector3 holsterOffset = new Vector3(0, -0.5f, 0.2f);

    void Update()
    {
        Vector3 originPos = xrOrigin.position;
        Quaternion originRot = xrOrigin.rotation;

        Quaternion flatRotation = Quaternion.Euler(0, originRot.eulerAngles.y, 0);
        transform.position = originPos + flatRotation * holsterOffset;
        transform.rotation = flatRotation;
    }
}
