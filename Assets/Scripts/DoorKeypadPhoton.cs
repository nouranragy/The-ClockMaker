using UnityEngine;
using TMPro; // stupid girl!!!!!!!!!!!!!!
using Photon.Pun;

public class DoorKeypadPhoton : MonoBehaviourPunCallbacks
{
    public TMP_InputField codeInputField; 
    public string correctCode = "0315";
    public PhotonView doorPhotonView;
    private void Awake()
    {
        if (codeInputField == null)
        {
            codeInputField = GetComponentInChildren<TMP_InputField>();
        }
    }
    public void OnSubmitCode()
    {
        if (codeInputField == null) return;

        string enteredCode = codeInputField.text.Trim().Replace(":", "");

        if (enteredCode == correctCode)
        {
            doorPhotonView.RPC("RPC_OpenDoor", RpcTarget.AllViaServer);
            transform.root.gameObject.SetActive(false);
        }
        else
        {
            Debug.Log("Incorrect Code!");
            codeInputField.text = "";
        }
    }
}