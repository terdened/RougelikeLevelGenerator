using UnityEngine;

namespace Assets.LevelExplore.AI.RoomAI
{
    public class RoomController : MonoBehaviour
    {
        public string Type;

        public GameObject LeftDoor;
        public GameObject RightDoor;
        public GameObject UpDoor;
        public GameObject DownDoor;

        public void CloseDoor()
        {
            switch (Type)
            {
                case "Left":
                    RightDoor.SetActive(true);
                    break;
                case "Right":
                    LeftDoor.SetActive(true);
                    break;
                case "Up":
                    DownDoor.SetActive(true);
                    break;
                case "Down":
                    UpDoor.SetActive(true);
                    break;
                default:
                    break;
            }
        }

        public void OpenDoor()
        {
            switch (Type)
            {
                case "Left":
                    RightDoor.SetActive(false);
                    break;
                case "Right":
                    LeftDoor.SetActive(false);
                    break;
                case "Up":
                    DownDoor.SetActive(false);
                    break;
                case "Down":
                    UpDoor.SetActive(false);
                    break;
                default:
                    break;
            }
        }
    }
}
