using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.U2D;

public class LevelRenderer : MonoBehaviour
{
    public SpriteShape _spriteShapeProfile;
    public GameObject _spriteShape;

    public void Draw(Level level)
    {
        CreateSpriteShape(level);
    }

    private void CreateSpriteShape(Level level)
    {
        var graph = level.ToGraph();
        var cycles = graph.GetCycles();
        var maxCycle = cycles.OrderBy(_ => _.Count).FirstOrDefault();

        if (maxCycle == null)
            return;

        var vertices = new List<Vector3>();

        foreach (var edge in maxCycle)
        {
            var vA = new Vector3(edge.VertexA.Data.x, edge.VertexA.Data.y, 0);
            if (!vertices.Contains(vA))
            {
                vertices.Add(vA);
            }
            var vB = new Vector3(edge.VertexB.Data.x, edge.VertexB.Data.y, 0);
            if (!vertices.Contains(vB))
            {
                vertices.Add(vB);
            }
        }

        if (_spriteShape != null)
            Destroy(_spriteShape);

        _spriteShape = new GameObject("LevelMask");
        var spriteShapeController = _spriteShape.AddComponent<SpriteShapeController>();

        for (var i = 0; i < vertices.Count; i++)
        {
            spriteShapeController.spline.InsertPointAt(i, vertices[i]);
        }

        spriteShapeController.splineDetail = 16;
        spriteShapeController.spriteShape = _spriteShapeProfile;
    }
}
