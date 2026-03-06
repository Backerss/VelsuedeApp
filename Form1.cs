using System;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace VelsuedeApp
{
    // ═══════════════════════════════════════════════════════════════
    //  MAIN FORM
    // ═══════════════════════════════════════════════════════════════
    public class Form1 : Form
    {
        private DataGridView dgvEmployees = null!;
        private TextBox      txtSearch    = null!;
        private Label        lblStatus    = null!;
        private Panel        dotPanel     = null!;

        private readonly string connStr =
            "Server=localhost;Port=3306;Database=Velsuede;Uid=root;Pwd=;CharSet=utf8;";

        public Form1()
        {
            BuildUI();
            LoadData();
        }

        // ──────────────────────────────────────────────────────────
        //  UI BUILD
        // ──────────────────────────────────────────────────────────
        private void BuildUI()
        {
            Text            = "Velsuede — ระบบจัดการข้อมูลพนักงาน";
            MinimumSize     = new Size(900, 600);
            Size            = new Size(1100, 700);
            StartPosition   = FormStartPosition.CenterScreen;
            BackColor       = Color.FromArgb(10, 14, 26);
            Font            = new Font("Segoe UI", 9f);

            // ── HEADER ──────────────────────────────────────────
            var header = new GradientPanel(
                Color.FromArgb(22, 32, 60),
                Color.FromArgb(14, 20, 42))
            {
                Dock   = DockStyle.Top,
                Height = 64
            };

            // left indigo bar inside header
            header.Controls.Add(new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 5,
                BackColor = Color.FromArgb(99, 102, 241)
            });

            var lblTitle = new Label
            {
                Text      = "  👥  ระบบจัดการข้อมูลพนักงาน",
                Font      = new Font("Segoe UI", 16f, FontStyle.Bold),
                ForeColor = Color.White,
                Dock      = DockStyle.Left,
                AutoSize  = false,
                Width     = 500,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            header.Controls.Add(lblTitle);

            var lblSub = new Label
            {
                Text      = "VELSUEDE  ·  HR Management System",
                Font      = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(130, 148, 200),
                Dock      = DockStyle.Right,
                AutoSize  = false,
                Width     = 280,
                TextAlign = ContentAlignment.MiddleRight,
                BackColor = Color.Transparent,
                Padding   = new Padding(0, 0, 16, 0)
            };
            header.Controls.Add(lblSub);

            // header bottom line
            var headerLine = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 2,
                BackColor = Color.FromArgb(99, 102, 241)
            };

            // ── TOOLBAR ─────────────────────────────────────────
            var toolbar = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 60,
                BackColor = Color.FromArgb(16, 22, 40),
                Padding   = new Padding(12, 10, 12, 10)
            };

            // Search box container (left side)
            var searchBox = new Panel
            {
                Dock      = DockStyle.Left,
                Width     = 300,
                BackColor = Color.Transparent
            };
            txtSearch = new TextBox
            {
                PlaceholderText = "🔍  ค้นหาพนักงาน...",
                Dock            = DockStyle.Left,
                Width           = 260,
                Height          = 36,
                BackColor       = Color.FromArgb(26, 36, 64),
                ForeColor       = Color.FromArgb(200, 210, 250),
                BorderStyle     = BorderStyle.FixedSingle,
                Font            = new Font("Segoe UI", 10f)
            };
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) LoadData(txtSearch.Text.Trim()); };
            searchBox.Controls.Add(txtSearch);

            // Buttons (right side)
            var btnBox = new FlowLayoutPanel
            {
                Dock          = DockStyle.Right,
                AutoSize      = true,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents  = false,
                BackColor     = Color.Transparent,
                Padding       = new Padding(0)
            };

            var btnSearch2 = MakeBtn("🔍  ค้นหา",  Color.FromArgb(99, 102, 241), Color.FromArgb(79, 82, 221));
            var btnAdd     = MakeBtn("➕  เพิ่ม",   Color.FromArgb(16, 185, 129), Color.FromArgb(5,  150, 105));
            var btnEdit    = MakeBtn("✏  แก้ไข",   Color.FromArgb(245, 158, 11), Color.FromArgb(217, 119, 6));
            var btnDelete  = MakeBtn("🗑  ลบ",      Color.FromArgb(239, 68, 68),  Color.FromArgb(220, 38, 38));
            var btnClear   = MakeBtn("✖  รีเซ็ต",  Color.FromArgb(55, 65, 100),  Color.FromArgb(35, 45, 80));

            btnSearch2.Click += (s, e) => LoadData(txtSearch.Text.Trim());
            btnAdd.Click     += BtnAdd_Click;
            btnEdit.Click    += BtnEdit_Click;
            btnDelete.Click  += BtnDelete_Click;
            btnClear.Click   += (s, e) => { txtSearch.Clear(); LoadData(); };

            btnBox.Controls.AddRange(new Control[] { btnSearch2, btnAdd, btnEdit, btnDelete, btnClear });

            toolbar.Controls.Add(btnBox);
            toolbar.Controls.Add(searchBox);

            // ── GRID WRAPPER ─────────────────────────────────────
            var gridWrapper = new Panel
            {
                Dock    = DockStyle.Fill,
                Padding = new Padding(12, 8, 12, 8),
                BackColor = Color.FromArgb(10, 14, 26)
            };

            var gridHeader = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 38,
                BackColor = Color.FromArgb(18, 26, 50),
                Padding   = new Padding(12, 0, 0, 0)
            };
            var lblGridTitle = new Label
            {
                Text      = "📋  รายการพนักงานทั้งหมด",
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.FromArgb(129, 140, 248),
                Dock      = DockStyle.Left,
                AutoSize  = false,
                Width     = 400,
                TextAlign = ContentAlignment.MiddleLeft,
                BackColor = Color.Transparent
            };
            gridHeader.Controls.Add(lblGridTitle);
            var gridHeaderLine = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 1,
                BackColor = Color.FromArgb(40, 55, 90)
            };

            dgvEmployees = new DataGridView
            {
                Dock                    = DockStyle.Fill,
                BackgroundColor         = Color.FromArgb(10, 14, 26),
                BorderStyle             = BorderStyle.None,
                RowHeadersVisible       = false,
                AllowUserToAddRows      = false,
                ReadOnly                = true,
                AutoSizeColumnsMode     = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode           = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect             = false,
                GridColor               = Color.FromArgb(28, 38, 65),
                CellBorderStyle         = DataGridViewCellBorderStyle.SingleHorizontal,
                Font                    = new Font("Segoe UI", 9.5f),
                ScrollBars              = ScrollBars.Both,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight     = 42
            };

            // Header style
            dgvEmployees.ColumnHeadersDefaultCellStyle.BackColor          = Color.FromArgb(44, 40, 100);
            dgvEmployees.ColumnHeadersDefaultCellStyle.ForeColor          = Color.FromArgb(200, 208, 255);
            dgvEmployees.ColumnHeadersDefaultCellStyle.Font               = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            dgvEmployees.ColumnHeadersDefaultCellStyle.SelectionBackColor = Color.FromArgb(44, 40, 100);
            dgvEmployees.ColumnHeadersDefaultCellStyle.Padding            = new Padding(10, 0, 0, 0);
            dgvEmployees.EnableHeadersVisualStyles = false;

            // Row style
            dgvEmployees.DefaultCellStyle.BackColor          = Color.FromArgb(10, 14, 26);
            dgvEmployees.DefaultCellStyle.ForeColor          = Color.FromArgb(210, 218, 245);
            dgvEmployees.DefaultCellStyle.SelectionBackColor = Color.FromArgb(60, 58, 130);
            dgvEmployees.DefaultCellStyle.SelectionForeColor = Color.White;
            dgvEmployees.DefaultCellStyle.Padding            = new Padding(10, 0, 0, 0);
            dgvEmployees.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(14, 20, 40);
            dgvEmployees.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(210, 218, 245);
            dgvEmployees.RowTemplate.Height = 38;

            dgvEmployees.CellDoubleClick += DgvEmployees_CellDoubleClick;

            var gridInner = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(18, 26, 50) };
            gridInner.Controls.Add(dgvEmployees);
            gridInner.Controls.Add(gridHeaderLine);
            gridInner.Controls.Add(gridHeader);

            gridWrapper.Controls.Add(gridInner);

            // ── STATUS BAR ───────────────────────────────────────
            var statusBar = new Panel
            {
                Dock      = DockStyle.Bottom,
                Height    = 34,
                BackColor = Color.FromArgb(14, 18, 36),
                Padding   = new Padding(14, 0, 10, 0)
            };
            var statusLine = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(40, 55, 90) };
            statusBar.Controls.Add(statusLine);

            dotPanel = new Panel
            {
                Size      = new Size(10, 10),
                Location  = new Point(16, 12),
                BackColor = Color.FromArgb(16, 185, 129)
            };
            dotPanel.Region = Region.FromHrgn(CreateRoundRectRgn(0, 0, 10, 10, 10, 10));

            lblStatus = new Label
            {
                Text      = "พร้อมใช้งาน",
                Location  = new Point(32, 9),
                AutoSize  = true,
                ForeColor = Color.FromArgb(120, 140, 190),
                Font      = new Font("Segoe UI", 8.5f),
                BackColor = Color.Transparent
            };
            statusBar.Controls.Add(dotPanel);
            statusBar.Controls.Add(lblStatus);

            // ── ASSEMBLE (bottom-up for Dock to work correctly) ──
            Controls.Add(gridWrapper);   // Fill — must be added before Top/Bottom
            Controls.Add(toolbar);
            Controls.Add(headerLine);
            Controls.Add(header);
            Controls.Add(statusBar);
        }

        // ──────────────────────────────────────────────────────────
        //  BUTTON FACTORY
        // ──────────────────────────────────────────────────────────
        private static Button MakeBtn(string text, Color bg, Color hover)
        {
            var b = new Button
            {
                Text      = text,
                Size      = new Size(116, 38),
                Margin    = new Padding(0, 0, 6, 0),
                BackColor = bg,
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 9f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            b.FlatAppearance.BorderSize          = 0;
            b.FlatAppearance.MouseOverBackColor  = hover;
            return b;
        }

        // ──────────────────────────────────────────────────────────
        //  DATABASE
        // ──────────────────────────────────────────────────────────
        private MySqlConnection GetConn() => new MySqlConnection(connStr);

        private void LoadData(string kw = "")
        {
            try
            {
                using var conn = GetConn();
                conn.Open();
                string q = @"SELECT emp_id AS `รหัสพนักงาน`,
                                    first_name AS `ชื่อ`,
                                    last_name  AS `นามสกุล`,
                                    position   AS `ตำแหน่งงาน`,
                                    department AS `แผนก`,
                                    phone      AS `เบอร์โทรศัพท์`,
                                    status     AS `สถานะ`
                             FROM employees";

                if (!string.IsNullOrWhiteSpace(kw))
                    q += " WHERE emp_id LIKE @kw OR first_name LIKE @kw OR last_name LIKE @kw" +
                         " OR position LIKE @kw OR department LIKE @kw OR phone LIKE @kw";

                var cmd = new MySqlCommand(q, conn);
                if (!string.IsNullOrWhiteSpace(kw))
                    cmd.Parameters.AddWithValue("@kw", $"%{kw}%");

                var dt = new DataTable();
                new MySqlDataAdapter(cmd).Fill(dt);
                dgvEmployees.DataSource = dt;
                SetStatus($"✔  พบ {dt.Rows.Count} รายการ", ok: true);
            }
            catch (Exception ex) { SetStatus("⚠  " + ex.Message, ok: false); }
        }

        private void SetStatus(string msg, bool ok = true)
        {
            lblStatus.Text      = msg;
            dotPanel.BackColor  = ok ? Color.FromArgb(16, 185, 129) : Color.FromArgb(239, 68, 68);
        }

        // ──────────────────────────────────────────────────────────
        //  EVENTS
        // ──────────────────────────────────────────────────────────
        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            using var dlg = new EmployeeDialog(connStr);
            if (dlg.ShowDialog(this) == DialogResult.OK) LoadData();
        }

        private void BtnEdit_Click(object? sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow == null) { Warn("กรุณาเลือกแถวที่ต้องการแก้ไขก่อน"); return; }
            string id = dgvEmployees.CurrentRow.Cells["รหัสพนักงาน"].Value?.ToString() ?? "";
            using var dlg = new EmployeeDialog(connStr, id);
            if (dlg.ShowDialog(this) == DialogResult.OK) LoadData();
        }

        private void BtnDelete_Click(object? sender, EventArgs e)
        {
            if (dgvEmployees.CurrentRow == null) { Warn("กรุณาเลือกแถวที่ต้องการลบก่อน"); return; }
            string id = dgvEmployees.CurrentRow.Cells["รหัสพนักงาน"].Value?.ToString() ?? "";
            if (MessageBox.Show($"ยืนยันลบรหัส: {id} ?", "ยืนยัน",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes) return;
            try
            {
                using var conn = GetConn(); conn.Open();
                var cmd = new MySqlCommand("DELETE FROM employees WHERE emp_id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                cmd.ExecuteNonQuery();
                LoadData();
            }
            catch (Exception ex) { ShowError(ex.Message); }
        }

        private void DgvEmployees_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            string id = dgvEmployees.Rows[e.RowIndex].Cells["รหัสพนักงาน"].Value?.ToString() ?? "";
            using var dlg = new EmployeeDialog(connStr, id);
            if (dlg.ShowDialog(this) == DialogResult.OK) LoadData();
        }

        private static void Warn(string msg)    => MessageBox.Show(msg, "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
        private static void ShowError(string m) => MessageBox.Show("เกิดข้อผิดพลาด:\n" + m, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

        [DllImport("Gdi32.dll")]
        private static extern IntPtr CreateRoundRectRgn(int l, int t, int r, int b, int we, int he);
    }

    // ═══════════════════════════════════════════════════════════════
    //  EMPLOYEE DIALOG  (Add / Edit)
    // ═══════════════════════════════════════════════════════════════
    public class EmployeeDialog : Form
    {
        private readonly string  connStr;
        private readonly string? editId;   // null = Add mode

        private TextBox txtEmpId = null!, txtFirstName = null!, txtLastName  = null!,
                        txtPos   = null!, txtDept      = null!, txtPhone     = null!;
        private ComboBox cmbStat = null!;

        public EmployeeDialog(string connStr, string? editId = null)
        {
            this.connStr = connStr;
            this.editId  = editId;
            BuildDialog();
            if (editId != null) LoadEmployee(editId);
        }

        private void BuildDialog()
        {
            Text          = editId == null ? "เพิ่มพนักงานใหม่" : $"แก้ไขพนักงาน — {editId}";
            Size          = new Size(480, 430);
            MinimumSize   = Size;
            MaximumSize   = Size;
            StartPosition = FormStartPosition.CenterParent;
            BackColor     = Color.FromArgb(18, 24, 48);
            Font          = new Font("Segoe UI", 9.5f);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox   = false;

            // Title strip
            var strip = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(28, 36, 70) };
            strip.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 3, BackColor = Color.FromArgb(99, 102, 241) });
            strip.Controls.Add(new Label
            {
                Text      = editId == null ? "➕  เพิ่มพนักงานใหม่" : "✏  แก้ไขข้อมูลพนักงาน",
                Dock      = DockStyle.Fill,
                ForeColor = Color.WhiteSmoke,
                Font      = new Font("Segoe UI", 11f, FontStyle.Bold),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding   = new Padding(14, 0, 0, 0),
                BackColor = Color.Transparent
            });

            // Fields panel
            var body = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent, Padding = new Padding(20, 14, 20, 10) };

            int labelH = 18, inputH = 34, gap = 14, rowH = labelH + inputH + gap;

            // Layout: label text, column (0=left,1=right), row index
            (string Lbl, int Col, int Row)[] layout =
            {
                ("รหัสพนักงาน *",  0, 0),
                ("ชื่อ *",          1, 0),
                ("นามสกุล *",       0, 1),
                ("ตำแหน่งงาน",     1, 1),
                ("แผนก",            0, 2),
                ("เบอร์โทรศัพท์",   1, 2),
            };

            int[] xPos = { 20, 240 };
            int[] rowY = { 14, 14 + rowH, 14 + rowH * 2 };
            int   colW = 200;

            var boxes = new TextBox[layout.Length];
            for (int i = 0; i < layout.Length; i++)
            {
                var (lbl, col, r) = layout[i];
                int x = xPos[col], y = rowY[r];
                var lLabel = new Label
                {
                    Text      = lbl,
                    Location  = new Point(x, y),
                    AutoSize  = false,
                    Size      = new Size(colW, labelH),
                    ForeColor = Color.FromArgb(130, 148, 200),
                    Font      = new Font("Segoe UI", 8f, FontStyle.Bold),
                    BackColor = Color.Transparent
                };
                boxes[i] = new TextBox
                {
                    Location    = new Point(x, y + labelH + 2),
                    Size        = new Size(colW, inputH),
                    BackColor   = Color.FromArgb(10, 14, 28),
                    ForeColor   = Color.FromArgb(210, 220, 255),
                    BorderStyle = BorderStyle.FixedSingle,
                    Font        = new Font("Segoe UI", 10f)
                };
                body.Controls.Add(lLabel);
                body.Controls.Add(boxes[i]);
            }
            txtEmpId     = boxes[0]; txtFirstName = boxes[1]; txtLastName = boxes[2];
            txtPos       = boxes[3]; txtDept      = boxes[4]; txtPhone    = boxes[5];

            // Status field (small, below grid)
            int statY = rowY[2] + labelH + inputH + gap + 4;
            body.Controls.Add(new Label
            {
                Text      = "สถานะพนักงาน",
                Location  = new Point(20, statY),
                AutoSize  = false,
                Size      = new Size(300, labelH),
                ForeColor = Color.FromArgb(130, 148, 200),
                Font      = new Font("Segoe UI", 8f, FontStyle.Bold),
                BackColor = Color.Transparent
            });
            cmbStat = new ComboBox
            {
                Location      = new Point(20, statY + labelH + 2),
                Size          = new Size(220, inputH),
                BackColor     = Color.FromArgb(10, 14, 28),
                ForeColor     = Color.FromArgb(210, 220, 255),
                FlatStyle     = FlatStyle.Flat,
                Font          = new Font("Segoe UI", 10f),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbStat.Items.Add("🟢  กำลังทำงาน");
            cmbStat.Items.Add("🔴  ลาออกแล้ว");
            cmbStat.SelectedIndex = 0;   // default = ทำงาน
            body.Controls.Add(cmbStat);

            if (editId != null)
            {
                txtEmpId.ReadOnly  = true;
                txtEmpId.BackColor = Color.FromArgb(20, 26, 50);
                txtEmpId.ForeColor = Color.FromArgb(100, 115, 165);
            }

            // ── Bottom buttons ────────────────────────────────
            var btnBar = new Panel { Dock = DockStyle.Bottom, Height = 56, BackColor = Color.FromArgb(14, 20, 42) };
            btnBar.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(40, 55, 90) });

            var btnSave = new Button
            {
                Text      = editId == null ? "➕  บันทึก" : "✔  อัปเดต",
                Size      = new Size(130, 38),
                Location  = new Point(188, 10),
                BackColor = editId == null ? Color.FromArgb(16, 185, 129) : Color.FromArgb(99, 102, 241),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor    = Cursors.Hand
            };
            btnSave.FlatAppearance.BorderSize = 0;

            var btnCancel = new Button
            {
                Text      = "ยกเลิก",
                Size      = new Size(100, 38),
                Location  = new Point(328, 10),
                BackColor = Color.FromArgb(55, 65, 100),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                Font      = new Font("Segoe UI", 10f),
                Cursor    = Cursors.Hand,
                DialogResult = DialogResult.Cancel
            };
            btnCancel.FlatAppearance.BorderSize = 0;

            btnSave.Click += BtnSave_Click;
            btnBar.Controls.Add(btnSave);
            btnBar.Controls.Add(btnCancel);

            Controls.Add(body);
            Controls.Add(strip);
            Controls.Add(btnBar);
        }

        private void LoadEmployee(string id)
        {
            try
            {
                using var conn = new MySqlConnection(connStr); conn.Open();
                var cmd = new MySqlCommand(
                    "SELECT * FROM employees WHERE emp_id=@id", conn);
                cmd.Parameters.AddWithValue("@id", id);
                using var r = cmd.ExecuteReader();
                if (!r.Read()) return;
                txtEmpId.Text     = r["emp_id"]?.ToString()     ?? "";
                txtFirstName.Text = r["first_name"]?.ToString() ?? "";
                txtLastName.Text  = r["last_name"]?.ToString()  ?? "";
                txtPos.Text       = r["position"]?.ToString()   ?? "";
                txtDept.Text      = r["department"]?.ToString() ?? "";
                txtPhone.Text     = r["phone"]?.ToString()      ?? "";
                // status: 1 = ทำงาน (index 0), 0 = ลาออก (index 1)
                cmbStat.SelectedIndex = (r["status"]?.ToString() == "0") ? 1 : 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("โหลดข้อมูลไม่สำเร็จ:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BtnSave_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtEmpId.Text) ||
                string.IsNullOrWhiteSpace(txtFirstName.Text) ||
                string.IsNullOrWhiteSpace(txtLastName.Text))
            {
                MessageBox.Show("กรุณากรอก รหัสพนักงาน / ชื่อ / นามสกุล ให้ครบ",
                    "แจ้งเตือน", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            // cmbStat is always valid (DropDownList), no extra validation needed

            try
            {
                using var conn = new MySqlConnection(connStr); conn.Open();
                MySqlCommand cmd;
                if (editId == null)
                {
                    cmd = new MySqlCommand(@"INSERT INTO employees
                        (emp_id,first_name,last_name,position,department,phone,status)
                        VALUES(@id,@fn,@ln,@pos,@dep,@ph,@st)", conn);
                }
                else
                {
                    cmd = new MySqlCommand(@"UPDATE employees SET
                        first_name=@fn,last_name=@ln,position=@pos,
                        department=@dep,phone=@ph,status=@st
                        WHERE emp_id=@id", conn);
                }
                cmd.Parameters.AddWithValue("@id",  txtEmpId.Text.Trim());
                cmd.Parameters.AddWithValue("@fn",  txtFirstName.Text.Trim());
                cmd.Parameters.AddWithValue("@ln",  txtLastName.Text.Trim());
                cmd.Parameters.AddWithValue("@pos", txtPos.Text.Trim());
                cmd.Parameters.AddWithValue("@dep", txtDept.Text.Trim());
                cmd.Parameters.AddWithValue("@ph",  txtPhone.Text.Trim());
                // index 0 = กำลังทำงาน → 1, index 1 = ลาออกแล้ว → 0
                cmd.Parameters.AddWithValue("@st",  cmbStat.SelectedIndex == 0 ? "1" : "0");
                cmd.ExecuteNonQuery();

                MessageBox.Show(editId == null ? "เพิ่มข้อมูลสำเร็จ ✔" : "อัปเดตสำเร็จ ✔",
                    "สำเร็จ", MessageBoxButtons.OK, MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show("เกิดข้อผิดพลาด:\n" + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  DATABASE INITIALIZER
    //  รันก่อน Form เปิด — สร้าง DB / ตาราง / คอลัมน์ที่ขาด
    // ═══════════════════════════════════════════════════════════════
    public static class DatabaseInitializer
    {
        // Connection ที่ไม่ระบุ Database (ใช้ตอนสร้าง DB ใหม่)
        private const string ServerConn =
            "Server=localhost;Port=3306;Uid=root;Pwd=;CharSet=utf8mb4;";

        private const string FullConn =
            "Server=localhost;Port=3306;Database=Velsuede;Uid=root;Pwd=;CharSet=utf8mb4;";

        // คอลัมน์ที่ต้องมีทั้งหมด: (ชื่อ, DDL ต่อท้าย ALTER TABLE ADD COLUMN)
        private static readonly (string Name, string Definition)[] RequiredColumns =
        {
            ("emp_id",     "VARCHAR(20)  NOT NULL"),
            ("first_name", "VARCHAR(100) NOT NULL"),
            ("last_name",  "VARCHAR(100) NOT NULL"),
            ("position",   "VARCHAR(100) NOT NULL DEFAULT ''"),
            ("department", "VARCHAR(100) NOT NULL DEFAULT ''"),
            ("phone",      "VARCHAR(20)  NOT NULL DEFAULT ''"),
            ("status",     "TINYINT(1)   NOT NULL DEFAULT 1"),
        };

        public static bool EnsureDatabase(out string errorMessage)
        {
            errorMessage = "";
            try
            {
                // ── 1. สร้าง Database ──────────────────────────────
                using (var conn = new MySqlConnection(ServerConn))
                {
                    conn.Open();
                    Execute(conn, @"CREATE DATABASE IF NOT EXISTS Velsuede
                                    CHARACTER SET utf8mb4
                                    COLLATE utf8mb4_unicode_ci;");
                }

                // ── 2. สร้างตาราง employees ────────────────────────
                using (var conn = new MySqlConnection(FullConn))
                {
                    conn.Open();
                    Execute(conn, @"CREATE TABLE IF NOT EXISTS employees (
                        emp_id      VARCHAR(20)  NOT NULL,
                        first_name  VARCHAR(100) NOT NULL,
                        last_name   VARCHAR(100) NOT NULL,
                        position    VARCHAR(100) NOT NULL DEFAULT '',
                        department  VARCHAR(100) NOT NULL DEFAULT '',
                        phone       VARCHAR(20)  NOT NULL DEFAULT '',
                        status      TINYINT(1)   NOT NULL DEFAULT 1,
                        PRIMARY KEY (emp_id)
                    ) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4;");

                    // ── 3. ตรวจสอบคอลัมน์ที่ขาด ──────────────────
                    var existing = GetExistingColumns(conn);
                    foreach (var (name, def) in RequiredColumns)
                    {
                        if (!existing.Contains(name.ToLower()))
                        {
                            Execute(conn,
                                $"ALTER TABLE employees ADD COLUMN {name} {def};");
                        }
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                errorMessage = ex.Message;
                return false;
            }
        }

        private static System.Collections.Generic.HashSet<string> GetExistingColumns(MySqlConnection conn)
        {
            var set = new System.Collections.Generic.HashSet<string>(StringComparer.OrdinalIgnoreCase);
            using var cmd = new MySqlCommand(
                "SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE TABLE_SCHEMA='Velsuede' AND TABLE_NAME='employees';", conn);
            using var r = cmd.ExecuteReader();
            while (r.Read()) set.Add(r.GetString(0));
            return set;
        }

        private static void Execute(MySqlConnection conn, string sql)
        {
            using var cmd = new MySqlCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }
    }

    // ═══════════════════════════════════════════════════════════════
    //  GRADIENT PANEL HELPER
    // ═══════════════════════════════════════════════════════════════
    public class GradientPanel : Panel
    {
        private readonly Color _a, _b;
        public GradientPanel(Color a, Color b) { _a = a; _b = b; }
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            if (ClientRectangle.Width <= 0 || ClientRectangle.Height <= 0) return;
            using var br = new LinearGradientBrush(
                ClientRectangle, _a, _b, LinearGradientMode.Horizontal);
            e.Graphics.FillRectangle(br, ClientRectangle);
        }
    }
}
