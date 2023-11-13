using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace TilesEditor
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField, Range(5,30)] private float _maxSpeed;
        [SerializeField] private Slider _slider;
        [SerializeField] private Map _map;

        private Camera _camera;
        private float _speed;

        private float _halfWidth;
        private float _halfHeight;

        private void Start()
        {
            _camera = GetComponent<Camera>();

            _halfWidth = _camera.orthographicSize * Screen.width / Screen.height;
            _halfHeight = _camera.orthographicSize;

            SetSpeed(_slider.value);
            _slider.onValueChanged.AddListener(SetSpeed);

            SetCameraPosition();
        }

        
        /// <summary>
        /// Set the speed according to the slider value.
        /// </summary>
        /// <param name="value"> Slider value. </param>
        private void SetSpeed(float value)
        {
            _speed = _maxSpeed * value;
            EventSystem.current.SetSelectedGameObject(null);
        }

        /// <summary>
        /// Set the camera to the left bottom corner of the map.
        /// </summary>
        private void SetCameraPosition()
        {
            transform.position = new Vector3(_halfWidth, _halfHeight, -10);
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
            
            transform.position = new Vector3(clampX, clampY, -10);
        }
    }
}