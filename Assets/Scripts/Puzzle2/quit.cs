using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class quit : MonoBehaviour
{
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
