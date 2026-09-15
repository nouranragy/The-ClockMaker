using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;
using Photon.Pun;
using Photon.Realtime;

public class MainMenuManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField nameInputField;

    private void Start()
    {
        if (!PhotonNetwork.IsConnected)
        {
            Debug.Log("Connecting to Photon Master Server...");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("Connected to Master Server! Joining or creating room...");
        PhotonNetwork.JoinOrCreateRoom("GlobalChatRoom", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Player successfully connected to the room!");
    }
    public void OnPlayButtonPressed()
    {
        string playerName = nameInputField.text;
        if (string.IsNullOrWhiteSpace(playerName))  playerName = "Player " + Random.Range(10, 99);
        PhotonNetwork.NickName = playerName;
        Debug.Log("Is Connected: " + PhotonNetwork.IsConnected + " | In Room: " + PhotonNetwork.InRoom);
        if (PhotonNetwork.InRoom)  RedirectToGameScene();
        else  Debug.LogWarning("Room Not Connected Yet. Please wait a few seconds and try again.");
    }

    private void RedirectToGameScene()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)  SceneManager.LoadScene("past");
        else   SceneManager.LoadScene("present");
    }
}