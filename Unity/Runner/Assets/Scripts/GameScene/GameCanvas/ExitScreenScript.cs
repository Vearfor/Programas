using TauriLand.Libreria;
using TauriLand.MysticRunner;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class ExitScreenScript : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    const string sButContinue = "butContinue";
    const string sButReturn = "butReturn";
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    GameScript game;
    //----------------------------------------------------------------------
    Button butContinue;
    Button butReturn;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        Tool.LogColor("Awake ExitScreenScript [" + name + "]", Color.green);

        foreach (Transform t in transform)
        {
            Tool.LogColor(" Hijo Directo: " + t.name, Color.yellow);
        }

        game = GameObject.Find(Constants.sGame).GetComponent<GameScript>();

        butContinue = game.exitScreen.transform.Find(sButContinue).GetComponent<Button>();
        butReturn = game.exitScreen.transform.Find(sButReturn).GetComponent<Button>();

        butContinue.onClick.AddListener(game.OnContinuePlay);
        butReturn.onClick.AddListener(OnReturn);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed && !Tool.isRepeatedKey)
        {
            Tool.setTeclaRepetida();
            OnReturn();
        }
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    public void OnReturn()
    {
        // Si nos vamos de aqui, la partida aun no esta terminada, pero
        // debemos tener en cuenta que si se termina aqui la partida
        GameManager.IsGameOver = true;
        game.toMenuScreen();
    }
    //----------------------------------------------------------------------
    #endregion
}