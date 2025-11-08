
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class VRPlanetRotation : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;
    private Transform controllerTransform;
    private Quaternion previousControllerRotation;
    private bool isGrabbed = false;

    public float rotationSpeed = 2f;

    void Start()
    {
        grabInteractable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        // Subscribe to grab events
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        controllerTransform = args.interactorObject.transform;
        previousControllerRotation = controllerTransform.rotation;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        controllerTransform = null;
    }

    void Update()
    {
        if (isGrabbed && controllerTransform != null)
        {
            // Calculate controller rotation delta
            Quaternion rotationDelta = controllerTransform.rotation * Quaternion.Inverse(previousControllerRotation);

            // Apply rotation to planet
            transform.rotation = rotationDelta * transform.rotation * Quaternion.Euler(0, 0, 0);
            transform.rotation *= Quaternion.Euler(rotationDelta.eulerAngles * rotationSpeed);

            previousControllerRotation = controllerTransform.rotation;
        }
    }

    void OnDestroy()
    {
        // Unsubscribe from events
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
            grabInteractable.selectExited.RemoveListener(OnRelease);
        }
    }
}