using System.Collections.Generic;
using Core.Ticks.Interfaces;
using UnityEngine;

namespace Core.Ticks
{
    public class UpdateService : MonoBehaviour, IUpdateService
    {
        private List<IUpdatable> _updates;
        private List<ILateUpdate> _lateUpdates;
        private List<IFixedUpdateRunner> _fixedUpdates;
        private List<IGizmoRunner> _gizmoRunners;

        private bool _enabled;
        
        private void Awake()
        {
            _enabled = true;
            _updates = new List<IUpdatable>();
            _lateUpdates = new List<ILateUpdate>();
            _fixedUpdates = new List<IFixedUpdateRunner>();
            _gizmoRunners = new List<IGizmoRunner>();
        }
        
        void Update()
        {
            if (!_enabled) return;
            for (int i = _updates.Count - 1; i >= 0; i--) 
            {
                var runner = _updates[i];

                runner.CustomUpdate();
                //FrameRateTiming.MeasureByStopwatch();
            }
        }
        
        void LateUpdate()
        {
            if (!_enabled) return;
            for (int i = _lateUpdates.Count - 1; i >= 0; i--) 
            {
                var runner = _lateUpdates[i];

                runner.CustomLateUpdate();
                //FrameRateTiming.MeasureByStopwatch();
            }
        }

        void FixedUpdate()
        {
            if (!_enabled) return;
            for (int i = _fixedUpdates.Count - 1; i >= 0; i--) 
            {
                var runner = _fixedUpdates[i];

                runner.CustomFixedUpdate();
                //FrameRateTiming.MeasureByStopwatch();
            }
        }

        private void OnDrawGizmos() {
            if (!_enabled) return;
            for (int i = _gizmoRunners.Count - 1; i >= 0; i--) 
            {
                var runner = _gizmoRunners[i];
                
                runner.DrawGizmo();
            }
        }

        public void RegisterUpdate(IUpdatable updatable) 
        {
            if (!_updates.Contains(updatable))
            {
                _updates.Add(updatable);
            }
        }

        public void RegisterLateUpdate(ILateUpdate lateUpdate)
        {
            if (!_lateUpdates.Contains(lateUpdate))
            {
                _lateUpdates.Add(lateUpdate);
            }
        }

        public void RegisterGizmoUpdate(IGizmoRunner gizmoRunner) {
            if (!_gizmoRunners.Contains(gizmoRunner)) {
                _gizmoRunners.Add(gizmoRunner);
            }
        }

        public void RegisterFixedUpdate(IFixedUpdateRunner fixedUpdateRunner)
        {
            if (!_fixedUpdates.Contains(fixedUpdateRunner))
            {
                _fixedUpdates.Add(fixedUpdateRunner);
            }
        }

        public void UnregisterUpdate(IUpdatable updatable)
        {
            if (_updates.Contains(updatable))
            {
                _updates.Remove(updatable);
            }
        }

        public void UnregisterFixedUpdate(IFixedUpdateRunner fixedUpdateRunner)
        {
            if (_fixedUpdates.Contains(fixedUpdateRunner))
            {
                _fixedUpdates.Remove(fixedUpdateRunner);
            }
        }

        public void UnregisterLateUpdate(ILateUpdate lateUpdate)
        {
            if (_lateUpdates.Contains(lateUpdate))
            {
                _lateUpdates.Remove(lateUpdate);
            }
        }

        public void UnregisterGizmoUpdate(IGizmoRunner gizmoRunner) {
            if (_gizmoRunners.Contains(gizmoRunner)) {
                _gizmoRunners.Remove(gizmoRunner);
            }
        }

        public void StopAll()
        {
            _enabled = false;
        }

        public void StartAll()
        {
            _enabled = true;
        }
    }
}