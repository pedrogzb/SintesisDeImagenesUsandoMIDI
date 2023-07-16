using OpcionesConexionPreprocesado;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AnimatorConexion", menuName = "ScriptableObjects/AnimatorConexionSO")]

public class AnimatorConexionSO : ScriptableObject
{
    [SerializeField] private bool ConOctavas;
    [SerializeField] List<string> nombresDeSalida;
    [SerializeField] public List<TipoSalidaAnimator> tipoSalida;
    [SerializeField] public List<ZonaEjecucion> zonaEjecucion;
    [SerializeField] List<OpcionesDeCalculo> opcionesDeCalculo;
    [SerializeField] public List<OpcionesDeRemapAnimator> opcionesDeRemap;
    [SerializeField] public List<Vector4> remap;
    [HideInInspector]public int Count;
    //Variables de calculo
    private int[] auxMaxMin;
    private float Max;
    private int indiceMax;
    private float Min;
    private int indiceMin;
    private float Media;//TODO
    private float UltimaPulsada;//TODO
    private float UltimaDesPulsada;//TODO
    
    public bool Inicializar() 
    {
        Media = 0;
        auxMaxMin = new int[12]{0,0,0,0,0,0,0,0,0,0,0,0};
        indiceMax = -1;
        indiceMin = -1;
        Count = nombresDeSalida.Count;
        return (Count == opcionesDeCalculo.Count) && 
               (Count == opcionesDeRemap.Count  ) &&
               (Count == zonaEjecucion.Count    ) &&
               (Count == tipoSalida.Count       ) &&
               (Count == remap.Count) ;
    }
    public void ActualizarEnNotaOn(Vector3Int notaOn) 
    {
        int valorNota = (ConOctavas) ? (notaOn.x + 12 * (notaOn.y + 1)) : notaOn.x;
        UltimaPulsada = valorNota;
        Media = (Media==0)? valorNota:((valorNota + Media) / 2);
        gestionarMax(notaOn.x);
        Max = indiceMax;
        Min = indiceMin;
    }
    public void ActualizarEnNotaOff(Vector3Int notaOff)=> UltimaDesPulsada = (ConOctavas)?(notaOff.x+12* (notaOff.y+1)): notaOff.x;
    public string getNombre(int num) => nombresDeSalida[num];
    public float getValor(int num) => remapear(variableCalculo(num), num);
    private float variableCalculo(int num)
    {
        return opcionesDeCalculo[num] switch
        {
            OpcionesDeCalculo.Media => Media,
            OpcionesDeCalculo.Max => Max,
            OpcionesDeCalculo.Min => Min,
            OpcionesDeCalculo.UltimaPulsada => UltimaPulsada,
            OpcionesDeCalculo.UltimaDesPulsada => UltimaDesPulsada,
            _ => -1,
        };
    }
    private float remapear(float numero, int num) 
    {
        Vector4 vec = remap[num];
        return opcionesDeRemap[num] switch
        {
            OpcionesDeRemapAnimator.ModuloReescalado => ((numero + vec.x) % vec.y + vec.z) * vec.w,
            OpcionesDeRemapAnimator.ClampReescalado =>  (Mathf.Min(Mathf.Max(numero,vec.x),vec.y)*vec.z+vec.w),
            OpcionesDeRemapAnimator.ReescaladoClamp => (Mathf.Min(Mathf.Max(numero * vec.z + vec.w, vec.x), vec.y) ),
            _ => -1,
        };
    }
    private void gestionarMax(int valor) 
    {
        auxMaxMin[valor]++;
        if (indiceMax == -1) { indiceMax = valor; indiceMin = valor; return;}
        for(int i=0; i < 12; i++) 
        {
            if (auxMaxMin[indiceMax] < auxMaxMin[i]) 
            {
                indiceMax = i;
                //valorMax = auxMaxMin[i];
            }
            if (auxMaxMin[i]!=0 && auxMaxMin[indiceMin] > auxMaxMin[i])
            {
                indiceMin = i;
            }
        }
        //Debug.Log($"El valor del array es " +
        //    $"[{auxMaxMin[0]},{auxMaxMin[1]},{auxMaxMin[2]},{auxMaxMin[3]}," +
        //    $" {auxMaxMin[4]},{auxMaxMin[5]},{auxMaxMin[6]},{auxMaxMin[7]}," +
        //    $" {auxMaxMin[8]},{auxMaxMin[9]},{auxMaxMin[10]},{auxMaxMin[11]}]");
        //Debug.Log($"El indice Min:[{indiceMin}] y el indice Max:[{indiceMax}]");
    }
    
}
