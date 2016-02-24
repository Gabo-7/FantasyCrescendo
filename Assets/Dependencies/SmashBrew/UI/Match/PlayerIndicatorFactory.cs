using HouraiTeahouse.Events;
using UnityEngine;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerIndicatorFactory : MonoBehaviour {

        [SerializeField]
        private PlayerIndicator _prefab;

        private Mediator _eventManager;

        void Awake() {
            if (!_prefab) {
                Destroy(this);
                return;
            }
            _eventManager = GlobalMediator.Instance;
            _eventManager.Subscribe<PlayerSpawnEvent>(OnSpawnPlayer);
        }

        private void OnDestroy() {
            if (_eventManager != null)
                _eventManager.Unsubscribe<PlayerSpawnEvent>(OnSpawnPlayer);
        }

        private void OnSpawnPlayer(PlayerSpawnEvent eventArgs) {
            if (eventArgs == null || eventArgs.Player == null)
                return;
            PlayerIndicator indicator = Instantiate(_prefab);
            indicator.Target = eventArgs.Player;
        }


    }

}