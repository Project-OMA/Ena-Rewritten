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
    bool lastPrimaryButtonState = false;

    InputDevice xrInputDevice;

    public AudioSource m_MyAudioSource;

    void Start()
    {
        //Fetch the AudioSource from the GameObject
        m_MyAudioSource = GetComponent<AudioSource>();
        
    }

    




    // Start is called before the first frame update
    void Update()
    {
        bool primaryButtonValue = GetPrimaryButtonValue();

        if (primaryButtonValue && !lastPrimaryButtonState)
        {
            isItemActive = !isItemActive;
            bengala.SetActive(isItemActive);

            if (isItemActive) {
                m_MyAudioSource.pitch = 1.0f;
            } else {
                m_MyAudioSource.pitch = 0.5f;
            }
            m_MyAudioSource.Play();
        }

        lastPrimaryButtonState = primaryButtonValue;
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
