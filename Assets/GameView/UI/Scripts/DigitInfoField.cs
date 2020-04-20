using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;
using Entities;

using Utilities.Events;

public class DigitInfoChangedEvent : GameEvent
{
    
}

public class DigitInfoField : MonoBehaviour, IEventListener<DigitInfoChangedEvent>
{
    private static string SizeRichText(float ratio) { return "<size=" + (int)(ratio * 100 % 101) + "%>"; }

    public GameObject TitleText;
    public GameObject InfoText;

    private TextMeshProUGUI _TitleText;
    private TextMeshProUGUI _InfoText;

    public string Title;
    public string Value;
    public string ValueSuffix;
    [Range(0.1f, 1)] public float ValueSuffixScale = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        _TitleText = TitleText.GetComponent<TextMeshProUGUI>();
        _InfoText = InfoText.GetComponent<TextMeshProUGUI>();

        UpdateTextFields();
    }

    public void UpdateTextFields()
    {
        _TitleText.text = Title;
        _InfoText.text = Value + SizeRichText(ValueSuffixScale) + ValueSuffix;
    }

    // Update is called once per frame
    void OnValidate()
    {
        Start();
    }

    public bool OnEvent(DigitInfoChangedEvent gameEvent)
    {
        throw new System.NotImplementedException();
    }
}
