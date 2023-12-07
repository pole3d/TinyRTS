using System.Collections.Generic;
using UnityEngine;

namespace Selection
{
    /// <summary>
    /// Detect and manage the selection of the unit in the game scene
    /// </summary>
    public class UnitSelectionController : MonoBehaviour
    {
        private const int MouseButtonToSelect = 0;
    
        [SerializeField] private Transform _selectionAreaTransform;
    
        private Vector3 _startPosition;
        private List<UnitSelectable> _selectedUnitList;

        private void Awake()
        {
            _selectedUnitList = new List<UnitSelectable>();
            _selectionAreaTransform.gameObject.SetActive(false);
        }

        private void Update()
        {
            CheckSelectionStart();

            CheckSelectionHold();

            CheckSelectionEnd();
        }
        
        /// <summary>
        /// Once the player end his selection, check the units in it and make selections depending on it
        /// </summary>
        private void CheckSelectionEnd()
        {
            if (Input.GetMouseButtonUp(MouseButtonToSelect) == false)
            {
                return;
            }
            
            _selectionAreaTransform.gameObject.SetActive(false);

            //if the player wasn't pressing shift, unselect all the current selected units
            if (Input.GetKey(KeyCode.LeftShift) == false)
            {
                foreach (UnitSelectable unit in _selectedUnitList)
                {
                    unit.SetSelectedVisible(false);
                }

                _selectedUnitList.Clear();
            }

            Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(_startPosition, Utils.GetMouseWorldPosition());

            bool allUnitsSelected = true;

            foreach (Collider2D colliderToSelect in collider2DArray)
            {
                if (colliderToSelect.TryGetComponent(out UnitSelectable unit) == false || _selectedUnitList.Contains(unit))
                {
                    continue;
                }

                allUnitsSelected = false;
                break;
            }

            foreach (Collider2D colliderToSelect in collider2DArray)
            {
                if (colliderToSelect.TryGetComponent(out UnitSelectable unit) == false)
                {
                    continue;
                }
                
                if (allUnitsSelected)
                {
                    unit.SetSelectedVisible(false);
                    _selectedUnitList.Remove(unit);
                }
                else if (_selectedUnitList.Contains(unit) == false)
                {
                    unit.SetSelectedVisible(true);
                    _selectedUnitList.Add(unit);
                }
            }

            Debug.Log(_selectedUnitList.Count + " unit(s) selected");
        }

        /// <summary>
        /// Check the current selection of the player while he's holding it
        /// </summary>
        private void CheckSelectionHold()
        {
            if (Input.GetMouseButton(MouseButtonToSelect) == false)
            {
                return;
            }
            
            Vector3 currentMousePosition = Utils.GetMouseWorldPosition();
            Vector3 lowerLeft = new Vector2
            (
                Mathf.Min(_startPosition.x, currentMousePosition.x),
                Mathf.Min(_startPosition.y, currentMousePosition.y)
            );
            Vector3 upperRight = new Vector2
            (
                Mathf.Max(_startPosition.x, currentMousePosition.x),
                Mathf.Max(_startPosition.y, currentMousePosition.y)
            );
            _selectionAreaTransform.position = lowerLeft;
            _selectionAreaTransform.localScale = upperRight - lowerLeft;
        }

        /// <summary>
        /// Check if the player start the selection and set the selection ui active if he do
        /// </summary>
        private void CheckSelectionStart()
        {
            if (Input.GetMouseButtonDown(MouseButtonToSelect) == false)
            {
                return;
            }
            
            _selectionAreaTransform.gameObject.SetActive(true);
            _startPosition = Utils.GetMouseWorldPosition();
        }
    }
}