using TauriLand.Libreria;
using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using System.Xml.Serialization;
using TauriLand.MysticRunner;
using UnityEngine;
using static ObjectController;

public class GameManager : MonoBehaviour
{
    #region Cadenas de Texto
    //----------------------------------------------------------------------
    // Cadenas de Texto
    //----------------------------------------------------------------------
    string sArboles = "Arboles";
    string sObjetos = "Objetos";
    //----------------------------------------------------------------------
    // Tenemos que probar para la curva, nos servira como padre de las
    // texturas de la curva
    //----------------------------------------------------------------------
    string sCurva = "Curva";
    //----------------------------------------------------------------------
    #endregion


    #region Statics Comunes
    //----------------------------------------------------------------------
    // Statics Comunes
    //----------------------------------------------------------------------
    public static bool IsInitGame = false;
    public static bool IsGameOver = false;
    public static bool IsPausa = false;
    //----------------------------------------------------------------------
    public static int numMaxOfGifts = 40;
    public static int numMaxOfKillers = 20;
    public static int numMaxOfHeallths = 5;
    //----------------------------------------------------------------------
    public static int numMaxOfTrees = 400;
    //----------------------------------------------------------------------
    public static int numOfGifts = 20;
    public static int numOfKillers = 10;
    public static int numOfHealths = 1;
    //----------------------------------------------------------------------
    // Que pueden ser mas.
    // - y no tienen que ser TreeControllers
    //----------------------------------------------------------------------
    public static int numOfTrees = 80;
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    public Runner runner;
    //----------------------------------------------------------------------
    // Referencias fijas a los elementos que vamos a crear.
    //----------------------------------------------------------------------
    public List<ObjectController> lisObjectsRef = new List<ObjectController>();
    //----------------------------------------------------------------------
    // Los elementos que vamos a crear.
    // - Trees
    // - Items
    // - obstaculos
    // - damges
    // - ... lo que se nos ocurra
    // + Todos dependientes de ObjectController
    //----------------------------------------------------------------------
    [HideInInspector] public List<ObjectController> lisObjects = new List<ObjectController>();
    [Tooltip("Rango de tiempo en segundos para crear nuevos objetos")]
    public float rangoMinTiempo = 2f;
    public float rangoMaxTiempo = 10f;
    //----------------------------------------------------------------------
    bool corrutinaLanzada = false;
    //----------------------------------------------------------------------
    // Enumerados con los indices de donde se encuentran los objetos
    // reference en la lista de : lisObjectsRef
    //----------------------------------------------------------------------
    enum eIndexReferences
    {
        MinIndexTree = 0,      // Los llamo Trees , pero puede haber mas.
        Jugle02 = 1,
        Abeto03 = 2,
        Pino04_Arboles = 3,
        Robles05 = 4,
        Pino06_Arbole = 5,
        MaxIndexTree = 5,
        // Los de arriba deberian ser fijos, son del paisaje
        // Los de abajo son los variables, una vez han llegado al final o
        // han cumplido su mision se borran:
        KillerBall = 6,                    // Puede haber mas
        MaxKillerBall = 7,                 // Puede haber mas
        MegaKillerBall = 8,                 // Puede haber mas
        GiftObjectController = 9,          // Puede haber mas
        HealthObjectController = 10,        // Puede haber mas.
    }
    //----------------------------------------------------------------------
    // Objetos padre de los objetos de lsiObjects
    //----------------------------------------------------------------------
    GameObject gameArboles;
    GameObject gameObjects;
    //----------------------------------------------------------------------
    // Para la generacion de la curva por codigo.
    // - podria terminar por hacer un circuito,
    // + por generar varios circuitos.
    //----------------------------------------------------------------------
    GameObject gameCurva;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    private void Awake()
    {
        string sInfo = "Awake GameManager: [" + gameObject.name + "]    Cont: [" + (Tool.iContador++) + "]";
        try
        {
            Tool.LogColor(sInfo, Color.green);
            Tool.LogLine(sInfo);

            runner = Names.getPlayer();
        }
        catch (Exception ex)
        {
            sInfo = "Exception: Awake GameManager: [" + ex.Message + "]";
            Tool.LogColor(sInfo, Color.red);
            Tool.LogLine(sInfo);
        }
    }

    void OnEnable()
    {
        string sInfo = "OnEnable GameManager: [" + gameObject.name + "]";
        Tool.LogColor(sInfo, Color.green);
        Tool.LogLine(sInfo);

        sInfo = "- GameManager.IsInitGame: [" + GameManager.IsInitGame + "]";
        Tool.LogColor(sInfo, Color.green);
        Tool.LogLine(sInfo);
        if (!GameManager.IsInitGame)
            return;

        gameArboles = Tool.createObject(transform, sArboles);
        gameObjects = Tool.createObject(transform, sObjetos);
        // Para tenerlo mas ordenadito que el padre sea el GameManager, en el que estamos:
        gameArboles.transform.SetParent(gameObject.transform, false);
        gameObjects.transform.SetParent(gameObject.transform, false);

        PlaySound.SetAudioBackgoundSource(GetComponent<AudioSource>());
        PlaySound.Play();
        createTrees();
        runner.inicio();
        corrutinaLanzada = false;
    }

    private void Start()
    {
        string sInfo = "Start GameManager: [" + gameObject.name + "] (Solo una vez)";
        Tool.LogColor(sInfo, Color.green);
        Tool.LogLine(sInfo);
    }

    void Update()
    {
        if (!GameManager.IsInitGame || GameManager.IsPausa || GameManager.IsGameOver)
            return;

        spawnObjects();
    }

    void OnDisable()
    {
        string sInfo = "OnDisable GameManager: [" + gameObject.name + "]";
        Tool.LogColor(sInfo, Color.green);
        Tool.LogLine(sInfo);

        sInfo = " - Game Over: [" + GameManager.IsGameOver.ToString() + "]";
        Tool.LogColor(sInfo, Color.green);
        Tool.LogLine(sInfo);

        PlaySound.Stop();

        destroyAllObjects();

        if (MainManager.gameList == null)
        {
            MainManager.gameList = GameRecordList.readingGameList(Constants.sDirFileGames, Constants.sNameFicFileGames);
        }

        // Entonces había partida
        if (GameManager.IsGameOver)
        {
            if (MainManager.gameList == null)
            {
                // No se ha podido abrir el fihero, no se ha creado la lista:
                MainManager.gameList = new GameRecordList(Constants.sDirFileGames, Constants.sNameFicFileGames);
            }

            // Marcamos la hora de salida:
            runner.gameRecord.when = DateTime.Now;

            sInfo = " + last Record: [" + runner.gameRecord + "]";
            Tool.LogColor(sInfo, Color.cyan);
            Tool.LogLine(sInfo);

            // Agregamos el registro:
            MainManager.gameList.Add(runner.gameRecord);

            GameRecord bestRecord = GameRecordList.getBestGameRecord(MainManager.gameList);
            Tool.LogColor(" best Record: [" + bestRecord.ToString() + "]", Color.cyan);

            GameRecordList.sortGameList(MainManager.gameList);
            GameRecordList.writingGameList(MainManager.gameList);

            sInfo = " - Fin de Juego: " + Tool.programName;
            Tool.LogColor(sInfo, Color.aliceBlue);
            Tool.LogLine(sInfo);
        }
        // Si no, no registramos, una partida que nunca fue.
    }

    //----------------------------------------------------------------------
    // Cuando nos salimos de la escena
    //----------------------------------------------------------------------
    void OnDestroy()
    {
        string sInfo = "OnDestroy GameManager: [" + gameObject.name + "]";
        Tool.LogColor(sInfo, Color.green);
        Tool.LogLine(sInfo);
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/


    #region Creacion de Objetos
    //----------------------------------------------------------------------
    // Create Objects
    //----------------------------------------------------------------------
    void createTrees()
    {
        try
        {
            string sInfo = " CreateTrees:";
            Tool.LogColor(sInfo, Color.cyan);
            Tool.LogLine(sInfo);
            int numObjectsInicio = lisObjects.Count;
            for (int i = 0; i < numOfTrees; i++)
            {
                int indexTreeReference = Tool.rangoAleatorio((int)eIndexReferences.MinIndexTree, (int)eIndexReferences.MaxIndexTree);
                ObjectController objReference = lisObjectsRef[indexTreeReference];
                objReference.init(gameArboles, this);
            }
            int numObjectsFinal = lisObjects.Count;

            sInfo = " CreateTrees:   Antes [" + numObjectsInicio + "]   Despues [" + numObjectsFinal + "]";
            Tool.LogColor(sInfo, Color.cyan);
            Tool.LogLine(sInfo);
        }
        catch (Exception ex)
        {
            string sInfo = "Exception: GameManager.createTrees: [" + ex.Message + "]";
            Tool.LogColor(sInfo, Color.red);
            Tool.LogLine(sInfo);
        }
    }

    void spawnObjects()
    {
        if (corrutinaLanzada)
            return;

        // Estamos lanzado la corrutina
        corrutinaLanzada = true;
        // Lanza Corrutina.
        // Ya esta controlado
        // - si esta en pausa,
        // - si la partida ha terminado
        // - si no se ha iniciado la partida
        float timeToBeCreated = UnityEngine.Random.Range(rangoMinTiempo, rangoMaxTiempo);
        StartCoroutine(initObject(timeToBeCreated, this, gameObjects));
    }

    IEnumerator initObject(float timeToBeCreated, GameManager gameManager, GameObject father)
    {
        int indexMin = (int)eIndexReferences.MaxIndexTree + 1;
        int indexMax = lisObjectsRef.Count - 1;
        int indexTree = Tool.rangoAleatorio(indexMin, indexMax);
        ObjectController obj = lisObjectsRef[indexTree];

        // Tiempo entre creacion.
        float timeBetweenCreates = .5f;
        int numObjectToBeCreated = getNumObjectsToBeCreated(obj.type);

        // Para crear varias de golpe:
        for (int i = 0; i < numObjectToBeCreated; i++)
        {
            if (i == 0)
                yield return new WaitForSeconds(timeToBeCreated);
            else
                yield return new WaitForSeconds(timeBetweenCreates);

            if (!GameManager.IsInitGame || GameManager.IsPausa || GameManager.IsGameOver)
                break;

            obj.init(father, gameManager);
        }

        // La corrutina ha terminado, se puede volver a lanzar otra.
        corrutinaLanzada = false;
    }

    int getNumObjectsToBeCreated(eObjectType type)
    {
        int numObjectsToBeCreated = 0;
        switch (type)
        {
            case eObjectType.Killer:
            case eObjectType.MaxKiller:
                numObjectsToBeCreated = GameManager.numOfKillers;
                break;

            case eObjectType.Gift:
                numObjectsToBeCreated = GameManager.numOfGifts;
                break;

            case eObjectType.Health:
                numObjectsToBeCreated = GameManager.numOfHealths;
                break;

            case eObjectType.Tree:
            case eObjectType.None:
            default:
                break;
        }
        return numObjectsToBeCreated;
    }
    //----------------------------------------------------------------------
    #endregion


    #region Destruccion de Objetos
    //----------------------------------------------------------------------
    // Destroy Objects
    //----------------------------------------------------------------------
    void destroyAllObjects()
    {
        if (lisObjects.Count > 0)
        {
            string sInfo = string.Format(" Destruimos los objetos restantes: {0} objetos", lisObjects.Count);
            Tool.LogColor(sInfo, Color.cyan);
            Tool.LogLine(sInfo);

            // Creo que podemos hacer esto.
            // El Garbage Collector se encarga del resto: ¿ o va a resultar que no ?
            lisObjects.Clear();

            sInfo = string.Format(" Quedan: {0} objetos", lisObjects.Count);
            Tool.LogColor(sInfo, Color.cyan);
            Tool.LogLine(sInfo);

            // Borramos tambien los padres:
            Destroy(gameArboles);
            Destroy(gameObjects);
        }
    }

    //----------------------------------------------------------------------
    // Destruccion del objeto
    //----------------------------------------------------------------------
    public void destroyObjectController(ObjectController obj, string sDonde)
    {
        obj.gameObject.SetActive(false);
        lisObjects.Remove(obj);
        Destroy(obj.gameObject);
    }
    //----------------------------------------------------------------------
    #endregion


    //----------------------------------------------------------------------
    // Generacion de la curva por codigo.
    //----------------------------------------------------------------------
    void createCurva()
    {
        // A ver qcomo lo hago. 
        // Y luego como lo guardo.
        gameCurva = Tool.createObject(transform, sCurva);
    }
    //----------------------------------------------------------------------
    #endregion
}
