using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Bolt : MonoBehaviour
{
    [SerializeField] UnityEvent ejectEvent;
    [SerializeField] UnityEvent gunCockEvent;
    [SerializeField] ConfigurableJoint joint;
    [SerializeField] Transform bolt;
    [SerializeField] XRGrabInteractable grabInteractable;

    public void CockGun(SelectEnterEventArgs args)
    {
        IXRSelectInteractor interactor = args.interactorObject;

        ejectEvent.Invoke();
        gunCockEvent.Invoke();
        grabInteractable.interactionManager.SelectCancel(interactor, grabInteractable);
    }

    //bool maxDistanceReached = false;

    //void Update()
    //{
    //    //if (maxDistanceReached && joint.connectedAnchor.magnitude <= 0.01)
    //    //{
    //    //    maxDistanceReached = false;
    //    //    gunCockEvent.Invoke();
    //    //}
    //    //else if (!maxDistanceReached && joint.connectedAnchor.magnitude >= joint.linearLimit.limit)
    //    //{
    //    //    maxDistanceReached = true;
    //    //    ejectEvent.Invoke();
    //    //}
    //    
    //
    //
    //}
    
}
