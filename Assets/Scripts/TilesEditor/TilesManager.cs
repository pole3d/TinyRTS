using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.WSA;

namespace TilesEditor
{
    public class TilesManager : MonoBehaviour
    {
        public static TilesManager Instance;
        private Inputs Inputs { get; set; }

        [SerializeField] private ToolSelection[] _tools;
        private ToolSelection _currentSelected;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Debug.Log("There is already an Instance");
            }
            
            Inputs = new Inputs();
        }

        public void SetCurrentTool(ToolSelection currentTool)
        {
            _currentSelected = currentTool;
        }

        public void UpdatePanels()
        {
            foreach (ToolSelection tool in _tools)
            {
                if (tool != _currentSelected)
                {
                    tool.ShowUI(false);
                }
            }
        }

        private void OnEnable()
        {
            Inputs.Enable();
        }

        private void OnDisable()
        {
            Inputs.Disable();
        }
    }
}