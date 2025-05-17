using UnityEngine;
using UnityEngine.Events;

public class Bolt : MonoBehaviour
{
    [SerializeField] UnityEvent ejectEvent;
    [SerializeField] UnityEvent gunCockEvent;
    [SerializeField] SpringJoint springJoint;

    bool maxDistanceReached = false;

    void Update()
    {
        if(maxDistanceReached && springJoint.connectedAnchor.magnitude <= springJoint.minDistance)
        {
            maxDistanceReached = false;
            gunCockEvent.Invoke();
        }
        else if(!maxDistanceReached && springJoint.connectedAnchor.magnitude >= springJoint.maxDistance)
        {
            maxDistanceReached = true;
            ejectEvent.Invoke();
        }
    }
}
