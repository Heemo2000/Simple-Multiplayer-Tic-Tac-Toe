using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Game.Gameplay;
namespace Game.UI
{
    public class NetworkManagerUI : MonoBehaviour
    {
        [SerializeField]private Button startHostButton;
        [SerializeField]private Button startClientButton;

        private Image backgroundImage;
        private Transform[] children;
        private void HideNetworkManagerScreen()
        {
            backgroundImage.enabled = false;
            int childCount = transform.childCount;
            for(int i = 0; i < childCount; i++)
            {
                children[i].gameObject.SetActive(false);
            }
        }

        private void Awake() 
        {
            backgroundImage = GetComponent<Image>();
        }

        // Start is called before the first frame update
        void Start()
        {
            if(children == null)
            {
                int childCount = transform.childCount;
                children = new Transform[childCount];
                for(int i = 0; i < childCount; i++)
                {
                    children[i] = transform.GetChild(i);
                }
            }

            startHostButton.onClick.AddListener(()=>{
                HideNetworkManagerScreen();
                NetworkManager.Singleton.StartHost();
            });

            startClientButton.onClick.AddListener(()=>{
                HideNetworkManagerScreen();
                NetworkManager.Singleton.StartClient();
            });
            
            GameManager.Instance.OnGameStarted += HideNetworkManagerScreen;
            
        }
    }
}
