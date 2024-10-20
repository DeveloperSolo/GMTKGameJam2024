using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EntityInfoScript : MonoBehaviour
{
    [Header("Info")]
    [SerializeField] private List<Info> infoList;

    [Header("Components")]
    [SerializeField] private GameObject infoCanvas;
    [SerializeField] private TextMeshProUGUI infoText;

    private ScaleMechanicComponent scaleMechanic = null;

    private void Awake()
    {
        if(infoCanvas != null)
        {
            ScaleMechanicListenerScript scaleMechanicListener = infoCanvas.GetComponent<ScaleMechanicRectTransformListenerScript>();
            if(scaleMechanicListener != null)
            {
                scaleMechanic = scaleMechanicListener.Source;
            }
        }
    }

    private void Start()
    {
        if(IsInfoShown())
        {
            HideInfo();
        }
    }

    private void Update()
    {
        if(scaleMechanic.GetGizmoMode() == ScaleGizmoMode.Shown)
        {
            if(!IsInfoShown())
            {
                ShowInfo();
            }
            UpdateInfo();
        }
        else
        {
            if(IsInfoShown())
            {
                HideInfo();
            }
        }
    }

    private void ShowInfo()
    {
        if(IsInfoShown())
        {
            return;
        }
        infoCanvas.SetActive(true);
    }

    private void HideInfo()
    {
        if (!IsInfoShown())
        {
            return;
        }
        infoCanvas.SetActive(false);
    }

    private bool IsInfoShown()
    {
        return infoCanvas.activeSelf;
    }

    private void UpdateInfo()
    {
        string text = "";
        foreach(Info info in infoList)
        {
            text += info.InfoName + ": " + info.GetInfoValue() + "\n";
        }
        if(text.Length > 0)
        {
            text.Remove(text.Length - 1, 1);
        }
        infoText.text = text;
    }

    [System.Serializable]
    public class Info
    {
        [SerializeField] private string infoName;
        [SerializeField] private UnityEvent<Info> getInfoValueEvent;

        public string InfoName { get { return infoName; } }
        public string InfoValue { get; set; }

        public string GetInfoValue()
        {
            getInfoValueEvent.Invoke(this);
            return InfoValue;
        }
    }
}