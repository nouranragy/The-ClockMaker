using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGamesHashtable = ExitGames.Client.Photon.Hashtable;

public class InGameMenuManager : MonoBehaviourPunCallbacks
{
    [Header("UI Panels")]
    [SerializeField] private GameObject settingsPanel;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    [Header("Audio Button UI Elements")]
    [SerializeField] private Image muteButtonImage;
    [SerializeField] private Image musicButtonImage;
    [SerializeField] private Image sfxButtonImage;

    [Header("Audio Icon Sprites")]
    [SerializeField] private Sprite activeSprite; // Checkmark icon
    [SerializeField] private Sprite mutedSprite;  // Mute / Cancel icon

    [Header("Scene Names")]
    [SerializeField] private string lobbySceneName = "Lobby";
    [SerializeField] private string masterSceneName = "past";
    [SerializeField] private string clientSceneName = "present";

    private const string GAME_STARTED = "GameStarted";

    private bool isMuted = false;
    private bool isMusicOn = true;
    private bool isSfxOn = true;

    private void Start()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
        UpdateAudioUI();
    }

    // 1. Back to Lobby
    public void OnBackToLobbyButtonClicked()
    {
        if (PhotonNetwork.InRoom)
        {
            PhotonNetwork.CurrentRoom.SetCustomProperties(new ExitGamesHashtable { { GAME_STARTED, false } });
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            SceneManager.LoadScene(lobbySceneName);
        }
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene(lobbySceneName);
    }

    // 2. Switch Roles / Scenes
    public void OnSwitchButtonClicked()
    {
        if (!PhotonNetwork.InRoom) return;

        string newTargetScene = PhotonNetwork.IsMasterClient ? clientSceneName : masterSceneName;
        PhotonNetwork.LoadLevel(newTargetScene);
    }

    // 3. Settings Panel Controls
    public void OnSettingsButtonClicked()
    {
        if (settingsPanel) settingsPanel.SetActive(true);
    }

    public void OnCloseSettingsButtonClicked()
    {
        if (settingsPanel) settingsPanel.SetActive(false);
    }

    // ---------- Audio Controls ----------

    public void OnToggleMuteButtonClicked()
    {
        isMuted = !isMuted;
        AudioListener.pause = isMuted;
        UpdateAudioUI();
    }

    public void OnToggleMusicButtonClicked()
    {
        isMusicOn = !isMusicOn;
        if (musicSource) musicSource.mute = !isMusicOn;
        UpdateAudioUI();
    }

    public void OnToggleSFXButtonClicked()
    {
        isSfxOn = !isSfxOn;
        if (sfxSource) sfxSource.mute = !isSfxOn;
        UpdateAudioUI();
    }

    private void UpdateAudioUI()
    {
        // Updates button icons based on active state
        if (muteButtonImage && activeSprite && mutedSprite)
            muteButtonImage.sprite = isMuted ? mutedSprite : activeSprite;

        if (musicButtonImage && activeSprite && mutedSprite)
            musicButtonImage.sprite = isMusicOn ? activeSprite : mutedSprite;

        if (sfxButtonImage && activeSprite && mutedSprite)
            sfxButtonImage.sprite = isSfxOn ? activeSprite : mutedSprite;
    }

    // 4. Quit Game
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