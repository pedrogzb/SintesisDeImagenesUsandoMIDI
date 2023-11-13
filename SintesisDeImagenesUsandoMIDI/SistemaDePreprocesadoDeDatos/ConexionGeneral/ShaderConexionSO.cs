using OpcionesConexionPreprocesado;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ShaderConexion", menuName = "ScriptableObjects/ShaderConexionSO")]

public class ShaderConexionSO : ScriptableObject
{
    [SerializeField] private bool ConOctavas;
    [SerializeField][Range(0.01f,1)] private float RapidezTransicion = 0.01f;
    [SerializeField] List<string> nombresDeSalida;
    [SerializeField] public List<TipoSalidaShader> tipoSalida;
    [SerializeField] public List<ZonaEjecucion> zonaEjecucion;
    [SerializeField] List<OpcionesDeCalculo> opcionesDeCalculo;
    [SerializeField] public List<OpcionesDeRemapShader> opcionesDeRemap;
    [SerializeField] public List<Vector4> remap;
    [HideInInspector]public int Count;
    //Variables de calculo
    private float Max;//pareceHecho
    private float MaxPorFrame;
    private int[] auxMaxMin;
    private int indiceMax;
    private float Min;
    private float MinPorFrame;
    private int indiceMin;
    private float Media;
    private float MediaPorFrame;
    private float UltimaPulsada;
    private float UltimaPulsadaPorFrame;
    private float UltimaDesPulsada;
    private float UltimaDesPulsadaPorFrame;

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
        gestionarMaxMin(notaOn.x);
        Max = indiceMax;
        Min = indiceMin;
    }
    public void ActualizarEnNotaOff(Vector3Int notaOff)=> UltimaDesPulsada = (ConOctavas)?(notaOff.x+12* (notaOff.y+1)): notaOff.x;
    public string getNombre(int num) => nombresDeSalida[num];
    public Vector4 getValor(int num,bool PorFrame) => remapear((PorFrame) ? variableCalculoPorFrame(num) : variableCalculo(num), num);

    private float variableCalculoPorFrame(int num) 
    {
        return opcionesDeCalculo[num] switch
        {
            OpcionesDeCalculo.Media => MediaPorFrame = Mathf.Lerp(MediaPorFrame, Media, RapidezTransicion),
            OpcionesDeCalculo.Max => MaxPorFrame = Mathf.Lerp(MaxPorFrame, Min, RapidezTransicion),
            OpcionesDeCalculo.Min => MinPorFrame = Mathf.Lerp(MinPorFrame,Min, RapidezTransicion),
            OpcionesDeCalculo.UltimaPulsada => UltimaPulsadaPorFrame = Mathf.Lerp(UltimaPulsadaPorFrame, UltimaPulsada, RapidezTransicion),
            OpcionesDeCalculo.UltimaDesPulsada => UltimaDesPulsadaPorFrame = Mathf.Lerp(UltimaDesPulsadaPorFrame, UltimaDesPulsada, RapidezTransicion),
            _ => -1,
        };
    }
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
    private Vector4 remapear(float numero, int num) 
    {
        Vector4 vec = remap[num];
        return opcionesDeRemap[num] switch
        { 
            OpcionesDeRemapShader.ModuloReescalado  => new Vector4(((numero + vec.x) % vec.y + vec.z) * vec.w,0),
            OpcionesDeRemapShader.ClampReescalado   => new Vector4((Mathf.Min(Mathf.Max(numero,vec.x),vec.y)*vec.z+vec.w),0),
            OpcionesDeRemapShader.ReescaladoClamp   => new Vector4((Mathf.Min(Mathf.Max(numero * vec.z + vec.w, vec.x), vec.y)),0),
            OpcionesDeRemapShader.Vector2Reescalado => new Vector4(numero*vec.x+vec.y, numero * vec.z + vec.w),
            OpcionesDeRemapShader.Vector3Reescalado => new Vector4(numero *vec.x + vec.y, numero * vec.x + vec.z, numero * vec.x + vec.w),
            OpcionesDeRemapShader.Vector4Reescalado => new Vector4(numero * vec.x, numero * vec.y, numero * vec.z, numero * vec.w),
            _ => new Vector4(-1, -1, -1, -1),
        };
    }
    private void gestionarMaxMin(int valor) 
    {
        auxMaxMin[valor]++;
        if (indiceMax == -1) { indiceMax = valor; indiceMin = valor; return;}
        for(int i=0; i < 12; i++) 
        {
            if (auxMaxMin[indiceMax] < auxMaxMin[i]) 
                indiceMax = i;
            if (auxMaxMin[i]!=0 && auxMaxMin[indiceMin] > auxMaxMin[i])
                indiceMin = i;
        }
        //Debug.Log($"El valor del array es " +
        //    $"[{auxMaxMin[0]},{auxMaxMin[1]},{auxMaxMin[2]},{auxMaxMin[3]}," +
        //    $" {auxMaxMin[4]},{auxMaxMin[5]},{auxMaxMin[6]},{auxMaxMin[7]}," +
        //    $" {auxMaxMin[8]},{auxMaxMin[9]},{auxMaxMin[10]},{auxMaxMin[11]}]");
        //Debug.Log($"El indice Min:[{indiceMin}] y el indice Max:[{indiceMax}]");
    }
    
}
