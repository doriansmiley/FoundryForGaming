using UnityEngine;
using GPF.UI;
using UnityEngine.UI;

public class TextAfterAnim : MonoBehaviour, IPropReceiver
{
    // Props give this ViewBinding a unique view of data from the DataStore
    public Props props { get; set; }

    // A DataPath field enables autocomplete in the Unity Editor
    public DataPath text;

    public DataPath flipping;

    Text textComponent;

    void Awake()
    {
        textComponent = GetComponent<Text>();

        // Get our props. Props can be provided from parent objects in the hierarchy
        Props.Inject(this);

        // We want the UI to be updated every time the data changes
        // The change listener will be called whenever the SO changes
        props.AddListener(flipping, Listener);
        props.AddListener(text, Listener);
    }


    void Listener()
    {
        var isFlipping = props.Get<bool>(flipping);
        if (!isFlipping)
        {
            var textValue = props.Get<string>(text);
            textComponent.text = textValue;
        }
    }
}