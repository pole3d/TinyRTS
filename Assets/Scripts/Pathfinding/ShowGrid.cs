using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ShowGrid<TGridObject>
{
    public TGridObject[,] GridArray;

    private int _width;
    private int _height;
    private float _cellSize;
    private Transform _parent;
    private Vector3 _originPos;
    private bool _showDebug = false;

    public ShowGrid(int width, int height, float cellSize, Vector3 originPos, Func<ShowGrid<TGridObject>, int, int, TGridObject> createGrid, Transform parent = null)
    {
        this._width = width;
        this._height = height;
        this._cellSize = cellSize;
        _parent = parent;
        _originPos = originPos;

        GridArray = new TGridObject[width, height];
        for (int x = 0; x < GridArray.GetLength(0); x++)
        {
            for (int y = 0; y < GridArray.GetLength(1); y++)
            {
                GridArray[x, y] = createGrid(this, x, y);
            }
        }

        if (_showDebug == true)
        #region Debug
        {
            TextMesh[,] _debugTextArray = new TextMesh[width, height];

            for (int x = 0; x < GridArray.GetLength(0); x++)
            {
                for (int y = 0; y < GridArray.GetLength(1); y++)
                {
                    _debugTextArray[x, y] = CreateWorldText(GridArray[x, y].ToString(), _parent, GetWorldPos(x, y) + new Vector3(cellSize, cellSize) * .5f, 20, Color.white, TextAnchor.MiddleCenter);
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x, y + 1), Color.white, 100f);
                    Debug.DrawLine(GetWorldPos(x, y), GetWorldPos(x + 1, y), Color.white, 100f);
                }
            }
            Debug.DrawLine(GetWorldPos(0, height), GetWorldPos(width, height), Color.white, 100f);
            Debug.DrawLine(GetWorldPos(width, 0), GetWorldPos(width, height), Color.white, 100f);
        }
        #endregion
    }
   

    private Vector3 GetWorldPos(int x, int y)
    {
        return new Vector3(x, y) * _cellSize + _originPos;
    }
    public void GetXY(Vector3 worldPos, out int x, out int y)
    {
        x = Mathf.FloorToInt((worldPos - _originPos).x / _cellSize);
        y = Mathf.FloorToInt((worldPos - _originPos).y / _cellSize);
    }

    public TGridObject GetGridObject(int x, int y)
    {
        if (x >= 0 && y >= 0 && x < _width && y < _height)
            return GridArray[x, y];
        else return default;
    }
    public float GetCellSize()
    {
        return _cellSize;
    }
    public int GetWidth()
    {
        return GridArray.GetLength(0);
    }
    public int GetHeight()
    {
        return GridArray.GetLength(1);
    }

    #region Debug Function
    public static TextMesh CreateWorldText(string text, Transform parent = null, Vector3 localPos = default(Vector3), int fontSize = 10, Color color = default, TextAnchor textAnchor = default)
    {
        return CreateWorldText(parent, text, localPos, fontSize, (Color)color, textAnchor);
    }
    public static TextMesh CreateWorldText(Transform parent, string text, Vector3 localPos, int fontSize, Color color, TextAnchor textAnchor)
    {
        GameObject go = new GameObject("World_Text", typeof(TextMesh));
        Transform t = go.transform;
        t.SetParent(parent, false);
        t.localPosition = localPos;
        TextMesh textMesh = go.GetComponent<TextMesh>();
        textMesh.text = text;
        textMesh.fontSize = fontSize;
        textMesh.color = color;
        textMesh.anchor = textAnchor;
        return textMesh;
    }
    #endregion
}
