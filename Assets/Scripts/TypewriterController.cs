using UnityEngine;
using TMPro;
using Photon.Pun;


public class TypewriterController : MonoBehaviourPun
{
    [Header  ("UI Reference")]
    public TMP_Text paperText;
    public TMP_Text presentPaperText;


    void Start()
{
    // لو اللعبة مش متصلة بسيرفر فوتون، بنفعل الأوفلاين مود عشان الـ RPC يشتغل محلياً للتست
    if (!PhotonNetwork.IsConnected)
    {
        PhotonNetwork.OfflineMode = true;
        PhotonNetwork.CreateRoom("TestRoom");
    }
}
    public void TypeLetter(string letter)
    {
        
         paperText.text += letter;
    }
     public void BackSpace()
    {
        if(paperText.text.Length > 0)
        {
            paperText.text = paperText.text.Substring(0,paperText.text.Length-1 );
        }
    }
    public void SendMessageOverNetwork()
    {
        if(paperText == null || string.IsNullOrEmpty(paperText.text))
        {
            Debug.Log("Send failed: paperText is empty or null!");
           return; 
        } 
        string messageToSend = paperText.text;
        Debug.Log("Sending message via Photon: " + messageToSend);
        // photonView.RPC(nameof(ReceiveMessageRPC), RpcTarget.All , messageToSend);
        if (PhotonNetwork.InRoom)
    {
        photonView.RPC(nameof(ReceiveMessageRPC), RpcTarget.All, messageToSend);
    }
    else
    {
        ReceiveMessageRPC(messageToSend);
    }
        paperText.text = " ";
    }
[PunRPC]
private void ReceiveMessageRPC(string messageReceived)
    {
        if(presentPaperText != null)
        {
            presentPaperText.text = messageReceived;
        }
    }
}
