using TauriLand.Libreria;
using TauriLand.MysticRunner;
using UnityEngine;

public class ScrollTexture : MonoBehaviour
{
    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    Renderer myRenderer;
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Maldito acoplamiento
    // - mientras no se te ocurre algo que lo solucione, primero lo haces
    //   y luego resuelves
    //----------------------------------------------------------------------
    // Si el objeto se mueve
    //----------------------------------------------------------------------
    MobileObjectController myMobileObjectController;
    //----------------------------------------------------------------------
    // Si el runner avanza
    //----------------------------------------------------------------------
    Runner runner;
    //----------------------------------------------------------------------
    [Header("- Si el Runner avanza o - Si el Objeto se mueve")]
    public float incOffset = 0;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones MonoBehabiuor
    \*--------------------------------------------------------------------*/
    void Awake()
    {
        try
        {
            myRenderer = GetComponent<Renderer>();

            runner = Names.getPlayer();

            // Si somos un MobileObjectController este no sera nulo. Si es nulo es que no lo somos.
            myMobileObjectController = gameObject.GetComponent<MobileObjectController>();
        }
        catch (System.Exception ex)
        {
            string sInfo = "Exception: Awake  ScrollTexture [" + ex.Message + "]";
            Tool.LogColor(sInfo, Color.red);
            Tool.LogLine(sInfo);
        }
    }

    void OnEnable()
    {
        incOffset = 0;
    }

    void Update()
    {
        // Para quitar el acoplamiento donde lo coloco:
        if (!GameManager.IsInitGame || GameManager.IsPausa || GameManager.IsGameOver)
            return;

        if (myMobileObjectController && !myMobileObjectController.toBeMoved)
            return;

        float deltaTime = Time.deltaTime;

        calcInc(deltaTime);
        moveTextura();
        decInc(deltaTime);
    }
    //----------------------------------------------------------------------
#endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    //----------------------------------------------------------------------
    // Estas serain las funciones que utilizariamos en las notificaciones
    // - que ni idean que son, si existen, y como se hacen.
    //----------------------------------------------------------------------
    //void Start()
    //{
    //    // Lo que ponia en el tutorial de principio:
    //    // NotificationCenter.DefaultCenter().AddObserver(this, "hhayMovimiento");
    //}
    //----------------------------------------------------------------------
    void hayMovimiento()
    {
        //isRunnig = true;
    }

    void noHayMovimiento()
    {
        //isRunnig = false;
    }
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Se calcula el 'incOffset' que se avanza.
    // - Si el Runner avanza
    // - o Si el Objeto se mueve
    //----------------------------------------------------------------------
    void calcInc(float deltaTime)
    {
        float incObjeto = 0;
        //--------------------------------------------------------------
        // Depende de si el objeto se mueve
        // Somos MobileObjectController, no somos nulos, y tenemos
        // velocidad:
        //--------------------------------------------------------------
        if (myMobileObjectController && myMobileObjectController.speedText > 0)
        {
            //--------------------------------------------------------------
            // Soluciona de donde viene este valor:
            //--------------------------------------------------------------
            incObjeto = deltaTime * myMobileObjectController.speedText;
        }
        //--------------------------------------------------------------

        float incRunner = 0;
        //--------------------------------------------------------------
        // Dependiente del runner
        // Depende de si el runner se mueve
        //--------------------------------------------------------------
        if (runner.hayRunning != 0)
        {
            //--------------------------------------------------------------
            // Soluciona de donde viene este valor:
            //--------------------------------------------------------------
            incRunner = deltaTime * runner.speedText;

            // Y con esto damos el sentido de si avanzamos o retrocedemos.
            incRunner *= runner.hayRunning;
            //--------------------------------------------------------------
        }

        //--------------------------------------------------------------
        // Lo que quede del incremento + la suma de los dos incrementos
        //--------------------------------------------------------------
        incOffset = incOffset + incRunner + incObjeto;
        //--------------------------------------------------------------
        // incOffset = incRunner + incObjeto;
        //--------------------------------------------------------------

        //--------------------------------------------------------------
        // Me guardo el signo:
        //--------------------------------------------------------------
        int signo = (incOffset < 0) ? -1 : 1;
        //--------------------------------------------------------------
        // Deberia ser un valor entre 0 y 1:
        //  - modificca el Offset de la textura.
        //--------------------------------------------------------------
        incOffset = Mathf.Clamp(Mathf.Abs(incOffset), 0f, 1f);
        //--------------------------------------------------------------

        // Aplico el signo:
        incOffset *= signo;
    }

    void moveTextura()
    {
        Vector2 currentOffset = myRenderer.material.mainTextureOffset;

        currentOffset.y = Mathf.Repeat(currentOffset.y + incOffset, 1f);

        //if (currentOffset.y + incOffset < 0f)
        //    currentOffset.y = 1 - (currentOffset.y + incOffset);
        //else if (currentOffset.y + incOffset > 1f)
        //    currentOffset.y = (currentOffset.y + incOffset) - 1;
        //else
        //    currentOffset.y += incOffset;

        myRenderer.material.mainTextureOffset = currentOffset;
    }

    //----------------------------------------------------------------------
    // Calcula el decremento
    // - se queda como un valor de friccion que se aplica siempre que
    //   el incOffset es distinto de 0.
    //----------------------------------------------------------------------
    void decInc(float deltaTime)
    {
        if (Mathf.Abs(incOffset) > 0)
        {
            float factor = Tool.calcRedFactor(runner.redFactorZ, deltaTime);
            incOffset *= factor;
            incOffset = (Mathf.Abs(incOffset) < Tool.cEpsilon) ? 0f : incOffset;
        }
    }
    //----------------------------------------------------------------------
    #endregion
}
