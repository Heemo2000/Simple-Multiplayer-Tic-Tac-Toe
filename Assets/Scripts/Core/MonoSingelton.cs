using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;


namespace Game.Core
{

    public abstract class MonoSingelton<T> : MonoBehaviour where T: MonoSingelton<T>
    {
        private static T instance;

        [SerializeField]private bool deactivateOnLoad;
        [SerializeField]private bool dontDestroyOnLoad;
        private bool isInitialized;

        public static T Instance
        {
            get
            {
                if(instance == null)
                {
                    var instances = GameObject.FindObjectsByType<T>(FindObjectsInactive.Include, FindObjectsSortMode.None);
                    if(instances == null || instances.Length == 0)
                    {
                        return null;
                    }

                    instance = instances.FirstOrDefault(i => i.gameObject.scene.buildIndex != -1);
                    instance?.Init();
                }

                return instance;
            }
        }

        protected virtual void Awake() 
        {
            if(instance == null || !instance.gameObject)
            {
                instance = (T)this;
            }    
            else if(instance != this)
            {

                Debug.LogError($"Another instance of {GetType()} already exists!! Destroying self...");
                Destroy(this);
                return;
            }

            instance.Init();
        }

        private void Start()
        {
            if(isInitialized)
            {
                InternalOnStart();
            }
        }

        public void Init()
        {
            if(isInitialized)
            {
                return;
            }

            if(dontDestroyOnLoad)
            {
                DontDestroyOnLoad(gameObject);
            }

            if(deactivateOnLoad)
            {
                gameObject.SetActive(false);
            }

            
            if(gameObject == null)
            {
                return;
            }
            

            SceneManager.activeSceneChanged += SceneManagerOnActiveSceneChanged;

            InternalInit();
            isInitialized = true;
        }

        private void SceneManagerOnActiveSceneChanged(Scene current, Scene next)
        {
            if(!instance || !gameObject || gameObject == null)
            {
                SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
                instance = null;
                return;
            }

            if(dontDestroyOnLoad)
            {
                return;
            }

            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;
            instance = null;
        }

        protected abstract void InternalInit();

        protected abstract void InternalOnStart();
        protected abstract void InternalOnDestroy();


        private void OnApplicationQuit() 
        {
            instance = null;    
        }

        private void OnDestroy() 
        {
            SceneManager.activeSceneChanged -= SceneManagerOnActiveSceneChanged;

            InternalOnDestroy();
            if(instance != this)
            {
                return;
            }

            instance = null;
            isInitialized = false;
        }
    }
}
