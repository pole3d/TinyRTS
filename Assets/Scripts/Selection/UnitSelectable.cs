using System;
using UnityEngine;

namespace Selection
{
    /// <summary>
    /// Allow the selection to select this unit and turn on/off the selection UI of it
    /// </summary>
    public class UnitSelectable : MonoBehaviour
    {
        [SerializeField] private GameObject _selectedGameObject;

        private void Awake()
        {
            _selectedGameObject.SetActive(false);
        }

        /// <summary>
        /// Set the selected UI of the unit visible or not
        /// </summary>
        /// <param name="visible">set selected UI visible</param>
        public void SetSelectedVisible(bool visible)
        {
            _selectedGameObject.SetActive(visible);
        }
    }
}