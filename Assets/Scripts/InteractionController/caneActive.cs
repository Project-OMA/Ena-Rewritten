using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;


public class caneActive : MonoBehaviour
{
    public GameObject bengala;
    public XRBaseController controller;

    bool isItemActive = false;
    readonly bool lastPrimaryButtonState = false;

    InputDevice xrInputDevice;


    // Start is called before the first frame update
    void Update()
    {
        bool primaryButtonValue = GetPrimaryButtonValue();

        if (primaryButtonValue && !lastPrimaryButtonState)
        {
            isItemActive = !isItemActive;
            bengala.SetActive(isItemActive);
        }

    }

    // Update is called once per frame
    bool GetPrimaryButtonValue()
    {
        var xrInputDevices = new List<InputDevice>();
        InputDevices.GetDevicesAtXRNode(XRNode.RightHand, xrInputDevices);
        xrInputDevice = xrInputDevices.FirstOrDefault();

        var isFeatureAvalible = xrInputDevice.TryGetFeatureValue(CommonUsages.primaryButton, out var buttonValue);

        return isFeatureAvalible && buttonValue;
    }
}
