using UnityEngine;

public class Parralax : MonoBehaviour
{
    public GameObject Player;
    public float ParallaxPower = 0.2f;

    private Vector3 _originalPosition;

    // Start is called before the first frame update
    void Start()
    {
        Player = GameObject.Find("Player");
        _originalPosition = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (Player == null)
            return;

        transform.position = new Vector3(_originalPosition.x + ((_originalPosition.x - Player.transform.position.x) * ParallaxPower),
            _originalPosition.y + ((_originalPosition.y - Player.transform.position.y) * ParallaxPower),
            _originalPosition.z);
    }
}
