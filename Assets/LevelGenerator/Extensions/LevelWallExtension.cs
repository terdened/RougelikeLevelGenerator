using UnityEngine;

public static class LevelWallExtension
{
    //  Returns Point of intersection if do intersect otherwise default Point (null)
    public static Vector2? FindIntersection(this LevelWall wallA, LevelWall wallB)
     => wallA.ToVector2().FindIntersection(wallB.ToVector2(), false);

    public static (Vector2 pointA, Vector2 pointB) ToVector2(this LevelWall wall)
        => (wall.Points.pointA, wall.Points.pointB);
}