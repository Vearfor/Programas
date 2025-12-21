using TauriLand.Libreria;
using TauriLand.MysticRunner;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MenuScreenScript : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    const string sButOptions = "butOptions";
    const string sButPlay = "butPlay";
    const string sButReturn = "butReturn";
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    GameScript game;
    //----------------------------------------------------------------------
    Button butOptions;
    Button butPlay;
    Button butReturn;
    //----------------------------------------------------------------------
    Image UIImagen;
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        Tool.LogColor("Awake MenuScreenScript [" + name + "]", Color.green);

        // Inicializacion del juego
        game = GameObject.Find(Constants.sGame).GetComponent<GameScript>();

        butOptions = game.menuScreen.transform.Find(sButOptions).GetComponent<Button>();
        butPlay = game.menuScreen.transform.Find(sButPlay).GetComponent<Button>();
        butReturn = game.menuScreen.transform.Find(sButReturn).GetComponent<Button>();

        butOptions.onClick.AddListener(game.OnOptions);
        butPlay.onClick.AddListener(game.OnPlay);
        butReturn.onClick.AddListener(game.OnReturn);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed && !Tool.isRepeatedKey)
        {
            Tool.setTeclaRepetida();
            game.OnReturn();
        }
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
