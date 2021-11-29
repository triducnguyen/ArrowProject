using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportManager : MonoBehaviour
{
    [SerializeField] private InputActionAsset actionAsset;
    [SerializeField] private XRRayInteractor rayInteractor;
    [SerializeField] private TeleportationProvider provider;
    [SerializeField] private GameObject reticle;


    private InputAction _thumbstick;
    private bool _isActivate;

    // Start is called before the first frame update
    void Start()
    {
        rayInteractor.enabled = false;
        reticle.SetActive(false);

        var activate = actionAsset.FindActionMap("XRI RightHand").FindAction("Teleport Mode Activate");
        activate.Enable();
        activate.performed += OnTeleportActivated;

        var cancel = actionAsset.FindActionMap("XRI RightHand").FindAction("Teleport Mode Cancel");
        cancel.Enable();
        cancel.performed += OnTeleportCancel;

        _thumbstick = actionAsset.FindActionMap("XRI RightHand").FindAction("Move");
        _thumbstick.Enable();
    }

    // Update is called once per frame
    void Update()
    {
        if (!_isActivate)
            return;

        if (_thumbstick.triggered)
        {
            return;
        }

        if(!rayInteractor.GetCurrentRaycastHit(out RaycastHit hit))
        {
            rayInteractor.enabled = false;
            _isActivate = false;
            reticle.SetActive(false);

            return;
        }

        TeleportRequest request = new TeleportRequest()
        {
            destinationPosition = hit.point
        };

        provider.QueueTeleportRequest(request);
        rayInteractor.enabled = false;
        _isActivate = false;
        reticle.SetActive(false);

    }

    private void OnTeleportActivated(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = true;
        _isActivate = true;
        reticle.SetActive(true);

        Debug.Log("activating");
    }

    private void OnTeleportCancel(InputAction.CallbackContext context)
    {
        rayInteractor.enabled = false;
        _isActivate = false;
        reticle.SetActive(false);


    }

}
