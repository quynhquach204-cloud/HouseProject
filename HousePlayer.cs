using UnityEngine;
using UnityEngine.InputSystem;

public class HousePlayer : MonoBehaviour
{
    public const int RequiredSignals = 2;
    Camera eyes; CharacterController body; float pitch, velocity; bool started; HouseInteraction target; Rigidbody held;
    public bool hasKey; public int switches; public bool won;
    string message = "Bật 2 đèn tín hiệu ở tầng trệt và tìm chìa khóa trong phòng làm việc.";
    void Start() { body = GetComponent<CharacterController>(); eyes = GetComponentInChildren<Camera>(); }
    void Update()
    {
        var k = Keyboard.current; var m = Mouse.current; if(k == null || m == null) return;
        if(k.escapeKey.wasPressedThisFrame) { started = false; Cursor.lockState = CursorLockMode.None; Cursor.visible = true; }
        if(!started || won) return;
        Vector2 look = m.delta.ReadValue() * .09f; transform.Rotate(0, look.x, 0); pitch = Mathf.Clamp(pitch-look.y,-85,85); eyes.transform.localEulerAngles = new Vector3(pitch,0,0);
        Vector3 move = transform.right*((k.dKey.isPressed?1:0)-(k.aKey.isPressed?1:0)) + transform.forward*((k.wKey.isPressed?1:0)-(k.sKey.isPressed?1:0));
        if(body.isGrounded && velocity<0) velocity=-2;
        if(body.isGrounded && k.spaceKey.wasPressedThisFrame) velocity=5;
        velocity -= 18*Time.deltaTime; body.Move((move.normalized*(k.leftShiftKey.isPressed?5:3)+Vector3.up*velocity)*Time.deltaTime);
        if(transform.position.y < -10) { body.enabled=false; transform.position=new Vector3(0,1,-12); body.enabled=true; }
        target=FindInteraction();
        if(k.eKey.wasPressedThisFrame && target != null) target.Use(this);
        if(k.fKey.wasPressedThisFrame) { var lamp=eyes.GetComponentInChildren<Light>(); lamp.enabled=!lamp.enabled; }
        if(k.qKey.wasPressedThisFrame && held != null) Drop();
        if(held != null) held.MovePosition(eyes.transform.position+eyes.transform.forward*1.6f);
    }
    HouseInteraction FindInteraction()
    {
        Vector3 origin = eyes.transform.position;
        Vector3 forward = eyes.transform.forward;

        // Decorative furniture can have a collider in front of a switch or
        // key. Find the interactive hit along the sightline, not merely the
        // first physics hit.
        var hits = Physics.RaycastAll(origin, forward, 4f, ~0, QueryTriggerInteraction.Ignore);
        System.Array.Sort(hits, (a, b) => a.distance.CompareTo(b.distance));
        foreach (var hit in hits)
        {
            var interaction = hit.collider.GetComponentInParent<HouseInteraction>();
            if (interaction != null && interaction.gameObject.activeInHierarchy) return interaction;
        }

        // Small switches and keys need not be aimed at pixel-perfectly.
        // Nearby items in front of the player are reachable with E.
        HouseInteraction best = null;
        float bestScore = float.NegativeInfinity;
        foreach (var collider in Physics.OverlapSphere(origin, 2.4f, ~0, QueryTriggerInteraction.Ignore))
        {
            var interaction = collider.GetComponentInParent<HouseInteraction>();
            if (interaction == null || !interaction.gameObject.activeInHierarchy) continue;
            Vector3 toward = collider.bounds.ClosestPoint(origin) - origin;
            float distance = toward.magnitude;
            if (distance < .01f) toward = collider.bounds.center - origin;
            float facing = Vector3.Dot(forward, toward.normalized);
            if (facing < .25f) continue;
            float score = facing * 3f - distance;
            if (score <= bestScore) continue;
            bestScore = score;
            best = interaction;
        }
        return best;
    }
    public void Notice(string s) { message=s; }
    public void Grab(Rigidbody item) { if(held != null) Drop(); held=item; held.isKinematic=true; foreach(var c in held.GetComponentsInChildren<Collider>()) Physics.IgnoreCollision(c,body,true); }
    void Drop() { foreach(var c in held.GetComponentsInChildren<Collider>()) Physics.IgnoreCollision(c,body,false); held.isKinematic=false; held=null; }
    void OnDisable() { Cursor.lockState=CursorLockMode.None; Cursor.visible=true; }
    void OnGUI()
    {
        GUI.skin.label.fontSize=18; GUI.skin.button.fontSize=20; GUI.color=Color.white;
        if(!started || won) {
            Rect panel=new Rect(Screen.width/2-290,Screen.height/2-190,580,380); GUI.Box(panel,"");
            GUI.Label(new Rect(panel.x+30,panel.y+30,530,50),won?"HOÀN THÀNH • BẠN ĐÃ MỞ CỔNG!":"NHÀ VƯỜN • HAI TẦNG");
            GUI.Label(new Rect(panel.x+30,panel.y+90,520,150),"W A S D: di chuyển    Chuột: nhìn xung quanh\nShift: chạy    Space: nhảy    E: tương tác\nQ: thả đồ vật    F: đèn pin    Esc: tạm dừng\n\nNhiệm vụ: bật 2 đèn tín hiệu tầng trệt, tìm chìa khóa\nrồi mở cổng sân vườn.");
            if(GUI.Button(new Rect(panel.x+110,panel.y+280,360,55),won?"TIẾP TỤC KHÁM PHÁ":"VÀO NHÀ / TIẾP TỤC")) { won=false; started=true; Cursor.lockState=CursorLockMode.Locked; Cursor.visible=false; }
            return;
        }
        GUI.Box(new Rect(20,20,520,105),""); GUI.Label(new Rect(35,30,490,35),"NHÀ VƯỜN    |    KHÁM PHÁ & TƯƠNG TÁC");
        GUI.Label(new Rect(35,68,480,45),"Đèn tín hiệu: "+switches+"/"+RequiredSignals+"     Chìa khóa: "+(hasKey?"Đã tìm thấy":"Chưa có"));
        GUI.Label(new Rect(Screen.width/2-6,Screen.height/2-16,30,30),"+");
        if(target!=null) GUI.Box(new Rect(Screen.width/2-230,Screen.height/2+45,460,38),"[E] "+target.label);
        GUI.Box(new Rect(20,Screen.height-90,Screen.width-40,65),message+"\nE tương tác  •  Q thả đồ  •  F đèn pin  •  Esc menu");
    }
}
