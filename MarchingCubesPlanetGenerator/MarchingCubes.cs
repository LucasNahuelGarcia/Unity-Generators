using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MarchingCubes
{
    public delegate bool Pertenece(Vector3 point);
    public delegate float Value(Vector3 point);
    private MeshBuilder mesh;
    private float tamañoCubos = 1;
    private float treshold;
    private float shift;
    private Vector3[] verticesCuboReferencia;
    private Value value;
    private static float epsilon = 5f;

    public MarchingCubes(float densidadCubos, MeshBuilder mesh, float treshold, Value delCalculateValue)
    {
        this.tamañoCubos = densidadCubos;
        this.mesh = mesh;
        this.treshold = treshold;
        this.value = delCalculateValue;
        CalcularCuboReferencia();
    }

    private void CalcularCuboReferencia()
    {
        shift = tamañoCubos / 2;
        verticesCuboReferencia = new Vector3[8]{
    new Vector3(-shift, -shift, -shift),//g
    new Vector3(shift, -shift, -shift),//h
    new Vector3(shift, -shift, shift),//f
    
    new Vector3(-shift, -shift, shift),//e
    new Vector3(-shift, shift, -shift),//c
    new Vector3(shift, shift, -shift),//d
    
    new Vector3(shift, shift, shift),//b
    new Vector3(-shift, shift, shift),//a
    };
    }

    public void setTamañoCubos(float tamaño)
    {
        tamañoCubos = tamaño;
        CalcularCuboReferencia();
    }

    public void ProcesarArea(Vector3 inicio, Vector3 fin)
    {
        for (float x = inicio.x; x <= fin.x; x += tamañoCubos)
            for (float y = inicio.y; y <= fin.y; y += tamañoCubos)
                for (float z = inicio.z; z <= fin.z; z += tamañoCubos)
                    ProcesarCuboConCentro(new Vector3(x, y, z));
    }

    public void ProcesarCuboConCentro(Vector3 centroCubo)
    {
        Vector3[] cubeCorners = calcularVertCuboEnPunto(centroCubo);

        int cubeIndex = calcularCubeIndex(value, cubeCorners);
        for (int i = 0; Tables.triangulation[cubeIndex][i] != -1; i += 3)
        {
            // Get indices of corner points A and B for each of the three edges
            // of the cube that need to be joined to form the triangle.
            int a0 = Tables.cornerIndexAFromEdge[Tables.triangulation[cubeIndex][i]];
            int b0 = Tables.cornerIndexBFromEdge[Tables.triangulation[cubeIndex][i]];

            int a1 = Tables.cornerIndexAFromEdge[Tables.triangulation[cubeIndex][i + 1]];
            int b1 = Tables.cornerIndexBFromEdge[Tables.triangulation[cubeIndex][i + 1]];

            int a2 = Tables.cornerIndexAFromEdge[Tables.triangulation[cubeIndex][i + 2]];
            int b2 = Tables.cornerIndexBFromEdge[Tables.triangulation[cubeIndex][i + 2]];

            Vector3 a = linearInterpolation(cubeCorners[a0], cubeCorners[b0]);
            Vector3 b = linearInterpolation(cubeCorners[a1], cubeCorners[b1]);
            Vector3 c = linearInterpolation(cubeCorners[a2], cubeCorners[b2]);

            Vector3 normal = calcularNormalTiangulo(a, b, c);

            int ia = mesh.addVertice(a);
            mesh.Normals.Add(normal);
            int ib = mesh.addVertice(b);
            mesh.Normals.Add(normal);
            int ic = mesh.addVertice(c);
            mesh.Normals.Add(normal);

            mesh.addTriangle(ia, ib, ic);
        }
    }

    private Vector3 calcularNormalTiangulo(Vector3 a, Vector3 b, Vector3 c)
    {
        Vector3 ab = a - b;
        Vector3 ac = a - c;
        return Vector3.Cross(ab, ac).normalized;
    }

    private int calcularCubeIndex(Value value, Vector3[] verticesCubo)
    {
        int cubeindex = 0;
        if (value(verticesCubo[0]) >= treshold) cubeindex |= 1;
        if (value(verticesCubo[1]) >= treshold) cubeindex |= 2;
        if (value(verticesCubo[2]) >= treshold) cubeindex |= 4;
        if (value(verticesCubo[3]) >= treshold) cubeindex |= 8;
        if (value(verticesCubo[4]) >= treshold) cubeindex |= 16;
        if (value(verticesCubo[5]) >= treshold) cubeindex |= 32;
        if (value(verticesCubo[6]) >= treshold) cubeindex |= 64;
        if (value(verticesCubo[7]) >= treshold) cubeindex |= 128;

        return cubeindex;
    }

    private Vector3[] calcularVertCuboEnPunto(Vector3 punto)
    {
        Vector3[] cuboEnPunto = new Vector3[8];
        for (int i = 0; i < 8; i++)
            cuboEnPunto[i] = verticesCuboReferencia[i] + punto;

        return cuboEnPunto;
    }

    private Vector3 linearInterpolation(Vector3 a, Vector3 b)
    {
        return a + (treshold - value(a)) * (b - a) / (value(b) - value(a));
    }
}
