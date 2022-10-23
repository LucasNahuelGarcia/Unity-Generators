using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshRenderer), typeof(MeshFilter), typeof(MeshCollider))]
public class RockGeneratorVoronoi : MonoBehaviour, Generator
{
    public bool updateOnChange = false;
    public RockVoronoiShapeSettings shapeSettings;
    private MeshBuilder meshBuilder;
    private Noise noiseGen;
    private Vector3 spin;

    private MeshFilter _meshFilter;
    private MeshRenderer _meshRenderer;
    public MeshCollider _meshCollider { get; private set; }

    public void Generar()
    {
        meshBuilder = new MeshBuilder();
        noiseGen = noiseGen ?? new Noise();
        _meshFilter = _meshFilter ?? GetComponent<MeshFilter>();
        _meshRenderer = _meshRenderer ?? GetComponent<MeshRenderer>();
        _meshCollider = _meshCollider ?? GetComponent<MeshCollider>();

        buildMesh();
    }

    public void buildMesh()
    {
        MarchingCubes marching = new MarchingCubes(
        shapeSettings.densidadCubos,
        meshBuilder,
        shapeSettings.treshold,
        CalcularValorParaPunto
        );

        float radio = shapeSettings.radio;
        Vector3 inicioArea = new Vector3(-radio, -radio, -radio);
        Vector3 finArea = new Vector3(radio, radio, radio);

        marching.ProcesarArea(inicioArea, finArea);

        Mesh meshResultado = meshBuilder.CreateMesh();
        _meshFilter.sharedMesh = meshResultado;
        _meshCollider.sharedMesh = meshResultado;
    }

    private float CalcularValorParaPunto(Vector3 punto)
    {
        float val = 0;
        float radio = shapeSettings.radio;
        float limiteInterior = radio * shapeSettings.porcentajeLimiteInterior;
        float factorSuavizado = shapeSettings.factorSuavizado;

        if (shapeSettings.noiseSuperficie)
            radio = CalcularNoiseSuperficie(punto, radio);

        if (punto.magnitude < limiteInterior)
            val = Mathf.InverseLerp(limiteInterior, limiteInterior - limiteInterior * factorSuavizado, punto.magnitude);
        else if (punto.magnitude < radio)
            val = Mathf.InverseLerp(-1, 1, noiseGen.Evaluate(punto * shapeSettings.escalaVoronoi + shapeSettings.shiftVoronoi));
        else if(noiseGen.Evaluate(punto * shapeSettings.escalaVoronoi + shapeSettings.shiftVoronoi) > shapeSettings.treshold)
        {
            float distanciaARadio = punto.magnitude - radio;
            val = Mathf.InverseLerp(1, 0, distanciaARadio);
        }

        return val;
    }

    private float CalcularNoiseSuperficie(Vector3 punto, float noiseRadio)
    {
        Vector3 puntoRadio = punto * shapeSettings.esacalaNoiseSuperficie + shapeSettings.shiftNoiseSuperficie;
        noiseRadio += noiseGen.Evaluate(puntoRadio) * shapeSettings.fuerzaNoiseSuperficie - 1;
        noiseRadio = Mathf.Clamp(noiseRadio, 0, shapeSettings.radio);
        return noiseRadio;
    }

    void Update()
    {
        this.transform.Rotate(spin * Time.deltaTime);
    }

    public void SetCalidadMesh(int cant)
    {

    }
}
