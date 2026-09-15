using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGamesHashtable = ExitGames.Client.Photon.Hashtable;
public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI")]
    [SerializeField] private TMP_InputField NameInputField;
    [SerializeField] private Button PlayButton;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TMP_Text statusText;
    [Header("Settings")]
    [SerializeField] private float loadingDuration = 3f;
    [SerializeField] private string roomName = "ClockmakerRoom";
    [SerializeField] private byte maxPlayers = 2;
    [SerializeField] private string masterSceneName = "past";
    [SerializeField] private string clientSceneName = "present";
    private const string GAME_STARTED = "GameStarted";
    private bool isStartingGame, isMasterForGame;
    private Coroutine loadingCoroutine;
    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        if (loadingPanel) loadingPanel.SetActive(false);
        if (NameInputField) NameInputField.onValueChanged.AddListener(_ => UpdateNickname());
        if (!ValidateScenes()) return;
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";//to make sure where is the error
            PhotonNetwork.NickName = GetNickname("Player");
            SetStatus("Connecting to Server...");
            PhotonNetwork.ConnectUsingSettings();
        }
        else if (PhotonNetwork.InRoom) UpdateUI();
    }
    private void OnDestroy()
    {
        if (NameInputField) NameInputField.onValueChanged.RemoveAllListeners();
    }
    public override void OnConnectedToMaster()
    {
        SetStatus("Connected,Joining matchmaking.");
        var opts = new RoomOptions { MaxPlayers = maxPlayers, CleanupCacheOnLeave = true, IsVisible = true, IsOpen = true };
        PhotonNetwork.JoinOrCreateRoom(roomName, opts, TypedLobby.Default);
    }
    public override void OnJoinedRoom()
    {
        UpdateNickname();
        isMasterForGame = PhotonNetwork.IsMasterClient;
        UpdateUI();
        if (PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue(GAME_STARTED, out var v) && v is true) StartGameSequence();
    }
    public override void OnPlayerEnteredRoom(Player p) => UpdateUI();
    public override void OnPlayerLeftRoom(Player p)
    {
        if (isStartingGame && PhotonNetwork.CurrentRoom?.PlayerCount < maxPlayers)
        {
            StopLoading();
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGamesHashtable { { GAME_STARTED, false } });
        }
        UpdateUI();
    }
    public override void OnMasterClientSwitched(Player newMaster)
    {
        if (!isStartingGame) isMasterForGame = PhotonNetwork.IsMasterClient;
        UpdateUI();
    }
    public override void OnRoomPropertiesUpdate(ExitGamesHashtable props)
    {
        if (props.ContainsKey(GAME_STARTED) && props[GAME_STARTED] is true) StartGameSequence();
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        StopLoading();
        SetStatus($"Disconnected: {cause}");
    }
    public override void OnJoinRoomFailed(short code, string msg) => SetStatus($"Join failed: {msg}");
    public void OnPlayButtonClicked()
    {
        if (!PhotonNetwork.IsMasterClient || isStartingGame || PhotonNetwork.CurrentRoom == null) return;
        UpdateNickname();
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGamesHashtable { { GAME_STARTED, true } });
    }
    private string GetNickname(string fallback) => string.IsNullOrWhiteSpace(NameInputField?.text) ? fallback : NameInputField.text.Trim();
    private void UpdateNickname()
    {
        if (!PhotonNetwork.IsConnectedAndReady || PhotonNetwork.LocalPlayer == null) return;
        string n = GetNickname("Player " + PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.NickName = PhotonNetwork.LocalPlayer.NickName = n;
        if (PhotonNetwork.InRoom) PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGamesHashtable { { "NickName", n } });
    }
    private void UpdateUI()
    {
        if (!PhotonNetwork.InRoom || isStartingGame) return;

        // Cleaned string without the region prefix
        SetStatus($"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{maxPlayers}");

        if (PlayButton)
            PlayButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers && PhotonNetwork.IsMasterClient;
    }
    private void SetStatus(string msg) { if (statusText) statusText.text = msg; }
    private void StartGameSequence()
    {
        if (isStartingGame) return;
        StopLoading();
        loadingCoroutine = StartCoroutine(LoadingRoutine());
    }
    private IEnumerator LoadingRoutine()
    {
        isStartingGame = true;
        if (PlayButton) PlayButton.interactable = false;
        if (loadingPanel) loadingPanel.SetActive(true);
        SetStatus("Setting Everything...");
        yield return new WaitForSeconds(loadingDuration);
        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsConnectedAndReady)
        {
            StopLoading();
            yield break;
        }
        string targetScene = isMasterForGame ? masterSceneName : clientSceneName;
        try { PhotonNetwork.LoadLevel(targetScene); }
        catch (System.Exception ex) { Debug.LogError($"Load error: {ex.Message}"); StopLoading(); }
        loadingCoroutine = null;
    }
    private void StopLoading()
    {
        if (loadingCoroutine != null) StopCoroutine(loadingCoroutine);
        loadingCoroutine = null;
        isStartingGame = false;
        if (loadingPanel) loadingPanel.SetActive(false);
    }
    private bool ValidateScenes()
    {
        bool valid = Application.CanStreamedLevelBeLoaded(masterSceneName) && Application.CanStreamedLevelBeLoaded(clientSceneName);
        if (!valid) SetStatus("Scene config error!");
        return valid;
    }
}