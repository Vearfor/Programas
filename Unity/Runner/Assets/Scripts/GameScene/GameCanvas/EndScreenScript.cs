using TauriLand.Libreria;
using TauriLand.MysticRunner;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class EndScreenScript : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    const string sButReturn = "butReturn";
    const string sTextEndGame = "textEndGame";
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    GameScript game;
    //----------------------------------------------------------------------
    Button butReturn;
    TextMeshProUGUI textEndGame;
    Runner runner;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        Tool.LogColor("Awake EndScreenScript [" + name + "]", Color.green);

        game = Names.getGame();

        butReturn = game.endScreen.transform.Find(sButReturn).GetComponent<Button>();
        textEndGame = game.endScreen.transform.Find(sTextEndGame).GetComponent<TextMeshProUGUI>();

        butReturn.onClick.AddListener(game.toMenuScreen);
    }

    void OnEnable()
    {
        GameManager gameManager = Tool.lookForGameObject(Constants.sGameManager).GetComponent<GameManager>();
        runner = gameManager.runner;

        // Fin de Juego/Partida
        // 10 Items conseguidos, en 2 minutos y 13 segundos.
        int items = runner.gameRecord.itemsReached;
        float seconds = runner.gameRecord.secondsOfPlay;

        float secondsToShow = seconds % 60f;
        int minutes = (int) (seconds/60f);

        string sFin = "Fin de Partida.\n" +
            items +
            " items conseguidos,\nen " + minutes + " minutos y " + string.Format("{0:00}", secondsToShow) + " segundos";

        textEndGame.text = sFin;
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed && !Tool.isRepeatedKey)
        {
            Tool.setTeclaRepetida();
            game.toMenuScreen();
        }
    }
    //----------------------------------------------------------------------
    #endregion
}
