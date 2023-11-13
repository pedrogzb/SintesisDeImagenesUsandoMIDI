using UnityEngine;
public class Bypass : MonoBehaviour, IConexionFilterPreprocesado
{
    [SerializeField] private ReferenciasNotasOctavasSO _referencias;
    private MaterialPropertyBlock _propertyBlock;
    private Renderer _renderer;

    void Awake()
    {
        _renderer = gameObject.GetComponent<Renderer>();
        _propertyBlock = new MaterialPropertyBlock();
    }
    public void NotaPulsada(Vector3Int pulsacion)
    {
        string _referenciaNota = _referencias.getReferenciaNota(pulsacion.x);
        string _referenciaOctava = _referencias.getReferenciaOctava(pulsacion.y);

        _propertyBlock.SetFloat(_referenciaNota, 1);
        _propertyBlock.SetFloat(_referenciaOctava, _propertyBlock.GetFloat(_referenciaOctava) + 1);
        _renderer.SetPropertyBlock(_propertyBlock);
    }
    public void NotaDesPulsada(Vector3Int pulsacion) 
    {
        string _referenciaNota = _referencias.getReferenciaNota(pulsacion.x);
        string _referenciaOctava = _referencias.getReferenciaOctava(pulsacion.y);

        _propertyBlock.SetFloat(_referenciaNota, 0);
        _propertyBlock.SetFloat(_referenciaOctava, _propertyBlock.GetFloat(_referenciaOctava) - 1);
        _renderer.SetPropertyBlock(_propertyBlock);
    }
}