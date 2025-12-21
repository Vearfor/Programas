using TauriLand.Libreria;
using TauriLand.MysticRunner;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class MainManager : MonoBehaviour
{
    #region Statics
    //----------------------------------------------------------------------
    // Debe quedar permanencia de elementos cargados al inicio
    // al menos la lista de partidas.
    //----------------------------------------------------------------------
    public static GameRecordList gameList = null;
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    [HideInInspector] public GameObject titleScreen;
    [HideInInspector] public GameObject gameListScreen;
    //----------------------------------------------------------------------
    [Tooltip("Clips para musicas de fondo, con el AudioSource de Playsound")]
    [SerializeField] AudioClip[] listaFondoClips;
    [Tooltip("Clips para efectos sonoros: pocos segundos, con el AudioSource del GameObject")]
    [SerializeField] AudioClip[] listaFxClips;
    //----------------------------------------------------------------------
    #endregion


    //----------------------------------------------------------------------
    // Hay que buscar esto:
    //  - preguntar a Jose o a Ruben.
    // utilizacion de texturas  con Scroll parallax en unity
    //----------------------------------------------------------------------


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        Tool.LogColor("Awake MainManager [" + name + "]    Cont: [" + Tool.iContador++ + "]  Se esta lanzando mas de una vez", Color.green);

        // Intentamos levantar Tool.Log en el Start de Tool:
        Tool.isActiveLog = true;

        if (gameList == null)
            gameList = GameRecordList.readingGameList(Constants.sDirFileGames, Constants.sNameFicFileGames);

        GameObject mainCanvas = gameObject;

        Transform trans = mainCanvas.transform.Find(Constants.sTitleScreen);
        titleScreen = trans.gameObject;

        trans = mainCanvas.transform.Find(Constants.sGameListScreen);
        gameListScreen = trans.gameObject;

        titleScreen.SetActive(false);
        gameListScreen.SetActive(false);

        // Este lo podemos poner aqui:
        // El AudioSource de PlaySound no se destruye entre escenas.
        PlaySound.InitClips(listaFondoClips);
        PlaySound.InitFondoClips(listaFondoClips);
    }

    private void OnEnable()
    {
        Tool.LogColor("OnEnable MainManager [" + name + "]", Color.green);

        PlaySound.SetAudioFxSource(GetComponent<AudioSource>());
        PlaySound.InitFxClips(listaFxClips);

        titleScreen.SetActive(true);
    }

    private void Start()
    {
        Tool.LogColor("Start MainManager [" + name + "]   Se esta lanzando Solo una vez", Color.green);
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
