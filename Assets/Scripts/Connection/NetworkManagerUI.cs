using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class ConnectUiScript : MonoBehaviour
{
    [SerializeField] private Button hostButton;
    [SerializeField] private Button clientButton;

    private void Start()
    {
    }

    private void HostButtonOnClick()
    {
        NetworkManager.Singleton.StartHost();
    }

    private void ClientButtonOnClick()
    {
        NetworkManager.Singleton.StartClient();
    }
}