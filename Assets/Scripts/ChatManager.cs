using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text chatDisplay;

    public void SendChatMessage()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) return;
        string senderName = PhotonNetwork.NickName;
        string fullMessage = $"{senderName}: {inputField.text}";
        photonView.RPC(nameof(ReceiveMessageRPC), RpcTarget.All, fullMessage);
        inputField.text = "";
        inputField.ActivateInputField();
    }
    [PunRPC]
    private void ReceiveMessageRPC(string message)
    {
        chatDisplay.text += message + "\n";
    }
}