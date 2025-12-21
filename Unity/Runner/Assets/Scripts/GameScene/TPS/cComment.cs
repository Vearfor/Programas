using UnityEngine;

/*--------------------------------------------------------------------*\
|* Metodos / Funciones Propias
|*
|* Este codigo sale de  Invector.Utils
\*--------------------------------------------------------------------*/
namespace TauriLand.Tps.Utils
{
    public class cComment : MonoBehaviour
    {
#if UNITY_EDITOR
            [SerializeField] protected string header = "COMMENT";
            [Multiline]
            [SerializeField] protected string comment;

            [SerializeField] protected bool inEdit;

#endif
    }
}
