using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TilesEditor
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Range(5, 30)] private float _minSpeed;
        [SerializeField, Range(5, 30)] private float _maxSpeed;
        [SerializeField] private Slider _slider;
        [SerializeField] private Map _map;
        [SerializeField] private TMP_Text _cameraPositionValue;

        private Camera _camera;
        private float _speed;

        private float _halfWidth;
        private float _halfHeight;

        private void Start()
        {
            _camera = GetComponent<Camera>();

            _halfWidth = (_camera.orthographicSize * Screen.width / Screen.height) + 1;
            _halfHeight = _camera.orthographicSize + 1;

            SetSizeSliderValues();
            SetStartCameraPosition();
        }

        private void SetPositionText()
        {
            _cameraPositionValue.text = $"Camera set to x {Math.Round(transform.position.x, 2)}, y {Math.Round(transform.position.y, 2)}";
        }

        /// <summary>
        /// Set the slider's initial values.
        /// </summary>
        private void SetSizeSliderValues()
        {
            SetSpeed(_slider.value);
            _slider.onValueChanged.AddListener(SetSpeed);
            _slider.minValue = _minSpeed;
            _slider.maxValue = _maxSpeed;
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
        public void SetStartCameraPosition()
        {
            transform.position = new Vector3(_halfWidth, _halfHeight, -10);
            SetPositionText();
        }

        void Update()
        {
            if (TilesEditor.Instance.MenuIsOpen())
            {
                return;
            }

            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");

            if (horizontal == 0 && vertical == 0)
            {
                return;
            }

            Vector3 newPos = transform.position + new Vector3(horizontal, vertical, 0f) * (_speed * Time.deltaTime);

            float clampX = Mathf.Clamp(newPos.x, _halfWidth, _map.MapSize.x - _halfWidth);
            float clampY = Mathf.Clamp(newPos.y, _halfHeight, _map.MapSize.y - _halfHeight);

            if (newPos != transform.position)
            {
                SetPositionText();
            }

            transform.position = new Vector3(clampX, clampY, -10);

        }
    }
}