using UnityEngine;
using System.Collections;
using System.Collections.Generic;
//[RequireComponent(typeof(InputFilter))]
public class FadeOut : MonoBehaviour, IConexionFilterPreprocesado
{
    [SerializeField] private ReferenciasNotasOctavasSO _referencias;
    [SerializeField] float _segundosFadeOut;
    private const float NUMERO_DE_PASOS = 10f;
    private readonly bool[] flagNotas = new bool[12];
    private readonly bool[] flagOctavas = new bool[11];
    private int[] _pulsacionesOctava = {0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0};
    private MaterialPropertyBlock _propertyBlock;
    private Renderer _renderer;
    
    void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }
    public void NotaPulsada(Vector3Int pulsacion)
    {
        int _indiceNota = pulsacion.x;
        int _indiceOctava = pulsacion.y + 1;

        flagNotas[_indiceNota] = true;
        flagOctavas[_indiceOctava] = true;

        _propertyBlock.SetFloat(_referencias.getReferenciaNota(pulsacion.x), 1);
        _propertyBlock.SetFloat(_referencias.getReferenciaOctava(pulsacion.y), 1);

        _pulsacionesOctava[_indiceOctava]++;
        //Debug.Log(string.Format("Pulsación -> Octava: {0};   Valor: {1}", pulsacion.y, _pulsacionesOctava[_indiceOctava]));
        _renderer.SetPropertyBlock(_propertyBlock);
    }
    public void NotaDesPulsada(Vector3Int pulsacion) 
    {
        int _indiceNota = pulsacion.x;
        int _indiceOctava = pulsacion.y + 1;

        flagNotas[_indiceNota] = false;
        StartCoroutine(FadeNota(_indiceNota));
        
        _pulsacionesOctava[_indiceOctava]--;
        //Debug.Log(string.Format("Despulsación -> Octava: {0};   Valor: {1}", pulsacion.y, _pulsacionesOctava[_indiceOctava]));
        if (_pulsacionesOctava[_indiceOctava] < 1){
            flagOctavas[_indiceOctava] = false;
            StartCoroutine(FadeOctava(_indiceOctava));
        }
         
    }

    IEnumerator FadeNota( int _indiceNota)
    {
        string _referenciaNota = _referencias.getReferenciaNota(_indiceNota);
        for (float i = 0; i < NUMERO_DE_PASOS; i++)
        {
            if (flagNotas[_indiceNota])break; 
            _propertyBlock.SetFloat(_referenciaNota, _propertyBlock.GetFloat(_referenciaNota) - (1 / NUMERO_DE_PASOS));
            _renderer.SetPropertyBlock(_propertyBlock);
            yield return new WaitForSeconds(_segundosFadeOut / NUMERO_DE_PASOS);
        }
    }
    IEnumerator FadeOctava(int _indiceOctava)
    {
        string _referenciaOctava = _referencias.getReferenciaOctava(_indiceOctava-1);
        for (int i = 0; i < NUMERO_DE_PASOS; i++)
        {
            if (flagNotas[_indiceOctava])break;
            _propertyBlock.SetFloat(_referenciaOctava, _propertyBlock.GetFloat(_referenciaOctava) - 1 / NUMERO_DE_PASOS);
            _renderer.SetPropertyBlock(_propertyBlock);
            yield return new WaitForSeconds(_segundosFadeOut / NUMERO_DE_PASOS);
        }
    }
}