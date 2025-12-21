using TauriLand.Libreria;
using TauriLand.MysticRunner;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameScript : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Parents Game Screens
    //----------------------------------------------------------------------
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    // GameObject comunes de todas las pantallas
    // - unificamos acqui las llamadas a todas las opciones del menu.
    //----------------------------------------------------------------------
    // Jerarquia:
    // 
    // Game - Con este script.
    //      + MenuCanvas    - Screen Space - Overlay
    //          - MenuScreen
    //          - OptionsScreen
    //
    //      * CanvasFondo
    //      + GameCanvas    - Screen Space - Camera - asocia MainCamera
    //          + Image/Fondo - al game.
    //          - GameScreen
    //          - ExitScreen
    //          - EndScreen
    //
    //      - GameManager
    //          (Runner1)
    //          - Player
    //          - Suelos
    //          - TreesRef
    //          - Damages
    //
    //----------------------------------------------------------------------
    [HideInInspector] public GameScript game;

    [HideInInspector] public Canvas menuCanvas;
    [HideInInspector] public GameObject menuScreen;
    [HideInInspector] public GameObject optionsScreen;
    // [HideInInspector] public GameObject enConstruccionScreen;

    [HideInInspector] public Canvas fondoCanvas;

    [HideInInspector] public Canvas gameCanvas;
    [HideInInspector] public GameObject gameScreen;
    [HideInInspector] public GameObject exitScreen;
    [HideInInspector] public GameObject endScreen;
    //----------------------------------------------------------------------
    // Todavia no se si lo voy a dejar aqui
    //----------------------------------------------------------------------
    [HideInInspector] public GameManager gameManager;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        string sInfo = "Awake GameScript [" + name + "]    Cont: [" + (Tool.iContador++) + "]";
        Tool.LogColor(sInfo, Color.green);
        Tool.LogLine(sInfo);

        game = Names.getGame();

        menuCanvas = game.transform.Find(Constants.sGameMenuCanvas).GetComponent<Canvas>();
        menuScreen = menuCanvas.transform.Find(Constants.sMenuScreen).gameObject;
        optionsScreen = menuCanvas.transform.Find(Constants.sOptionsScreen).gameObject;

        fondoCanvas = game.transform.Find(Constants.sGameFondoCanvas).GetComponent<Canvas>();

        gameCanvas = game.transform.Find(Constants.sGameGameCanvas).GetComponent<Canvas>();
        gameScreen = gameCanvas.transform.Find(Constants.sGameScreen).gameObject;
        exitScreen = gameCanvas.transform.Find(Constants.sExitScreen).gameObject;
        endScreen = gameCanvas.transform.Find(Constants.sEndScreen).gameObject;

        //----------------------------------------------------------------------
        // Todavia no se si lo voy a dejar aqui
        // - El GameManager no creo que deba de estar activo al entrar en la
        //   GameScene
        //   El problema tambien es que si no esta activo con esta sentencia
        //   no se encuentra:
        //
        //   gameManager = GameObject.Find(sGameManager).GetComponent<GameManager>();
        //
        // Hay que preparar otras que ya construimos en Tool:
        // -  que busca en todos los objetos de la Jerarquia esten activos o
        //    no.
        //----------------------------------------------------------------------
        gameManager = Names.getGameManager(gameCanvas, game);
        //----------------------------------------------------------------------
    }

    void OnEnable()
    {
        string sInfo = "OnEnable GameCanvasScript [" + name + "]    Cont: [" + (Tool.iContador++) + "]";
        Tool.LogColor(sInfo, Color.green);
        Tool.LogLine(sInfo);
        PlaySound.SetAudioFxSource(GetComponent<AudioSource>());
        toMenuScreen();
    }

    void Update()
    {
        Tool.controlTeclaRepetida(Time.deltaTime);

    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    public void toMenuScreen()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        game.gameObject.SetActive(true);

        // La parte del juego se desactiva
        fondoCanvas.gameObject.SetActive(false);
        gameCanvas.gameObject.SetActive(false);
        exitScreen.SetActive(false);
        endScreen.SetActive(false);
        gameScreen.SetActive(false);
        gameManager.gameObject.SetActive(false);
        GameManager.IsInitGame = false;

        // La parte del menu se activa
        menuCanvas.gameObject.SetActive(true);
        menuScreen.SetActive(true);
        optionsScreen.SetActive(false);
        // enConstruccionScreen.SetActive(false);
    }

    public void OnOptions()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        menuScreen.SetActive(false);
        optionsScreen.SetActive(true);
    }

    public void OnPlay()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        exitScreen.SetActive(false);
        endScreen.SetActive(false);

        menuCanvas.gameObject.SetActive(false);
        menuScreen.SetActive(false);
        optionsScreen.SetActive(false);
        //enConstruccionScreen.SetActive(false);

        // Habra que ver como hacer el Reinicio siempre desde aqui.
        GameManager.IsInitGame = true;
        GameManager.IsPausa = false;
        GameManager.IsGameOver = false;
        fondoCanvas.gameObject.SetActive(true);
        gameCanvas.gameObject.SetActive(true);
        gameScreen.SetActive(true);
        gameManager.gameObject.SetActive(true);
    }

    public void OnExitScreen()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        GameManager.IsPausa = true;
        gameManager.runner.togglePause();

        gameScreen.SetActive(false);
        exitScreen.SetActive(true);
    }

    public void OnContinuePlay()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        GameManager.IsPausa = false;
        gameManager.runner.togglePause();

        gameScreen.SetActive(true);
        exitScreen.SetActive(false);
    }

    public void OnEndScreen()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        gameScreen.SetActive(false);
        GameManager.IsGameOver = true;
        gameManager.runner.stopAnimator();
        endScreen.SetActive(true);
    }

    public void OnReturn()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        SceneManager.LoadScene(Constants.sInitScene);
    }
    //----------------------------------------------------------------------
    #endregion
}
