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
        // isLeaving = false;
        // if (settingsPanel) settingsPanel.SetActive(false);

        // // Initialize Sliders to match current AudioSource volumes
        // if (musicSource != null && musicSlider != null)
        // {
        //     musicSlider.value = musicSource.volume;
        //     musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        // }

        // if (sfxSource != null && sfxSlider != null)
        // {
        //     sfxSlider.value = sfxSource.volume;
        //     sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        // }
        isLeaving = false;
    if (settingsPanel) settingsPanel.SetActive(false);

    // 1. ربط صوت الباك جراوند (لو محطوط مباشرة على GameObject في السين)
    if (musicSource == null)
    {
        // هيجيب أول AudioSource موجود في المشهد للباك جراوند
        musicSource = FindFirstObjectByType<AudioSource>();
    }

    // 2. ربط صوت SFX مع SoundManager
    if (SoundManager.Instance != null && SoundManager.Instance.sfxSource != null) //music
    {
        sfxSource = SoundManager.Instance.sfxSource;
    }

    // 3. ضبط السليدرز
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
        if (PhotonNetwork.InRoom)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGamesHashtable { { GAME_STARTED, false } });
                PhotonNetwork.CurrentRoom.IsOpen = true;
                PhotonNetwork.CurrentRoom.IsVisible = true;
            }
            PhotonNetwork.LeaveRoom();
            StartCoroutine(ForceLobbyLoadTimeout(0.5f));
        }
        else LoadLobbyScene();
    }

    public override void OnLeftRoom()
    {
        StopAllCoroutines();
        LoadLobbyScene();
    }

    private IEnumerator ForceLobbyLoadTimeout(float delay)
    {
        yield return new WaitForSecondsRealtime(delay);
        LoadLobbyScene();
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
            string currentLocal = SceneManager.GetActiveScene().name;
            string targetLocal = (currentLocal.Equals(masterSceneName, System.StringComparison.OrdinalIgnoreCase))
                ? clientSceneName : masterSceneName;

            if (CurtainTransition.Instance != null)
                CurtainTransition.Instance.LoadScene(targetLocal);
            else
                SceneManager.LoadScene(targetLocal);

            return;
        }

        if (photonView == null)
        {
            Debug.LogError($"[{nameof(InGameMenuManager)}] No PhotonView found on this GameObject; cannot send RPC_ExecuteSceneSwap.");
            return;
        }

        string senderCurrentScene = SceneManager.GetActiveScene().name;
        bool isSenderInMaster = senderCurrentScene.Equals(masterSceneName, System.StringComparison.OrdinalIgnoreCase);
        string senderTargetScene = isSenderInMaster ? clientSceneName : masterSceneName;
        string otherTargetScene = isSenderInMaster ? masterSceneName : clientSceneName;
        photonView.RPC(nameof(RPC_ExecuteSceneSwap), RpcTarget.All, PhotonNetwork.LocalPlayer.ActorNumber, senderTargetScene, otherTargetScene);
    }

    [PunRPC]
    private void RPC_ExecuteSceneSwap(int senderActorNumber, string senderTargetScene, string otherTargetScene)
    {
        string finalTarget = (PhotonNetwork.LocalPlayer.ActorNumber == senderActorNumber)
            ? senderTargetScene
            : otherTargetScene;

        if (CurtainTransition.Instance != null)
            CurtainTransition.Instance.LoadScene(finalTarget);
        else
            SceneManager.LoadScene(finalTarget);
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