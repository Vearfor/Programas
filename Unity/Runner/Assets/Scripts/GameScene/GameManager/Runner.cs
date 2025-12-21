using TauriLand.Libreria;
using TauriLand.MysticRunner;
using TauriLand.Tps.Character;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class Runner : MonoBehaviour
{
    #region Camara
    //----------------------------------------------------------------------
    // Valores de la camara TP
    // Camara Normal
    // - pos  ( 0, 3, -3)
    // - rot  (20, 0,  0)
    // - esc  ( 1, 1,  1)
    // cThirdPersonCamara : con los mismos valores
    // - esc  ( 0, 0,  0)  cambia, supongo que con esto puede estar.
    //----------------------------------------------------------------------
    #endregion


    #region Variables y Constantes
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    // Limites X e Y de la pantalla
    // - La Y dependera de donde pongamos la camara, donde ponemos el suelo
    //----------------------------------------------------------------------
    // Camara posicion: 0, 0, -10 (Hacia atras de profundidad)
    // Asi el personaje ahora esta en el 0,0,0.
    // - probaremos y ya veremos.
    //----------------------------------------------------------------------
    float limLeft = -9.5f;
    float limRight = 9.5f;
    //----------------------------------------------------------------------
    float limTop = 4.5f;
    //----------------------------------------------------------------------
    // Y un suelo a las -4.5 o por ahi.
    //----------------------------------------------------------------------
    // Barra de Vida
    //----------------------------------------------------------------------
    float maxLifeBar = 100;
    float currentLife = 100;
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Propias del Runner
    //----------------------------------------------------------------------
    [Header("Impulsos:")]
    public float speedX = 10f;
    public float speedY = 40.0f;
    public float speedZ = 1f;       // simulando que avanza aunque Z sea 0
    public float speedText = 0.4f;  // Speed de textura
    //----------------------------------------------------------------------
    [Header("Factores de Reduccion:")]
    public float redFactorX = 3f;
    public float redFactorY = 5f;
    public float redFactorZ = 2f;
    //----------------------------------------------------------------------
    [Header("Incrementos por cada Frame:")]
    [SerializeField] float incX = 0f;           // speedX
    [SerializeField] float incY = 0f;           // speedY
    [SerializeField] float simIncZ = 0f;        // speedZ
    //----------------------------------------------------------------------
    [Header("Booleanos:")]
    public int hayRunning = 0;
    //----------------------------------------------------------------------
    [Header("Para pruebas:")]
    public bool isInmortal = false;
    public bool isActiveGoBack = false;
    //----------------------------------------------------------------------

    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    GameScript game;
    GameManager gameManager;
    [HideInInspector] public GameRecord gameRecord;
    [HideInInspector] public LifeBar lifeMarker;
    [HideInInspector] public TextMeshProUGUI textItemsMarker;
    [HideInInspector] public TextMeshProUGUI textTimeMarker;
    //----------------------------------------------------------------------
    // Controlador de la animacion
    //----------------------------------------------------------------------
    cMyPersonAnimator myAnimator;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        Tool.LogColor("Awake  Runner  [" + name + "]", Color.green);

        initAnimator();
    }

    void OnEnable()
    {
        Tool.LogColor("OnEnable  Runner  position: [" + transform.position.ToString() + "]", Color.green);
        inicio();
    }

    void Update()
    {
        if (!GameManager.IsInitGame || GameManager.IsPausa || GameManager.IsGameOver)
            return;

        float deltaTime = Time.deltaTime;

        hayRunning = 0;

        incTime(deltaTime);

        inputEjeX(deltaTime);
        inputEjeY(deltaTime);
        inputEjeZ(deltaTime);

        checkLimit();
        move();
        decSpeed(deltaTime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        string sTag = collision.gameObject.tag;
        if (sTag == Constants.sTagSuelo)
            makeToGround();
        else
        if (sTag == Constants.sTagHasDamage)
        {
            makeDamageCollision(collision);
        }
        else
        if (sTag == Constants.sGift)
        {
            makeGiftCollision(collision);
        }
        else
        if (sTag == Constants.sHealth)
        {
            makeHealthCollision(collision);
        }
        else
        {
            Tool.LogColor(string.Format(" Colision de objetos con tag no registrado: {0}:{1}", sTag, collision.gameObject.name), Color.yellow);
        }
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    public void inicio()
    {
        currentLife = maxLifeBar;
        gameRecord = new GameRecord();
        gameRecord.reset();
        game = Names.getGame(); 
        gameManager = Names.getGameManager(null, game);  
        if (game.gameScreen)
        {
            lifeMarker = game.gameScreen.GetComponent<GameScreenScript>().lifeMarker;
            textItemsMarker = game.gameScreen.GetComponent<GameScreenScript>().textItemsMarker;
            textTimeMarker = game.gameScreen.GetComponent<GameScreenScript>().textTimeMarker;
        }

        // Icrementos a cero:
        incX = 0f;           // speedX
        incY = 0f;           // speedY
        simIncZ = 0f;        // speedZ

        initAnimator();

        currentLife = maxLifeBar;
        updateLifeBar();
        updateItemsMarker();
    }

    public void initAnimator()
    {
        if (myAnimator==null)
        {
            myAnimator = GetComponent<cMyPersonAnimator>();
            if (myAnimator)
                myAnimator.ReInit();
        }

        myAnimator.reset();
    }


    #region EjeX
    //----------------------------------------------------------------------
    void inputEjeX(float deltaTime)
    {
        //------------------------------------------------------------------
        // Eje X
        //------------------------------------------------------------------
        if (Keyboard.current.leftArrowKey.isPressed || Keyboard.current.aKey.isPressed)
        {
            calcIncX(-1, deltaTime);
        }

        if (Keyboard.current.rightArrowKey.isPressed || Keyboard.current.dKey.isPressed)
        {
            calcIncX(+1, deltaTime);
        }
    }

    void calcIncX(int sentidoX, float deltaTime)
    {
        // Se ha pulsado tecla:
        incX = speedX * deltaTime * (float)sentidoX;

        myAnimator.run();
    }
    //----------------------------------------------------------------------
    #endregion


    #region EjeY
    //----------------------------------------------------------------------
    void inputEjeY(float deltaTime)
    {
        //------------------------------------------------------------------
        // Eje Y
        //------------------------------------------------------------------
        if (Keyboard.current.spaceKey.isPressed)
        {
            calcJump(deltaTime);
        }
    }

    void calcJump(float deltaTime)
    {
        // Si no estamos saltando, saltamos.
        if (!myAnimator.isJumping)
        {
            incY = speedY * deltaTime;
            myAnimator.jump();
        }
    }
    //----------------------------------------------------------------------
    #endregion


    #region EjeZ
    //----------------------------------------------------------------------
    void inputEjeZ(float deltaTime)
    {
        //------------------------------------------------------------------
        // Eje Z
        //------------------------------------------------------------------
        if (Keyboard.current.upArrowKey.isPressed || Keyboard.current.wKey.isPressed)
        {
            calcAvanceZ(-1, deltaTime);
        }

        if (isActiveGoBack)
        {
            if (Keyboard.current.downArrowKey.isPressed || Keyboard.current.sKey.isPressed)
            {
                calcAvanceZ(1, deltaTime);
            }
        }
    }

    void calcAvanceZ(int pRunnig, float deltaTime)
    {
        hayRunning = pRunnig;
        if (hayRunning != 0)
        {
            //--------------------------------------------------------------
            // hayRunning = -1;    // Avanzar parece ser -1.
            // hayRunning = 1;    // Retroceder parece ser 1.
            //--------------------------------------------------------------
            // El incremento de texturas ya se hace en ScroolTexture
            // El runner nada
            //--------------------------------------------------------------
            // Incremento simulado de Z, si
            //--------------------------------------------------------------
            simIncZ = deltaTime * speedZ;
            //--------------------------------------------------------------
            // Y con esto damos el sentido de si avanzamos o retrocedemos.
            // -1 era avanzar, y 1 retoceder.
            //--------------------------------------------------------------
            int runnigSentido = (-1) * hayRunning;
            simIncZ *= runnigSentido;

            myAnimator.run();
        }
    }
    //----------------------------------------------------------------------
    #endregion


    #region Check Limits, Move y DecSpeed
    //----------------------------------------------------------------------
    // Comprobamos los limites
    //----------------------------------------------------------------------
    void checkLimit()
    {
        Vector3 pos = transform.position;
        // Calculados los incrementos comprobamos si con ellos nos salimos
        // de los limites:
        // - si eso ocurre los incrementos los hacemos 0.

        // EjeX:
        if (pos.x + incX < limLeft)
        {
            // O mejor la diferencia para que se queden pegaditos.
            incX = limLeft - pos.x;
        }
        if (pos.x + incX > limRight)
        {
            // O mejor la diferencia para que se queden pegaditos.
            incX = limRight - pos.x;
        }

        // EjeY: idem:
        if (pos.y + incY > limTop)
        {
            incY = limTop - pos.y;
        }
    }

    void move()
    {
        Vector3 pos = transform.position;
        pos.x += incX;
        pos.y += incY;
        transform.position = pos;

        gameRecord.distance += simIncZ;
    }

    void decSpeed(float deltaTime)
    {
        // Por aplicar el deltaTime el delta es bastante pequenio
        controlStop(simIncZ , .00005f);
        //controlStop(incX    , .00005f);

        if (Mathf.Abs(incX) > 0)
        {
            float factor = Tool.calcRedFactor(redFactorX, deltaTime);
            incX *= factor;
            incX = (Mathf.Abs(incX) < Tool.cEpsilon) ? 0f : incX;
        }

        if (Mathf.Abs(incY) > 0)
        {
            float factor = Tool.calcRedFactor(redFactorY, deltaTime);
            incY *= factor;
            incY = (Mathf.Abs(incY) < Tool.cEpsilon) ? 0f : incY;
        }

        if (Mathf.Abs(simIncZ) > 0)
        {
            float factor = Tool.calcRedFactor(redFactorZ, deltaTime);
            simIncZ *= factor;
            simIncZ = (Mathf.Abs(simIncZ) < Tool.cEpsilon) ? 0f : simIncZ;
        }
    }

    void controlStop(float inc, float delta)
    {
        //------------------------------------------------------------------
        // Pasamos de animacion de correr a animacion de caminar o parar
        //------------------------------------------------------------------
        if (Mathf.Abs(inc) > 0)
        {
            if (Mathf.Abs(inc) < delta)
            {
                myAnimator.walk();
            }
        }
        else
        {
            myAnimator.stop();
        }
        //------------------------------------------------------------------
    }
    //----------------------------------------------------------------------
    #endregion


    #region Marcadores: Barra Vida, Items, Tiempo
    //----------------------------------------------------------------------
    // Incrementamos el tiempo de juego
    //----------------------------------------------------------------------
    void incTime(float deltaTime)
    {
        gameRecord.secondsOfPlay += deltaTime;

        if (textTimeMarker)
            textTimeMarker.text = Tool.getSecondsTimeString(gameRecord.secondsOfPlay);
    }

    void updateLifeBar()
    {
        if (lifeMarker)
            lifeMarker.setMarcador(currentLife, 0f, maxLifeBar);
    }

    void updateItemsMarker()
    {
        if (textItemsMarker)
            textItemsMarker.text = gameRecord.itemsReached.ToString();
    }
    //----------------------------------------------------------------------
    #endregion


    #region Gestion Colisiones
    //----------------------------------------------------------------------
    void makeToGround()
    {
        myAnimator.stopJump();
    }

    void makeDamageCollision(Collision collision)
    {
        // el Damage se saca del collosion;
        DamageObjectController objectDamage = collision.gameObject.GetComponent<DamageObjectController>();
        if (objectDamage)
        {
            PlaySound.PlayFxClip((int)Sounds.smash);
            float damage = objectDamage.damage;
            gameManager.destroyObjectController(objectDamage, "Damage collision");
            if (!isInmortal)
            {
                currentLife -= objectDamage.damage;
                currentLife = (currentLife < 0) ? 0 : currentLife;
                updateLifeBar();
            }
            // La partida se ha terminado
            if (currentLife == 0f)
                game.OnEndScreen();
        }
        else
        {
            Tool.LogColor(
                "Colisionamos con un objeto con Tag \"" + Constants.sTagHasDamage + "\" sin DamageObjectController: [" +
                collision.gameObject.name
                + "]", Color.yellow);
        }
    }

    void makeGiftCollision(Collision collision)
    {
        // El Gift se saca de la colision
        GiftObjectController giftObject = collision.gameObject.GetComponent<GiftObjectController>();
        if (giftObject)
        {
            PlaySound.PlayFxClip((int)Sounds.gift);
            gameRecord.itemsReached += giftObject.giftItem;
            updateItemsMarker();
            gameManager.destroyObjectController(giftObject, "Gift collision");
        }
        else
        {
            Tool.LogColor(
                "Colisionamos con un objeto con Tag \"" + Constants.sGift + "\" sin GiftObjectController: [" +
                collision.gameObject.name
                + "]", Color.yellow);
        }
    }

    void makeHealthCollision(Collision collision)
    {
        // El Health se saca de la colision
        HealthObjectController healthObject = collision.gameObject.GetComponent<HealthObjectController>();
        if (healthObject)
        {
            PlaySound.PlayFxClip((int)Sounds.health);
            currentLife += healthObject.giftLife;
            currentLife = (currentLife > maxLifeBar) ? maxLifeBar : currentLife;
            updateLifeBar();
            gameManager.destroyObjectController(healthObject, "Health collision");
        }
        else
        {
            Tool.LogColor(
                "Colisionamos con un objeto con Tag \"" + Constants.sHealth + "\" sin HealthObjectController: [" +
                collision.gameObject.name
                + "]", Color.yellow);
        }
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Publicos
    //----------------------------------------------------------------------
    // Ejercer la pausa
    //----------------------------------------------------------------------
    public void togglePause()
    {
        myAnimator.togglePause();
    }

    //----------------------------------------------------------------------
    // Detener la animacion
    //----------------------------------------------------------------------
    public void stopAnimator()
    {
        myAnimator.stop();
    }
    //----------------------------------------------------------------------
    #endregion

    //----------------------------------------------------------------------
    #endregion
}
