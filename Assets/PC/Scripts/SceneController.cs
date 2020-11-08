using UnityEngine;

namespace Assets.PC.Scripts
{
    public class SceneController : MonoBehaviour
    {
        public GameObject Player;

        public void Start()
        {
            if (LevelHolder.PortalIndex == null)
                return;

            var portals = GameObject.FindGameObjectsWithTag("Respawn");

            foreach(var portal in portals)
            {
                var doorToSceneController = portal.GetComponent<DoorToSceneController>();

                if (doorToSceneController == null)
                    continue;

                if(doorToSceneController.Index == LevelHolder.PortalIndex)
                {
                    Player.transform.position = portal.transform.position;
                    LevelHolder.PortalIndex = null;
                }
            }
        }
    }
}
