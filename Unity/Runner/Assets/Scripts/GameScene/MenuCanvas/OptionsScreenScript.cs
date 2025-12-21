using TauriLand.MysticRunner;
using TauriLand.Libreria;
using TMPro;
using UnityEditor.XR;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class OptionsScreenScript : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    const string sButReturn = "butReturn";
    const string sTogFx = "TogFx";
    const string sTogMusic = "TogMusic";
    const string sSliderNumTrees = "SliderNumTrees";
    const string sSliderNumGifts = "SliderNumGifts";
    const string sSliderNumKillers = "SliderNumKillers";
    const string sSliderNumHealths = "SliderNumHealths";
    //----------------------------------------------------------------------
    const string sTextoUnitSlider = "TextUnit";
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    GameScript game;
    //----------------------------------------------------------------------
    Button butReturn;
    //----------------------------------------------------------------------
    Toggle togFx;
    Toggle togMusic;

    Slider sliderNumTrees;
    TextMeshProUGUI textValorTrees;

    Slider sliderNumGifts;
    TextMeshProUGUI textValorGifts;

    Slider sliderNumKillers;
    TextMeshProUGUI textValorKillers;

    Slider sliderNumHealths;
    TextMeshProUGUI textValorHealths;
    //----------------------------------------------------------------------
    Color colorOn = Color.yellow;
    Color colorOff = Color.cyan;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        Tool.LogColor("Awake OptionsScreenScript [" + name + "]", Color.green);

        // Inicializacion del juego
        game = Names.getGame();

        butReturn = game.optionsScreen.transform.Find(sButReturn).GetComponent<Button>();

        togFx = game.optionsScreen.transform.Find(sTogFx).GetComponent<Toggle>();
        togMusic = game.optionsScreen.transform.Find(sTogMusic).GetComponent<Toggle>();

        sliderNumTrees = game.optionsScreen.transform.Find(sSliderNumTrees).GetComponent<Slider>();
        textValorTrees = sliderNumTrees.transform.Find(sTextoUnitSlider).GetComponent<TextMeshProUGUI>();

        sliderNumGifts = game.optionsScreen.transform.Find(sSliderNumGifts).GetComponent<Slider>();
        textValorGifts = sliderNumGifts.transform.Find(sTextoUnitSlider).GetComponent<TextMeshProUGUI>();

        sliderNumKillers = game.optionsScreen.transform.Find(sSliderNumKillers).GetComponent<Slider>();
        textValorKillers = sliderNumKillers.transform.Find(sTextoUnitSlider).GetComponent<TextMeshProUGUI>();

        sliderNumHealths = game.optionsScreen.transform.Find(sSliderNumHealths).GetComponent<Slider>();
        textValorHealths = sliderNumHealths.transform.Find(sTextoUnitSlider).GetComponent<TextMeshProUGUI>();

        sliderNumTrees.minValue = 1;
        sliderNumTrees.maxValue = GameManager.numMaxOfTrees;

        sliderNumGifts.minValue = 1;
        sliderNumGifts.maxValue = GameManager.numMaxOfGifts;

        sliderNumKillers.minValue = 1;
        sliderNumKillers.maxValue = GameManager.numMaxOfKillers;

        sliderNumHealths.minValue = 1;
        sliderNumHealths.maxValue = GameManager.numMaxOfHeallths;

        butReturn.onClick.AddListener(OnReturn);
        togFx.onValueChanged.AddListener(delegate { OnFxToggle(); });
        togMusic.onValueChanged.AddListener(delegate { OnMusicToggle(); });
        sliderNumTrees.onValueChanged.AddListener((value) => { OnNumTreesSlider(); });
        sliderNumGifts.onValueChanged.AddListener((value) => { OnNumGiftsSlider(); });
        sliderNumKillers.onValueChanged.AddListener((value) => { OnNumKillersSlider(); });
        sliderNumHealths.onValueChanged.AddListener((value) => { OnNumHealthsSlider(); });
    }

    void OnEnable()
    {
        togFx.isOn = PlaySound.isActiveSounds;
        togMusic.isOn = PlaySound.isActiveBakcgroundSound;
        sliderNumTrees.value = GameManager.numOfTrees;
        sliderNumGifts.value = GameManager.numOfGifts;
        sliderNumKillers.value = GameManager.numOfKillers;
        sliderNumHealths.value = GameManager.numOfHealths;

        OnFxToggle();
        OnMusicToggle();
        OnNumTreesSlider();
        OnNumGiftsSlider();
        OnNumKillersSlider();
        OnNumHealthsSlider();
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.isPressed && !Tool.isRepeatedKey)
        {
            Tool.setTeclaRepetida();
            OnReturn();
        }
    }
    //----------------------------------------------------------------------
    #endregion


    #region Metodos Propios
    /*--------------------------------------------------------------------*\
    |* Metodos / Funciones Propias
    \*--------------------------------------------------------------------*/
    //----------------------------------------------------------------------
    // Return
    //----------------------------------------------------------------------
    public void OnReturn()
    {
        PlaySound.isActiveSounds = togFx.isOn;
        PlaySound.isActiveBakcgroundSound = togMusic.isOn;
        GameManager.numOfTrees = (int)sliderNumTrees.value;
        GameManager.numOfGifts = (int)sliderNumGifts.value;
        GameManager.numOfKillers = (int)sliderNumKillers.value;
        GameManager.numOfHealths = (int)sliderNumHealths.value;
        game.toMenuScreen();
    }

    //----------------------------------------------------------------------
    // Toggle
    //----------------------------------------------------------------------
    public void OnFxToggle()
    {
        togFx.GetComponentInChildren<Text>().color = (togFx.isOn) ? colorOn : colorOff;
    }

    public void OnMusicToggle()
    {
        togMusic.GetComponentInChildren<Text>().color = (togMusic.isOn) ? colorOn : colorOff;
    }

    //----------------------------------------------------------------------
    // Slider
    //----------------------------------------------------------------------
    public void OnNumTreesSlider()
    {
        GameManager.numOfTrees = (int)sliderNumTrees.value;
        textValorTrees.text = GameManager.numOfTrees.ToString() + ((GameManager.numOfTrees > 1) ? " trees" : " tree");
    }

    public void OnNumGiftsSlider()
    {
        GameManager.numOfGifts = (int)sliderNumGifts.value;
        textValorGifts.text = GameManager.numOfGifts.ToString() + ((GameManager.numOfGifts > 1) ? " gifts" : " gift");
    }

    public void OnNumKillersSlider()
    {
        GameManager.numOfKillers = (int)sliderNumKillers.value;
        textValorKillers.text = GameManager.numOfKillers.ToString() + ((GameManager.numOfKillers > 1) ? " Killers" : " Killer");
    }

    public void OnNumHealthsSlider()
    {
        GameManager.numOfHealths = (int)sliderNumHealths.value;
        textValorHealths.text = GameManager.numOfHealths.ToString() + ((GameManager.numOfHealths > 1) ? " Healths" : " Health");
    }
    //----------------------------------------------------------------------
    #endregion
}
