using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private float startposX, startposY;
    public GameObject cam;
    public float parallaxEffect;
    void Start()
    {
        startposX = transform.position.x;
        startposY = transform.position.y;
    }

    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect));
        float distX = (cam.transform.position.x * parallaxEffect);
        float distY = (cam.transform.position.y);

        transform.position = new Vector3(startposX + distX, startposY + distY, transform.position.z);
    }
}
