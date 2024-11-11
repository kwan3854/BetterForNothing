using UnityEngine;

// use as "spy" to get all the roots from DontdestroyOnLoad from the "inside" :)
namespace BetterForNothing.Scripts.Utils
{
    public class DontDestroyOnLoadAccessor : MonoBehaviour
    {
        private static DontDestroyOnLoadAccessor _instance;
        public static DontDestroyOnLoadAccessor Instance {
            get {
                return _instance;
            }
        }

        void Awake()
        {
            if (_instance != null) Destroy(this);
            this.gameObject.name = this.GetType().ToString();
            _instance = this;
            DontDestroyOnLoad(this);
        }

        public GameObject[] GetAllRootsOfDontDestroyOnLoad() {
            return this.gameObject.scene.GetRootGameObjects();
        }
    }
}