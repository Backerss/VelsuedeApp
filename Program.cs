using System.Windows.Forms;

namespace VelsuedeApp;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // ── ตรวจสอบ / สร้างฐานข้อมูลก่อนเปิดโปรแกรม ──────────
        while (true)
        {
            if (DatabaseInitializer.EnsureDatabase(out string err))
                break;  // สำเร็จ → เปิด Form ปกติ

            // ล้มเหลว → แจ้งเตือนพร้อมให้ลองใหม่
            var result = MessageBox.Show(
                $"ไม่สามารถเชื่อมต่อหรือสร้างฐานข้อมูลได้\n\n" +
                $"รายละเอียด: {err}\n\n" +
                "กด  Retry  เพื่อลองใหม่\n" +
                "กด  Cancel  เพื่อปิดโปรแกรม",
                "ข้อผิดพลาดฐานข้อมูล",
                MessageBoxButtons.RetryCancel,
                MessageBoxIcon.Error);

            if (result == DialogResult.Cancel)
                return;   // ออกจากโปรแกรม
        }

        Application.Run(new Form1());
    }
}
