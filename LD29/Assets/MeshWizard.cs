#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

public class MeshWizard : MonoBehaviour
{
  /*
  [MenuItem("LudumDare/GenerateBlockMesh")]
  public static void GenerateBlockMesh()
  {
    int[] intRes = { 
      Mathf.Max(1, Mathf.FloorToInt(resolution.x)),
      Mathf.Max(1, Mathf.FloorToInt(resolution.y)),
      Mathf.Max(1, Mathf.FloorToInt(resolution.z))
    };

    List<int> aTriList = new List<int>();

    int vCount = (intRes [0] * intRes [1] * intRes [2]) * 8;
    Vector3[] vertList = new Vector3[vCount];
    Vector2[] uvListA = new Vector2[vCount];
    Vector2[] uvListB = new Vector2[vCount];

    Vector3 startPos = actualWidth * -0.5f;
    Vector3 step = new Vector3(actualWidth.x / resolution.x,
                               actualWidth.y / resolution.y,
                               actualWidth.z / resolution.z);

    Vector3[] vOffset = {
      new Vector3(step.x * -0.5f, step.y * -0.5f, step.z * -0.5f),
      new Vector3(step.x * +0.5f, step.y * -0.5f, step.z * -0.5f),
      new Vector3(step.x * -0.5f, step.y * +0.5f, step.z * -0.5f),
      new Vector3(step.x * +0.5f, step.y * +0.5f, step.z * -0.5f),
      new Vector3(step.x * -0.5f, step.y * -0.5f, step.z * +0.5f),
      new Vector3(step.x * +0.5f, step.y * -0.5f, step.z * +0.5f),
      new Vector3(step.x * -0.5f, step.y * +0.5f, step.z * +0.5f),
      new Vector3(step.x * +0.5f, step.y * +0.5f, step.z * +0.5f)
    };

    Vector3 uvStep = new Vector3((maxSubUV.x - minSubUV.x) / intRes [0],
                                 (maxSubUV.y - minSubUV.y) / intRes [1],
                                 (maxSubUV.z - minSubUV.z) / intRes [2]);
   
    int vIndex = 0;
    for (int x = intRes[0]-1; x >= 0; --x)
      for (int y = intRes[1]-1; y >= 0; --y)
        for (int z = intRes[2]-1; z >= 0; --z)
        {
          Vector3 blockMid = new Vector3(startPos.x + step.x * x,
                                       startPos.y + step.y * y,
                                       startPos.z + step.z * z);

          Vector3 uvLocA = new Vector3(minSubUV.x + uvStep.x * x,
                                    minSubUV.y + uvStep.y * y);

          Vector3 uvLocB = new Vector3(minSubUV.z + uvStep.z * z, 0.0f);

          vertList [vIndex + 0] = blockMid + vOffset [0];
          vertList [vIndex + 1] = blockMid + vOffset [1];
          vertList [vIndex + 2] = blockMid + vOffset [2];
          vertList [vIndex + 3] = blockMid + vOffset [3];
          vertList [vIndex + 4] = blockMid + vOffset [4];
          vertList [vIndex + 5] = blockMid + vOffset [5];
          vertList [vIndex + 6] = blockMid + vOffset [6];
          vertList [vIndex + 7] = blockMid + vOffset [7];

          uvListA [vIndex + 0] = uvLocA;
          uvListA [vIndex + 1] = uvLocA;
          uvListA [vIndex + 2] = uvLocA;
          uvListA [vIndex + 3] = uvLocA;
          uvListA [vIndex + 4] = uvLocA;
          uvListA [vIndex + 5] = uvLocA;
          uvListA [vIndex + 6] = uvLocA;
          uvListA [vIndex + 7] = uvLocA;

          uvListB [vIndex + 0] = uvLocB;
          uvListB [vIndex + 1] = uvLocB;
          uvListB [vIndex + 2] = uvLocB;
          uvListB [vIndex + 3] = uvLocB;
          uvListB [vIndex + 4] = uvLocB;
          uvListB [vIndex + 5] = uvLocB;
          uvListB [vIndex + 6] = uvLocB;
          uvListB [vIndex + 7] = uvLocB;

          // Bottom 0123
          aTriList.Add(vIndex + 0);
          aTriList.Add(vIndex + 1);
          aTriList.Add(vIndex + 2);
          aTriList.Add(vIndex + 1);
          aTriList.Add(vIndex + 3);
          aTriList.Add(vIndex + 2);

          // Top 4567
          aTriList.Add(vIndex + 7);
          aTriList.Add(vIndex + 6);
          aTriList.Add(vIndex + 5);
          aTriList.Add(vIndex + 6);
          aTriList.Add(vIndex + 4);
          aTriList.Add(vIndex + 5);

          // West 0246
          aTriList.Add(vIndex + 0);
          aTriList.Add(vIndex + 2);
          aTriList.Add(vIndex + 4);
          aTriList.Add(vIndex + 2);
          aTriList.Add(vIndex + 6);
          aTriList.Add(vIndex + 4);

          // East 1357
          aTriList.Add(vIndex + 1);
          aTriList.Add(vIndex + 3);
          aTriList.Add(vIndex + 5);
          aTriList.Add(vIndex + 3);
          aTriList.Add(vIndex + 5);
          aTriList.Add(vIndex + 7);

          // South 0145
          aTriList.Add(vIndex + 0);
          aTriList.Add(vIndex + 1);
          aTriList.Add(vIndex + 4);
          aTriList.Add(vIndex + 1);
          aTriList.Add(vIndex + 5);
          aTriList.Add(vIndex + 4);
        
          // North 2367
          aTriList.Add(vIndex + 2);
          aTriList.Add(vIndex + 3);
          aTriList.Add(vIndex + 6);
          aTriList.Add(vIndex + 3);
          aTriList.Add(vIndex + 7);
          aTriList.Add(vIndex + 6);

          vIndex += 8;
        }

    // Setup Mesh
    Mesh genMesh = new Mesh();
    genMesh.vertices = vertList;
    genMesh.uv = uvListA;
    genMesh.uv1 = uvListB;
    genMesh.triangles = aTriList.ToArray();
    genMesh.RecalculateBounds();
    genMesh.RecalculateNormals(); 
    genMesh.Optimize();

    string path = EditorUtility.SaveFilePanel("Create Puzzle", "Assets/Resources/", "puzzle.asset", "asset");
    path = FileUtil.GetProjectRelativePath(path);
    
    AssetDatabase.CreateAsset(genMesh, path);
    AssetDatabase.SaveAssets();
  }
  */

  [MenuItem("LudumDare/GenerateFlatMesh")]
  public static void GenerateFlatMesh()
  {
    // Params
    int blockRes = 64;
    Vector3 blockSize = new Vector3(1.0f, 1.0f, 1.0f);
    Vector3 startPos = blockSize * -0.5f * (float)blockRes;
    //
    
    List<int> aTriList = new List<int>();
    
    int vCount = blockRes * blockRes * 8;
    Vector3[] vertList = new Vector3[vCount];
    Vector2[] uvList = new Vector2[vCount];
    
    
    Vector3[] vOffset = {
      new Vector3(blockSize.x * -0.5f, blockSize.y * -0.5f, blockSize.z * -0.5f),
      new Vector3(blockSize.x * +0.5f, blockSize.y * -0.5f, blockSize.z * -0.5f),
      new Vector3(blockSize.x * -0.5f, blockSize.y * +0.5f, blockSize.z * -0.5f),
      new Vector3(blockSize.x * +0.5f, blockSize.y * +0.5f, blockSize.z * -0.5f),
      new Vector3(blockSize.x * -0.5f, blockSize.y * -0.5f, blockSize.z * +0.5f),
      new Vector3(blockSize.x * +0.5f, blockSize.y * -0.5f, blockSize.z * +0.5f),
      new Vector3(blockSize.x * -0.5f, blockSize.y * +0.5f, blockSize.z * +0.5f),
      new Vector3(blockSize.x * +0.5f, blockSize.y * +0.5f, blockSize.z * +0.5f)
    };

    Vector3 uvStep = Vector3.one / blockRes;
    
    int vIndex = 0;
    for (int x = 0; x < blockRes; ++x)
      for (int y = 0; y < blockRes; ++y)
      {
        Vector3 blockMid = new Vector3(startPos.x + blockSize.x * x,
                                     0,
                                     startPos.y + blockSize.y * y);
      
        Vector3 uvLocA = new Vector3(uvStep.x * x, uvStep.y * y);
      
        vertList [vIndex + 0] = blockMid + vOffset [0];
        vertList [vIndex + 1] = blockMid + vOffset [1];
        vertList [vIndex + 2] = blockMid + vOffset [2];
        vertList [vIndex + 3] = blockMid + vOffset [3];
        vertList [vIndex + 4] = blockMid + vOffset [4];
        vertList [vIndex + 5] = blockMid + vOffset [5];
        vertList [vIndex + 6] = blockMid + vOffset [6];
        vertList [vIndex + 7] = blockMid + vOffset [7];
      
        uvList [vIndex + 0] = uvLocA;
        uvList [vIndex + 1] = uvLocA;
        uvList [vIndex + 2] = uvLocA;
        uvList [vIndex + 3] = uvLocA;
        uvList [vIndex + 4] = uvLocA;
        uvList [vIndex + 5] = uvLocA;
        uvList [vIndex + 6] = uvLocA;
        uvList [vIndex + 7] = uvLocA;
      
        // X- 0123
        aTriList.Add(vIndex + 0);
        aTriList.Add(vIndex + 2);
        aTriList.Add(vIndex + 1);
        aTriList.Add(vIndex + 1);
        aTriList.Add(vIndex + 2);
        aTriList.Add(vIndex + 3);
      
        // X+ 4567
        aTriList.Add(vIndex + 7);
        aTriList.Add(vIndex + 6);
        aTriList.Add(vIndex + 5);
        aTriList.Add(vIndex + 6);
        aTriList.Add(vIndex + 4);
        aTriList.Add(vIndex + 5);
      
        // Y- 0246
        aTriList.Add(vIndex + 0);
        aTriList.Add(vIndex + 4);
        aTriList.Add(vIndex + 2);
        aTriList.Add(vIndex + 2);
        aTriList.Add(vIndex + 4);
        aTriList.Add(vIndex + 6);
      
        // Y+ 1357
        aTriList.Add(vIndex + 1);
        aTriList.Add(vIndex + 3);
        aTriList.Add(vIndex + 5);
        aTriList.Add(vIndex + 3);
        aTriList.Add(vIndex + 7);
        aTriList.Add(vIndex + 5);
      
        // Z- 0145
        aTriList.Add(vIndex + 0);
        aTriList.Add(vIndex + 1);
        aTriList.Add(vIndex + 4);
        aTriList.Add(vIndex + 1);
        aTriList.Add(vIndex + 5);
        aTriList.Add(vIndex + 4);
      
        // Z+ 2367
        aTriList.Add(vIndex + 2);
        aTriList.Add(vIndex + 6);
        aTriList.Add(vIndex + 3);
        aTriList.Add(vIndex + 3);
        aTriList.Add(vIndex + 6);
        aTriList.Add(vIndex + 7);
        vIndex += 8;
      }
    
    // Setup Mesh
    Mesh genMesh = new Mesh();
    genMesh.vertices = vertList;
    genMesh.uv = uvList;
    genMesh.triangles = aTriList.ToArray();
    genMesh.RecalculateBounds();
    genMesh.RecalculateNormals(); 
    genMesh.Optimize();
    
    string path = EditorUtility.SaveFilePanel("Create Puzzle", "Assets/Resources/", "puzzle.asset", "asset");
    path = FileUtil.GetProjectRelativePath(path);
    
    AssetDatabase.CreateAsset(genMesh, path);
    AssetDatabase.SaveAssets();
  }
}

#endif