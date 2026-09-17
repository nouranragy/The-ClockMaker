using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using ExitGamesHashtable = ExitGames.Client.Photon.Hashtable;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    [Header("UI Controls")]
    [SerializeField] private TMP_InputField NameInputField;
    [SerializeField] private Button PlayButton;
    [SerializeField] private Button RefreshButton;
    [SerializeField] private GameObject loadingPanel;
    [SerializeField] private TMP_Text statusText;
    [Header("Matchmaking Settings")]
    [SerializeField] private float loadingDuration = 3f;
    [SerializeField] private string roomName = "ClockmakerRoom";
    [SerializeField] private byte maxPlayers = 2;
    [Header("Scene Routing")]
    [SerializeField] private string masterSceneName = "Past";
    [SerializeField] private string clientSceneName = "Present";
    private const string GAME_STARTED = "GameStarted";
    private bool isStartingGame;
    private bool isMasterForGame;
    private Coroutine loadingCoroutine;

    private void Start()
    {
        Application.runInBackground = true;
        PhotonNetwork.KeepAliveInBackground = 60f;
        PhotonNetwork.AutomaticallySyncScene = false;
        if (loadingPanel) loadingPanel.SetActive(false);
        if (NameInputField) NameInputField.onValueChanged.AddListener(_ => UpdateNickname());
        if (RefreshButton) RefreshButton.onClick.AddListener(OnRefreshButtonClicked);
        if (!ValidateScenes()) return;

        if (PhotonNetwork.IsConnected && PhotonNetwork.IsConnectedAndReady)
        {
            OnConnectedToMaster();
        }
        else
        {
            ConnectToPhoton();
        }
    }

    private void OnDestroy()
    {
        if (NameInputField) NameInputField.onValueChanged.RemoveAllListeners();
        if (RefreshButton) RefreshButton.onClick.RemoveAllListeners();
    }

    private void ConnectToPhoton()
    {
        if (PhotonNetwork.IsConnected) return;
        string uniqueInstanceId = System.Guid.NewGuid().ToString().Substring(0, 5);
        PhotonNetwork.AuthValues = new AuthenticationValues(uniqueInstanceId);
        PhotonNetwork.PhotonServerSettings.AppSettings.FixedRegion = "eu";
        PhotonNetwork.PhotonServerSettings.AppSettings.AppVersion = "1.0";
        PhotonNetwork.NickName = GetNickname("Player_" + uniqueInstanceId);
        SetStatus("Connecting...");
        PhotonNetwork.ConnectUsingSettings();
    }

    public void OnPlayButtonClicked()
    {
        if (!PhotonNetwork.IsMasterClient || isStartingGame || PhotonNetwork.CurrentRoom == null) return;
        if (PhotonNetwork.CurrentRoom.PlayerCount != maxPlayers) return;

        UpdateNickname();
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGamesHashtable { { GAME_STARTED, true } });
        PhotonNetwork.CurrentRoom.IsOpen = false;
        PhotonNetwork.CurrentRoom.IsVisible = false;
    }

    public void OnRefreshButtonClicked()
    {
        StopLoading();
        SetStatus("Refreshing connection...");
        if (PhotonNetwork.IsConnected) PhotonNetwork.Disconnect();
        else ConnectToPhoton();
    }

    public override void OnConnectedToMaster()
    {
        SetStatus("Joining room...");
        var opts = new RoomOptions
        {
            MaxPlayers = maxPlayers,
            CleanupCacheOnLeave = true,
            IsVisible = true,
            IsOpen = true
        };
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
        if (isStartingGame && PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers)
        {
            StopLoading();
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGamesHashtable { { GAME_STARTED, false } });
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
        }
        UpdateUI();
    }

    public override void OnMasterClientSwitched(Player newMaster)
    {
        if (isStartingGame)
        {
            StopLoading();
            if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom != null)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGamesHashtable { { GAME_STARTED, false } });
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
        }
        isMasterForGame = PhotonNetwork.IsMasterClient;
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
        UpdateUI();
        if (cause == DisconnectCause.DisconnectByClientLogic) ConnectToPhoton();
    }

    public override void OnJoinRoomFailed(short code, string msg) => SetStatus($"Join failed: {msg}");

    private string GetNickname(string fallback) => string.IsNullOrWhiteSpace(NameInputField?.text) ? fallback : NameInputField.text.Trim();

    private void UpdateNickname()
    {
        if (!PhotonNetwork.IsConnectedAndReady || PhotonNetwork.LocalPlayer == null) return;
        string n = GetNickname("Player_" + PhotonNetwork.LocalPlayer.ActorNumber);
        PhotonNetwork.NickName = PhotonNetwork.LocalPlayer.NickName = n;
        if (PhotonNetwork.InRoom) PhotonNetwork.LocalPlayer.SetCustomProperties(new ExitGamesHashtable { { "NickName", n } });
    }

    private void UpdateUI()
    {
        if (!PhotonNetwork.InRoom || isStartingGame)
        {
            if (PlayButton) PlayButton.interactable = false;
            return;
        }
        SetStatus($"Players: {PhotonNetwork.CurrentRoom.PlayerCount}/{maxPlayers}");
        if (PlayButton) PlayButton.interactable = PhotonNetwork.CurrentRoom.PlayerCount == maxPlayers && PhotonNetwork.IsMasterClient;
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

        if (!PhotonNetwork.InRoom || !PhotonNetwork.IsConnectedAndReady ||
            PhotonNetwork.CurrentRoom.PlayerCount < maxPlayers)
        {
            StopLoading();
            yield break;
        }

        string targetScene = isMasterForGame ? masterSceneName : clientSceneName;
        try
        {
            PhotonNetwork.LoadLevel(targetScene);
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Load error: {ex.Message}");
            StopLoading();
        }
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