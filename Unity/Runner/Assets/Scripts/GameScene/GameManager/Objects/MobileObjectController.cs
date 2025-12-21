using UnityEngine;

public class MobileObjectController : ObjectController
{
    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    // Los Mobile tienen velocidad propia para mover a si mismos
    //----------------------------------------------------------------------
    public float speedMobile = .25f;
    //----------------------------------------------------------------------
    //----------------------------------------------------------------------
    // Los Mobile tienen velocidad propia para mover su textura (?)
    //----------------------------------------------------------------------
    public float speedText = .5f;
    //----------------------------------------------------------------------
    #endregion


    #region Constructor
    /*--------------------------------------------------------------------*\
    |* Constructor
    \*--------------------------------------------------------------------*/
    public MobileObjectController(eObjectType type) : base(type)
    {
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    //----------------------------------------------------------------------
    // Init de MobileObjectController
    // - llamado desde el  GameManager
    // - y puesto aqui porque es especifico del MobileObjectController.
    //----------------------------------------------------------------------
    public override void init(GameObject father, GameManager pGameManager)
    {
        if (GameManager.IsGameOver)
            return;

        // Esto es del DamageObjectController: especifico.
        float x = UnityEngine.Random.Range(limLeft, limRight);
        float y = UnityEngine.Random.Range(1.5f, inicioY);
        float z = finZ;

        Vector3 pos = new Vector3(x, y, z);

        init(pos, father, pGameManager);
    }
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Se calcula el 'incZ' que se avanza.
    //----------------------------------------------------------------------
    protected override void calcInc(float deltaTime)
    {
        base.calcInc(deltaTime);

        // Su propia velocidad como mobile.
        incZ += deltaTime * speedMobile * (-1f);
    }

    //----------------------------------------------------------------------
    // Ejecutamos una funcion propia de moveObject para
    // MobileObjectController
    //----------------------------------------------------------------------
    public override void moveObject()
    {
        base.moveObject();

        //----------------------------------------------------------------------
        // Hay que matener los arboles en la escena de juego
        // Si llego al final de mi suelo vuelvo al principio
        //----------------------------------------------------------------------
        Vector3 position = transform.position;
        if (position.z < inicioZ)
        {
            gameManager.destroyObjectController(this, "MobileObjectController Final Z");
        }
    }
    //----------------------------------------------------------------------
    #endregion
}
