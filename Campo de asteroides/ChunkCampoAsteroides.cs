using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkCampoAsteroides : MonoBehaviour
{
    public GameObject objetoAGenerar;
    public float size;

    [ContextMenu("Generar")]
    public void Generar()
    {
        GameObject generado = Instantiate(objetoAGenerar, transform.position, transform.rotation);
        generado.transform.parent = this.transform;
    }
}
