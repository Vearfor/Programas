using System.ComponentModel;
using TMPro;
using UnityEngine;

public class GameRecordLine : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Constantes
    //----------------------------------------------------------------------
    const string sTextItems = "Items";
    const string sTectDistance = "Distance";
    const string sTextTime = "Time";
    const string sTextDate = "Date";
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Variables
    //----------------------------------------------------------------------
    [HideInInspector] public TextMeshProUGUI textItems;
    [HideInInspector] public TextMeshProUGUI textDistance;
    [HideInInspector] public TextMeshProUGUI textTime;
    [HideInInspector] public TextMeshProUGUI textDate;
    //----------------------------------------------------------------------
    #endregion


    #region MonoBehaviour
    /*====================================================================*\
    |* Funciones MonoBehaviour
    |* - pondremos las funciones en el orden en que son llamadas
    \*====================================================================*/
    void Awake()
    {
        textItems = transform.Find(sTextItems).GetComponent<TextMeshProUGUI>();
        textDistance = transform.Find(sTectDistance).GetComponent<TextMeshProUGUI>();
        textTime = transform.Find(sTextTime).GetComponent<TextMeshProUGUI>();
        textDate = transform.Find(sTextDate).GetComponent<TextMeshProUGUI>();

        textItems.gameObject.SetActive(true);
        textDistance.gameObject.SetActive(true);
        textTime.gameObject.SetActive(true);
        textDate.gameObject.SetActive(true);
    }
    //----------------------------------------------------------------------
    #endregion
}
