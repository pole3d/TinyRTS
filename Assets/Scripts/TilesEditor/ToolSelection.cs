using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TilesEditor
{
    public class ToolSelection : MonoBehaviour
    {
        [SerializeField] private Image _seletedDisplay;
        [SerializeField] private GameObject _panelToDisplay;
        private Button _button;

        private void Start()
        {
            _button = GetComponent<Button>();
            
            _button.onClick.AddListener(DisplayData);
        }

        public void DisplayData()
        {
            ShowUI(true);

            TilesManager.Instance.SetCurrentTool(this);
            TilesManager.Instance.UpdatePanels();
        }

        public void ShowUI(bool display)
        {
            _seletedDisplay.enabled = display;
            _panelToDisplay.SetActive(display);
        }
    }
}