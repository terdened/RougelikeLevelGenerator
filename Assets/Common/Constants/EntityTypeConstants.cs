using UnityEngine;

public static class EntityTypeConstants
{
    public static EntityType Unknown => new EntityType();

    // Skeleton
    public static EntityType Floor => new EntityType(new Color(0.72f, 0.47f, 0.34f), "Floor");
    public static EntityType Elevator => new EntityType(Color.green, "Elevator");

    public static EntityType EmptySpace => new EntityType(Color.cyan, "Empty Space");

    public static EntityType EmptySpaceElevatorLeft => new EntityType(new Color(0.12f, 0.32f, 0.32f), "Empty Space Elevator Left");
    public static EntityType EmptySpaceElevatorRight => new EntityType(new Color(0.09f, 0.16f, 0.16f), "Empty Space Elevator Right");
    public static EntityType EmptySpaceFloor => new EntityType(new Color(0.0f, 0.5f, 0.5f), "Empty Space Floor");
    public static EntityType EmptySpaceTop => new EntityType(Color.cyan, "Empty Space Top");

    public static EntityType InsideElevator => new EntityType(new Color(0.25f, 0.25f, 0.25f), "Inside Elevator");
    public static EntityType InsideFloor => new EntityType(Color.grey, "Inside Floor");

    // Level
    public static EntityType LevelWall => new EntityType(Color.grey, "Level Wall");
    public static EntityType LevelElevator => new EntityType(new Color(0.33f, 0.24f, 0.21f), "Level Elevator");
    public static EntityType AirPlatform => new EntityType(Color.yellow, "Air Plartform");
    public static EntityType Deadlock => new EntityType(Color.red, "Deadlock");
    public static EntityType Exit => new EntityType(Color.yellow, "Exit");
}
