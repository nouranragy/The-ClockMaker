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
    private bool isTransitioning = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else Destroy(gameObject);
    }
    private void Start()
    {
        leftCurtain.anchoredPosition = leftClosedPosition;
        rightCurtain.anchoredPosition = rightClosedPosition;
        OpenCurtains();
    }
    public void OpenCurtains()
    {
        Sequence sequence = DOTween.Sequence();
        sequence.Join(leftCurtain.DOAnchorPos(leftOpenPosition, duration));
        sequence.Join(rightCurtain.DOAnchorPos(rightOpenPosition, duration));
    }
    public void LoadScene(string sceneName)
    {
        if (isTransitioning) return;
        isTransitioning = true;
        Sequence sequence = DOTween.Sequence();
        sequence.Join(leftCurtain.DOAnchorPos(leftClosedPosition, duration));
        sequence.Join(rightCurtain.DOAnchorPos(rightClosedPosition, duration));
        sequence.AppendCallback(() => {SceneManager.LoadScene(sceneName);});
    }
}