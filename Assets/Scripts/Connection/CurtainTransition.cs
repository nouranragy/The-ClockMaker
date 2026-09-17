using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class CurtainTransition : MonoBehaviour
{
    public static CurtainTransition Instance;

    [Header("Curtains")]
    [SerializeField] private RectTransform leftCurtain;
    [SerializeField] private RectTransform rightCurtain;

    [Header("Open Positions")]
    [SerializeField] private Vector2 leftOpenPosition;
    [SerializeField] private Vector2 rightOpenPosition;

    [Header("Closed Positions")]
    [SerializeField] private Vector2 leftClosedPosition;
    [SerializeField] private Vector2 rightClosedPosition;

    [Header("Settings")]
    [SerializeField] private float duration = 1f;
    [SerializeField] private Ease easeType = Ease.InOutQuad;

    private bool isTransitioning = false;
    private Sequence curtainSequence;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnDestroy()
    {
        curtainSequence?.Kill();
    }

    private void Start()
    {
        // Set curtains immediately to closed position at start, then open them up
        SetCurtainsClosed();
        OpenCurtains();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // When a new scene finishes loading, trigger the opening animation
        OpenCurtains();
    }

    public void OpenCurtains()
    {
        curtainSequence?.Kill();

        curtainSequence = DOTween.Sequence();
        curtainSequence.Join(leftCurtain.DOAnchorPos(leftOpenPosition, duration).SetEase(easeType));
        curtainSequence.Join(rightCurtain.DOAnchorPos(rightOpenPosition, duration).SetEase(easeType));
        curtainSequence.OnComplete(() => { isTransitioning = false; });
    }

    public void LoadScene(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        CloseCurtains(() =>
        {
            SceneManager.LoadScene(sceneName);
        });
    }

    public void LoadPhotonScene(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;

        CloseCurtains(() =>
        {
            Photon.Pun.PhotonNetwork.LoadLevel(sceneName);
        });
    }

    private void CloseCurtains(System.Action onClosed)
    {
        curtainSequence?.Kill();

        curtainSequence = DOTween.Sequence();
        curtainSequence.Join(leftCurtain.DOAnchorPos(leftClosedPosition, duration).SetEase(easeType));
        curtainSequence.Join(rightCurtain.DOAnchorPos(rightClosedPosition, duration).SetEase(easeType));
        curtainSequence.OnComplete(() => onClosed?.Invoke());
    }

    private void SetCurtainsClosed()
    {
        leftCurtain.anchoredPosition = leftClosedPosition;
        rightCurtain.anchoredPosition = rightClosedPosition;
    }
}