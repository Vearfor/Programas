using UnityEngine;

public class HealthObjectController : ObjectController
{
    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    // Los Mobile tienen velocidad propia para mover a si mismos
    //----------------------------------------------------------------------
    public float speedHealth = .25f;
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // GiftObjectController: tendra el minimo daño.
    //----------------------------------------------------------------------
    public float giftLife = 10f;
    //----------------------------------------------------------------------
    // GiftObjectController: tendra el minimo daño.
    //----------------------------------------------------------------------
    public float rotationSpeed = 100f;
    //----------------------------------------------------------------------
    #endregion


    #region Constructor
    /*--------------------------------------------------------------------*\
    |* Constructor
    \*--------------------------------------------------------------------*/
    public HealthObjectController() : base(eObjectType.Health)
    {
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    //----------------------------------------------------------------------
    // Init de HealthObjectController
    //----------------------------------------------------------------------
    public override void init(GameObject father, GameManager pGameManager)
    {
        if (GameManager.IsGameOver)
            return;

        float x = UnityEngine.Random.Range(limLeft, limRight);
        float y = UnityEngine.Random.Range(1.5f, inicioY);
        float z = finZ;

        Vector3 pos = new Vector3(x, y, z);

        init(pos, father, pGameManager);
    }


    //----------------------------------------------------------------------
    // Se calcula el 'incZ' que se avanza.
    //----------------------------------------------------------------------
    protected override void calcInc(float deltaTime)
    {
        base.calcInc(deltaTime);

        // Su propia velocidad como mobile.
        incZ += deltaTime * speedHealth * (-1f);
    }

    //----------------------------------------------------------------------
    // Ejecutamos una funcion propia de moveObject para TreeContrller
    //----------------------------------------------------------------------
    public override void moveObject()
    {
        base.moveObject();

        //------------------------------------------------------------------
        // Hay que matener los arboles en la escena de juego
        // Si llego al final de mi suelo vuelvo al principio
        //------------------------------------------------------------------
        Vector3 position = transform.position;
        if (position.z < inicioZ)
        {
            gameManager.destroyObjectController(this, "HealthObjectController Final Z");
        }

        //------------------------------------------------------------------
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        //------------------------------------------------------------------
    }
    //----------------------------------------------------------------------
    #endregion
}
