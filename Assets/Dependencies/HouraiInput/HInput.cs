using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEngine;

namespace HouraiTeahouse.HouraiInput {

    public static class HInput {

        public static event Action OnSetup;
        public static event Action<ulong, float> OnUpdate;
        public static event Action<InputDevice> OnDeviceAttached;
        public static event Action<InputDevice> OnDeviceDetached;
        public static event Action<InputDevice> OnActiveDeviceChanged;

        private static readonly List<InputDeviceManager> DeviceManagers = new List<InputDeviceManager>();

        private static InputDevice _activeDevice = InputDevice.Null;
        private static readonly List<InputDevice> devices = new List<InputDevice>();
        public static ReadOnlyCollection<InputDevice> Devices;

        public static string Platform { get; private set; }
        public static bool InvertYAxis { get; set; }

        private static bool _isSetup;

        private static float _initialTime;
        private static float _currentTime;
        private static float _lastUpdateTime;

        private static ulong _currentTick;

        internal static void SetupInternal() {
            if (_isSetup) 
                return;

            Platform = "{0} {1}".With(SystemInfo.operatingSystem, SystemInfo.deviceModel).ToUpper();

            _initialTime = 0.0f;
            _currentTime = 0.0f;
            _lastUpdateTime = 0.0f;
            _currentTick = 0;

            DeviceManagers.Clear();
            devices.Clear();
            Devices = new ReadOnlyCollection<InputDevice>(devices);
            _activeDevice = InputDevice.Null;

            _isSetup = true;

#if UNITY_STANDALONE_WIN || UNITY_EDITOR_WIN
            if (EnableXInput)
                XInputDeviceManager.Enable();
#endif

            if (OnSetup != null) {
                OnSetup.Invoke();
                OnSetup = null;
            }

#if !(UNITY_ANDROID && INCONTROL_OUYA && !UNITY_EDITOR)
            AddDeviceManager<UnityInputDeviceManager>();
#endif
        }

        internal static void ResetInternal() {
            OnSetup = null;
            OnUpdate = null;
            OnActiveDeviceChanged = null;
            OnDeviceAttached = null;
            OnDeviceDetached = null;

            DeviceManagers.Clear();
            devices.Clear();
            _activeDevice = InputDevice.Null;

            _isSetup = false;
        }

        private static void AssertIsSetup() {
            if (!_isSetup) 
                SetupInternal();
        }

        internal static void UpdateInternal() {
            AssertIsSetup();
            if (OnSetup != null) {
                OnSetup.Invoke();
                OnSetup = null;
            }

            _currentTick++;
            UpdateCurrentTime();
            float deltaTime = _currentTime - _lastUpdateTime;

            UpdateDeviceManagers(deltaTime);

            PreUpdateDevices(deltaTime);
            UpdateDevices(deltaTime);
            PostUpdateDevices(deltaTime);

            UpdateActiveDevice();

            _lastUpdateTime = _currentTime;
        }

        internal static void OnApplicationFocus(bool focusState) {
            if (focusState)
                return;
            for (var i = 0; i < devices.Count; i++) {
                InputDevice device = devices[i];
                for (var j = 0; j < device.Controls.Length; j++) {
                    InputControl control = device.Controls[j];
                    if (control != null)
                        control.SetZeroTick();
                }
            }
        }

        private static void UpdateActiveDevice() {
            InputDevice lastActiveDevice = ActiveDevice;
            for (var i = 0; i < devices.Count; i++) {
                InputDevice device = devices[i];
                if (ActiveDevice == InputDevice.Null ||
                    device.LastChangedAfter(ActiveDevice)) {
                    ActiveDevice = device;
                }
            }

            if (lastActiveDevice == ActiveDevice)
                return;
            OnActiveDeviceChanged.SafeInvoke(ActiveDevice);
        }

        public static void AddDeviceManager(InputDeviceManager inputDeviceManager) {
            AssertIsSetup();

            DeviceManagers.Add(inputDeviceManager);
            inputDeviceManager.Update(_currentTick, _currentTime - _lastUpdateTime);
        }

        public static void AddDeviceManager<T>() where T : InputDeviceManager, new() {
            if (!HasDeviceManager<T>())
                AddDeviceManager(new T());
        }

        public static bool HasDeviceManager<T>() where T : InputDeviceManager {
            return DeviceManagers.OfType<T>().Any();
        }

        private static void UpdateCurrentTime() {
            // Have to do this hack since Time.realtimeSinceStartup is not set until AFTER Awake().
            if (_initialTime < float.Epsilon) 
                _initialTime = Time.realtimeSinceStartup;

            _currentTime = Mathf.Max(0.0f, Time.realtimeSinceStartup - _initialTime);
        }

        private static void UpdateDeviceManagers(float deltaTime) {
            int count = DeviceManagers.Count;
            for (var i = 0; i < count; i++) 
                DeviceManagers[i].Update(_currentTick, deltaTime);
        }

        private static void PreUpdateDevices(float deltaTime) {
            int count = devices.Count;
            for (var i = 0; i < count; i++) 
                devices[i].PreUpdate(_currentTick, deltaTime);
        }

        private static void UpdateDevices(float deltaTime) {
            int count = devices.Count;
            for (var i = 0; i < count; i++)
                 devices[i].Update(_currentTick, deltaTime);
            OnUpdate.SafeInvoke(_currentTick, deltaTime);
        }

        private static void PostUpdateDevices(float deltaTime) {
            int count = devices.Count;
            for (var i = 0; i < count; i++)
                devices[i].PostUpdate(_currentTick, deltaTime);
        }

        public static void AttachDevice(InputDevice inputDevice) {
            AssertIsSetup();

            if (!inputDevice.IsSupportedOnThisPlatform) 
                return;

            devices.Add(inputDevice);
            devices.Sort((d1, d2) => d1.SortOrder.CompareTo(d2.SortOrder));

            OnDeviceAttached.SafeInvoke(inputDevice);

            if (ActiveDevice == InputDevice.Null) 
                ActiveDevice = inputDevice;
        }

        public static void DetachDevice(InputDevice inputDevice) {
            AssertIsSetup();

            devices.Remove(inputDevice);
            devices.Sort((d1, d2) => d1.SortOrder.CompareTo(d2.SortOrder));

            if (ActiveDevice == inputDevice)
                ActiveDevice = InputDevice.Null;
            OnDeviceDetached.SafeInvoke(inputDevice);
        }

        public static void HideDevicesWithProfile(Type type) {
#if !UNITY_EDITOR && UNITY_METRO
			if (type.GetTypeInfo().IsAssignableFrom( typeof( UnityInputDeviceProfile ).GetTypeInfo() ))
			#else
            if (type.IsSubclassOf(typeof (UnityInputDeviceProfile)))
#endif
                UnityInputDeviceProfile.Hide(type);
        }

        static InputDevice DefaultActiveDevice {
            get { return (devices.Count > 0) ? devices[0] : InputDevice.Null; }
        }

        public static InputDevice ActiveDevice {
            get { return _activeDevice ?? InputDevice.Null; }
            private set { _activeDevice = value ?? InputDevice.Null; }
        }

        public static bool EnableXInput { get; set; }
    }
}
