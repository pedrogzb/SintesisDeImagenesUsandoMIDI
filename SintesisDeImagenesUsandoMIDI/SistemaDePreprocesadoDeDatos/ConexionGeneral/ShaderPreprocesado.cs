using OpcionesConexionPreprocesado;
using UnityEngine;
public class ShaderPreprocesado : MonoBehaviour, IConexionFilterPreprocesado
{
    [SerializeField] private Material _material;
    [SerializeField] private ShaderConexionSO _shaderConexion;
    [SerializeField] private bool PorFrame;

    private void Start()
    {
        if(!_shaderConexion.Inicializar())Debug.LogWarning("No se ha inicializado correctamente la conexión");
    }
    public void Update()
    {
        if (!PorFrame) return;
        for(int i = 0; i < _shaderConexion.Count; i++) 
        {
            selector(i,true);
        }
    }
    public void NotaPulsada(Vector3Int NoteOn)
    {
        _shaderConexion.ActualizarEnNotaOn(NoteOn);
        if (PorFrame) return;
        for (int i = 0; i < _shaderConexion.Count; i++)
        {
            if (_shaderConexion.zonaEjecucion[i] != ZonaEjecucion.EnNotaOff)
                selector(i,false);
        }
    }
    public void NotaDesPulsada(Vector3Int NoteOff)
    {
        _shaderConexion.ActualizarEnNotaOff(NoteOff);
        if (PorFrame) return;
        for (int i = 0; i < _shaderConexion.Count; i++)
        {
            if (_shaderConexion.zonaEjecucion[i] != ZonaEjecucion.EnNotaOn)
                selector(i,false);
        }
    }
    private void selector(int num,bool Frame) 
    {
        switch (_shaderConexion.tipoSalida[num])
        {
            case TipoSalidaShader.Vector:
                Vector4 actual = _material.GetVector(_shaderConexion.getNombre(num));
                Vector4 nuevo = _shaderConexion.getValor(num, Frame);
                Vector4 resultado = new Vector4(nuevo.x, nuevo.y, (nuevo.z == 0) ? actual.z : nuevo.z, (nuevo.w == 0) ? actual.w : nuevo.w);
                _material.SetVector(_shaderConexion.getNombre(num), resultado);
                break;
            case TipoSalidaShader.Float:
                _material.SetFloat(_shaderConexion.getNombre(num), _shaderConexion.getValor(num,Frame).x);
                break;
            case TipoSalidaShader.Int:

                _material.SetInteger(_shaderConexion.getNombre(num), (int)_shaderConexion.getValor(num,Frame).x);
                break;
            case TipoSalidaShader.Color:
                actual = _material.GetColor(_shaderConexion.getNombre(num));
                nuevo = _shaderConexion.getValor(num, Frame);
                resultado = new Vector4(nuevo.x, nuevo.y, (nuevo.z == 0) ? actual.z : nuevo.z, (nuevo.w == 0) ? actual.w : nuevo.w);
                _material.SetColor(_shaderConexion.getNombre(num),new Color(resultado.x, resultado.y, resultado.z,resultado.w));
                break;
        }
    }
}
