using UnityEngine;
/// คลาสสำหรับจัดการและควบคุมการสลับ State
public class stateMacines
{
    // ตัวแปรเก็บ State ที่กำลังทำงานอยู่ในปัจจุบัน
    public State state;

    public void set(State newState , bool forceReset = false )
    {
        // จะเปลี่ยนสถานะก็ต่อเมื่อ สถานะใหม่ไม่ใช่สถานะเดิม หรือ ถูกบังคับให้รีเซ็ต (forceReset)
        if(state != newState || forceReset)
        {
            // 1. เรียกใช้งานฟังก์ชัน Exit ของสถานะเดิม (ถ้ามี)
            state?.Exit();
            
            // 2. กำหนดให้สถานะปัจจุบันกลายเป็นสถานะใหม่
            state = newState;
            
            // 3. รีเซ็ตค่าเวลาและการทำงาน initialise ของสถานะใหม่ (ถ้ามี)
            state?.initialise();
            
            // 4. เรียกใช้งานฟังก์ชัน Enter ของสถานะใหม่ (ถ้ามี)
            state?.Enter();
        }
    }
}