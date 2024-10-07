using UnityEngine;

namespace TilesEditor.Tiles
{
    [CreateAssetMenu(menuName = "TilesEditor/TileData")]
    public class TileDataScriptable : ScriptableObject
    {
        [field:SerializeField] public TileData Data { get; private set; }
    }
}