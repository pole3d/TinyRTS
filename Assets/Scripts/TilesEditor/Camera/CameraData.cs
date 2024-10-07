using UnityEngine;

namespace TilesEditor
{
    [CreateAssetMenu(fileName = "New TilesEditorCameraData", menuName = "TilesEditor/TilesEditorCameraData")]
    public class CameraData : ScriptableObject
    {
        [field:SerializeField, Range(5, 30)] public float MinSpeed {get; private set; }
        [field:SerializeField, Range(5, 30)] public float MaxSpeed {get; private set; }
        [field: SerializeField, Range(0,1)] public float PanSpeed { get; private set; } = 1f;
        [field: SerializeField] public float ScrollSensitivity { get; private set; } = 2f;
        [field: SerializeField] public float ScrollZoomMin { get; private set; } = 2f;
        [field: SerializeField] public float ScrollZoomMax { get; private set; } = 7f;
    }
}