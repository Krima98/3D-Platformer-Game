using UnityEngine;
using System.Collections;

public class FallingPlatform : MonoBehaviour
{
    private float fallingSpeed;

    [SerializeField] private float fallTime;
    [SerializeField] private float destoryTime;
    [SerializeField] private float fallSpeed;

    [SerializeField] private Material fallColor;
    [SerializeField] private Renderer objectRendrer;

    private bool isFalling = false;

    void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Player"))
        {
            StartCoroutine(Fall(fallTime));   
        }
    }

    // -------------------------------------------- //
    void Update()
    {
        if(isFalling)
        {
            fallingSpeed += fallSpeed * Time.deltaTime;
            transform.position += Vector3.down * fallingSpeed * Time.deltaTime;
        }
    }

    // -------------------------------------------- //
    IEnumerator Fall(float fallTime)
    {
        yield return new WaitForSeconds(fallTime);
        isFalling = true;
        objectRendrer.material = fallColor;

        yield return new WaitForSeconds(destoryTime);
        Destroy(gameObject);
    }
}
