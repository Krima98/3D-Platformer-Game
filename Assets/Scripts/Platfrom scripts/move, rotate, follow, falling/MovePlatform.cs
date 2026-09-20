using UnityEngine;

public class MovePlatform : MonoBehaviour
{
    [Header("Platform Movement")]
    [SerializeField] private float speed = 2f;
    [SerializeField] private float distance = 10f;

    [Header("Direction")]
    [SerializeField] private bool moveAlongZ;
    [SerializeField] private bool moveAlongX;
    [SerializeField] private bool moveAlongY;

    private Vector3 startPos;

    void Start()
    {
        startPos = transform.position;
    }

    // -------------------------------------------- //
    void Update()
    {
        MovePlatfrom();
    }

    // -------------------------------------------- //
    void MovePlatfrom()
    {
        
        float offset = Mathf.PingPong(Time.time * speed, distance);

        if(moveAlongZ)
        {
            transform.position = startPos + new Vector3(0, 0, offset);
        }

        if(moveAlongX)
        {
            transform.position = startPos + new Vector3(offset, 0, 0);
        }

        if(moveAlongY)
        {
            transform.position = startPos + new Vector3(0, offset, 0);
        }        
    }

}
