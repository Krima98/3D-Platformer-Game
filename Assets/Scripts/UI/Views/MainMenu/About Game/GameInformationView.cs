using UnityEngine;
using UnityEngine.UI;

public class GameInformationView : MonoBehaviour
{

    public GameObject currentActive;
    
    [Header("Header menu button list")]
    [SerializeField] private Button aboutGameBtn;
    [SerializeField] private Button gameMechanicsBtn;
    [SerializeField] private Button controllersBtn;

    [Header("Main views in game info")]
    [SerializeField] private GameObject aboutGameView;
    [SerializeField] private GameObject gameMechanicsView;
    [SerializeField] private GameObject controllersView;
    [SerializeField] private GameObject videoOfMapsView;

    [Header("Mechanics views")]
    public GameObject currentActiveMechView;

    [SerializeField] private GameObject jumpView;
    [SerializeField] private GameObject slamdownView;
    [SerializeField] private GameObject superJumpView;
    [SerializeField] private GameObject groundslideView;
    [SerializeField] private GameObject airSpinView;

    [Header("Mechanics menu button list")]
    [SerializeField] private Button jumpBtn;
    [SerializeField] private Button slamdownBtn;
    [SerializeField] private Button superJumpBtn;
    [SerializeField] private Button groundslideBtn;
    [SerializeField] private Button airSpinBtn;

    [Header("Controller view")]
    public GameObject currentActiveControllerView;

    [SerializeField] private Button keyboardBtn;
    [SerializeField] private Button gamepadBtn;
    [SerializeField] private GameObject keyboardView;
    [SerializeField] private GameObject gamepadView;

    // --------------------------- //
    void Start()
    {
        currentActive = aboutGameView;
        currentActiveMechView = jumpView;
        currentActiveControllerView = keyboardView;
        
        SetClickEventsHeader();
        SetClickEventsMechanics();
        SetClickEventsControllerView();
    }

    // --------------------------- //
    void SetClickEventsHeader()
    {
        aboutGameBtn.onClick.AddListener(() =>
        {
            SwapView(aboutGameView, ref currentActive);
        });

        gameMechanicsBtn.onClick.AddListener(() =>
        {
            SwapView(gameMechanicsView, ref currentActive);
        });

        controllersBtn.onClick.AddListener(() =>
        {
            SwapView(controllersView, ref currentActive);
        });

    }

    // --------------------------- //
    void SetClickEventsMechanics()
    {
        jumpBtn.onClick.AddListener(() =>
        {
            SwapView(jumpView, ref currentActiveMechView);
        });

        slamdownBtn.onClick.AddListener(() =>
        {
            SwapView(slamdownView, ref currentActiveMechView);
        });

        superJumpBtn.onClick.AddListener(() =>
        {
            SwapView(superJumpView, ref currentActiveMechView);
        });

        groundslideBtn.onClick.AddListener(() =>
        {
            SwapView(groundslideView, ref currentActiveMechView);
        });

        airSpinBtn.onClick.AddListener(() =>
        {
            SwapView(airSpinView, ref currentActiveMechView);
        });
    }

    void SetClickEventsControllerView()
    {
        keyboardBtn.onClick.AddListener(() =>
        {
            SwapView(keyboardView, ref currentActiveControllerView);
        });

        gamepadBtn.onClick.AddListener(() =>
        {
            SwapView(gamepadView, ref currentActiveControllerView);
        });
    }

    // --------------------------- //
    void SwapView(GameObject newView, ref GameObject activeView)
    {
        if(newView == activeView)
        {
            return;
        } else
        {        
            activeView.SetActive(false);
            newView.SetActive(true);
            activeView = newView;
        }
    }
}
