using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSelector : MonoBehaviour
{
    [SerializeField] private Transform _selectionAreaTransform;

    private Vector3 _startPosition;
    public List<UnitSelectable> _selectedUnitList;
    public List<Unit> _Units;

    [SerializeField] private bool IsHomogeneous;

    private void Awake()
    {
        _selectedUnitList = new List<UnitSelectable>();
        _selectionAreaTransform.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            _selectionAreaTransform.gameObject.SetActive(true);
            _startPosition = Utils.GetMouseWorldPosition();
        }

        if (Input.GetMouseButton(0) && EventSystem.current.IsPointerOverGameObject() == false)
        {
            Vector3 currentMousePosition = Utils.GetMouseWorldPosition();
            Vector3 lowerLeft = new Vector3(
                Mathf.Min(_startPosition.x, currentMousePosition.x),
                Mathf.Min(_startPosition.y, currentMousePosition.y)
            );
            Vector3 upperRight = new Vector3(
                Mathf.Max(_startPosition.x, currentMousePosition.x),
                Mathf.Max(_startPosition.y, currentMousePosition.y)
            );
            _selectionAreaTransform.position = lowerLeft;
            _selectionAreaTransform.localScale = upperRight - lowerLeft;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _selectionAreaTransform.gameObject.SetActive(false);

            if (!Input.GetKey(KeyCode.LeftShift))
            {
                foreach (UnitSelectable unit in _selectedUnitList)
                {
                    unit.SetSelectedVisible(false);
                }

                _selectedUnitList.Clear();
                _Units.Clear();
            }

            Collider2D[] collider2DArray = Physics2D.OverlapAreaAll(_startPosition, Utils.GetMouseWorldPosition());

            bool allUnitsSelected = true;

            foreach (Collider2D collider2D in collider2DArray)
            {
                UnitSelectable unit = collider2D.GetComponent<UnitSelectable>();
                if (unit != null)
                {
                    if (!_selectedUnitList.Contains(unit))
                    {
                        allUnitsSelected = false;
                        break;
                    }
                }
            }

            foreach (Collider2D collider2D in collider2DArray)
            {
                UnitSelectable unit = collider2D.GetComponent<UnitSelectable>();
                if (unit != null)
                {
                    if (allUnitsSelected)
                    {
                        unit.SetSelectedVisible(false);
                        _selectedUnitList.Remove(unit);
                        _Units.Remove(unit.GetComponent<Unit>());
                    }
                    else
                    {
                        if (_selectedUnitList.Contains(unit) == false)
                        {
                            unit.SetSelectedVisible(true);
                            _selectedUnitList.Add(unit);
                            _Units.Add(unit.GetComponent<Unit>());
                        }
                    }
                }
            }

            //IsHomogeneous = AreAllItemsOfSameEnumType(_selectedUnitList);

            UISelection.Instance.UpdateUISelection(_selectedUnitList.Count, IsHomogeneous, _Units);
        }
    }
    
    /// <summary>
    /// Check if all selected units have the same UnitType
    /// </summary>
    // private bool AreAllItemsOfSameEnumType(List<UnitSelectable> list)
    // {
    //     if (list.Count <= 0) return true;
    //
    //     UnitData.Type firstUnitType = list[0].unitData.UnitType;
    //
    //     return list.All(item => item.unitData.UnitType == firstUnitType);
    // }
}