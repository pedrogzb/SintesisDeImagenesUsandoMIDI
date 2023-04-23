using UnityEngine;
using System;
using System.Collections.Generic;
//[RequireComponent(typeof(InputFilter))]
public class Bypass : MonoBehaviour, IConexionFilterPreprocesado
{
    //[SerializeField] List<string> referenciasNotas = new List<string>();
    //[SerializeField] List<string> referenciasOctavas = new List<string>();
    [SerializeField] private ReferenciasNotasOctavasSO _referencias;
    private MaterialPropertyBlock _propertyBlock;
    [SerializeField] private Renderer _renderer;
    // Start is called before the first frame update
    void Awake()
    {
        //_renderer = gameObject.GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }
    public void MensajePrueba(string mensaje) => Debug.Log(mensaje);
    public void NotaPulsada(Vector3Int pulsacion)
    {
        Debug.Log("hola");
        //string _referenciaNota = referenciasNotas[pulsacion.x];
        //string _referenciaOctava = referenciasOctavas[pulsacion.y + 1];
        string _referenciaNota = _referencias.getReferenciaNota(pulsacion.x);
        string _referenciaOctava = _referencias.getReferenciaOctava(pulsacion.y);
        Debug.Log(_renderer == null);
        
        _propertyBlock.SetFloat(_referenciaNota, 1);
        _propertyBlock.SetFloat(_referenciaOctava, _propertyBlock.GetFloat(_referenciaOctava) + 1);
        try { _renderer.SetPropertyBlock(_propertyBlock); } catch(Exception e){ Debug.Log(e.Message); }
        
        
        Debug.Log("hola2");

    }
    public void NotaDesPulsada(Vector3Int pulsacion) 
    {
        Debug.Log("hola3");
        //string _referenciaNota = referenciasNotas[pulsacion.x];
        //string _referenciaOctava = referenciasOctavas[pulsacion.y + 1];
        string _referenciaNota = _referencias.getReferenciaNota(pulsacion.x);
        string _referenciaOctava = _referencias.getReferenciaOctava(pulsacion.y);
        _propertyBlock.SetFloat(_referenciaNota, 0);
        _propertyBlock.SetFloat(_referenciaOctava, _propertyBlock.GetFloat(_referenciaOctava) - 1);
        _renderer.SetPropertyBlock(_propertyBlock);
        Debug.Log("hola4");
    }
}