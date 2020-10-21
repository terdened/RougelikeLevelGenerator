using UnityEngine;

public class AmmoController : MonoBehaviour
{
    public GameObject BuletPrefab;
    public float BuletDistance = 5f;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        HandleRotation();

        if (Input.GetMouseButtonDown(0))
        {
            Fire();
        }
    }

    private void HandleRotation()
    {
        var mousePosition = Input.mousePosition;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        var angle = Vector2.Angle(Vector2.right, mousePosition - transform.position);
        transform.eulerAngles = new Vector3(0f, 0f, transform.position.y < mousePosition.y ? angle : -angle);
    }

    private void Fire()
    {
        var bulet = Instantiate(BuletPrefab, transform.parent.position, transform.parent.rotation);
        var buletTrailController = bulet.GetComponent<BuletTrailController>();
        buletTrailController.InitBuletTrail(transform.eulerAngles.z);

        //Vector2 destVector = new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad));

        //var dest = new Vector3(transform.position.x + 5 * Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), transform.position.y + 5 * Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad), 0);


        //// Cast a ray straight down.
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, new Vector2(Mathf.Cos(transform.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.eulerAngles.z * Mathf.Deg2Rad)), BuletDistance, 1 << LayerMask.NameToLayer("Ground"));


        //if (hit.collider != null)
        //{
        //    dest = new Vector3(hit.point.x, hit.point.y, 0);

        //    var normalAngle = Vector2.zero.GetAngle(hit.normal);

        //    var distance = BuletDistance - hit.distance;

        //    var bulet2 = Instantiate(BuletPrefab, dest, transform.parent.rotation);
        //    var dest2 = new Vector2(
        //        dest.x + distance * Mathf.Cos(Mathf.Deg2Rad * (normalAngle + Vector2.SignedAngle(destVector, -hit.normal))),
        //        dest.y + distance * Mathf.Sin(Mathf.Deg2Rad * (normalAngle + Vector2.SignedAngle(destVector, -hit.normal))));
        //    var buletTrailController2 = bulet2.GetComponent<BuletTrailController>();
        //    buletTrailController2.SetPosition(dest2);

        //    buletTrailController.SetPosition(dest);
        //}

    }
}
