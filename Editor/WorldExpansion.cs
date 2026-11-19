using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Adds scenery around the existing property without rebuilding the house or yard.
[InitializeOnLoad]
public static class WorldExpansion
{
    const string ScenePath = "Assets/HouseProject/GardenHouse.unity";
    const string RootName = "10 • DISTANT LANDSCAPE";
    const string MeshPath = "Assets/HouseProject/Materials/ExpandedTerrain.asset";
    const string GrassPath = "Assets/HouseProject/Materials/MeadowGrass.mat";
    const string NaturePrefabs = "Assets/Free Low Poly Nature Pack/Prefabs/";
    const string TreePrefabs = "Assets/Darth_Artisan/Free_Trees/Prefabs/";

    static WorldExpansion() { EditorApplication.delayCall += ApplyOnce; }

    static void ApplyOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        if (SceneManager.GetActiveScene().path != ScenePath) return;
        GameObject current = GameObject.Find(RootName);
        if (current != null && current.transform.Find("Store mountains on horizon") != null) return;
        Apply();
    }

    [MenuItem("House Project/Expand Hills And Sky")]
    public static void Apply()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        Scene scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath) scene = EditorSceneManager.OpenScene(ScenePath);

        // Running this command again replaces only scenery created by this command.
        GameObject old = GameObject.Find(RootName);
        if (old != null) Object.DestroyImmediate(old);
        var root = new GameObject(RootName);

        Material grass = AssetDatabase.LoadAssetAtPath<Material>(GrassPath);
        if (grass == null)
        {
            grass = new Material(Shader.Find("Universal Render Pipeline/Lit"));
            grass.SetColor("_BaseColor", new Color(0.29f, 0.47f, 0.24f));
            grass.SetFloat("_Smoothness", 0.02f);
            AssetDatabase.CreateAsset(grass, GrassPath);
        }

        Mesh mesh = AssetDatabase.LoadAssetAtPath<Mesh>(MeshPath);
        if (mesh == null)
        {
            mesh = MakeTerrain();
            AssetDatabase.CreateAsset(mesh, MeshPath);
        }
        var terrain = new GameObject("Meadow and rolling hills", typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider));
        terrain.transform.SetParent(root.transform);
        terrain.GetComponent<MeshFilter>().sharedMesh = mesh;
        terrain.GetComponent<MeshRenderer>().sharedMaterial = grass;
        terrain.GetComponent<MeshCollider>().sharedMesh = mesh;

        Material sky = FindStoreSky();
        if (sky != null) RenderSettings.skybox = sky;

        var camera = GameObject.Find("Player Camera")?.GetComponent<Camera>();
        if (camera != null)
        {
            camera.clearFlags = CameraClearFlags.Skybox;
            camera.farClipPlane = 260f;
            EditorUtility.SetDirty(camera);
        }
        AddStoreAssets(root.transform);
        ReplacePrimitiveYardPlants();
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        AssetDatabase.SaveAssets();
        Debug.Log("Store landscape, garden plants and sky saved; house and interior untouched.");
    }

    static float Height(float x, float z)
    {
        float r = Mathf.Sqrt(x * x + z * z);
        float edge = Mathf.Clamp01((r - 26f) / 35f);
        edge = edge * edge * (3f - 2f * edge);
        float waves = 7.5f + 4f * Mathf.Sin(x * 0.095f) + 3.5f * Mathf.Cos(z * 0.12f)
            + 1.5f * Mathf.Sin((x + z) * 0.25f);
        return -0.12f + edge * Mathf.Max(2f, waves);
    }

    static Mesh MakeTerrain()
    {
        const int n = 80;
        const float span = 180f;
        var vertices = new Vector3[(n + 1) * (n + 1)];
        var uv = new Vector2[vertices.Length];
        var triangles = new int[n * n * 6];
        for (int z = 0; z <= n; z++)
        for (int x = 0; x <= n; x++)
        {
            float px = (x / (float)n - .5f) * span;
            float pz = (z / (float)n - .5f) * span;
            int i = z * (n + 1) + x;
            vertices[i] = new Vector3(px, Height(px, pz), pz);
            uv[i] = new Vector2(x / 8f, z / 8f);
        }
        int t = 0;
        for (int z = 0; z < n; z++)
        for (int x = 0; x < n; x++)
        {
            int i = z * (n + 1) + x;
            triangles[t++] = i;
            triangles[t++] = i + n + 1;
            triangles[t++] = i + 1;
            triangles[t++] = i + 1;
            triangles[t++] = i + n + 1;
            triangles[t++] = i + n + 2;
        }
        var mesh = new Mesh { name = "Expanded meadow terrain", indexFormat = UnityEngine.Rendering.IndexFormat.UInt32 };
        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
        return mesh;
    }

    static Material FindStoreSky()
    {
        Material sky = AssetDatabase.LoadAssetAtPath<Material>("Packages/xyz.staggartcreations.skyboxes/Skyboxes/Sky_Noon (Cloudy).mat");
        if (sky != null) return sky;
        Debug.LogWarning("Stylized Skyboxes Noon material was not found; existing sky retained.");
        return null;
    }

    static void UpgradeStoreMaterials()
    {
        string[] folders = { "Assets/Free Low Poly Nature Pack/Material", "Assets/Darth_Artisan/Free_Trees/Meshes/Materials" };
        Shader lit = Shader.Find("Universal Render Pipeline/Lit");
        foreach (string folder in folders)
        foreach (string guid in AssetDatabase.FindAssets("t:Material", new[] { folder }))
        {
            Material mat = AssetDatabase.LoadAssetAtPath<Material>(AssetDatabase.GUIDToAssetPath(guid));
            if (mat == null || mat.shader == lit) continue;
            Texture texture = mat.mainTexture;
            Color color = mat.HasProperty("_Color") ? mat.color : Color.white;
            mat.shader = lit;
            mat.SetColor("_BaseColor", color);
            if (texture != null) mat.SetTexture("_BaseMap", texture);
            mat.SetFloat("_Smoothness", 0f);
            EditorUtility.SetDirty(mat);
        }
    }

    static GameObject Place(string path, Transform parent, string label, float x, float z, float height, float yaw, float sink = 0f)
    {
        GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
        if (prefab == null) { Debug.LogWarning("Missing scenery prefab: " + path); return null; }
        GameObject item = (GameObject)PrefabUtility.InstantiatePrefab(prefab, parent);
        item.name = label;
        item.transform.position = new Vector3(x, Height(x, z), z);
        item.transform.rotation = Quaternion.Euler(0f, yaw, 0f);
        Renderer[] renderers = item.GetComponentsInChildren<Renderer>();
        if (renderers.Length == 0) return item;
        Bounds bounds = renderers[0].bounds;
        for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
        if (bounds.size.y > 0.001f)
        {
            float factor = height / bounds.size.y;
            item.transform.localScale *= factor;
            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++) bounds.Encapsulate(renderers[i].bounds);
            item.transform.position += Vector3.up * (Height(x, z) - bounds.min.y - sink);
        }
        foreach (Collider collider in item.GetComponentsInChildren<Collider>()) Object.DestroyImmediate(collider);
        return item;
    }

    static void AddStoreAssets(Transform root)
    {
        UpgradeStoreMaterials();
        var mountains = new GameObject("Store mountains on horizon").transform;
        var woodland = new GameObject("Store trees beyond fence").transform;
        var understory = new GameObject("Store rocks grasses and plants").transform;
        mountains.SetParent(root); woodland.SetParent(root); understory.SetParent(root);
        string[] mountainPaths = { NaturePrefabs + "Mountain_3 Variant.prefab", NaturePrefabs + "Mountain_9 Variant.prefab" };
        for (int i = 0; i < 9; i++)
        {
            float a = i * Mathf.PI * 2f / 9f;
            float r = 81f + (i % 3) * 3f;
            Place(mountainPaths[i % 2], mountains, "Mountain " + (i + 1), Mathf.Sin(a) * r, Mathf.Cos(a) * r,
                11f + (i % 4) * 2f, i * 53f, 1.5f);
        }

        string[] trees = {
            NaturePrefabs + "S3 Tree_13 Variant.prefab", NaturePrefabs + "S3 Tree_17 Variant.prefab",
            NaturePrefabs + "S3 Tree_2 Variant.prefab", NaturePrefabs + "S3 Tree_24 Variant.prefab",
            TreePrefabs + "Fir_Tree.prefab", TreePrefabs + "Oak_Tree.prefab", TreePrefabs + "Poplar_Tree.prefab"
        };
        var random = new System.Random(4819);
        for (int i = 0; i < 70; i++)
        {
            float x, z;
            do { x = (float)(random.NextDouble() * 136 - 68); z = (float)(random.NextDouble() * 136 - 68); }
            while (Mathf.Abs(x) < 26f && Mathf.Abs(z) < 24f);
            int kind = random.Next(trees.Length);
            Place(trees[kind], woodland, "Tree " + (i + 1), x, z, 3.5f + (float)random.NextDouble() * 3f, random.Next(360));
        }
        for (int i = 0; i < 28; i++)
        {
            float a = i * Mathf.PI * 2f / 28f + (float)random.NextDouble() * .1f;
            float r = 29f + (float)random.NextDouble() * 13f;
            int kind = random.Next(trees.Length);
            Place(trees[kind], woodland, "Fence line tree " + (i + 1), Mathf.Sin(a) * r, Mathf.Cos(a) * r,
                4f + (float)random.NextDouble() * 2.5f, random.Next(360));
        }

        string[] rocks = { NaturePrefabs + "Rock_8 Variant.prefab", NaturePrefabs + "Rock_12 Variant.prefab" };
        string[] grasses = { NaturePrefabs + "Grass_2 Variant.prefab", NaturePrefabs + "Grass_3 Variant.prefab", NaturePrefabs + "Grass_4 Variant.prefab" };
        for (int i = 0; i < 38; i++)
        {
            float x, z;
            do { x = (float)(random.NextDouble() * 128 - 64); z = (float)(random.NextDouble() * 128 - 64); }
            while (Mathf.Abs(x) < 21f && Mathf.Abs(z) < 19f);
            Place(rocks[i % 2], understory, "Rock " + (i + 1), x, z, .5f + (float)random.NextDouble() * 1.5f, random.Next(360), .08f);
        }
        for (int i = 0; i < 165; i++)
        {
            float x, z;
            do { x = (float)(random.NextDouble() * 140 - 70); z = (float)(random.NextDouble() * 140 - 70); }
            while (Mathf.Abs(x) < 19f && Mathf.Abs(z) < 17f);
            Place(grasses[i % 3], understory, "Grass " + (i + 1), x, z, .25f + (float)random.NextDouble() * .55f, random.Next(360));
        }
        for (int i = 0; i < 18; i++)
        {
            float a = i * Mathf.PI * 2f / 18f;
            float r = 26f + (i % 3) * 2f;
            float x = Mathf.Sin(a) * r, z = Mathf.Cos(a) * r;
            Place(NaturePrefabs + "Plant_2 Variant.prefab", understory, "Plant " + (i + 1), x, z, .5f, i * 43f);
        }
    }

    static void ReplacePrimitiveYardPlants()
    {
        GameObject yard = GameObject.Find("01 • LANDSCAPE");
        if (yard != null)
        {
            var children = new System.Collections.Generic.List<Transform>();
            foreach (Transform child in yard.transform) children.Add(child);
            var pots = children.FindAll(t => t.name == "Chậu cây");
            for (int i = 0; i < pots.Count; i++)
            {
                Vector3 position = pots[i].position;
                foreach (Transform child in children)
                {
                    if (child == null) continue;
                    if (child.name != "Chậu cây" && child.name != "Thân cây" && child.name != "Tán lá") continue;
                    Vector2 distance = new Vector2(child.position.x - position.x, child.position.z - position.z);
                    if (distance.sqrMagnitude < 2.5f) Object.DestroyImmediate(child.gameObject);
                }
                Place(NaturePrefabs + (i % 2 == 0 ? "S3 Tree_2 Variant.prefab" : "S3 Tree_24 Variant.prefab"),
                    yard.transform, "Garden tree (Store)", position.x, position.z, 3.8f, i * 67f);
            }
        }

        GameObject garden = GameObject.Find("Lush garden landscape");
        if (garden == null) return;
        var shrubs = new System.Collections.Generic.List<Transform>();
        foreach (Transform child in garden.transform)
            if (child.name == "Garden shrub") shrubs.Add(child);
        foreach (Transform shrub in shrubs)
        {
            Vector3 position = shrub.position;
            Object.DestroyImmediate(shrub.gameObject);
            Place(NaturePrefabs + "Plant_2 Variant.prefab", garden.transform,
                "Garden shrub (Store)", position.x, position.z, 1.1f, (position.x + position.z) * 17f);
        }
    }
}
