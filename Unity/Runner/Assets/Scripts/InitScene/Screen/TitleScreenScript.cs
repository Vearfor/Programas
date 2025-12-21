using TauriLand.Libreria;
using TauriLand.MysticRunner;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleScreenScript : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    const string sButGameList = "butGameList";
    const string sButContinue = "butContinue";
    const string sButExit = "butExit";
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    MainManager mainManager;
    //----------------------------------------------------------------------
    Button butGameList;
    Button butContinue;
    Button butExit;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        Tool.LogColor("Awake TileScreenScript [" + name + "]", Color.green);

        Transform hijoTrans;

        mainManager = GameObject.Find(Constants.sMainManagerObject).GetComponent<MainManager>();

        hijoTrans = transform.Find(sButGameList);
        butGameList = hijoTrans.gameObject.GetComponent<Button>();

        hijoTrans = transform.Find(sButContinue);
        butContinue = hijoTrans.gameObject.GetComponent<Button>();

        hijoTrans = transform.Find(sButExit);
        butExit = hijoTrans.gameObject.GetComponent<Button>();

        butGameList.onClick.AddListener(OnGameList);
        butContinue.onClick.AddListener(OnContinue);
        butExit.onClick.AddListener(OnExit);
    }

    void OnEnable()
    {
        Tool.LogColor("OnEnable TileScreenScript [" + name + "]", Color.green);
    }

    void Start()
    {
        Tool.LogColor("Start TileScreenScript [" + name + "]", Color.green);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed && !Tool.isRepeatedKey)
        {
            Tool.setTeclaRepetida();
            Tool.Salir();
        }
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    void OnGameList()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        mainManager.titleScreen.SetActive(false);
        mainManager.gameListScreen.SetActive(true);
    }

    void OnContinue()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        SceneManager.LoadScene(Constants.sGameScene);
    }

    void OnExit()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        Tool.Salir();
    }
    //----------------------------------------------------------------------
    #endregion
}
