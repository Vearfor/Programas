using NUnit.Framework;
using TauriLand.Libreria;
using TauriLand.MysticRunner;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class GameListScreenScript : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    const string sButReturn = "butReturn";
    //----------------------------------------------------------------------
    const string sTextNoHayPartidas = "No hay Partidas";
    const string sTextCabecera = "Cabecera";
    const string sFondoLista = "FondoLista";
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    MainManager mainManager;
    //----------------------------------------------------------------------
    Button butReturn;
    GameObject regNoHayPartidas;
    GameObject regTextCabecera;
    GameObject fondoLista;
    //----------------------------------------------------------------------
    GameObject listaPadre;
    List<GameObject> regLineas;
    //----------------------------------------------------------------------
    // Me tengo que fabricar una seccion de valores fijos aunque luego
    // los cambie en el inspector
    //----------------------------------------------------------------------
    int listRecordLimit = 8;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        Tool.LogColor("Awake GameListScreenScript [" + name + "]", Color.green);

        mainManager = GameObject.Find(Constants.sMainManagerObject).GetComponent<MainManager>();

        Transform transButton = transform.Find(sButReturn);
        butReturn = transButton.gameObject.GetComponent<Button>();

        butReturn.onClick.AddListener(OnReturn);

        Transform transNoHayPartidas = transform.Find(sTextNoHayPartidas);
        regNoHayPartidas = transNoHayPartidas.gameObject;

        Transform transRegPartida = transform.Find(sTextCabecera);
        regTextCabecera = transRegPartida.gameObject;

        Transform transFondo = transform.Find(sFondoLista);
        fondoLista = transFondo.gameObject;
    }

    void OnEnable()
    {
        Tool.LogColor("OnEnable GameListScreenScript [" + name + "]", Color.green);

        showGameList();
    }

    void Start()
    {
        Tool.LogColor("Start GameListScreenScript [" + name + "]", Color.green);
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed && !Tool.isRepeatedKey)
        {
            Tool.setTeclaRepetida();
            OnReturn();
        }
    }

    void OnDisable()
    {
        Tool.LogColor("OnEnable GameListScreenScript [" + name + "]", Color.green);

        destroyGameList();
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    void OnReturn()
    {
        PlaySound.PlayFxClip((int)Sounds.transicion);
        mainManager.gameListScreen.SetActive(false);
        mainManager.titleScreen.SetActive(true);
    }
    //----------------------------------------------------------------------

    void showGameList()
    {
        // Mostraremos lo que podamos.
        if (MainManager.gameList != null)
        {
            if (MainManager.gameList.Count > 0)
            {
                fondoLista.SetActive(true);
                regNoHayPartidas.SetActive(false);
                listaPadre = Tool.createObject(transform, "Lista");
                listaPadre.transform.parent = gameObject.transform;

                for (int iPos = 0; iPos < MainManager.gameList.Count && iPos < listRecordLimit; iPos++)
                {
                    createLine(iPos, MainManager.gameList[iPos], listaPadre);
                }
            }
            else
            {
                fondoLista.SetActive(false);
                regNoHayPartidas.SetActive(true);
            }
        }
    }

    void createLine(int ipos, GameRecord record, GameObject padre)
    {
        if (regLineas == null)
            regLineas = new List<GameObject>();

        Vector3 pos = regTextCabecera.transform.position;
        pos.y -= 125;
        pos.y -= (ipos * 75);

        GameObject line = Instantiate(regTextCabecera, pos, Quaternion.identity);

        GameRecordLine gameRecordLine = line.GetComponent<GameRecordLine>();
        if (ipos==listRecordLimit-1)
        {
            line.name = "RecordGame_Vacio";
            gameRecordLine.textItems.text = "...";
            gameRecordLine.textDistance.text = "...";
            gameRecordLine.textTime.text = "...";
            gameRecordLine.textDate.text = "...";
        }
        else
        {
            line.name = string.Format("RecordGame_{0:00}", ipos);

            gameRecordLine.textItems.text = record.itemsReached.ToString();

            gameRecordLine.textDistance.text = record.distance.ToString("00.00");

            int seconds = (int)(record.secondsOfPlay % 60);
            int minutes = (int)(record.secondsOfPlay / 60);
            int hours = (int)(minutes / 60);
            gameRecordLine.textTime.text =
                string.Format("{0}:{1:00}:{2:00}", hours, minutes, seconds);

            gameRecordLine.textDate.text = record.when.ToString("yyyy/MM/dd HH:mm:ss");
        }

        line.transform.SetParent(padre.transform, false);
        line.SetActive(true);
        regLineas.Add(line);
    }

    void destroyGameList()
    {
        if (regLineas!=null)
        {
            Tool.LogColor("destroyGameList GameListScreenScript [" + name + "]", Color.green);

            regLineas.Clear();
        }
    }
    //----------------------------------------------------------------------
    #endregion
}
