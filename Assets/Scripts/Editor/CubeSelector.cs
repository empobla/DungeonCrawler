using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

class CubeSelector : EditorWindow
{
    /// <summary>Selects all red cubes (walls) in the scene.</summary>
    [MenuItem("CubeSelector/Select Red Cubes")]
    static void SelectRed()
    {
        Object[] gos = GameObject.FindObjectsOfType(typeof(GameObject));
        List<Object> redCubesList = new List<Object>();
        foreach(Object go in gos) 
            if (go.name == "Cube_Red") redCubesList.Add(go);
        Object[] redCubes = new Object[redCubesList.Count];
        for (int i = 0; i < redCubesList.Count; i++)
            redCubes[i] = redCubesList[i];

        Selection.objects = redCubes;
    }

    /// <summary>Selects all blue cubes (walls) in the scene.</summary>
    [MenuItem("CubeSelector/Select Blue Cubes")]
    static void SelectBlue()
    {
        Object[] gos = GameObject.FindObjectsOfType(typeof(GameObject));
        List<Object> blueCubesList = new List<Object>();
        foreach(Object go in gos) 
            if (go.name == "Cube_Blue") blueCubesList.Add(go);
        Object[] blueCubes = new Object[blueCubesList.Count];
        for (int i = 0; i < blueCubesList.Count; i++)
            blueCubes[i] = blueCubesList[i];

        Selection.objects = blueCubes;
    }
    
    /// <summary>Selects all white cubes (floors) in the scene.</summary>
    [MenuItem("CubeSelector/Select White Cubes")]
    static void SelectWhite()
    {
        Object[] gos = GameObject.FindObjectsOfType(typeof(GameObject));
        List<Object> whiteCubesList = new List<Object>();
        foreach(Object go in gos) 
            if (go.name == "Cube_White") whiteCubesList.Add(go);
        Object[] whiteCubes = new Object[whiteCubesList.Count];
        for (int i = 0; i < whiteCubesList.Count; i++)
            whiteCubes[i] = whiteCubesList[i];

        Selection.objects = whiteCubes;
    }
}
