using UnityEngine;

public class Spin : MonoBehaviour
{
    GameData gameData;

    [SerializeField] private Vector3 _rotation;
    public float speed;

    void Awake()
    {
        gameData = SaveReadManager.Instance.gameData;
    }

    // -------------------------------------------- //
    void OnEnable()
    {
        PauseView.OnSettingChange += ResetRotation;
    }

    // -------------------------------------------- //
    void OnDisable()
    {
        PauseView.OnSettingChange -= ResetRotation;
    }

    // -------------------------------------------- // 
    void Update()
    {
        if(gameData.turnOffRotation)
        {
            return;
        } else
        {
            Rotate();
        }
    }

    // -------------------------------------------- //
    void Rotate()
    {
        transform.Rotate(_rotation * speed * Time.deltaTime);
    }

    // -------------------------------------------- //
    void ResetRotation()
    {
        if(gameData.turnOffRotation)
        {
          transform.rotation = Quaternion.Euler(0, 0, 0);
        } 
    }
}
