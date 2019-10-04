using UnityEngine;

public static class LevelElevatorExtension
{
    public static GameObject ToElevator(this LevelElevator elevator, GameObject prefab)
    {
        var result = Object.Instantiate(prefab);
        var elevatorController = result.GetComponent<ElevatorController>();
        elevatorController.Init(elevator);

        return result;
    }
}