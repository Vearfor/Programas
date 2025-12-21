using UnityEngine;

namespace TauriLand.MysticRunner
{
    #region Efectos
    public enum Sounds
    {
        transicion = 0,     // Transicion de pantalla, botones, menus, etc.
        smash = 1,          // Colision con obstaculo
        gift = 2,           // Recoger regalo
        health = 3,         // Recoger vida
    }
    #endregion

    public static class Constants
    {
        public const string sProgramName = "MysticRunner";
        //----------------------------------------------------------------------
        // Main Camera
        //----------------------------------------------------------------------
        public const string sMainCamera = "Main Camera";
        //----------------------------------------------------------------------
        // Lista de Games
        //----------------------------------------------------------------------
        public const string sDirFileGames = "Files";
        public const string sNameFicFileGames = "FicGames.txt";
        //----------------------------------------------------------------------
        // Escenas del Programa
        //----------------------------------------------------------------------
        public const string sInitScene = "InitScene";
        public const string sGameScene = "GameScene";
        //----------------------------------------------------------------------
        // Canvas InitScene
        //----------------------------------------------------------------------
        public const string sMainManagerObject = "MainCanvas";
        //----------------------------------------------------------------------
        // Screen/Paneles InitScene
        //----------------------------------------------------------------------
        public const string sTitleScreen = "TitleScreen";
        public const string sGameListScreen = "GameListScreen";
        //----------------------------------------------------------------------
        // Canvas GameScene
        //----------------------------------------------------------------------
        public const string sGame = "Game";     // GameObject
        public const string sGameMenuCanvas = "MenuCanvas";
        public const string sGameFondoCanvas = "FondoCanvas";
        public const string sGameGameCanvas = "GameCanvas";
        //----------------------------------------------------------------------
        // Screen/Paneles GameScene
        //----------------------------------------------------------------------
        public const string sMenuScreen = "MenuScreen";
        public const string sOptionsScreen = "OptionsScreen";
        public const string sGameScreen = "GameScreen";
        public const string sExitScreen = "ExitScreen";         // Exit con pausa
        public const string sEndScreen = "EndScreen";           // Fin de partida + hemos perdido se supone.
        //----------------------------------------------------------------------
        // El nombre del GameObject que mantiene el GameManager
        // para sacarlo alli donde se necesite
        // - Si. Hemos creado un objeto vacio que tiene el script de
        //       GameManager (lo mas simple porfa)
        //----------------------------------------------------------------------
        public const string sGameManager = "GameManager";
        public const string sLisRunners = "Players";
        public const string sRunner = "myPlayer";
        //----------------------------------------------------------------------
        // Tags
        //----------------------------------------------------------------------
        public const string sTagSuelo = "Suelo";
        public const string sTagHasDamage = "HasDamage";
        public const string sGift = "Gift";
        public const string sHealth = "Health";
        //----------------------------------------------------------------------
    }

    public static class Names
    {
        public static GameScript getGame()
        {
            GameScript game = null;
            GameObject go = GameObject.Find(Constants.sGame);
            if (go)
                game = go.GetComponent<GameScript>();
            return game;
        }

        public static Canvas getGameCanvas(GameScript game = null)
        {
            Canvas canvas = null;
            if (!game)
                game = Names.getGame();
            if (game)
            {
                Transform trans = game.transform.Find(Constants.sGameGameCanvas);
                canvas = trans.GetComponent<Canvas>();
            }
            return canvas;
        }

        public static GameManager getGameManager(Canvas canvas = null, GameScript game = null)
        {
            GameManager gm = null;
            if (!canvas)
                canvas = Names.getGameCanvas(game);
            if (canvas)
            {
                Transform trans = canvas.transform.Find(Constants.sGameManager);
                gm = trans.gameObject.GetComponent<GameManager>();
            }
            return gm;
        }

        //------------------------------------------------------------------
        // Get Runner
        // - generado sin "elses"
        //------------------------------------------------------------------
        public static Runner getPlayer(GameManager gameManager = null, Canvas canvas = null, GameScript game = null)
        {
            Runner runner = null;
            if (!gameManager)
                gameManager = Names.getGameManager(canvas, game);

            if (!gameManager)
                return runner;

            Transform players = gameManager.transform.Find(Constants.sLisRunners);

            if (!players)
                return runner;

            Transform trans = players.Find(Constants.sRunner);

            if (!trans)
                return runner;

            runner = trans.GetComponent<Runner>();

            return runner;
        }

        //------------------------------------------------------------------
        // Main Camera
        //------------------------------------------------------------------
        public static Camera getMainCamera()
        {
            Camera camera = null;
            GameObject go = GameObject.Find(Constants.sMainCamera);
            if (go)
                camera = go.GetComponent<Camera>();
            return camera;
        }
        //----------------------------------------------------------------------

        //----------------------------------------------------------------------
        //game = GameObject.Find(Constants.sGame);
        //----------------------------------------------------------------------
        //menuCanvas = game.transform.Find(Constants.sGameMenuCanvas).GetComponent<Canvas>();
        //menuScreen = menuCanvas.transform.Find(Constants.sMenuScreen).gameObject;
        //optionsScreen = menuCanvas.transform.Find(Constants.sOptionsScreen).gameObject;
        //----------------------------------------------------------------------
        //fondoCanvas = game.transform.Find(Constants.sGameFondoCanvas).GetComponent<Canvas>();
        //----------------------------------------------------------------------
        //gameCanvas = game.transform.Find(Constants.sGameGameCanvas).GetComponent<Canvas>();
        //gameScreen = gameCanvas.transform.Find(Constants.sGameScreen).gameObject;
        //exitScreen = gameCanvas.transform.Find(Constants.sExitScreen).gameObject;
        //endScreen = gameCanvas.transform.Find(Constants.sEndScreen).gameObject;
        //----------------------------------------------------------------------
    }
}