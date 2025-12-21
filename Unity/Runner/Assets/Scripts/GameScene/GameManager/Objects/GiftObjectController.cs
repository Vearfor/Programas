using TauriLand.Libreria;
using TauriLand.MysticRunner;
using UnityEngine;
using UnityEngine.InputSystem;

public class GiftObjectController : MobileObjectController
{
    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    // GiftObjectController: tendra el minimo daño.
    //----------------------------------------------------------------------
    public float rotationSpeed = 500f;
    //----------------------------------------------------------------------
    [HideInInspector] public int giftItem = 1;
    [HideInInspector] public bool ejeXoZ = false;
    //----------------------------------------------------------------------
    #endregion


    #region Constructor
    /*--------------------------------------------------------------------*\
    |* Constructor
    \*--------------------------------------------------------------------*/
    public GiftObjectController() : base(eObjectType.Gift)
    {
    }
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void OnEnable()
    {
        // transform.Rotate(0f, 0f, 90f);
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    //----------------------------------------------------------------------
    // Init comun de ObjectController: el comun.
    // - llamado desde el  GameManager
    //----------------------------------------------------------------------
    public override ObjectController init(Vector3 pos, GameObject father, GameManager pGameManager)
    {
        ObjectController newController = base.init(pos, father, pGameManager);

        GiftObjectController giftObjectController = newController as GiftObjectController;

        // Es imposible que de nulo. Solo puedes llegar aqui si eres un GiftObjectController
        giftObjectController.ejeXoZ = Tool.caraOcruz();

        return newController;
    }
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Ejecutamos una funcion propia de moveObject para
    // MobileObjectController
    //----------------------------------------------------------------------
    public override void moveObject()
    {
        base.moveObject();

        //------------------------------------------------------------------
        // transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        //------------------------------------------------------------------
        transform.Rotate(
            (ejeXoZ) ? rotationSpeed * Time.deltaTime : 0f,
            0f,
            (!ejeXoZ) ? rotationSpeed * Time.deltaTime : 0f
        );
        //------------------------------------------------------------------
        //transform.Rotate(rotationSpeed * Time.deltaTime, 0f, 0f);
        //------------------------------------------------------------------
    }
    //----------------------------------------------------------------------
    #endregion
}
