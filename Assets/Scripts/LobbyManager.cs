using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGamesHashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI References")]
    [SerializeField] private TMP_InputField NameInputField;
    [SerializeField] private Button PlayButton;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TMP_Text statusText;
    private const string GAME_STARTED_PROP = "GameStarted";
    private bool isStartingGame;

    private void Start()
    {
        if (loadingPanel != null) loadingPanel.SetActive(false);
        PhotonNetwork.AutomaticallySyncScene = false;
        if (NameInputField != null) NameInputField.onValueChanged.AddListener(OnNameInputValueChanged);
        UpdatePlayButtonState();
        if (!PhotonNetwork.IsConnected)
        {
            if (statusText != null) statusText.text = "Connecting to Master Server....";
            PhotonNetwork.ConnectUsingSettings();
        }
    }
    public override void OnConnectedToMaster()
    {
        if (statusText != null) statusText.text = "Connected. Joining Room...";
        PhotonNetwork.JoinOrCreateRoom("ClockmakerRoom", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        UpdateLocalNickname();
        UpdateStatusUI();
        if (PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GAME_STARTED_PROP, out object gameStarted) && (bool)gameStarted) StartCoroutine(LoadingSequenceCoroutine());
    }
    private void OnNameInputValueChanged(string newName) => UpdateLocalNickname();
    private void UpdateLocalNickname()
    {
        string nameToSet = string.IsNullOrWhiteSpace(NameInputField?.text) ? "Player " + PhotonNetwork.LocalPlayer.ActorNumber : NameInputField.text;
        PhotonNetwork.NickName = nameToSet;
    }
    public override void OnPlayerEnteredRoom(Player newPlayer) => UpdateStatusUI();
    public override void OnPlayerLeftRoom(Player otherPlayer) => UpdateStatusUI();
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGamesHashtable changedProps) => UpdateStatusUI();
    public override void OnMasterClientSwitched(Player newMasterClient) => UpdateStatusUI();

    private void UpdateStatusUI()
    {
        if (!PhotonNetwork.InRoom) return;
        if (statusText != null) statusText.text = $"Players in Lobby: {PhotonNetwork.CurrentRoom.PlayerCount}/2";
        UpdatePlayButtonState();
    }
    private void UpdatePlayButtonState()
    {
        if (PlayButton == null || isStartingGame) return;
        PlayButton.interactable = PhotonNetwork.InRoom && PhotonNetwork.CurrentRoom.PlayerCount == 2 && PhotonNetwork.IsMasterClient;
    }
    public void OnPlayButtonClicked()
    {
        if (!PhotonNetwork.IsMasterClient || isStartingGame) return;
        UpdateLocalNickname();
        StartCoroutine(LoadingSequenceCoroutine());
        ExitGamesHashtable props = new ExitGamesHashtable { { GAME_STARTED_PROP, true } };
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
    }
    public override void OnRoomPropertiesUpdate(ExitGamesHashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey(GAME_STARTED_PROP) && (bool)propertiesThatChanged[GAME_STARTED_PROP])
        {
            if (!isStartingGame) StartCoroutine(LoadingSequenceCoroutine());
        }
    }
    private IEnumerator LoadingSequenceCoroutine()
    {
        isStartingGame = true;
        if (PlayButton != null) PlayButton.interactable = false;
        if (loadingPanel != null) loadingPanel.SetActive(true);
        if (statusText != null) statusText.text = "Setting EveryThing...";
        yield return new WaitForSeconds(3.0f);
        if (loadingPanel != null) loadingPanel.SetActive(false);
        if (PhotonNetwork.IsMasterClient) PhotonNetwork.LoadLevel("past");
        else PhotonNetwork.LoadLevel("present");
    }
}
