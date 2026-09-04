using UnityEngine;
using TMPro;
using Photon.Pun;
using UnityEngine.SceneManagement;

public class NameInputManager : MonoBehaviour
{
    [SerializeField] private TMP_InputField nameInputField;

    public void OnSubmitNameAndPlay()
    {
        string chosenName = nameInputField.text;
        if (string.IsNullOrWhiteSpace(chosenName))
        {
            chosenName = "Player " + Random.Range(10, 99);
        }
        PhotonNetwork.NickName = chosenName;

        Debug.Log("Saved Player Name: " + PhotonNetwork.NickName);
        }
}