using UnityEngine;

public abstract class Core : MonoBehaviour
{
    public Animator animator;
    public Rigidbody2D rb;
    
    // ตัวแปร State Machine หลักที่ใช้ควบคุมสถานะของ Core นี้
    public stateMacines stateMacines;

    /// ฟังก์ชันสำหรับเซ็ตอัปค่าเริ่มต้น
    public void setupInstances()
    {
        // สร้างอินสแตนซ์ของ State Machine ใหม่
        stateMacines = new stateMacines();

        // ค้นหา Component ประเภท State ทั้งหมดที่อยู่ใน GameObject ลูก (Child Objects)
        State[] allChidState = GetComponentsInChildren<State>();
        
        // วนลูปเพื่อส่งค่าตัวเอง (Core) กลับไปให้ทุกๆ State ได้รู้จัก
        foreach (State state in allChidState)
        {
            state.SetCore(this);
        }
    }
}