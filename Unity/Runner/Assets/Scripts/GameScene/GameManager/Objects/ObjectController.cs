using TauriLand.Libreria;
using TauriLand.MysticRunner;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.UIElements;


#region Esquema de herencia
//----------------------------------------------------------------------
// Esquema de Herencia
// - lo ponemos aqui al ser la principal clase base.
//----------------------------------------------------------------------
//  Clase base   ObjectController
//  - clases hijas:
//          - TreeController  (No se mueven)
//          + ... ya veremos si ponemos mas.
//          + ... un par de arbolitos se queda cortisimo.
//
//          - MovileObjectController (Se mueven, tienen velocidad propia)
//            | - DamageObjectController  (Todas las que produzcan Damage)
//            |     + KillerBall
//            |         * Al tener textura propia deberiamos hacer que se
//            |           viese 'rodar'.
//            |            * agregarle el 'ScrollTexture' como script
//            |
//            |     + .. hay que inventarse mas, estamos obligados.
//            |     + MaxKillerBall
//            |
//            | - GiftObjectController
//            |     + los que nos dan los puntos de items conseguidos.
//            |         * Habria que crear mas de uno.
//
//          - HealthObjectController
//              + los que nos dan la opcion de recuperar parte de los
//                danios recibidos.
//
//----------------------------------------------------------------------
#endregion


public abstract class ObjectController : MonoBehaviour
{
    #region Tipos de ObjectsController
    //----------------------------------------------------------------------
    public enum eObjectType
    {
        None = -1,
        Tree = 0,     // Ponemos los que se van a crear
        Killer = 1,
        MaxKiller = 2,
        MegaKiller = 3,
        Gift = 4,
        Health = 5
    }
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    protected GameManager gameManager;
    //----------------------------------------------------------------------
    [Header("El Runner")]
    protected Runner runner;
    public float incZ = 0;
    //----------------------------------------------------------------------
    protected float limLeft = -10f;
    protected float limRight = 10f;
    //----------------------------------------------------------------------
    protected float finZ = 330f;
    protected float inicioY = 2.5f;       // Depende de scale. por ahora fijos.
    //----------------------------------------------------------------------
    // La Velocidad de los Objects:
    // Debe de estar relacionado con la velocidad con la cual
    // movemos el resto de elementos
    // - el valor comun era runner.speed
    //   pero hay que balancear para cada elemento el factor
    //   adecuado
    //----------------------------------------------------------------------
    // + En pruebas con el valor de 20f la textura y los objetos parecen
    //   que se mueven al unisono.
    // +  Se mueve en Z.
    //----------------------------------------------------------------------
    public float speedFactor = 40f;
    //----------------------------------------------------------------------
    // Los que estan como referencia, no se mueven, no se borran
    //----------------------------------------------------------------------
    [HideInInspector] public bool toBeMoved = false;
    //----------------------------------------------------------------------
    protected float inicioZ = -60f;
    //----------------------------------------------------------------------
    [HideInInspector] public eObjectType type;
    //----------------------------------------------------------------------
    static int iContador = 0;
    //----------------------------------------------------------------------
    #endregion


    #region Constructor
    /*--------------------------------------------------------------------*\
    |* Constructor
    \*--------------------------------------------------------------------*/
    public ObjectController(eObjectType type)
    {
        this.type = type;
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos MonoBehaviour
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones MonoBehaviour
    \*--------------------------------------------------------------------*/
    virtual protected void Awake()
    {
        Tool.LogColor("Awake ObjectController [" + name + "]", Color.green);
    }

    private void OnEnable()
    {
        string sInfo = " OnEnable ObjectController [" + name + "]";
        Tool.LogColor(sInfo, Color.green);
        Tool.LogLine(sInfo);
    }

    void Update()
    {
        if (!GameManager.IsInitGame || GameManager.IsPausa || GameManager.IsGameOver || !toBeMoved)
            return;

        float deltaTime = Time.deltaTime;

        calcInc(deltaTime);
        moveObject();
        decInc(deltaTime);
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    //----------------------------------------------------------------------
    // Init variable de ObjectController: el comun.
    // - especifico segun el que llama
    //   + TreeController
    //   + MobileObjectController
    //   + HealthObjectController
    //----------------------------------------------------------------------
    public abstract void init(GameObject father, GameManager gameManager);

    //----------------------------------------------------------------------
    // Init comun de ObjectController: el comun.
    // - llamado desde el  GameManager
    //----------------------------------------------------------------------
    public virtual ObjectController init(Vector3 pos, GameObject father, GameManager pGameManager)
    {
        string sInfo;

        // La primera vez
        if (gameManager == null)
        {
            sInfo = " GameManager es nulo";
            Tool.LogColor(sInfo, Color.yellow);
            Tool.LogLine(sInfo);
            gameManager = pGameManager;
            runner = gameManager.runner;
        }

        iContador++;
        ObjectController refController = this;
        ObjectController newController = Instantiate(refController, pos, Quaternion.identity);
        newController.toBeMoved = true;
        newController.name = refController.name + "_" + iContador.ToString();
        newController.gameManager = gameManager;
        newController.runner = gameManager.runner;

        sInfo = string.Format(" Iniciamos [{0}]: [{1,3}]   Pos: [{2}]",
            newController.name,
            iContador,
            pos.ToString());
        Tool.LogColor(sInfo, Color.cyan);
        Tool.LogLine(sInfo);

        if (father != null)
            newController.transform.SetParent(father.transform, false);

        // La primera vez
        if (!newController.gameObject.activeSelf)
        {
            sInfo = " No estaba activa la referencia (?)";
            Tool.LogColor(sInfo, Color.yellow);
            Tool.LogLine(sInfo);
            newController.gameObject.SetActive(true);
        }

        // Se agrega a la lista:
        gameManager.lisObjects.Add(newController);

        return newController;
    }
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Se calcula el 'incZ' que se avanza.
    // - solo se mueve si el Runner se mueve.
    //----------------------------------------------------------------------
    protected virtual void calcInc(float deltaTime)
    {
        if (runner.hayRunning != 0)
        {
            //--------------------------------------------------------------
            // inc = deltaTime * runner.speedText * speedFactor;
            //--------------------------------------------------------------
            incZ = deltaTime * speedFactor;
            incZ *= runner.hayRunning;
        }
    }

    public virtual void moveObject()
    {
        Vector3 position = transform.position;
        position.z += incZ;
        transform.position = position;
    }

    //----------------------------------------------------------------------
    // Calcula el decremento
    //----------------------------------------------------------------------
    void decInc(float deltaTime)
    {
        if (Mathf.Abs(incZ) > 0)
        {
            float factor = Tool.calcRedFactor(runner.redFactorZ, deltaTime);
            incZ *= factor;
            incZ = (Mathf.Abs(incZ) < Tool.cEpsilon) ? 0f : incZ;
        }
    }
    //----------------------------------------------------------------------
    #endregion
}
