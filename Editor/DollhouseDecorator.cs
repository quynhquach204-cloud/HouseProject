using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

// Populates GardenHouse with the real prefabs supplied by DOLLHOUSE Free.
[InitializeOnLoad]
public static class DollhouseDecorator
{
    const string Root = "Assets/LowPolyInteriorsFree/Prefabs/";
    static DollhouseDecorator() { EditorApplication.delayCall += ApplyOnce; }
    static void ApplyOnce()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var scene = SceneManager.GetActiveScene();
        if (scene.path != "Assets/HouseProject/GardenHouse.unity") return;
        if (GameObject.Find("DOLLHOUSE • FURNISHED • COMPLETE") != null) return;
        if (AssetDatabase.LoadAssetAtPath<GameObject>(Root + "04_Seating/SM_LPI_Seat_Sofa_01.prefab") == null) return;
        Apply();
    }
    static GameObject Add(Transform parent, string relativePath, Vector3 pos, float yaw = 0, float scale = 1)
    {
        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(Root + relativePath);
        if (prefab == null) { Debug.LogWarning("Missing prefab: " + relativePath); return null; }
        var obj = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
        obj.transform.SetPositionAndRotation(pos, Quaternion.Euler(0, yaw, 0));
        obj.transform.localScale = Vector3.one * scale;
        return obj;
    }
    static Transform Room(Transform root, string name) { var r = new GameObject(name).transform; r.SetParent(root); return r; }
    static void Art(Transform r, Vector3 p, float yaw = 180) { Add(r,"14_Decor/SM_LPI_Decor_Picture_0"+(Random.Range(1,6))+".prefab",p,yaw); }
    [MenuItem("House Project/Apply DOLLHOUSE Furniture")]
    public static void Apply()
    {
        var scene = SceneManager.GetActiveScene();
        if (scene.path != "Assets/HouseProject/GardenHouse.unity") { EditorSceneManager.OpenScene("Assets/HouseProject/GardenHouse.unity"); scene = SceneManager.GetActiveScene(); }
        var old = GameObject.Find("DOLLHOUSE • FURNISHED • COMPLETE") ?? GameObject.Find("DOLLHOUSE • FURNISHED");
        if (old != null) Object.DestroyImmediate(old);
        var root = new GameObject("DOLLHOUSE • FURNISHED • COMPLETE").transform;
        // Lounge: real sofa set, rug, media unit, reading corner and wall decor.
        var lounge=Room(root,"Living room");
        Add(lounge,"16_Textiles/SM_LPI_Text_RugRect_01.prefab",new Vector3(-4.7f,.05f,-2.7f),0,1.35f);
        Add(lounge,"04_Seating/SM_LPI_Seat_Sofa_01.prefab",new Vector3(-6.7f,.05f,-2.6f),90);
        Add(lounge,"04_Seating/SM_LPI_Seat_Armchair_03.prefab",new Vector3(-3.1f,.05f,-4.2f),-35);
        Add(lounge,"05_Tables/SM_LPI_Table_Coffee_03.prefab",new Vector3(-4.8f,.05f,-2.5f),0);
        Add(lounge,"06_Storage/SM_LPI_Storage_TVUnit_01.prefab",new Vector3(-2.1f,.05f,-2.6f),-90);
        Add(lounge,"15_Plants/SM_LPI_Plant_FiddleLeaf_01.prefab",new Vector3(-7.1f,.05f,-5.1f),0);
        Add(lounge,"15_Plants/SM_LPI_Plant_Monstera_01.prefab",new Vector3(-2.6f,.05f,-5.0f),0);
        Add(lounge,"13_Lighting/SM_LPI_Light_Table_01.prefab",new Vector3(-3.25f,.82f,-4.45f),0);
        Art(lounge,new Vector3(-7.85f,2.0f,-2.4f),90); Art(lounge,new Vector3(-7.85f,2.0f,-3.7f),90);
        // Kitchen and dining: cabinets, appliances, table setting and greenery.
        var kitchen=Room(root,"Kitchen and dining");
        for(int i=0;i<4;i++) Add(kitchen,"08_Kitchen/SM_LPI_Kitchen_Base_01.prefab",new Vector3(3.5f+i*1.05f,.05f,5.15f),180);
        Add(kitchen,"08_Kitchen/SM_LPI_Kitchen_SinkBase_01.prefab",new Vector3(6.65f,.05f,5.15f),180);
        Add(kitchen,"08_Kitchen/SM_LPI_Kitchen_Sink_01.prefab",new Vector3(6.65f,.9f,5.15f),180);
        Add(kitchen,"08_Kitchen/SM_LPI_Kitchen_Tap_01.prefab",new Vector3(6.65f,1.0f,5.15f),180);
        Add(kitchen,"09_Appliances/SM_LPI_Appl_Fridge_01.prefab",new Vector3(7.35f,.05f,3.8f),-90);
        Add(kitchen,"09_Appliances/SM_LPI_Appl_Oven_01.prefab",new Vector3(4.55f,.05f,5.15f),180);
        Add(kitchen,"05_Tables/SM_LPI_Table_Dining_01.prefab",new Vector3(5.0f,.05f,-1.9f),0);
        foreach(var item in new[]{(4.0f,-3.0f,180f),(5.0f,-3.0f,180f),(6.0f,-3.0f,180f),(4.0f,-.8f,0f),(5.0f,-.8f,0f),(6.0f,-.8f,0f)}) Add(kitchen,"04_Seating/SM_LPI_Seat_Chair_01.prefab",new Vector3(item.Item1,.05f,item.Item2),item.Item3);
        for(int i=0;i<4;i++) { Add(kitchen,"10_Kitchenware/SM_LPI_Ware_PlateDinner_01.prefab",new Vector3(4.1f+i*.6f,.82f,-1.8f),0); Add(kitchen,"10_Kitchenware/SM_LPI_Ware_WineGlass_01.prefab",new Vector3(4.1f+i*.6f,.82f,-2.15f),0); }
        Add(kitchen,"10_Kitchenware/SM_LPI_Ware_Kettle_01.prefab",new Vector3(4.0f,.92f,5.1f),180); Add(kitchen,"10_Kitchenware/SM_LPI_Ware_SpiceRack_01.prefab",new Vector3(5.6f,.92f,5.1f),180); Add(kitchen,"15_Plants/SM_LPI_Plant_Herb_01.prefab",new Vector3(7.1f,.92f,5.1f),180);
        // Bedroom: complete bed, bedside tables, wardrobe, reading chair, plants and artwork.
        var bedroom=Room(root,"Bedroom");
        Add(bedroom,"07_Bedroom/SM_LPI_Bed_Double_01.prefab",new Vector3(-5.0f,3.35f,-3.6f),0);
        Add(bedroom,"07_Bedroom/SM_LPI_Bed_Nightstand_01.prefab",new Vector3(-6.8f,3.35f,-4.6f),0); Add(bedroom,"07_Bedroom/SM_LPI_Bed_Nightstand_01.prefab",new Vector3(-3.2f,3.35f,-4.6f),0);
        Add(bedroom,"13_Lighting/SM_LPI_Light_Table_01.prefab",new Vector3(-6.8f,4.05f,-4.6f),0); Add(bedroom,"13_Lighting/SM_LPI_Light_Table_01.prefab",new Vector3(-3.2f,4.05f,-4.6f),0);
        Add(bedroom,"06_Storage/SM_LPI_Storage_Wardrobe_01.prefab",new Vector3(-7.25f,3.35f,-1.4f),90); Add(bedroom,"16_Textiles/SM_LPI_Text_RugRunner_01.prefab",new Vector3(-4.9f,3.32f,-1.5f),0);
        Add(bedroom,"04_Seating/SM_LPI_Seat_Armchair_06.prefab",new Vector3(-2.75f,3.35f,-1.0f),-90); Add(bedroom,"15_Plants/SM_LPI_Plant_SnakePlant_01.prefab",new Vector3(-7.2f,3.35f,-5.1f),0); Art(bedroom,new Vector3(-5f,5.2f,-5.85f),180);
        // Office: desk chair, bookcases, decorative plants and task lamp.
        var office=Room(root,"Study");
        Add(office,"19_Office/SM_LPI_Office_Desk_01.prefab",new Vector3(-5f,3.35f,4.7f),180); Add(office,"19_Office/SM_LPI_Office_ChairTask_01.prefab",new Vector3(-5f,3.35f,3.6f),0);
        Add(office,"06_Storage/SM_LPI_Storage_Bookcase_04.prefab",new Vector3(-7.35f,3.35f,2.3f),90); Add(office,"06_Storage/SM_LPI_Storage_Bookcase_07.prefab",new Vector3(-7.35f,3.35f,4.9f),90);
        Add(office,"13_Lighting/SM_LPI_Light_Desk_01.prefab",new Vector3(-5.65f,4.12f,4.65f),180); Add(office,"15_Plants/SM_LPI_Plant_RubberPlant_01.prefab",new Vector3(-2.3f,3.35f,5.0f),0); Add(office,"14_Decor/SM_LPI_Decor_WallClock_01.prefab",new Vector3(-5,5.35f,5.85f),180);
        // Bathroom: full suite with textiles and small accessories.
        var bath=Room(root,"Bathroom");
        Add(bath,"12_Bathroom/SM_LPI_Bath_Tub_01.prefab",new Vector3(6.9f,3.35f,4.5f),180); Add(bath,"12_Bathroom/SM_LPI_Bath_Vanity_01.prefab",new Vector3(4.25f,3.35f,5.1f),180); Add(bath,"12_Bathroom/SM_LPI_Bath_Basin_01.prefab",new Vector3(4.25f,4.15f,5.1f),180);
        Add(bath,"12_Bathroom/SM_LPI_Bath_Mirror_01.prefab",new Vector3(4.25f,5.2f,5.72f),180); Add(bath,"12_Bathroom/SM_LPI_Bath_Toilet_01.prefab",new Vector3(6.7f,3.35f,2.0f),180); Add(bath,"16_Textiles/SM_LPI_Text_BathMat_01.prefab",new Vector3(4.25f,3.32f,4.15f),0);
        Add(bath,"12_Bathroom/SM_LPI_Bath_Dispenser_01.prefab",new Vector3(4.65f,4.25f,5.05f),180); Add(bath,"12_Bathroom/SM_LPI_Bath_Towel_01.prefab",new Vector3(5.1f,4.5f,5.72f),180); Add(bath,"12_Bathroom/SM_LPI_Bath_Bin_01.prefab",new Vector3(5.55f,3.35f,2.4f),0);
        // Hall and patio accents use the pack's real objects too.
        var hall=Room(root,"Hall and patio"); Add(hall,"05_Tables/SM_LPI_Table_Hall_01.prefab",new Vector3(-.1f,.05f,-4.7f),0); Add(hall,"14_Decor/SM_LPI_Decor_Vase_01.prefab",new Vector3(-.1f,.82f,-4.7f),0); Add(hall,"15_Plants/SM_LPI_Plant_Cactus_01.prefab",new Vector3(11.1f,.05f,2.0f),0); Add(hall,"15_Plants/SM_LPI_Plant_Flowers_01.prefab",new Vector3(12.9f,.05f,2.0f),0);
        Polish(root);
        HideLegacyPlaceholders();
        EditorSceneManager.MarkSceneDirty(scene); EditorSceneManager.SaveScene(scene); Debug.Log("DOLLHOUSE_FURNISH_SUCCESS");
    }

    // Finishing pass: small functional objects, layered lighting and textiles
    // make the package feel like a lived-in house instead of a showroom.
    static void Polish(Transform root)
    {
        var living = Room(root, "Living details");
        Add(living,"04_Seating/SM_LPI_Seat_Sofa_02.prefab",new Vector3(-5.7f,.05f,-1.1f),180);
        Add(living,"13_Lighting/SM_LPI_Light_Floor_02.prefab",new Vector3(-7.15f,.05f,-1.1f),0);
        Add(living,"05_Tables/SM_LPI_Table_Side_01.prefab",new Vector3(-3.3f,.05f,-1.1f),0);
        Add(living,"16_Textiles/SM_LPI_Text_Cushion_01.prefab",new Vector3(-6.0f,.82f,-1.1f),15);
        Add(living,"16_Textiles/SM_LPI_Text_Cushion_03.prefab",new Vector3(-5.25f,.82f,-1.1f),-15);
        Add(living,"14_Decor/SM_LPI_Decor_Magazine_01.prefab",new Vector3(-4.8f,.63f,-2.5f),15);
        Add(living,"14_Decor/SM_LPI_Decor_CandleSet_01.prefab",new Vector3(-4.45f,.64f,-2.5f),0);
        Add(living,"14_Decor/SM_LPI_Decor_PictureSet_01.prefab",new Vector3(-2.0f,1.95f,-3.6f),-90);

        var kitchen = Room(root, "Kitchen details");
        Add(kitchen,"08_Kitchen/SM_LPI_Kitchen_Island_01.prefab",new Vector3(4.7f,.05f,2.2f),0);
        Add(kitchen,"04_Seating/SM_LPI_Seat_Stool_01.prefab",new Vector3(3.8f,.05f,1.15f),0);
        Add(kitchen,"04_Seating/SM_LPI_Seat_Stool_01.prefab",new Vector3(5.6f,.05f,1.15f),0);
        Add(kitchen,"09_Appliances/SM_LPI_Appl_Toaster_01.prefab",new Vector3(5.4f,.92f,5.1f),180);
        Add(kitchen,"09_Appliances/SM_LPI_Appl_Hob_01.prefab",new Vector3(4.55f,.94f,5.12f),180);
        Add(kitchen,"08_Kitchen/SM_LPI_Kitchen_Extractor_01.prefab",new Vector3(4.55f,2.05f,5.12f),180);
        Add(kitchen,"13_Lighting/SM_LPI_Light_Pendant_01.prefab",new Vector3(4.7f,2.85f,2.2f),0);
        Add(kitchen,"15_Plants/SM_LPI_Plant_Succulent_01.prefab",new Vector3(4.7f,.96f,2.2f),0);

        var bed = Room(root, "Bedroom details");
        Add(bed,"06_Storage/SM_LPI_Storage_Drawers_01.prefab",new Vector3(-2.45f,3.35f,-3.5f),-90);
        Add(bed,"14_Decor/SM_LPI_Decor_Mirror_01.prefab",new Vector3(-2.05f,4.9f,-3.5f),-90);
        Add(bed,"16_Textiles/SM_LPI_Text_Cushion_02.prefab",new Vector3(-5.6f,4.15f,-3.7f),0);
        Add(bed,"16_Textiles/SM_LPI_Text_CushionLumbar_01.prefab",new Vector3(-4.6f,4.15f,-3.7f),0);
        Add(bed,"16_Textiles/SM_LPI_Text_Throw_01.prefab",new Vector3(-5f,3.95f,-2.1f),0);
        Add(bed,"13_Lighting/SM_LPI_Light_Pendant_02.prefab",new Vector3(-5f,6.0f,-3.6f),0);
        Add(bed,"14_Decor/SM_LPI_Decor_Basket_01.prefab",new Vector3(-2.35f,3.35f,-1.8f),0);

        var study = Room(root, "Study details");
        Add(study,"19_Office/SM_LPI_Office_FileCabinet_01.prefab",new Vector3(-2.4f,3.35f,3.8f),90);
        Add(study,"19_Office/SM_LPI_Office_PenPot_01.prefab",new Vector3(-4.7f,4.13f,4.65f),0);
        Add(study,"19_Office/SM_LPI_Office_PaperStack_01.prefab",new Vector3(-5.25f,4.13f,4.65f),0);
        Add(study,"19_Office/SM_LPI_Office_Pinboard_01.prefab",new Vector3(-5.0f,5.15f,5.72f),180);
        Add(study,"14_Decor/SM_LPI_Decor_Globe_01.prefab",new Vector3(-7.25f,4.95f,2.3f),90);

        var bath = Room(root, "Bathroom details");
        Add(bath,"12_Bathroom/SM_LPI_Bath_ShowerHead_01.prefab",new Vector3(7.0f,5.3f,4.55f),180);
        Add(bath,"12_Bathroom/SM_LPI_Bath_Shelf_01.prefab",new Vector3(7.55f,4.4f,4.65f),-90);
        Add(bath,"12_Bathroom/SM_LPI_Bath_Mug_01.prefab",new Vector3(4.55f,4.27f,5.05f),180);
        Add(bath,"12_Bathroom/SM_LPI_Bath_RollHolder_01.prefab",new Vector3(6.1f,4.05f,2.05f),180);
        Add(bath,"13_Lighting/SM_LPI_Light_Sconce_01.prefab",new Vector3(4.25f,5.55f,5.55f),180);

        var entry = Room(root, "Entry and patio details");
        Add(entry,"14_Decor/SM_LPI_Decor_Doormat_01.prefab",new Vector3(0f,.05f,-6.2f),0);
        Add(entry,"06_Storage/SM_LPI_Storage_HallUnit_01.prefab",new Vector3(-1.35f,.05f,-4.7f),90);
        Add(entry,"14_Decor/SM_LPI_Decor_UmbrellaStand_01.prefab",new Vector3(-1.75f,.05f,-5.35f),0);
        Add(entry,"19_Office/SM_LPI_Office_CoatStand_01.prefab",new Vector3(-1.65f,.05f,-4.0f),0);
        Add(entry,"13_Lighting/SM_LPI_Light_Lantern_01.prefab",new Vector3(-.9f,.05f,-6.0f),0);
        Add(entry,"13_Lighting/SM_LPI_Light_Festoon_01.prefab",new Vector3(10.8f,3.0f,2.0f),0);
    }

    // Retain the original interaction objects, but hide old block-model props
    // and their colliders once the packaged furniture is in place.
    static void HideLegacyPlaceholders()
    {
        string[] sections = { "03 • LIVING ROOM", "04 • KITCHEN & DINING", "05 • BEDROOM", "06 • STUDY & LIBRARY", "07 • BATHROOM", "08 • UPSTAIRS LOUNGE" };
        foreach (var sectionName in sections)
        {
            var section = GameObject.Find(sectionName);
            if (section == null) continue;
            foreach (var renderer in section.GetComponentsInChildren<Renderer>(true))
                if (renderer.GetComponentInParent<HouseInteraction>() == null) renderer.enabled = false;
            foreach (var collider in section.GetComponentsInChildren<Collider>(true))
                if (collider.GetComponentInParent<HouseInteraction>() == null) collider.enabled = false;
        }
    }
}
