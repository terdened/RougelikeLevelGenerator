using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Experimental.U2D;
using UnityEngine.U2D;

public class LevelRenderer : MonoBehaviour
{
    public SpriteShape _spriteShapeProfile;
    public List<GameObject> _spriteShapes;

    public void Draw(Level level)
    {
        CreateSpriteShape(level);
    }

    private void CreateSpriteShape(Level level)
    {
        var graph = level.ToGraph();
        _spriteShapes?.ForEach(Destroy);

        var attempts = 0;

        while (attempts < GeneratorConstants.MaxGenerationAttempts && graph.Vertexes.Count > 0)
        {
            attempts++;

            var cycles = graph.GetCycles();

            if (cycles == null)
                return;
            
            var cycleWeight = new Dictionary<List<Edge<Vector2>>, int>();

            foreach (var cycle in cycles)
            {
                var otherCycles = cycles.Where(_ => !_.IsCycleEquals(cycle)).ToList();
                var pointACounts = otherCycles.SelectMany(_ => _).Count(_ => cycle.IsPointBelongs(_.VertexA));
                var pointBCounts = otherCycles.SelectMany(_ => _).Count(_ => cycle.IsPointBelongs(_.VertexB));

                cycleWeight.Add(cycle, pointACounts + pointBCounts);
            }

            if (!cycleWeight.Any())
                return; 

            var maxCycle = cycles.OrderBy(_ => cycleWeight[_]).Last();

            if (maxCycle == null)
                return;

            graph.RemoveEdge(maxCycle.First().VertexA, maxCycle.First().VertexB);
            var vertices = graph.FindWay(maxCycle.First().VertexA, maxCycle.First().VertexB);

            if (vertices == null)
                return;

            if (attempts == 1)
            {
                var maxYIndex = vertices.FindIndex(v => Math.Abs(v.Data.y - vertices.Max(_ => _.Data.y)) < 0.001);

                var outerRect = new List<Vertex<Vector2>>
                {
                    new Vertex<Vector2>(new Vector2(vertices[maxYIndex].Data.x, 6)),
                    new Vertex<Vector2>(new Vector2(12, 6)),
                    new Vertex<Vector2>(new Vector2(12, -6)),
                    new Vertex<Vector2>(new Vector2(-12, -6)),
                    new Vertex<Vector2>(new Vector2(-12, 6)),
                    new Vertex<Vector2>(new Vector2(vertices[maxYIndex].Data.x, 6)),
                    new Vertex<Vector2>(new Vector2(vertices[maxYIndex].Data.x, vertices[maxYIndex].Data.y))
                };

                vertices.InsertRange(maxYIndex + 1, outerRect);
            }

            VertexToSpriteShape(vertices);
            vertices.ForEach(graph.RemoveVertex);
        }
    }

    private void VertexToSpriteShape(IEnumerable<Vertex<Vector2>> vertices)
    {
        var spriteShape = new GameObject("LevelMask");
        var spriteShapeController = spriteShape.AddComponent<SpriteShapeController>();

        spriteShapeController.spline.isOpenEnded = true;

        vertices = vertices.Reverse();
        foreach (var vertex in vertices)
        {
            spriteShapeController.spline.InsertPointAt(0, new Vector3(vertex.Data.x, vertex.Data.y));
        }

        spriteShapeController.spline.isOpenEnded = false;

        spriteShapeController.splineDetail = 16;
        spriteShapeController.spriteShape = _spriteShapeProfile;

        var spriteShapeRenderer = spriteShape.GetComponent<SpriteShapeRenderer>();
        spriteShapeRenderer.sortingOrder = _spriteShapes.Count;

        _spriteShapes.Add(spriteShape);
    }
}
