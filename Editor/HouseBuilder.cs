using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;
using System.IO;

[InitializeOnLoad]
public static class HouseBuilder
{
    const string ScenePath="Assets/HouseProject/GardenHouse.unity";
    static Material cream,wood,dark,white,green,blue,metal,glass,stone,cloth;
    static Transform group;
    static HouseBuilder() { EditorApplication.delayCall += AutoBuild; }
    static void AutoBuild() {
        if(EditorApplication.isPlayingOrWillChangePlaymode) return;
        if(!File.Exists(ScenePath)) Build();
    }
    static Material Mat(string name, Color color, float smooth=.3f) {
        string path="Assets/HouseProject/Materials/"+name+".mat"; var m=AssetDatabase.LoadAssetAtPath<Material>(path);
        if(m==null) { m=new Material(Shader.Find("Universal Render Pipeline/Lit")); AssetDatabase.CreateAsset(m,path); }
        m.color=color; m.SetFloat("_Smoothness",smooth); return m;
    }
    static GameObject Box(string n,Vector3 p,Vector3 s,Material m, PrimitiveType shape=PrimitiveType.Cube) {
        var o=GameObject.CreatePrimitive(shape); o.name=n; o.transform.SetParent(group); o.transform.position=p; o.transform.localScale=s; o.GetComponent<Renderer>().sharedMaterial=m; return o;
    }
    static void Group(string n) { group=new GameObject(n).transform; }
    static void Label(string text,Vector3 p,float size=.16f) { /* No room labels in the environment. */ }
    static HouseInteraction Interactive(GameObject o,string label,int kind) { var i=o.AddComponent<HouseInteraction>(); i.label=label; i.kind=kind; return i; }
    static void Door(string name,Vector3 p,float width=1.5f,int kind=0) {
        var pivot=new GameObject(name); pivot.transform.SetParent(group); pivot.transform.position=p;
        var leaf=Box("Cánh cửa",p+new Vector3(width/2,1.15f,0),new Vector3(width,2.3f,.12f),wood); leaf.transform.SetParent(pivot.transform,true);
        var handle=Box("Tay nắm đồng",p+new Vector3(width-.16f,1.1f,-.12f),new Vector3(.18f,.07f,.12f),metal); handle.transform.SetParent(pivot.transform,true);
        Interactive(pivot,kind==3?"Mở cổng • cần chìa khóa + 3 đèn":"Mở / đóng "+name,kind);
    }
    static void Window(Vector3 p,bool side=false) {
        var root=new GameObject("Khung cửa sổ"); root.transform.SetParent(group); root.transform.position=p;
        foreach(float x in new[]{-1.15f,0f,1.15f}) { var o=Box("Thanh khung",p+new Vector3(x,0,0),new Vector3(.07f,1.5f,.12f),dark); o.transform.SetParent(root.transform,true); }
        foreach(float y in new[]{-.75f,.75f}) { var o=Box("Thanh ngang",p+new Vector3(0,y,0),new Vector3(2.4f,.08f,.14f),dark); o.transform.SetParent(root.transform,true); }
        var sill=Box("Bậu cửa",p+new Vector3(0,-.8f,0),new Vector3(2.6f,.12f,.45f),white); sill.transform.SetParent(root.transform,true);
        if(side) root.transform.Rotate(0,90,0);
    }
    static void Exterior(float y) {
        // Front and rear facades are segmented around real window and door openings.
        foreach(float z in new[]{-6f,6f}) {
            Box("Tường chân cửa sổ",new Vector3(0,y+.5f,z),new Vector3(16,1,.24f),cream);
            Box("Tường trên cửa sổ",new Vector3(0,y+2.75f,z),new Vector3(16,.9f,.24f),cream);
            foreach(float x in new[]{-7.7f,-3.2f,3.2f,7.7f}) Box("Trụ tường",new Vector3(x,y+1.65f,z),new Vector3(x== -3.2f || x==3.2f?4.1f:.6f,1.3f,.24f),cream);
            foreach(float x in new[]{-6f,6f}) Window(new Vector3(x,y+1.65f,z));
            if(y>1 || z>0) Box("Tường giữa",new Vector3(0,y+1.65f,z),new Vector3(2.4f,1.3f,.24f),cream);
        }
        // Side windows.
        foreach(float x in new[]{-8f,8f}) {
            Box("Tường hông dưới",new Vector3(x,y+.5f,0),new Vector3(.24f,1,12),cream);
            Box("Tường hông trên",new Vector3(x,y+2.75f,0),new Vector3(.24f,.9f,12),cream);
            foreach(float z in new[]{-5f,0f,5f}) Box("Trụ tường hông",new Vector3(x,y+1.65f,z),new Vector3(.24f,1.3f,z==0?3.6f:2),cream);
            foreach(float z in new[]{-3f,3f}) Window(new Vector3(x,y+1.65f,z),true);
        }
    }
    static void Table(Vector3 p,Vector2 size) {
        Box("Mặt bàn gỗ",p+Vector3.up*.8f,new Vector3(size.x,.12f,size.y),wood);
        foreach(float x in new[]{-size.x/2+.12f,size.x/2-.12f}) foreach(float z in new[]{-size.y/2+.12f,size.y/2-.12f}) Box("Chân bàn",p+new Vector3(x,.4f,z),new Vector3(.09f,.8f,.09f),dark);
    }
    static void Chair(Vector3 p,float angle=0) {
        var root=new GameObject("Ghế"); root.transform.SetParent(group); root.transform.position=p;
        var seat=Box("Đệm ghế",p+new Vector3(0,.48f,0),new Vector3(.65f,.12f,.65f),cloth); seat.transform.SetParent(root.transform,true);
        var back=Box("Tựa ghế",p+new Vector3(0,.92f,.28f),new Vector3(.65f,.8f,.09f),wood); back.transform.SetParent(root.transform,true);
        foreach(float x in new[]{-.25f,.25f}) foreach(float z in new[]{-.25f,.25f}) { var leg=Box("Chân ghế",p+new Vector3(x,.24f,z),new Vector3(.06f,.48f,.06f),dark); leg.transform.SetParent(root.transform,true); }
        root.transform.Rotate(0,angle,0);
    }
    static void Plant(Vector3 p,float scale=1) {
        Box("Chậu cây",p+Vector3.up*.22f*scale,new Vector3(.5f,.45f,.5f)*scale,white,PrimitiveType.Cylinder);
        Box("Thân cây",p+Vector3.up*.7f*scale,new Vector3(.06f,.5f,.06f)*scale,wood,PrimitiveType.Cylinder);
        for(int j=0;j<5;j++) { float a=j*1.256f; Box("Tán lá",p+new Vector3(Mathf.Cos(a)*.22f,.95f+j*.07f,Mathf.Sin(a)*.22f)*scale,new Vector3(.5f,.65f,.38f)*scale,green,PrimitiveType.Sphere); }
    }
    static void Fixture(Vector3 p,bool signal=false) {
        Box("Đế đèn trần",p,new Vector3(.65f,.08f,.65f),metal,PrimitiveType.Cylinder);
        Box("Chụp đèn",p-Vector3.up*.1f,new Vector3(.5f,.12f,.5f),white,PrimitiveType.Sphere);
        var l=new GameObject("Ánh sáng phòng").AddComponent<Light>(); l.transform.SetParent(group); l.transform.position=p-Vector3.up*.3f; l.type=LightType.Point; l.range=9; l.intensity=1.5f; l.color=new Color(1,.84f,.65f); l.enabled=!signal;
        var button=Box(signal?"Công tắc tín hiệu":"Công tắc đèn",new Vector3(p.x,p.y-1.5f,p.z+1.5f),new Vector3(.25f,.35f,.12f),signal?blue:white);
        var interact=Interactive(button,signal?"Bật / tắt đèn tín hiệu":"Bật / tắt đèn phòng",1); interact.lamp=l; interact.signal=signal;
    }
    static void Book(Vector3 p,int index) { Box("Sách",p,new Vector3(.13f,.38f,.27f),index%2==0?blue:wood); }
    [MenuItem("House Project/Create Garden House")]
    public static void Build() {
        if(File.Exists(ScenePath)) { EditorSceneManager.OpenScene(ScenePath); return; }
        Directory.CreateDirectory("Assets/HouseProject/Materials"); AssetDatabase.Refresh();
        var previous=SceneManager.GetActiveScene(); var scene=EditorSceneManager.NewScene(NewSceneSetup.EmptyScene,NewSceneMode.Additive); SceneManager.SetActiveScene(scene);
        cream=Mat("Warm plaster",new Color(.85f,.81f,.7f)); wood=Mat("Walnut",new Color(.32f,.16f,.07f)); dark=Mat("Charcoal",new Color(.07f,.1f,.12f)); white=Mat("Porcelain",new Color(.91f,.91f,.85f)); green=Mat("Leaves",new Color(.15f,.34f,.17f)); blue=Mat("Teal",new Color(.08f,.4f,.47f)); metal=Mat("Brass",new Color(.65f,.45f,.19f),.7f); glass=Mat("Water",new Color(.12f,.55f,.68f),.95f); stone=Mat("Stone",new Color(.48f,.49f,.43f)); cloth=Mat("Linen",new Color(.65f,.52f,.37f));
        Group("01 • LANDSCAPE"); Box("Thảm cỏ",new Vector3(0,-.3f,0),new Vector3(42,.4f,38),green);
        Box("Đường vào",new Vector3(0,-.04f,-12),new Vector3(3,.1f,12),stone);
        for(int i=0;i<12;i++) Box("Viền lối đi",new Vector3(0,.02f,-7-i*.85f),new Vector3(2.8f,.03f,.04f),cream);
        for(int i=0;i<18;i++) foreach(float x in new[]{-19f,19f}) Box("Thanh hàng rào",new Vector3(x,.7f,-17+i*2),new Vector3(.15f,1.4f,.12f),white);
        foreach(float x in new[]{-19f,19f}) foreach(float y in new[]{.4f,1.1f}) Box("Thanh ngang hàng rào",new Vector3(x,y,0),new Vector3(.1f,.1f,35),white);
        foreach(float z in new[]{-17f,17f}) { foreach(float x in new[]{-10f,10f}) Box("Hàng rào trước sau",new Vector3(x,.65f,z),new Vector3(17,1.3f,.16f),cream); }
        Door("Cổng sân vườn",new Vector3(-1.5f,0,-17),3,3);
        foreach(var p in new[]{new Vector3(-14,0,-10),new Vector3(14,0,-9),new Vector3(-14,0,10),new Vector3(14,0,10),new Vector3(0,0,13)}) Plant(p,3);
        Box("Sân lát đá",new Vector3(12,0,1),new Vector3(6,.12f,9),stone); Table(new Vector3(12,0,1),new Vector2(2,1.2f)); Chair(new Vector3(12,0,2.2f)); Chair(new Vector3(12,0,-.2f),180);
        for(int i=0;i<5;i++) { Box("Cột giàn pergola",new Vector3(9.3f,.0f+i*.6f,5),new Vector3(.15f,.6f,.15f),wood); }
        foreach(float x in new[]{9.3f,14.7f}) foreach(float z in new[]{-3f,5f}) Box("Cột pergola",new Vector3(x,1.5f,z),new Vector3(.16f,3,.16f),wood);
        for(int i=0;i<10;i++) Box("Xà pergola",new Vector3(12,3,-3+i*.9f),new Vector3(6,.15f,.16f),wood);
        Group("02 • HOUSE STRUCTURE"); Box("Nền tầng trệt",new Vector3(0,-.1f,0),new Vector3(16,.2f,12),wood);
        // Stairwell occupies x=0.5..3, z=-3..3.5; upper slabs leave it open.
        Box("Sàn tầng hai phía tây",new Vector3(-3.8f,3.2f,0),new Vector3(8.4f,.2f,12),wood);
        Box("Sàn tầng hai phía đông",new Vector3(5.6f,3.2f,0),new Vector3(4.8f,.2f,12),wood);
        Box("Sàn hành lang trước",new Vector3(1.8f,3.2f,-4.6f),new Vector3(2.8f,.2f,2.8f),wood);
        Box("Chiếu nghỉ phía sau",new Vector3(1.8f,3.2f,4.9f),new Vector3(2.8f,.2f,2.2f),wood);
        Exterior(0); Exterior(3.3f);
        // Remove the front door's sill by replacing only its central part with no collider.
        foreach(var o in Object.FindObjectsByType<Transform>()) if(o.gameObject.scene==scene && o.name=="Tường chân cửa sổ" && Mathf.Abs(o.position.z+6)<.1f && o.position.y<1) { Object.DestroyImmediate(o.gameObject); break; }
        foreach(float x in new[]{-4.5f,4.5f}) Box("Chân tường mặt tiền",new Vector3(x,.5f,-6),new Vector3(7,1,.24f),cream);
        Door("cửa chính",new Vector3(-1,0,-6),2);
        Box("Mái nhà phẳng",new Vector3(0,6.65f,0),new Vector3(16.7f,.24f,12.7f),dark);
        for(int i=0;i<18;i++) Box("Bậc cầu thang "+(i+1),new Vector3(1.8f,(i+1)*3.3f/18/2,-3+i*.38f),new Vector3(2.2f,(i+1)*3.3f/18,.4f),wood);
        for(int i=0;i<18;i++) Box("Trụ lan can cầu thang",new Vector3(.6f,(i+1)*3.3f/18+.45f,-3+i*.38f),new Vector3(.055f,.9f,.055f),metal);
        var rail=Box("Tay vịn cầu thang",new Vector3(.6f,2.65f,.2f),new Vector3(.08f,.08f,7.6f),wood); rail.transform.rotation=Quaternion.Euler(-26,0,0);
        foreach(float x in new[]{.45f,3.15f}) { Box("Lan can giếng thang",new Vector3(x,3.8f,.25f),new Vector3(.08f,1,6.4f),dark); }
        // Interior partitions with broad passage openings.
        foreach(float y in new[]{0f,3.3f}) { foreach(float z in new[]{-4.5f,2.8f}) Box("Vách hành lang",new Vector3(-1.3f,y+1.55f,z),new Vector3(.18f,3.1f,z<0?3:6.4f),cream); }
        Box("Vách phòng ngủ",new Vector3(-5.7f,4.85f,0),new Vector3(4.4f,3.1f,.16f),cream);
        Box("Vách phòng tắm",new Vector3(5.6f,4.85f,1),new Vector3(4.8f,3.1f,.16f),cream);
        // Access from hall to east rooms via front and rear openings.
        Box("Vách đông hành lang",new Vector3(3.3f,4.85f,.2f),new Vector3(.15f,3.1f,3.7f),cream);
        Group("03 • LIVING ROOM"); Label("PHÒNG KHÁCH",new Vector3(-4.6f,2.5f,5.8f));
        Box("Thảm phòng khách",new Vector3(-4.7f,.025f,-2.8f),new Vector3(4.5f,.035f,3.6f),cloth);
        Box("Đế sofa",new Vector3(-6.8f,.35f,-2.5f),new Vector3(1.3f,.6f,3.4f),blue); Box("Lưng sofa",new Vector3(-7.3f,.9f,-2.5f),new Vector3(.3f,1.2f,3.4f),blue);
        foreach(float z in new[]{-4.1f,-.9f}) Box("Tay sofa",new Vector3(-6.8f,.75f,z),new Vector3(1.3f,.5f,.2f),blue);
        for(int i=0;i<3;i++) Box("Gối tựa",new Vector3(-6.9f,.95f,-3.5f+i),new Vector3(.25f,.55f,.65f),cloth);
        Table(new Vector3(-4.8f,0,-2.5f),new Vector2(1.4f,2));
        Box("Kệ TV",new Vector3(-1.9f,.35f,-3.5f),new Vector3(.7f,.7f,2.6f),wood); Box("TV",new Vector3(-1.85f,1.4f,-3.5f),new Vector3(.1f,1.1f,2.2f),dark);
        Plant(new Vector3(-7,0,-5)); Fixture(new Vector3(-4.5f,2.9f,-2),true);
        Interactive(Box("Ghi chú nhiệm vụ",new Vector3(-4.8f,.88f,-2.5f),new Vector3(.45f,.025f,.6f),white),"Đọc ghi chú",5);
        var ball=Box("Quả bóng nhặt được",new Vector3(-3,.4f,-4.7f),Vector3.one*.45f,blue,PrimitiveType.Sphere); ball.AddComponent<Rigidbody>(); Interactive(ball,"Nhặt quả bóng (Q để thả)",4);
        Group("04 • KITCHEN & DINING"); Label("BẾP & BÀN ĂN",new Vector3(5,2.6f,5.8f));
        for(int i=0;i<5;i++) { float x=3.8f+i*.78f; Box("Tủ bếp dưới",new Vector3(x,.47f,5.4f),new Vector3(.74f,.94f,1),wood); Box("Tay nắm tủ",new Vector3(x,.75f,4.87f),new Vector3(.32f,.04f,.04f),metal); Box("Mặt đá bếp",new Vector3(x,.98f,5.4f),new Vector3(.79f,.1f,1.1f),white); Box("Tủ bếp trên",new Vector3(x,2.2f,5.65f),new Vector3(.74f,.8f,.6f),cream); }
        Box("Bếp từ",new Vector3(4.6f,1.05f,5.3f),new Vector3(1,.04f,.65f),dark);
        foreach(float x in new[]{4.35f,4.85f}) Box("Vòng bếp",new Vector3(x,1.08f,5.3f),new Vector3(.3f,.01f,.3f),metal,PrimitiveType.Cylinder);
        Box("Chậu rửa",new Vector3(6.4f,1.05f,5.3f),new Vector3(.8f,.04f,.6f),metal); Box("Vòi nước",new Vector3(6.4f,1.25f,5.6f),new Vector3(.06f,.4f,.06f),metal);
        Box("Tủ lạnh",new Vector3(7.2f,1.05f,3.6f),new Vector3(1.1f,2.1f,1),white); Box("Tay nắm tủ lạnh",new Vector3(6.85f,1.15f,3.05f),new Vector3(.06f,.5f,.06f),dark);
        Table(new Vector3(5,0,-1.9f),new Vector2(2.4f,1.5f)); foreach(float x in new[]{4.3f,5.7f}) { Chair(new Vector3(x,0,-3),180); Chair(new Vector3(x,0,-.8f)); }
        foreach(float x in new[]{4.3f,5.7f}) foreach(float z in new[]{-2.3f,-1.5f}) { Box("Đĩa ăn",new Vector3(x,.89f,z),new Vector3(.38f,.018f,.38f),white,PrimitiveType.Cylinder); Box("Ly",new Vector3(x+.24f,.99f,z),new Vector3(.12f,.12f,.12f),blue,PrimitiveType.Cylinder); }
        Fixture(new Vector3(5,2.9f,1),true); Plant(new Vector3(7,0,-5));
        Group("05 • BEDROOM"); Label("PHÒNG NGỦ",new Vector3(-4.5f,5.9f,-.15f));
        Box("Khung giường",new Vector3(-5,3.6f,-3.5f),new Vector3(2.8f,.5f,3.8f),wood); Box("Nệm",new Vector3(-5,3.95f,-3.5f),new Vector3(2.7f,.25f,3.7f),white); Box("Chăn",new Vector3(-5,4.1f,-3),new Vector3(2.72f,.08f,2.4f),blue);
        foreach(float x in new[]{-5.7f,-4.3f}) Box("Gối",new Vector3(x,4.15f,-4.8f),new Vector3(1.1f,.2f,.65f),cloth);
        Box("Đầu giường",new Vector3(-5,4.15f,-5.4f),new Vector3(2.9f,1.4f,.18f),wood);
        foreach(float x in new[]{-7f,-3f}) { Box("Tủ đầu giường",new Vector3(x,3.65f,-4.8f),new Vector3(.65f,.7f,.7f),wood); Box("Đèn đầu giường",new Vector3(x,4.2f,-4.8f),new Vector3(.35f,.4f,.35f),white,PrimitiveType.Cylinder); }
        Box("Tủ quần áo",new Vector3(-7.3f,4.6f,-1.4f),new Vector3(.9f,2.6f,2),cream); Fixture(new Vector3(-4.5f,6.2f,-2),true);
        Group("06 • STUDY & LIBRARY"); Label("PHÒNG LÀM VIỆC",new Vector3(-4.5f,5.9f,5.8f));
        Table(new Vector3(-5,3.3f,4.7f),new Vector2(2.7f,1.2f)); Chair(new Vector3(-5,3.3f,3.5f),180);
        Box("Màn hình máy tính",new Vector3(-5,4.6f,4.9f),new Vector3(1.1f,.65f,.07f),dark); Box("Bàn phím",new Vector3(-5,4.18f,4.4f),new Vector3(.8f,.04f,.27f),dark);
        for(int j=0;j<4;j++) { Box("Kệ sách",new Vector3(-7.5f,3.65f+j*.6f,3),new Vector3(.65f,.06f,3),wood); for(int i=0;i<9;i++) Book(new Vector3(-7.4f,3.87f+j*.6f,1.8f+i*.29f),i+j); }
        var key=Box("Chìa khóa cổng",new Vector3(-4,4.18f,4.4f),new Vector3(.35f,.07f,.13f),metal); Interactive(key,"Nhặt chìa khóa cổng",2); Fixture(new Vector3(-4,6.2f,2)); Plant(new Vector3(-2.3f,3.3f,5));
        Group("07 • BATHROOM"); Label("PHÒNG TẮM",new Vector3(5.5f,5.9f,5.8f)); Box("Sàn gạch",new Vector3(5.7f,3.32f,3.6f),new Vector3(4.5f,.03f,4.7f),white);
        Box("Bồn tắm",new Vector3(6.8f,3.65f,4.6f),new Vector3(1.5f,.65f,2.3f),white); Box("Nước bồn tắm",new Vector3(6.8f,4,4.6f),new Vector3(1.15f,.03f,1.95f),glass);
        Box("Tủ lavabo",new Vector3(4.2f,3.75f,5.2f),new Vector3(1.3f,.9f,.8f),wood); Box("Chậu lavabo",new Vector3(4.2f,4.25f,5.2f),new Vector3(1.35f,.15f,.85f),white); Box("Gương",new Vector3(4.2f,5.1f,5.82f),new Vector3(1.2f,1.3f,.04f),metal);
        Box("Bồn cầu",new Vector3(6.7f,3.65f,2),new Vector3(.65f,.6f,.9f),white,PrimitiveType.Cylinder); Box("Két nước",new Vector3(6.7f,4,1.6f),new Vector3(.65f,.85f,.25f),white); Fixture(new Vector3(5,6.2f,3));
        Group("08 • UPSTAIRS LOUNGE"); Table(new Vector3(5.5f,3.3f,-3.5f),new Vector2(1.3f,1)); Chair(new Vector3(5.5f,3.3f,-4.5f),180); Plant(new Vector3(7,3.3f,-5));
        Group("09 • LIGHTING & PLAYER"); var sun=new GameObject("Afternoon sun").AddComponent<Light>(); sun.type=LightType.Directional; sun.intensity=1.4f; sun.shadows=LightShadows.Soft; sun.transform.rotation=Quaternion.Euler(48,-30,0); RenderSettings.ambientMode=AmbientMode.Flat; RenderSettings.ambientLight=new Color(.55f,.6f,.68f);
        var player=new GameObject("PC Player • WASD + Mouse"); player.transform.position=new Vector3(0,.2f,-12); var cc=player.AddComponent<CharacterController>(); cc.height=1.8f; cc.center=new Vector3(0,.9f,0); cc.radius=.28f; cc.stepOffset=.3f; player.AddComponent<HousePlayer>();
        var camera=new GameObject("Player Camera"); camera.tag="MainCamera"; camera.transform.SetParent(player.transform); camera.transform.localPosition=new Vector3(0,1.65f,0); var cam=camera.AddComponent<Camera>(); cam.nearClipPlane=.05f; cam.farClipPlane=150; cam.fieldOfView=72; cam.backgroundColor=new Color(.55f,.73f,.84f); cam.clearFlags=CameraClearFlags.SolidColor; camera.AddComponent<AudioListener>();
        var flashlight=new GameObject("Flashlight"); flashlight.transform.SetParent(camera.transform,false); var fl=flashlight.AddComponent<Light>(); fl.type=LightType.Spot; fl.range=15; fl.spotAngle=65; fl.intensity=4; fl.enabled=false;
        EditorSceneManager.SaveScene(scene,ScenePath);
        if(previous.IsValid() && !previous.isDirty) EditorSceneManager.CloseScene(previous,true);
        // Preserve any other open scenes; the generated scene can be opened alone from the menu.
        EditorBuildSettings.scenes=new[]{new EditorBuildSettingsScene(ScenePath,true)};
        AssetDatabase.SaveAssets(); Debug.Log("HOUSE_BUILD_SUCCESS: "+ScenePath);
    }
}
