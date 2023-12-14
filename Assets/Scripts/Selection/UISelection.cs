using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISelection : MonoBehaviour
{
    public static UISelection Instance;

    public List<GameObject> UnitFaceImages;
    public List<Sprite> IconUnitsSprites;

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
    }
}