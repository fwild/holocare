using HoloToolkit.Examples.InteractiveElements;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class PulseManager : Singleton<PulseManager>
    {
        [SerializeField]
        private Material _pulseMaterial;
        [SerializeField]
        private Material _occlusionMaterial;

        [SerializeField]
        private SpatialMappingManager _spatialMappingManager;

        [SerializeField]
        float _interval = 2.5f, _speed = 2f;
        private float _pulseWidth = .35f;

        bool _isPulsing;
        float _timeSinceLastPulse;

        [SerializeField]
        private SliderGestureControl _speedSlider, _pulseWidthSlider, _intervalSlider;

        private void Start()
        {
            _interval = 2.5f;
            _speed = 0.9f;
            _pulseWidth = 0.45f;

            _pulseMaterial.SetFloat("_PulseWidth", _pulseWidth);
        }
        public void StartPulse(Vector3 center)
        {
            Debug.Log("Started pulsing");
            _pulseMaterial.SetVector("_Center", new Vector4(center.x, center.y, center.z, -1));
            _pulseMaterial.SetFloat("_Radius", 0);


            _spatialMappingManager.SetSurfaceMaterial(_pulseMaterial);

            _timeSinceLastPulse = 0;
            _isPulsing = true;
        }

        public void StopPulse()
        {
            Debug.Log("Stopped pulsing");
            _pulseMaterial.SetFloat("_Radius", 100);
            _isPulsing = false;

            _spatialMappingManager.SetSurfaceMaterial(_occlusionMaterial);
        }
        public float PulseWidth
        {
            get
            {
                return _pulseWidth;
            }
            set
            {
                _pulseWidth = value;
                _pulseMaterial.SetFloat("_PulseWidth", value);
            }
        }
        public float Interval
        {
            get
            {
                return _interval;
            }
            set
            {
                _interval = value;
            }
        }
        public float Speed
        {
            get { return _speed; }
            set { _speed = value; }
        }

        private void Update()
        {
            if (!_isPulsing)
                return;

            if (_timeSinceLastPulse >= Interval)
            {
                _timeSinceLastPulse = 0;
                var hitInfo = GazeManager.Instance.HitPosition;
                _pulseMaterial.SetFloat("_Radius", 0);
                _pulseMaterial.SetVector("_Center", new Vector4(hitInfo.x, hitInfo.y, hitInfo.z, -1));
            }
            else
                _timeSinceLastPulse += Time.deltaTime;

            _pulseMaterial.SetFloat("_Radius", _speed * _timeSinceLastPulse);
        }
    }
}
