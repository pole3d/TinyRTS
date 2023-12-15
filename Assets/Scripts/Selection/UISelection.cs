using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UISelection : MonoBehaviour
{
    public static UISelection Instance;

    public List<GameObject> UnitFaceImages;

    public List<Button> ActionUnitsButtons;

    public List<Sprite> ActionUnitsIcons;

    [field: SerializeField]
    public Dictionary<UnitData.ActionType, Sprite> ActionIcons = new Dictionary<UnitData.ActionType, Sprite>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.LogError("There is already another UISelection script in this scene !");
        }
    }

    private void Start()
    {
        ActionIcons.Add(UnitData.ActionType.Move, ActionUnitsIcons[0]);
        ActionIcons.Add(UnitData.ActionType.Attack, ActionUnitsIcons[1]);
        ActionIcons.Add(UnitData.ActionType.Build, ActionUnitsIcons[2]);
        ActionIcons.Add(UnitData.ActionType.Repair, ActionUnitsIcons[3]);
        ActionIcons.Add(UnitData.ActionType.Protect, ActionUnitsIcons[4]);
    }

    public void UpdateUISelection(int nbUnitSelected, bool isHomogeneous, List<Unit> unitsList)
    {
        //Reset Images
        foreach (var go in UnitFaceImages)
        {
            go.GetComponent<Image>().sprite = null;
            go.SetActive(false);
        }

        //Active nb of image by nb of units selected
        for (int i = 0; i < unitsList.Count; i++)
        {
            UnitFaceImages[i].SetActive(true);

            UnitFaceImages[i].GetComponent<Image>().sprite = unitsList[i]._unitData.IconSprite;
        }

        Debug.Log(unitsList.Select(unit => unit._unitData.UnitType).Distinct().Count());

        //Reset Unit Buttons
        foreach (var button in ActionUnitsButtons)
        {
            button.gameObject.SetActive(false);
        }

        //Check the types of the selection and active buttons
        if (unitsList.Select(unit => unit._unitData.UnitType).Distinct().Count() == 1)
        {
            for (int i = 0; i < unitsList[0]._unitData.UnitActions.Count; i++)
            {
                ActionUnitsButtons[i].gameObject.SetActive(true);

                if (ActionIcons.ContainsKey(unitsList[0]._unitData.UnitActions[i]))
                {
                    ActionUnitsButtons[i].GetComponent<Image>().sprite =
                        ActionIcons[unitsList[0]._unitData.UnitActions[i]];

                    // //Text Button Action
                    // ActionUnitsButtons[i].GetComponentInChildren<TextMeshProUGUI>().text =
                    //     unitsList[0]._unitData.UnitActions[i].ToString();
                }
            }
        }
    }
}