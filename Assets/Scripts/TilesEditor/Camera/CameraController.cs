using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace TilesEditor
{
    /// <summary>
    /// The movement manager of the camera in the editor scene according to the player's inputs. 
    /// </summary>
    public class CameraController : MonoBehaviour
    {
        [SerializeField] private Camera _camera;
        [SerializeField] private CameraData _data;
        [SerializeField] private Slider _slider;
        [SerializeField] private Map _map;
        [SerializeField] private TMP_Text _cameraPositionValue;

        private float _speed;
        private float _startCameraZoom;
        private float _maxCameraZoom;
        
        private Vector3 _mouseOffsetFromCamera;

        private void Start()
        {
            _startCameraZoom = _camera.orthographicSize;

            SetSizeSliderValues();
            SetStartCameraPosition();
            SetStartCameraZoom();
        }
        
        private void Update()
        {
            if (TilesEditor.Instance.MenuIsOpen()) return;

            UpdateCameraPosition();

            UpdateCameraZoom();
        }
        
        /// <summary>
        /// Reset the camera.
        /// </summary>
        public void Reset()
        {
            SetStartCameraPosition();
            SetStartCameraZoom();
        }

        /// <summary>
        /// Get the half the width of the camera
        /// </summary>
        /// <returns>Half Width.</returns>
        private float GetHalfWidth()
        {
            return (_camera.orthographicSize * Screen.width / Screen.height) + 1;
        }
        
        /// <summary>
        /// Get the half the height of the camera
        /// </summary>
        /// <returns>Half Height.</returns>
        private float GetHalfHeight()
        {
            return _camera.orthographicSize + 1;
        }

        /// <summary>
        /// Update the text values.
        /// </summary>
        private void SetPositionTextValues()
        {
            _cameraPositionValue.text = $"Camera set to x {Math.Round(_camera.transform.position.x, 2)}, y {Math.Round(_camera.transform.position.y, 2)}";
        }

        private void SetSizeSliderValues()
        {
            SetSpeed(_slider.value);
            _slider.onValueChanged.AddListener(SetSpeed);
            _slider.minValue = _data.MinSpeed;
            _slider.maxValue = _data.MaxSpeed;
        }

        /// <summary>
        /// Set the speed according to the slider value.
        /// </summary>
        /// <param name="value"> Slider value. </param>
        private void SetSpeed(float value)
        {
            _speed = value;
        }

        /// <summary>
        /// Set the camera to the left bottom corner of the map.
        /// </summary>
        private void SetStartCameraPosition()
        {
            _camera.transform.position = new Vector3(GetHalfWidth(), GetHalfHeight(), _camera.transform.position.z);
            _camera.orthographicSize = _startCameraZoom;
            SetPositionTextValues();
        }

        private void SetStartCameraZoom()
        {
            var maxWidth = _map.MapSize.x / 2f / 16f * 9f - 1f;
            var maxHeight = _map.MapSize.y / 2f - 1f;
            _maxCameraZoom = Mathf.Min(maxWidth, maxHeight);
        }

        private void UpdateCameraPosition()
        {
            Vector3 newPos = _camera.transform.position + GetKeyboardPanDirection() + GetMousePanDirection();

            float clampX = Mathf.Clamp(newPos.x, GetHalfWidth(), _map.MapSize.x - GetHalfWidth());
            float clampY = Mathf.Clamp(newPos.y, GetHalfHeight(), _map.MapSize.y - GetHalfHeight());

            if (newPos != _camera.transform.position)
            {
                SetPositionTextValues();
            }

            _camera.transform.position = new Vector3(clampX, clampY, _camera.transform.position.z);
        }

        /// <summary>
        /// Get a target direction to apply to the camera position, given by keyboard inputs.
        /// </summary>
        /// <returns>A direction given by the inputs.</returns>
        private Vector3 GetKeyboardPanDirection()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (Mathf.Approximately(horizontal, 0) && Mathf.Approximately(vertical, 0))
            {
                return Vector3.zero;
            }

            return new Vector3(horizontal, vertical, 0f) * (_speed * Time.deltaTime);
        }

        /// <summary>
        /// Get a target direction to apply to the camera position, given by the middle mouse button.
        /// </summary>
        /// <returns>A direction given by the middle mouse button.</returns>
        private Vector3 GetMousePanDirection()
        {
            var worldPosition = _camera.ScreenToWorldPoint(Input.mousePosition);

            // Move camera
            if (Input.GetMouseButtonDown(2))
            {
                _mouseOffsetFromCamera = worldPosition;
            }

            if (Input.GetMouseButton(2))
            {
                Vector3 difference = _mouseOffsetFromCamera - worldPosition;
                if (difference.sqrMagnitude < 0.0001f) _mouseOffsetFromCamera = worldPosition;
                return difference * _data.PanSpeed;
            }

            return Vector3.zero;
        }
        
        /// <summary>
        /// Update the zoom of the camera, controlled by the mouse scroll wheel.
        /// </summary>
        private void UpdateCameraZoom()
        {
            if (Input.GetAxis("Mouse ScrollWheel") != 0f)
            {
                float zoomDistance = _camera.orthographicSize;
                var scrollAmount = Input.GetAxis("Mouse ScrollWheel") * _data.ScrollSensitivity;
                scrollAmount *= zoomDistance * 0.3f;
                zoomDistance -= scrollAmount;
                
                zoomDistance = Mathf.Clamp(zoomDistance, _data.ScrollZoomMin, Mathf.Min(_maxCameraZoom, _data.ScrollZoomMax));
                _camera.orthographicSize = zoomDistance;
            }
        }
    }
}