using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Entities;
using Entities.Bodies;
using Entities.Bodies.Health;
using Entities.Bodies.Damages;

using Utilities.Events;

using TMPro;

public class VitalsMonitoringMenu : MonoBehaviour, IEventListener<AgentChangedEvent>
{
    public static string StatusField = "INJURY: ";

    public GameObject StatusText;
    public GameObject LeftInfoField;
    public GameObject RightInfoField;

    private Agent agent;

    private TextMeshProUGUI _StatusTextMesh;
    private TextMeshProUGUI _LeftInfoFieldTextMesh;
    private TextMeshProUGUI _RightInfoFieldTextMesh;

    void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        _StatusTextMesh = StatusText.GetComponent<TextMeshProUGUI>();
        _LeftInfoFieldTextMesh = LeftInfoField.GetComponent<TextMeshProUGUI>();
        _RightInfoFieldTextMesh = RightInfoField.GetComponent<TextMeshProUGUI>();
    }

    void UpdateView()
    {
        if (this.agent == null)
            return;

        _StatusTextMesh.text = GetStatusString(this.agent.GetDamageState());
    }

    public string GetStatusString(EDamageState DamageState)
    {
        return StatusField + DamageStates.DamageStateToStrWithColor(DamageState);
    }

    public void SetObservedAgent(Agent agent)
    {
        if (this.agent != agent)
        {
            this.agent = agent;
            agent.AddListener(this);
            UpdateView();
        }
    }

    public bool OnEvent(AgentChangedEvent gameEvent)
    {
        bool active = gameEvent.entity == this.agent;

        if (active)
            UpdateView();

        return active;
    }
}
