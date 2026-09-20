using System;
using UnityEngine;

public class PlayerMenuScript : MonoBehaviour
{

    Animator animator;
    public Renderer head;
    public Renderer body;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    // -------------------------------------------- //
    void Start()
    {
        SetSkin();
    }

    // -------------------------------------------- //
    void OnEnable()
    {
        GameManager.OnSkinChange += SetSkin;
    }

    // -------------------------------------------- //
    void OnDisable()
    {
        GameManager.OnSkinChange -= SetSkin;
    }

    // -------------------------------------------- //
    void SetSkin()
    {
        SkinDataSO skin = GameManager.Instance.currentSkin;
        head.material = skin.materialHead;
        body.material = skin.materialBody;
    }


}
