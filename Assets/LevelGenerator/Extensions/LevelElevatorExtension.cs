using UnityEngine;

public static class LevelWallExtension
{
    //  Returns Point of intersection if do intersect otherwise default Point (null)
    public static Vector2? FindIntersection(this LevelWall wallA, LevelWall wallB, bool checkIsInLine = false)
     => wallA.ToVector2().FindIntersection(wallB.ToVector2(), checkIsInLine);

    public static (Vector2 pointA, Vector2 pointB) ToVector2(this LevelWall wall)
        => (wall.Points.pointA, wall.Points.pointB);

    public static GameObject ToPlatform(this LevelWall wall)
    {
        var result = new GameObject($"Wall_{wall.Id}");
        var edgeCollider = result.AddComponent<EdgeCollider2D>();
        edgeCollider.points = new []
        {
            wall.Points.pointA,
            wall.Points.pointB,
        };
        result.layer = LayerMask.NameToLayer("Ground");

        return result;
    }
}