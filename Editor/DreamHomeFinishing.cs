using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// A deliberate second pass for a coherent, lived-in home.  This only uses
// prefabs in LowPolyInteriorsFree; basic cubes are reserved for facade trim.
[InitializeOnLoad]
public static class DreamHomeFinishing
{
    const string ScenePath = "Assets/HouseProject/GardenHouse.unity";
    const string Pack = "Assets/LowPolyInteriorsFree/Prefabs/";
    const string Marker = "DREAM HOME • EXTERIOR & LAYOUT • V3";

    static DreamHomeFinishing() { EditorApplication.delayCall += ApplyOnce; }

    static void ApplyOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath || GameObject.Find(Marker) != null) return;
        if (AssetDatabase.LoadAssetAtPath<GameObject>(Pack + "02_Openings/SM_LPI_Open_Window_01.prefab") == null) return;
        DollhouseDecorator.Apply();
        Apply();
    }

    static GameObject Add(Transform parent, string path, Vector3 position, float yaw = 0, float scale = 1)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Pack + path);
        if (prefab == null) { Debug.LogWarning("Missing Dream Home prefab: " + path); return null; }
        var item = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        item.transform.SetPositionAndRotation(position, Quaternion.Euler(0, yaw, 0));
        item.transform.localScale = Vector3.one * scale;
        return item;
    }

    static Transform Zone(Transform root, string name)
    {
        var zone = new GameObject(name).transform;
        zone.SetParent(root);
        return zone;
    }

    static void Cube(Transform parent, string name, Vector3 p, Vector3 s, Material material)
    {
        var item = GameObject.CreatePrimitive(PrimitiveType.Cube);
        item.name = name;
        item.transform.SetParent(parent);
        item.transform.SetPositionAndRotation(p, Quaternion.identity);
        item.transform.localScale = s;
        item.GetComponent<Renderer>().sharedMaterial = material;
    }

    static void RestoreFunctionalOpenings()
    {
        foreach (var t in Object.FindObjectsByType<Transform>())
        {
            if (t.gameObject.scene.path != ScenePath) continue;
            if (t.name != "Khung cửa sổ" && t.name != "cửa chính") continue;
            foreach (var renderer in t.GetComponentsInChildren<Renderer>(true)) renderer.enabled = true;
            foreach (var collider in t.GetComponentsInChildren<Collider>(true)) collider.enabled = true;
        }
    }

    static void Exterior(Transform root)
    {
        var exterior = Zone(root, "Balanced facade, windows and porch");
        var wood = AssetDatabase.LoadAssetAtPath<Material>("Assets/HouseProject/Materials/Walnut.mat");
        var dark = AssetDatabase.LoadAssetAtPath<Material>("Assets/HouseProject/Materials/Charcoal.mat");
        var cream = AssetDatabase.LoadAssetAtPath<Material>("Assets/HouseProject/Materials/Warm plaster.mat");

        // Existing scene openings match the wall dimensions and include the
        // working front-door interaction. Add clean glass panes to close the
        // oversized original openings without skewed prefab geometry.
        var glass = AssetDatabase.LoadAssetAtPath<Material>("Assets/HouseProject/Materials/Water.mat");
        foreach (float y in new[] { 1.65f, 4.95f })
        {
            Cube(exterior, "Glass window left", new Vector3(-6f, y, -6.1f), new Vector3(2.18f, 1.42f, .05f), glass);
            Cube(exterior, "Glass window right", new Vector3(6f, y, -6.1f), new Vector3(2.18f, 1.42f, .05f), glass);
            Cube(exterior, "Rear glass left", new Vector3(-6f, y, 6.1f), new Vector3(2.18f, 1.42f, .05f), glass);
            Cube(exterior, "Rear glass right", new Vector3(6f, y, 6.1f), new Vector3(2.18f, 1.42f, .05f), glass);
        }

        // Straight facade trim, covered porch and a calm, symmetrical entrance.
        Cube(exterior, "Porch canopy", new Vector3(0, 3.15f, -7.15f), new Vector3(7.4f, .18f, 2.2f), dark);
        Cube(exterior, "Left porch post", new Vector3(-3.25f, 1.55f, -7.05f), new Vector3(.22f, 3.1f, .22f), wood);
        Cube(exterior, "Right porch post", new Vector3(3.25f, 1.55f, -7.05f), new Vector3(.22f, 3.1f, .22f), wood);
        Cube(exterior, "Facade timber band", new Vector3(0, 3.22f, -6.22f), new Vector3(15.9f, .13f, .16f), wood);
        Cube(exterior, "Facade base band", new Vector3(0, .25f, -6.22f), new Vector3(15.9f, .14f, .16f), dark);
        Cube(exterior, "Front landing", new Vector3(0, .05f, -7.35f), new Vector3(6.7f, .14f, 2.2f), cream);
        Add(exterior, "14_Decor/SM_LPI_Decor_Doormat_01.prefab", new Vector3(0, .14f, -6.75f), 0);
        Add(exterior, "13_Lighting/SM_LPI_Light_Lantern_01.prefab", new Vector3(-3.0f, 2.35f, -6.9f), 0);
        Add(exterior, "13_Lighting/SM_LPI_Light_Lantern_01.prefab", new Vector3(3.0f, 2.35f, -6.9f), 0);
        Add(exterior, "15_Plants/SM_LPI_Plant_FiddleLeaf_01.prefab", new Vector3(-3.85f, .12f, -6.8f), 0, 1.15f);
        Add(exterior, "15_Plants/SM_LPI_Plant_FiddleLeaf_01.prefab", new Vector3(3.85f, .12f, -6.8f), 0, 1.15f);
        Add(exterior, "15_Plants/SM_LPI_Plant_Flowers_01.prefab", new Vector3(-2.2f, .12f, -7.45f), 0);
        Add(exterior, "15_Plants/SM_LPI_Plant_Flowers_01.prefab", new Vector3(2.2f, .12f, -7.45f), 0);

        // Porch seating faces the garden; every item aligns to 0/90/180 degrees.
        Add(exterior, "04_Seating/SM_LPI_Seat_Bench_01.prefab", new Vector3(10.6f, .1f, .7f), -90);
        Add(exterior, "05_Tables/SM_LPI_Table_Occasional_01.prefab", new Vector3(12.6f, .1f, .7f), 0);
        Add(exterior, "04_Seating/SM_LPI_Seat_Armchair_01.prefab", new Vector3(14.1f, .1f, .7f), 90);
        Add(exterior, "13_Lighting/SM_LPI_Light_Festoon_01.prefab", new Vector3(12f, 3.05f, 1.0f), 0);
    }

    static void LivedInDetails(Transform root)
    {
        var living = Zone(root, "Lounge finishing");
        Add(living, "21_Fireplace/SM_LPI_Fire_Surround_01.prefab", new Vector3(-2.15f, .05f, .2f), -90);
        Add(living, "21_Fireplace/SM_LPI_Fire_Logs_01.prefab", new Vector3(-2.25f, .18f, .2f), -90);
        Add(living, "14_Decor/SM_LPI_Decor_BookStack_01.prefab", new Vector3(-3.55f, .64f, -2.5f), 0);
        Add(living, "14_Decor/SM_LPI_Decor_Vase_01.prefab", new Vector3(-3.15f, .64f, -2.5f), 0);
        Add(living, "16_Textiles/SM_LPI_Text_Curtain_01.prefab", new Vector3(-5.7f, 1.45f, -5.82f), 0);
        Add(living, "16_Textiles/SM_LPI_Text_Curtain_01.prefab", new Vector3(-5.7f, 4.75f, -5.82f), 0);
        Add(living, "20_Utility/SM_LPI_Util_PetBed_01.prefab", new Vector3(-3.15f, .05f, -5.0f), 0);
        Add(living, "20_Utility/SM_LPI_Util_PetBowl_01.prefab", new Vector3(-2.5f, .05f, -5.0f), 0);

        var kitchen = Zone(root, "Kitchen practical details");
        Add(kitchen, "09_Appliances/SM_LPI_Appl_Dishwasher_01.prefab", new Vector3(7.15f, .05f, 5.15f), 180);
        Add(kitchen, "10_Kitchenware/SM_LPI_Ware_SpiceRack_01.prefab", new Vector3(6.0f, 1.03f, 5.12f), 180);
        Add(kitchen, "11_Food/SM_LPI_Food_Jar_01.prefab", new Vector3(5.0f, .97f, 2.25f), 0);
        Add(kitchen, "11_Food/SM_LPI_Food_Onion_01.prefab", new Vector3(4.35f, .97f, 2.25f), 0);
        Add(kitchen, "16_Textiles/SM_LPI_Text_TeaTowelFolded_01.prefab", new Vector3(6.72f, 1.03f, 5.1f), 0);

        var bedroom = Zone(root, "Bedroom comfort details");
        Add(bedroom, "14_Decor/SM_LPI_Decor_PhotoFrame_01.prefab", new Vector3(-6.8f, 4.12f, -4.6f), 0);
        Add(bedroom, "14_Decor/SM_LPI_Decor_Book_01.prefab", new Vector3(-3.2f, 4.12f, -4.6f), 0);
        Add(bedroom, "20_Utility/SM_LPI_Util_Hamper_01.prefab", new Vector3(-2.45f, 3.35f, -1.65f), 0);
        Add(bedroom, "16_Textiles/SM_LPI_Text_Curtain_01.prefab", new Vector3(-5.7f, 4.75f, -5.82f), 0);

        var office = Zone(root, "Office technology details");
        Add(office, "17_Electronics/SM_LPI_Tech_MonitorWide_01.prefab", new Vector3(-5.0f, 4.18f, 4.9f), 180);
        Add(office, "17_Electronics/SM_LPI_Tech_Keyboard_01.prefab", new Vector3(-5.0f, 4.16f, 4.35f), 180);
        Add(office, "17_Electronics/SM_LPI_Tech_Mouse_01.prefab", new Vector3(-4.35f, 4.17f, 4.35f), 180);
        Add(office, "17_Electronics/SM_LPI_Tech_PcTower_01.prefab", new Vector3(-6.0f, 3.35f, 4.6f), 180);
        Add(office, "17_Electronics/SM_LPI_Tech_Printer_01.prefab", new Vector3(-2.45f, 4.1f, 3.8f), 90);

        var nursery = Zone(root, "Upstairs family nook");
        Add(nursery, "18_Kids/SM_LPI_Kids_Rug_01.prefab", new Vector3(5.5f, 3.32f, -3.6f), 0);
        Add(nursery, "18_Kids/SM_LPI_Kids_ToyBox_01.prefab", new Vector3(7.1f, 3.35f, -4.7f), 180);
        Add(nursery, "18_Kids/SM_LPI_Kids_Teddy_01.prefab", new Vector3(5.4f, 3.75f, -3.55f), 0);
        Add(nursery, "18_Kids/SM_LPI_Kids_RockingHorse_01.prefab", new Vector3(4.2f, 3.35f, -4.8f), 0);
        Add(nursery, "18_Kids/SM_LPI_Kids_Blocks_01.prefab", new Vector3(6.6f, 3.62f, -4.65f), 0);

        var utility = Zone(root, "Utility corner");
        Add(utility, "09_Appliances/SM_LPI_Appl_Washer_01.prefab", new Vector3(7.2f, .05f, -4.6f), 180);
        Add(utility, "09_Appliances/SM_LPI_Appl_Dryer_01.prefab", new Vector3(6.0f, .05f, -4.6f), 180);
        Add(utility, "20_Utility/SM_LPI_Util_Hamper_01.prefab", new Vector3(5.0f, .05f, -4.6f), 0);
    }

    static void Bush(Transform parent, Vector3 p, float size, Material leaves)
    {
        var bush = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        bush.name = "Garden shrub";
        bush.transform.SetParent(parent);
        bush.transform.position = p + Vector3.up * (size * .45f);
        bush.transform.localScale = new Vector3(size, size * .9f, size);
        bush.GetComponent<Renderer>().sharedMaterial = leaves;
    }

    static void Landscape(Transform root)
    {
        var garden = Zone(root, "Lush garden landscape");
        var leaves = AssetDatabase.LoadAssetAtPath<Material>("Assets/HouseProject/Materials/Leaves.mat");
        var soil = AssetDatabase.LoadAssetAtPath<Material>("Assets/HouseProject/Materials/Walnut.mat");
        var stone = AssetDatabase.LoadAssetAtPath<Material>("Assets/HouseProject/Materials/Stone.mat");

        // Two planted beds frame the approach while keeping the main walkway open.
        Cube(garden, "Left flower bed", new Vector3(-5.5f, .02f, -9.3f), new Vector3(6.2f, .14f, 3.3f), soil);
        Cube(garden, "Right flower bed", new Vector3(5.5f, .02f, -9.3f), new Vector3(6.2f, .14f, 3.3f), soil);
        foreach (float x in new[] { -7.8f, -5.6f, -3.4f, 3.4f, 5.6f, 7.8f })
        {
            Add(garden, "15_Plants/SM_LPI_Plant_Flowers_01.prefab", new Vector3(x, .1f, -9.8f), 0, 1.25f);
            Add(garden, "15_Plants/SM_LPI_Plant_Flowers_01.prefab", new Vector3(x, .1f, -8.6f), 0, 1.1f);
        }

        // Border shrubs and high planting make the garden feel enclosed and green.
        foreach (float z in new[] { -14f, -11.5f, -6.5f, -3.8f, 3f, 6.5f, 10.5f, 14f })
        {
            Bush(garden, new Vector3(-15.7f, 0, z), 1.25f, leaves);
            Bush(garden, new Vector3(15.7f, 0, z), 1.25f, leaves);
        }
        foreach (var p in new[]
        {
            new Vector3(-13f,0,-11f), new Vector3(13f,0,-11f),
            new Vector3(-14f,0,-2f), new Vector3(14f,0,-2f),
            new Vector3(-13.5f,0,8f), new Vector3(13.5f,0,8f),
            new Vector3(-8.5f,0,13f), new Vector3(8.5f,0,13f)
        })
            Add(garden, "15_Plants/SM_LPI_Plant_FiddleLeaf_01.prefab", p, 0, 2.25f);

        // Small grass clumps break up the flat lawn without obstructing movement.
        foreach (var p in new[]
        {
            new Vector3(-11,0,-6),new Vector3(-9,0,-4),new Vector3(-12,0,1),new Vector3(-10,0,5),
            new Vector3(-8,0,8),new Vector3(-5,0,11),new Vector3(11,0,-6),new Vector3(9,0,-4),
            new Vector3(12,0,1),new Vector3(10,0,5),new Vector3(8,0,8),new Vector3(5,0,11)
        })
            Add(garden, "15_Plants/SM_LPI_Plant_SnakePlant_01.prefab", p, 0, 1.35f);

        // A calm rear sitting spot, edged by flowers and simple stone stepping pads.
        Cube(garden, "Rear garden patio", new Vector3(0, .02f, 13.8f), new Vector3(7.6f, .12f, 3.6f), stone);
        Add(garden, "04_Seating/SM_LPI_Seat_Bench_01.prefab", new Vector3(-2.4f, .1f, 13.9f), 0);
        Add(garden, "05_Tables/SM_LPI_Table_Occasional_01.prefab", new Vector3(0, .1f, 13.9f), 0);
        Add(garden, "04_Seating/SM_LPI_Seat_Bench_01.prefab", new Vector3(2.4f, .1f, 13.9f), 180);
        foreach (float x in new[] { -3.2f, 3.2f })
            Add(garden, "15_Plants/SM_LPI_Plant_Monstera_01.prefab", new Vector3(x, .1f, 12.3f), 0, 1.35f);
    }

    [MenuItem("House Project/Apply Dream Home Finish")]
    public static void Apply()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.path != ScenePath) { EditorSceneManager.OpenScene(ScenePath); scene = SceneManager.GetActiveScene(); }
        var previous = GameObject.Find(Marker) ?? GameObject.Find("DREAM HOME • EXTERIOR & LAYOUT • V2") ?? GameObject.Find("DREAM HOME • EXTERIOR & LAYOUT");
        if (previous != null) Object.DestroyImmediate(previous);
        RestoreFunctionalOpenings();
        var root = new GameObject(Marker).transform;
        Exterior(root);
        LivedInDetails(root);
        Landscape(root);
        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("DREAM_HOME_FINISH_SUCCESS");
    }
}
