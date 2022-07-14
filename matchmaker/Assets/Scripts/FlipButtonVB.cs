using UnityEngine;
using GPF.UI;
using UnityEngine.UI;
using System;

public class FlipButtonVB : MonoBehaviour, IPropReceiver
{
    // Props give this ViewBinding a unique view of data from the DataStore
    public Props props { get; set; }

    // A DataPath field enables autocomplete in the Unity Editor
    public DataPath flipping;

    public GameObject Enabled;
    public GameObject Disabled;

    void Awake()
    {
        // Get our props. Props can be provided from parent objects in the hierarchy
        Props.Inject(this);

        // We want the UI to be updated every time the data changes
        // The change listener will be called whenever the SO changes
        props.AddListener<bool>(flipping, Listener);
    }

    void Listener(bool isFlipping)
    {
        Enabled.SetActive(!isFlipping);
        Disabled.SetActive(isFlipping);
    }
}