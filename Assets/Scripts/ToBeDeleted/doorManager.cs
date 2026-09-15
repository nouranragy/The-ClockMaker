using UnityEngine;
using Photon.Pun;
using DG.Tweening;

public class DoorManagerDOTween : MonoBehaviourPunCallbacks
{
    public AudioSource doorOpenAudio;

    [Header("DOTween Swing Settings")]
    public Vector3 openRotation = new Vector3(0f, -55.13f, 0f); 
    public float rotateDuration = 1.5f;
    public Ease rotateEase = Ease.OutBack;

    private Vector3 targetRotation;
    private bool isOpened = false;

    private void Start()
    {
        targetRotation = transform.localEulerAngles + openRotation;
    }

    [PunRPC]
    public void RPC_OpenDoor()
    {
        if (isOpened) return;
        isOpened = true;

        Collider2D doorCollider = GetComponent<Collider2D>();
        if (doorCollider != null)
        {
            doorCollider.enabled = false;
        }

        if (doorOpenAudio != null)
        {
            doorOpenAudio.Play();
        }

        transform.DOLocalRotate(targetRotation, rotateDuration, RotateMode.Fast)
                 .SetEase(rotateEase)
                 .OnComplete(() => Debug.Log("Door swung open via DOTween!"));
    }
}