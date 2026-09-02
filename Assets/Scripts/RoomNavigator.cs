using UnityEngine;

public class RoomNavigator : MonoBehaviour
{

    [System.Serializable]
    public struct WAllData
    {
        public string wallName;
        public Sprite wallSprite;
        public GameObject wallInteractables;
    }

    [Header("UI & Background Refrences")]
    public SpriteRenderer backgroundRenderer;
    public WAllData[] roomWalls;

    public int currentWallIndex = 0;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        UpdateWallDisPlay();
    }

    public void OnClickRighttArrow(){
        currentWallIndex = (currentWallIndex + 1) % roomWalls.Length;
        UpdateWallDisPlay();
    }

    public void OnClickLeftArrow(){
        currentWallIndex--;
        if(currentWallIndex < 0){
            currentWallIndex = roomWalls.Length - 1;
        }
        UpdateWallDisPlay();
    }

   public void UpdateWallDisPlay(){
    if(backgroundRenderer != null && roomWalls[currentWallIndex].wallSprite != null)
        {
            backgroundRenderer.sprite = roomWalls[currentWallIndex].wallSprite;
        }

        for(int i = 0; i< roomWalls.Length ; i++)
        {
            if(roomWalls[i].wallInteractables != null)
            {
                roomWalls[i].wallInteractables.SetActive(i == currentWallIndex);
            }
        }
   }
}
