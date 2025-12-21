using TauriLand.Libreria;
using UnityEngine;
using UnityEngine.UI;

public class LifeBar : MonoBehaviour
{
    #region Constantes
    //----------------------------------------------------------------------
    // Constantes
    //----------------------------------------------------------------------
    string sForeImage = "ForeImage";
    //----------------------------------------------------------------------
    #endregion


    #region Variables
    //----------------------------------------------------------------------
    // Vairbles
    //----------------------------------------------------------------------
    Image foreImage;
    //----------------------------------------------------------------------
    #endregion


    void Awake()
    {
        //------------------------------------------------------------------
        // Evitamos tambien un nulo por llamadas en distinto orden.
        // Parece ser que no existe en un primer momento:
        //
        //  gameObject.transform.Find(sForeImage)
        //
        //  es nulo
        //------------------------------------------------------------------
        Transform trans = gameObject.transform.Find(sForeImage);
        if (trans)
        {
            foreImage = trans.gameObject.GetComponent<Image>();
        }
        else
        {
            Tool.LogColor("Awake LifeBar [" + name + "]   Es nulo: [" + sForeImage + "]", Color.yellow);
        }
        //------------------------------------------------------------------
    }

    public void setMarcador(float amount, float min, float max)
    {
        float diff = max - min;

        foreImage.fillAmount = amount / diff;
    }
}
