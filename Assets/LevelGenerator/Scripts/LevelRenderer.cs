using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

using UnityEngine.U2D;

public class LevelRenderer : MonoBehaviour
{
    public SpriteShape _spriteShapeProfile;
    public List<GameObject> _spriteShapes;
    private List<GameObject> _lineRendererGameObjects;

    public GameObject LineRendererPrefab;
    private float _lineWidth;

    public void Draw(Level level)
    {
        CreateSpriteShape(level);
        
        //_lineRendererGameObjects = new List<GameObject>();
        //level.Walls.ToList()
        //    .Where(_ => _.Type == EntityTypeConstants.AirPlatform)
        //    .ToList()
        //    .ForEach(_ =>
        //    {
        //        var lineRendererGameObject = CreateLine(_.Points.pointA, _.Points.pointB, _.Type, _.Id);
        //        _lineRendererGameObjects.Add(lineRendererGameObject);
        //    });
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
            {
                RenderLevelMap(level);
                return;
            }
                
            var cycleWeight = new Dictionary<List<Edge<Vector2>>, int>();

            foreach (var cycle in cycles)
            {
                var otherCycles = cycles.Where(_ => !_.IsCycleEquals(cycle)).ToList();
                var pointACounts = otherCycles.SelectMany(_ => _).Count(_ => cycle.IsPointBelongs(_.VertexA));
                var pointBCounts = otherCycles.SelectMany(_ => _).Count(_ => cycle.IsPointBelongs(_.VertexB));

                cycleWeight.Add(cycle, pointACounts + pointBCounts);
            }

            if (!cycleWeight.Any())
            {
                RenderLevelMap(level);
                return;
            }

            var maxCycle = cycles.OrderBy(_ => cycleWeight[_]).Last();

            if (maxCycle == null)
            {
                RenderLevelMap(level);
                return;
            }

            graph.RemoveEdge(maxCycle.First().VertexA, maxCycle.First().VertexB);
            var vertices = graph.FindWay(maxCycle.First().VertexA, maxCycle.First().VertexB);

            if (vertices == null)
            {
                RenderLevelMap(level);
                return;
            }

            var anglesSum = 0f;

            for (var i = 0; i < vertices.Count; i++)
            {
                if (i < vertices.Count - 2)
                {
                    anglesSum +=
                        (vertices[i + 1].Data - vertices[i].Data).AngleBetweenVector2(
                            vertices[i + 2].Data - vertices[i + 1].Data);
                } else
                if (i == vertices.Count - 2)
                {
                    anglesSum +=
                        (vertices[i + 1].Data - vertices[i].Data).AngleBetweenVector2(
                            vertices[0].Data - vertices[i + 1].Data);
                }
                else if (i == vertices.Count - 1)
                {
                    anglesSum +=
                        (vertices[0].Data - vertices[i].Data).AngleBetweenVector2(
                            vertices[1].Data - vertices[0].Data);
                }
            }

            


            if (attempts == 1)
            {
                if (anglesSum > 0)
                    vertices.Reverse();

                var maxYIndex = vertices.FindIndex(v => Math.Abs(v.Data.y - vertices.Max(_ => _.Data.y)) < 0.001);

                var outerRect = new List<Vertex<Vector2>>
                {
                    new Vertex<Vector2>(new Vector2(vertices[maxYIndex].Data.x, 60)),

                    new Vertex<Vector2>(new Vector2(-120, 60)),
                    new Vertex<Vector2>(new Vector2(-120, -60)),
                    new Vertex<Vector2>(new Vector2(120, -60)),
                    new Vertex<Vector2>(new Vector2(120, 60)),
                    new Vertex<Vector2>(new Vector2(vertices[maxYIndex].Data.x, 60)),
                    new Vertex<Vector2>(new Vector2(vertices[maxYIndex].Data.x, vertices[maxYIndex].Data.y))
                };

                vertices.InsertRange(maxYIndex + 1, outerRect);
            } else if (anglesSum < 0)
            {
                vertices.Reverse();
            }

            VertexToSpriteShape(vertices);
            vertices.ForEach(graph.RemoveVertex);
        }
        
    }

    private void RenderLevelMap(Level level)
    {
        var levelMapRenderer = new GameObject();
        var renderer = levelMapRenderer.AddComponent<LevelMapRenderer>();
        renderer.LineRendererPrefab = LineRendererPrefab;
        renderer.Draw(level);
    }

    private void VertexToSpriteShape(IEnumerable<Vertex<Vector2>> vertices)
    {
        var spriteShape = new GameObject("LevelMask");
        var spriteShapeController = spriteShape.AddComponent<SpriteShapeController>();

        spriteShapeController.spline.isOpenEnded = true;

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

    private GameObject CreateLine(Vector3 pointAPosition, Vector3 pointBPosition, EntityType type, string entityId)
    {
        var lineRendererGameObject = Instantiate(LineRendererPrefab, Vector3.zero, Quaternion.identity, gameObject.transform);

        var idHolder = lineRendererGameObject.AddComponent<EntityIdHolder>();
        idHolder.SetId(entityId);

        var lineRenderer = lineRendererGameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = _lineWidth;
        lineRenderer.endWidth = _lineWidth;
        lineRenderer.material.color = type.Color;

        lineRenderer.SetPosition(0, pointAPosition);
        lineRenderer.SetPosition(1, pointBPosition);

        return lineRendererGameObject;
    }
}
