using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkLauncher : MonoBehaviourPunCallbacks
{
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        // Set MaxPlayers = 2 to cap room size strictly for 2 players
        PhotonNetwork.JoinOrCreateRoom("GlobalChatRoom", new RoomOptions { MaxPlayers = 2 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log("Connected to Chat Room! Current Players: " + PhotonNetwork.CurrentRoom.PlayerCount);
    }
}