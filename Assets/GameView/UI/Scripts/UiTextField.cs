using System;
using System.Collections.Generic;

using UnityEngine.UI;

using UnityEngine;
using TMPro;
using System.Collections;

namespace UI.Menus
{
    [System.Serializable]
    public class UiTextField
    {
        public GameObject GameObject;

        private TextMeshProUGUI TextMesh;

        public string DefaultText = "Text Field";

        public string Text
        {
            set { this.TextMesh.text = value; }
            get { return this.TextMesh.text; }
        }

        public float FontSize { get { return this.TextMesh.fontSize; } }
        public int NumLines { get { return this.TextMesh.textInfo.lineCount; } }

        public void Initialize()
        {
            this.TextMesh = this.GameObject.GetComponent<TextMeshProUGUI>();
            this.Text = DefaultText;
        }
    }

    [System.Serializable]
    public class UiTwoTextField : MonoBehaviour
    {
        public UiTextField TextLeft = new UiTextField();
        public UiTextField TextRight = new UiTextField();

        private LayoutElement LayoutElement;

        private bool UpdatingLayoutSize = false;

        public virtual void Awake()
        {
            Initialize();
        }

        public void Initialize()
        {
            this.LayoutElement = this.GetComponent<LayoutElement>();

            TextLeft.Initialize();
            TextRight.Initialize();
        }

        public void TriggerUpdateLayoutSize()
        {
            if (!UpdatingLayoutSize)
            {
                try
                {
                    if (this.gameObject.activeSelf)
                    {
                        UpdatingLayoutSize = true;
                        StartCoroutine(UpdateLayoutSize());
                    }
                }
                catch (MissingReferenceException) { Debug.Log("Tried to update layout size of an inactive UiTwoTextField."); }
            }
        }

        public IEnumerator UpdateLayoutSize()
        {
            // need to wait until next Frame for textmesh to render once so that NumLines is updated
            yield return new WaitForEndOfFrame();

            // try/catch in case object is destroyed while waiting
            try
            {
                int numLines = Mathf.Max(TextLeft.NumLines, TextRight.NumLines) - 1;
                float fontSize = Mathf.Max(TextLeft.FontSize, TextRight.FontSize);
                this.LayoutElement.preferredHeight = Mathf.Max(
                    this.LayoutElement.minHeight,
                    this.LayoutElement.minHeight + numLines * fontSize);

                UpdatingLayoutSize = false;
            }
            catch (MissingReferenceException) { Debug.Log("Tried to update layout size of a null UiTwoTextField."); }
        }

        // Update is called once per frame
        void OnValidate()
        {
            Awake();
        }

        private void OnEnable()
        {
            TriggerUpdateLayoutSize();
        }
    }

}
