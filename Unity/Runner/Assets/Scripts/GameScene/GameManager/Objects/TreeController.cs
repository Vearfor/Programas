using TauriLand.Libreria;
using System;
using System.Collections;
using TauriLand.MysticRunner;
using UnityEngine;

public class TreeController : ObjectController
{
    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    // True a la derecha: multiplicamos por 1,
    // False a la izquierda: multiplicamos por -1.
    // Izquierda donde mi mano izquierda (derecha de la pantalla -1)
    //----------------------------------------------------------------------
    bool derechaIzquierda = false;
    //----------------------------------------------------------------------
    #endregion


    #region Constructor
    /*--------------------------------------------------------------------*\
    |* Constructor
    \*--------------------------------------------------------------------*/
    public TreeController() : base(eObjectType.Tree)
    {
        speedFactor = 80f;
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos MonoBehaviour
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones MonoBehaviour
    \*--------------------------------------------------------------------*/
    override protected void Awake()
    {
        Tool.LogColor("Awake TreeController [" + name + "]", Color.green);
        //------------------------------------------------------------------
        limLeft = 40f;
        limRight = 10f;
        //------------------------------------------------------------------
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    //----------------------------------------------------------------------
    // Init de TreeController
    // - llamado desde el  GameManager
    // - y puesto aqui porque es especifico del TreeController.
    //----------------------------------------------------------------------
    public override void init(GameObject father, GameManager pGameManager)
    {
        if (GameManager.IsGameOver)
            return;

        // Esto es del TreeController: especifico.
        float x = UnityEngine.Random.Range(limLeft, limRight);
        float z = UnityEngine.Random.Range(inicioZ, finZ);
        derechaIzquierda = Tool.caraOcruz();
        x *= (derechaIzquierda) ? 1 : -1;

        Vector3 pos = new Vector3(x, inicioY, z);

        init(pos, father, pGameManager);
    }

    //----------------------------------------------------------------------
    // Ejecutamos una funcion propia de moveObject para TreeContrller
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
            position.z = finZ;
            transform.position = position;
        }
    }
    //----------------------------------------------------------------------
    #endregion
}
