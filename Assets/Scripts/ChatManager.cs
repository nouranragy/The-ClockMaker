using UnityEngine;
using TMPro;
using Photon.Pun;

public class ChatManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private TMP_InputField inputField;
    [SerializeField] private TMP_Text chatDisplay;

    private void Start()
{
    // Fixes the Player -1 issue by defaulting to Player 1 if not fully assigned an ActorNumber yet
    int actorNum = PhotonNetwork.LocalPlayer != null && PhotonNetwork.LocalPlayer.ActorNumber > 0 
        ? PhotonNetwork.LocalPlayer.ActorNumber 
        : 1;

    PhotonNetwork.NickName = "Player " + actorNum;
}

    public void SendChatMessage()
    {
        if (string.IsNullOrWhiteSpace(inputField.text)) return;

        string message = $"{PhotonNetwork.NickName}: {inputField.text}";
        photonView.RPC(nameof(ReceiveMessageRPC), RpcTarget.All, message);

        inputField.text = "";
        inputField.ActivateInputField();
    }

    [PunRPC]
    private void ReceiveMessageRPC(string message)
    {
        chatDisplay.text += message + "\n";
    }
}