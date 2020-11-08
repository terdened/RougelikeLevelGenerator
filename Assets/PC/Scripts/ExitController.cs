using UnityEngine;

namespace Assets.PC.Scripts
{
    public class ExitController : MonoBehaviour
    {
        public void Init(int index, int indexTo, string scene)
        {
            var doorController = GetComponentInChildren<DoorToSceneController>();

            if (doorController == null)
                return;

            doorController.Index = index;
            doorController.IndexTo = indexTo;
            doorController.TargetScene = scene;
        }
    }
}
