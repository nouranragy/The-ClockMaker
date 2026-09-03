using UnityEngine;
using TMPro;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text chatDisplay;

    public void SendChatMessage()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) return;

        string nickname = PhotonNetwork.NickName;
        if (string.IsNullOrEmpty(nickname)) nickname = "Player " + Random.Range(100, 999);

        string formattedMessage = $"{nickname}: {inputField.text}";// Send RPC to all clients in the room, including self
        photonView.RPC(nameof(ReceiveMessageRPC), RpcTarget.All, formattedMessage);

        inputField.text = "";
        inputField.ActivateInputField();
    }

    [PunRPC]
    private void ReceiveMessageRPC(string message)
    {
        chatDisplay.text += message + "\n";
    }
}