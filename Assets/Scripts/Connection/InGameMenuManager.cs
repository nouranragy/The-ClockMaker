using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using ExitGamesHashtable = ExitGames.Client.Photon.Hashtable;

public class InGameMenuManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio UI Sliders")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    [Header("Scene Names")]
    [SerializeField] private string lobbySceneName = "Lobby";
    [SerializeField] private string masterSceneName = "Past";
    [SerializeField] private string clientSceneName = "Present";

    private const string GAME_STARTED = "GameStarted";
    private bool isLeaving = false;

    private void Start()
    {
        isLeaving = false;
    if (settingsPanel) settingsPanel.SetActive(false);

    
    if (musicSource == null)
    {
       
        musicSource = FindFirstObjectByType<AudioSource>();
    }

   
    if (SoundManager.Instance != null && SoundManager.Instance.sfxSource != null) //music
    {
        sfxSource = SoundManager.Instance.sfxSource;
    }

    if (musicSource != null && musicSlider != null)
    {
        musicSlider.value = musicSource.volume;
        musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
    }

    if (sfxSource != null && sfxSlider != null)
    {
        sfxSlider.value = sfxSource.volume;
        sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
    }
    }

    private void OnDestroy()
    {
        if (musicSlider != null) musicSlider.onValueChanged.RemoveListener(OnMusicVolumeChanged);
        if (sfxSlider != null) sfxSlider.onValueChanged.RemoveListener(OnSFXVolumeChanged);
    }

    public void OnMusicVolumeChanged(float value)
    {
        if (musicSource != null)
        {
            musicSource.volume = value;
            musicSource.mute = (value <= 0.001f);
        }
    }

    public void OnSFXVolumeChanged(float value)
    {
        if (sfxSource != null)
        {
            sfxSource.volume = value;
            sfxSource.mute = (value <= 0.001f);
        }
    }

    public void OnBackToLobbyButtonClicked()
    {
    
    if (isLeaving) return;
    isLeaving = true;

   
    UIController.IsExitingGame = false;

    
    if (!PhotonNetwork.InRoom)
    {
        LoadLobbyScene();
        return;
    }

   
    if (PhotonNetwork.IsMasterClient && PhotonNetwork.CurrentRoom != null)
    {
        PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGamesHashtable { { GAME_STARTED, false } });
        PhotonNetwork.CurrentRoom.IsOpen = true;
        PhotonNetwork.CurrentRoom.IsVisible = true;
    }

    StopAllCoroutines();
    PhotonNetwork.RemoveRPCs(PhotonNetwork.LocalPlayer);

    StartCoroutine(SafetyLobbyTimeout());
    
    PhotonNetwork.LeaveRoom();
    }

    private IEnumerator SafetyLobbyTimeout()
{
    yield return new WaitForSecondsRealtime(2f);
    if (SceneManager.GetActiveScene().name != lobbySceneName)
    {
        LoadLobbyScene();
    }
}

    public override void OnLeftRoom()
    {
        if (UIController.IsExitingGame || SceneManager.GetActiveScene().name == "exit game") 
        return;

        
        StopAllCoroutines();
        LoadLobbyScene();
    }

    private IEnumerator ForceLobbyLoadTimeout(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadLobbyScene();
    }

    public override void OnDisconnected(DisconnectCause cause)
{
    isLeaving = false;
    
    if (SceneManager.GetActiveScene().name != lobbySceneName)
    {
        LoadLobbyScene();
    }
}

    private void LoadLobbyScene()
    {
        isLeaving = false;
        if (SceneManager.GetActiveScene().name != lobbySceneName)
        {
            if (CurtainTransition.Instance != null)
                CurtainTransition.Instance.LoadScene(lobbySceneName);
            else
                SceneManager.LoadScene(lobbySceneName);
        }
    }


    public void OnSwitchButtonClicked()
{
    if (!PhotonNetwork.InRoom)
    {
        SwapLocalScene();
        return;
    }

    if (photonView == null)
    {
        Debug.LogError($"[{nameof(InGameMenuManager)}] No PhotonView found on this GameObject; cannot send RPC_ExecuteSceneSwap.");
        return;
    }

   
    photonView.RPC(nameof(RPC_ExecuteSceneSwap), RpcTarget.All);
}

[PunRPC]
private void RPC_ExecuteSceneSwap()
{
    SwapLocalScene();
}

private void SwapLocalScene()
{
    string currentScene = SceneManager.GetActiveScene().name;
    
   
    string targetScene = currentScene.Equals(masterSceneName, System.StringComparison.OrdinalIgnoreCase) 
        ? clientSceneName 
        : masterSceneName;

   
    ExitGamesHashtable customProps = new ExitGamesHashtable
    {
        { "CurrentScene", targetScene }
    };
    PhotonNetwork.LocalPlayer.SetCustomProperties(customProps);

    
    if (CurtainTransition.Instance != null)
        CurtainTransition.Instance.LoadScene(targetScene);
    else
        SceneManager.LoadScene(targetScene);
}

    public void OnSettingsButtonClicked()
    {
        if (settingsPanel) settingsPanel.SetActive(true);
    }

    public void OnCloseSettingsButtonClicked()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    public void OnQuitButtonClicked()
    {
        if (PhotonNetwork.InRoom) PhotonNetwork.LeaveRoom();

#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}