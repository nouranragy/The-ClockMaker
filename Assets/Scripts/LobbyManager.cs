using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField NameInputField;
    [SerializeField] private Button PlayButton;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TMP_Text statusText;
    [Header("Scene Names")]
    [SerializeField] private string pastSceneName = "past";
    [SerializeField] private string presentSceneName = "present";
    private void Start()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = false;
        PlayButton.interactable = PhotonNetwork.InRoom;
        if (!PhotonNetwork.IsConnected)
        {
            statusText.text = "Connecting to Master Server....";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {
        statusText.text = "Connected. Joining Room...";
        PhotonNetwork.JoinOrCreateRoom("ClockmakerRoom", new RoomOptions{MaxPlayers = 2}, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        PlayButton.interactable = true;
        UpdateStatusText();
    }
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        UpdateStatusText();
    }
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        UpdateStatusText();
    }
    private void UpdateStatusText()
    {
        int count = PhotonNetwork.CurrentRoom.PlayerCount;
        statusText.text = $"Players in Lobby: {count}/2";
    }
    public void OnPlayButtonClicked()
    {
        string playerName = string.IsNullOrWhiteSpace(NameInputField.text) 
            ? "Player " + PhotonNetwork.LocalPlayer.ActorNumber 
            : NameInputField.text;
        PhotonNetwork.NickName = playerName;
        photonView.RPC("RPC_TriggerLoadingState", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.ActorNumber);
    }
    [PunRPC]
    private void RPC_TriggerLoadingState(int triggeringPlayerActorNumber)
    {
        StartCoroutine(LoadingSequenceCoroutine(triggeringPlayerActorNumber));
    }
    private IEnumerator LoadingSequenceCoroutine(int triggeringPlayerActorNumber)
    {
        PlayButton.interactable = false;
        loadingPanel.SetActive(true);
        if (PhotonNetwork.LocalPlayer.ActorNumber == triggeringPlayerActorNumber)  statusText.text = "Loading game room...";
        else  statusText.text = $"{PhotonNetwork.CurrentRoom.GetPlayer(triggeringPlayerActorNumber).NickName} started the game!";
        yield return new WaitForSeconds(3.0f);
        loadingPanel.SetActive(false);
        if (PhotonNetwork.LocalPlayer.ActorNumber == 1)  PhotonNetwork.LoadLevel(pastSceneName);
        else  PhotonNetwork.LoadLevel(presentSceneName);
    }
}
