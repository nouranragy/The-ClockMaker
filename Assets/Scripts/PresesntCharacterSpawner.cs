using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;
public class PresentCharacterSpawner : MonoBehaviourPunCallbacks
{
    [Header("Character to Reveal in Present")]
    public GameObject presentCharacter;
    private void Start()
    {
        if (presentCharacter != null)  presentCharacter.SetActive(false);
        CheckIfPuzzleAlreadySolved();
    }
    public override void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        if (propertiesThatChanged.ContainsKey("WallPuzzleSolved"))
        {
            bool isSolved = (bool)propertiesThatChanged["WallPuzzleSolved"];
            if (isSolved)  RevealCharacter();
        }
    }
    private void CheckIfPuzzleAlreadySolved()
    {
        if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties.TryGetValue("WallPuzzleSolved", out object isSolved))
        {
            if ((bool)isSolved)  RevealCharacter();
        }
    }
    private void RevealCharacter()
    {
        if (presentCharacter != null && !presentCharacter.activeSelf)
        {
            presentCharacter.SetActive(true);
            Debug.Log("The character has appeared in the Present scene!");
        }
    }
}