using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.EventSystems;
using TMPro;

using Entities;
using Entities.Bodies;
using Entities.Bodies.Health;
using Entities.Bodies.Damages;

using Utilities.Events;

using UI.Utils;

namespace UI.Menus
{
    public struct VitalLogInfo
    {
        public UiFieldVitalsLog log;
        public GameObject go;

        public VitalLogInfo(UiFieldVitalsLog log, GameObject go) { this.log = log; this.go = go; }
    }

    public class UiVitalsLog : UiWithScrollableItems, IEventListener<AgentChangedEvent>
    {
        public int NumLogsInPool = 20;

        private static string MenuTitle = "VITALS MONITORING";
        private static string StatusField = "INJURY: ";

        public GameObject VitalsLogEntryPrefab;

        private List<VitalLogInfo> VitalLogs;
        private Agent agent;
        private int ActiveLogs = 0;

        override public void Awake()
        {
            base.Awake();

            TitleTextLeft.Text = MenuTitle;
            TitleTextRight.Text = GetStatusString(EDamageState.None);
        }

        public void Start()
        {
            // generate pool of available vital logs
            VitalLogs = new List<VitalLogInfo>();
            for (int i = 0; i < NumLogsInPool; i++)
                AddNewVitalLogToPool();
        }

        private void AddNewVitalLogToPool()
        {
             var vitalsLogObj = Instantiate(VitalsLogEntryPrefab);
             var uiFieldVitalsLog = vitalsLogObj.GetComponent<UiFieldVitalsLog>();

             vitalsLogObj.transform.parent = ContentRoot.transform;
             vitalsLogObj.SetActive(false);

             VitalLogs.Add(new VitalLogInfo(uiFieldVitalsLog, vitalsLogObj));
        }

        public void SetObservedAgent(Agent agent)
        {
            // only update if currently observed agent is not the same as the agent we want to observe
            if (this.agent != agent)
            {
                this.agent = agent;

                agent.AddListener(this);

                UpdateVitalsLog();
            }
        }

        void DeactivateVitalsLog()
        {
            foreach (var vli in VitalLogs)
            {
                vli.go.SetActive(false);
            }

            ActiveLogs = 0;
        }

        void UpdateVitalsLog()
        {
            TitleTextRight.Text = GetStatusString(this.agent.GetDamageState());

            DeactivateVitalsLog();
            ProcessVitalsLog(agent.Body);
        }

        public string GetStatusString(EDamageState DamageState)
        {
            return StatusField + DamageStates.DamageStateToStrWithColor(DamageState);
        }

        public string HpSystemToRichString(HPSystem hpSystem)
        {
            return RichStrings.WithColor("[" + hpSystem.HpCurrent + "/" + hpSystem.HpBase + "]", DamageStates.DamageStateToColor(hpSystem.GetDamageState()));
        }

        public void ProcessVitalsLog(BodyPart bodyPart, int depth=0)
        {
            if (bodyPart.HasHpSystem && bodyPart.IsDamaged)
            {
                if (ActiveLogs == this.VitalLogs.Count)
                    AddNewVitalLogToPool();

                var vli = this.VitalLogs[ActiveLogs];

                vli.log.Initialize(bodyPart);
                vli.go.SetActive(true);

                ActiveLogs++;
            }

            if (bodyPart is BodyPartContainer bpc)
                foreach (var bp in bpc.BodyParts)
                    ProcessVitalsLog(bp, depth + 1);
        }

        public bool OnEvent(AgentChangedEvent gameEvent)
        {
            bool active = gameEvent.entity == this.agent;

            if (active)
                UpdateVitalsLog();

            return active;
        }
    }
}