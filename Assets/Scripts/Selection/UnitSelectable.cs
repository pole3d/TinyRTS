using UnityEngine;

public class UnitSelectable : MonoBehaviour
{
    [SerializeField] private GameObject _selectedGameObject;

    public void SetSelectedVisible(bool visible)
    {
        _selectedGameObject.SetActive(visible);
    }
}