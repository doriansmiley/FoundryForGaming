using GPF;
using UnityEngine;

namespace Coin
{
    public class VersionCheckVB : MonoBehaviour
    {
        public Transform versionMismatchPopup;

        bool ignore;

        void Awake()
        {
            DataStore.Instance.AddListener(Syncer.DataStoreVariables.VERSION_STATE, Listener);
        }

        void Listener()
        {
            var versionState = DataStore.Instance.Get<Syncer.VersionState>(Syncer.DataStoreVariables.VERSION_STATE);
            versionMismatchPopup.gameObject.SetActive(!ignore && versionState == Syncer.VersionState.MISMATCH);
        }

        public void IgnoreMistmatch()
        {
            ignore = true;
            Listener();
        }
    }
}
