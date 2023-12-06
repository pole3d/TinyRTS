using System.Collections;
using System.Collections.Generic;
using Gameplay.Units;
using UnityEngine;

public class UnitTests : MonoBehaviour
{
    [SerializeField] GameplayData _data;
    [SerializeField] Unit _unit;

    // Start is called before the first frame update
    IEnumerator Start()
    {
        _unit.Initialize(_data.Units[0]);
        yield return new WaitForSeconds(1);
        _unit.Move(new Vector2(3, 3));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
