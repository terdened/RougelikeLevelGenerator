using UnityEngine;
using UnityEngine.Experimental.U2D;
using UnityEngine.U2D;

[RequireComponent(typeof(SpriteShapeController))]
[RequireComponent(typeof(SpriteShapeRenderer))]
public class SpriteShapeScript : MonoBehaviour
{
    private SpriteShapeController _spriteShapeController;

    // Start is called before the first frame update
    void Start()
    {
        _spriteShapeController = GetComponent<SpriteShapeController>();

        var a = new UnityEngine.Experimental.U2D.ShapeControlPoint();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
