using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BakeMeshesIntoOne : MonoBehaviour
{
    public Shader shader_SliceHQ;
    public GameObject[] objectsToCombine; // The objects to combine, each should have a mesh filter and renderer with a single material.
    public bool useMipMaps = true;
    public TextureFormat textureFormat = TextureFormat.RGB24;

    void Start()
    {
        Transform[] allChildren = transform.GetComponentsInChildren<Transform>();
        List<GameObject> objsToCombine = new List<GameObject>();
        for (int i = 0; i < allChildren.Length; i++)
        {
            var go = allChildren[i].gameObject;
            if (go = this.gameObject) continue;

            if (go.GetComponent<MeshRenderer>() && go.GetComponent<MeshFilter>())
                objsToCombine.Add(go);
        }
        objectsToCombine = objsToCombine.ToArray();
        Combine();

        for (int i = 0; i < allChildren.Length; i++)
        {
            if (allChildren[i] == this.transform) continue;
            Destroy(allChildren[i].gameObject);
        }
    }

    /*
     * Combines all object textures into a single texture then creates a material used by all objects.
     * The materials properties are based on those of the material of the object at position[0].
     *
     * Also combines any meshes marked as static into a single mesh.
     */
    private void Combine()
    {
        int size;
        int originalSize;
        int pow2;
        Texture2D combinedTexture;
        Material material;
        Texture2D texture;
        Mesh mesh;
        Hashtable textureAtlas = new Hashtable();

        if (objectsToCombine.Length > 1)
        {
            originalSize = objectsToCombine[0].GetComponent<MeshRenderer>().material.mainTexture.width;
            pow2 = GetTextureSize(objectsToCombine);
            size = pow2 * originalSize;
            combinedTexture = new Texture2D(size, size, textureFormat, useMipMaps);

            // Create the combined texture (remember to ensure the total size of the texture isn't
            // larger than the platform supports)
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                var mats = objectsToCombine[i].GetComponent<MeshRenderer>().materials;
                texture = new Texture2D(10, 10);
                for (int j = 0; j < mats.Length; j++)
                {
                    texture = (Texture2D)mats[j].mainTexture;
                }
                
                if (!textureAtlas.ContainsKey(texture))
                {
                    combinedTexture.SetPixels((i % pow2) * originalSize, (i / pow2) * originalSize, originalSize, originalSize, texture.GetPixels());
                    textureAtlas.Add(texture, new Vector2(i % pow2, i / pow2));
                }
            }
            combinedTexture.Apply();
            material = new Material(shader_SliceHQ);
            //material = new Material(objectsToCombine[0].GetComponent<MeshRenderer>().material);
            material.mainTexture = combinedTexture;
            material.shader = shader_SliceHQ;

            // Update texture co-ords for each mesh (this will only work for meshes with coords betwen 0 and 1).
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                Vector2[] uv = new Vector2[mesh.uv.Length];
                Vector2 offset;
                if (textureAtlas.ContainsKey(objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture))
                {
                    offset = (Vector2)textureAtlas[objectsToCombine[i].GetComponent<MeshRenderer>().material.mainTexture];
                    for (int u = 0; u < mesh.uv.Length; u++)
                    {
                        uv[u] = mesh.uv[u] / (float)pow2;
                        uv[u].x += ((float)offset.x) / (float)pow2;
                        uv[u].y += ((float)offset.y) / (float)pow2;
                    }
                }
                else
                {
                    // This happens if you use the same object more than once, don't do it :)
                }

                mesh.uv = uv;
                objectsToCombine[i].GetComponent<MeshRenderer>().material = material;
            }

            // Combine each mesh marked as static
            int staticCount = 0;
            CombineInstance[] combine = new CombineInstance[objectsToCombine.Length];
            for (int i = 0; i < objectsToCombine.Length; i++)
            {
                if (objectsToCombine[i].isStatic)
                {
                    staticCount++;
                    combine[i].mesh = objectsToCombine[i].GetComponent<MeshFilter>().mesh;
                    combine[i].transform = objectsToCombine[i].transform.localToWorldMatrix;
                }
            }

            // Create a mesh filter and renderer
            if (staticCount > 1)
            {
                MeshFilter filter = gameObject.AddComponent<MeshFilter>();
                MeshRenderer renderer = gameObject.AddComponent<MeshRenderer>();
                filter.mesh = new Mesh();
                filter.mesh.CombineMeshes(combine);
                renderer.material = material;
                print(material.name);

                // Disable all the static object renderers
                for (int i = 0; i < objectsToCombine.Length; i++)
                {
                    if (objectsToCombine[i].isStatic)
                    {
                        objectsToCombine[i].GetComponent<MeshFilter>().mesh = null;
                        objectsToCombine[i].GetComponent<MeshRenderer>().material = null;
                        objectsToCombine[i].GetComponent<MeshRenderer>().enabled = false;
                    }
                }
            }

            Resources.UnloadUnusedAssets();
        }
    }

    private int GetTextureSize(GameObject[] o)
    {
        ArrayList textures = new ArrayList();
        // Find unique textures
        for (int i = 0; i < o.Length; i++)
        {
            if (!textures.Contains(o[i].GetComponent<MeshRenderer>().material.mainTexture))
            {
                textures.Add(o[i].GetComponent<MeshRenderer>().material.mainTexture);
            }
        }
        if (textures.Count == 1) return 1;
        if (textures.Count < 5) return 2;
        if (textures.Count < 17) return 4;
        if (textures.Count < 65) return 8;
        // Doesn't handle more than 64 different textures but I think you can see how to extend
        return 0;
    }
}