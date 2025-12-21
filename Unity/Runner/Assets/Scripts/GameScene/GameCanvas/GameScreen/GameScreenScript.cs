using TauriLand.Libreria;
using TauriLand.MysticRunner;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameScreenScript : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Constantes
    //----------------------------------------------------------------------
    string sLifeBar = "LifeBar";
    string sItemsMarker = "ItemsMark";
    string sTextMarker = "TextMarker";
    string sTimeMarker = "TimeMarker";
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    GameScript game;
    //----------------------------------------------------------------------
    [HideInInspector] public LifeBar lifeMarker;
    [HideInInspector] public TextMeshProUGUI textItemsMarker;
    [HideInInspector] public TextMeshProUGUI textTimeMarker;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        Tool.LogColor("Awake GameScreenScript [" + name + "]", Color.green);

        game = GameObject.Find(Constants.sGame).GetComponent<GameScript>();
        //lifeMarker = game.gameScreen.transform.Find(sLifeBar).GetComponent<LifeBar>();
        //textItemsMarker = game.gameScreen.transform.Find(sItemsMarker).Find(sTextMarker).gameObject.GetComponent<TextMeshProUGUI>();
        //textTimeMarker = game.gameScreen.transform.Find(sTimeMarker).gameObject.GetComponent<TextMeshProUGUI>();
        lifeMarker = transform.Find(sLifeBar).GetComponent<LifeBar>();
        textItemsMarker = transform.Find(sItemsMarker).Find(sTextMarker).gameObject.GetComponent<TextMeshProUGUI>();
        textTimeMarker = transform.Find(sTimeMarker).gameObject.GetComponent<TextMeshProUGUI>();
    }

    void OnEnable()
    {
        // Inicializacion del juego
        Tool.LogColor("OnEnable GameScreenScript [" + name + "]", Color.green);
    }

    void Update()
    {
        if (
                !Tool.isRepeatedKey &&
                (Keyboard.current.escapeKey.isPressed || Keyboard.current.pKey.isPressed)
           )
        {
            Tool.setTeclaRepetida();
            game.OnExitScreen();
        }

        // Evidentemente esto ya aqui, me sobra.
        if (!GameManager.IsInitGame || GameManager.IsPausa || GameManager.IsGameOver)
            return;
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    //----------------------------------------------------------------------
    #endregion
}
