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
    [Header("Audio Button UI Elements")]
    [SerializeField] private Image muteButtonImage;
    [SerializeField] private Image musicButtonImage;
    [SerializeField] private Image sfxButtonImage;
    [Header("Audio Icon Sprites")]
    [SerializeField] private Sprite activeSprite;
    [SerializeField] private Sprite mutedSprite;
    [Header("Scene Names")]
    [SerializeField] private string lobbySceneName = "Lobby";
    [SerializeField] private string masterSceneName = "Past";
    [SerializeField] private string clientSceneName = "Present";
    private const string GAME_STARTED = "GameStarted";
    private bool isMuted = false;
    private bool isMusicOn = true;
    private bool isSfxOn = true;
    private bool isLeaving = false;
    private void Start()
    {
        isLeaving = false;
        if (settingsPanel) settingsPanel.SetActive(false);
        UpdateAudioUI();
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
        if (SceneManager.GetActiveScene().name != lobbySceneName) SceneManager.LoadScene(lobbySceneName);
    }
    public void OnSwitchButtonClicked()
    {
        if (!PhotonNetwork.InRoom)
        {
            string currentLocal = SceneManager.GetActiveScene().name;
            string targetLocal = (currentLocal.Equals(masterSceneName, System.StringComparison.OrdinalIgnoreCase))
                ? clientSceneName : masterSceneName;
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
        if (muteButtonImage && activeSprite && mutedSprite) muteButtonImage.sprite = isMuted ? mutedSprite : activeSprite;
        if (musicButtonImage && activeSprite && mutedSprite) musicButtonImage.sprite = isMusicOn ? activeSprite : mutedSprite;
        if (sfxButtonImage && activeSprite && mutedSprite) sfxButtonImage.sprite = isSfxOn ? activeSprite : mutedSprite;
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