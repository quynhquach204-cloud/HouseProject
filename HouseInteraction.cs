using UnityEngine;
public class HouseInteraction : MonoBehaviour
{
    public string label; public int kind; public Light lamp; public bool signal; bool active; Quaternion closed;
    void Start()
    {
        closed = transform.localRotation;
        if (kind == 1 && lamp != null)
        {
            // The room lights were serialized as already on. Start them off
            // at runtime so the first press of E always turns a light on.
            lamp.enabled = false;
            active = false;
        }
    }
    public void Use(HousePlayer player)
    {
        if(kind==0) { active=!active; transform.localRotation=closed*Quaternion.Euler(0,active?95:0,0); }
        if(kind==1)
        {
            if(lamp==null) { player.Notice("Công tắc chưa được nối với đèn."); return; }
            active=!lamp.enabled;
            lamp.enabled=active;
            // Only the two ground-floor signal lamps count toward the gate.
            bool requiredSignal = signal && lamp.transform.position.y < 3.3f;
            if(requiredSignal) player.switches+=active?1:-1;
            player.Notice(active
                ? (requiredSignal ? "Đã bật đèn tín hiệu." : "Đã bật đèn phòng.")
                : (requiredSignal ? "Đã tắt đèn tín hiệu." : "Đã tắt đèn phòng."));
        }
        if(kind==2) { player.hasKey=true; player.Notice("Đã nhặt chìa khóa cổng. Hãy bật 2 đèn tín hiệu ở tầng trệt."); gameObject.SetActive(false); }
        if(kind==3) { if(player.hasKey && player.switches>=HousePlayer.RequiredSignals) { transform.localRotation=closed*Quaternion.Euler(0,95,0); player.won=true; Cursor.lockState=CursorLockMode.None; Cursor.visible=true; } else player.Notice("Cổng cần chìa khóa và 2 đèn tín hiệu tầng trệt đang bật."); }
        if(kind==4) player.Grab(GetComponent<Rigidbody>());
        if(kind==5) player.Notice("Ghi chú: bật đèn tín hiệu ở phòng khách và bếp. Chìa khóa ở bàn làm việc tầng hai.");
    }
}
