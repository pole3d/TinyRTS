using System.Collections.Generic;
using UnityEngine;

public class UnitSelector : MonoBehaviour
{
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
        if (Input.GetMouseButtonDown(0))
        {
            _selectionAreaTransform.gameObject.SetActive(true);
            _startPosition = Utils.GetMouseWorldPosition();
        }

        if (Input.GetMouseButton(0))
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
                    }
                    else
                    {
                        if (_selectedUnitList.Contains(unit) == false)
                        {
                            unit.SetSelectedVisible(true);
                            _selectedUnitList.Add(unit);
                        }
                    }
                }
            }

            Debug.Log(_selectedUnitList.Count);
        }
    }
}