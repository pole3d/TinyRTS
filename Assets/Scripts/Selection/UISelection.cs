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
    
    private Dictionary<UnitData.ActionType, Sprite> ActionIcons = new Dictionary<UnitData.ActionType, Sprite>();

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
        ActionIcons.Add(UnitData.ActionType.Stop, ActionUnitsIcons[2]);
        ActionIcons.Add(UnitData.ActionType.Patrol, ActionUnitsIcons[3]);
        ActionIcons.Add(UnitData.ActionType.Repair, ActionUnitsIcons[4]);
        ActionIcons.Add(UnitData.ActionType.Build, ActionUnitsIcons[5]);
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

        //Debug.Log(unitsList.Select(unit => unit._unitData.UnitType).Distinct().Count());

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
        else if(unitsList.Select(unit => unit._unitData.UnitType).Distinct().Count() == 2)
        {
            List<UnitData.ActionType> commonActions = new List<UnitData.ActionType>(unitsList[0]._unitData.UnitActions);

            for (int i = 1; i < unitsList.Count; i++)
            {
                commonActions = commonActions.Intersect(unitsList[i]._unitData.UnitActions).ToList();
            }

            // Now 'commonActions' contains the ActionType(s) that are common to all units
            Debug.Log("Common Actions: " + string.Join(", ", commonActions.Select(action => action.ToString())));


            for (int i = 0; i < commonActions.Count; i++)
            {
                ActionUnitsButtons[i].gameObject.SetActive(true);
                ActionUnitsButtons[i].GetComponent<Image>().sprite = ActionIcons[commonActions[i]];
            }

            // foreach (UnitData.ActionType action in commonActions)
            // {
            //     GameObject buttonGO = Instantiate(buttonPrefab, buttonParent);
            //     Button button = buttonGO.GetComponent<Button>();
            //     button.onClick.AddListener(() => HandleButtonClick(action));
            //     // Set button text, image, etc., based on the action type
            // }
        }
    }
}