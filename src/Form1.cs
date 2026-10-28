#nullable disable
using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace QuanLyHocVienv
{
    public partial class Form1 : Form
    {
        private Label lblUsername, lblPassword;
        private TextBox txtUsername, txtPassword;
        private CheckBox chkShowPassword;
        private Button btnLogin, btnExit;
        private ErrorProvider errorProvider;

        private string connectionString = "Data Source=AppDB.db";

        public Form1()
        {
            InitializeComponentCustom();
            InitDatabase();
        }

        // Tự động tạo Cơ sở dữ liệu SQLite & Tài khoản mẫu
        private void InitDatabase()
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = @"
                    CREATE TABLE IF NOT EXISTS Users (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Username TEXT UNIQUE NOT NULL,
                        Password TEXT NOT NULL
                    );
                    INSERT OR IGNORE INTO Users (Username, Password) VALUES ('admin', '123456');
                ";
                using (var cmd = new SqliteCommand(sql, conn))
                {
                    cmd.ExecuteNonQuery();
                }
            }
        }

        // Dựng giao diện chuẩn theo bài 4.1
        private void InitializeComponentCustom()
        {
            this.lblUsername = new Label { Text = "Tên đăng nhập:", Location = new Point(30, 30), AutoSize = true };
            this.txtUsername = new TextBox { Location = new Point(140, 27), Size = new Size(180, 23) };

            this.lblPassword = new Label { Text = "Mật khẩu:", Location = new Point(30, 70), AutoSize = true };
            this.txtPassword = new TextBox { Location = new Point(140, 67), Size = new Size(180, 23), UseSystemPasswordChar = true };

            // CheckBox Ẩn / Hiện mật khẩu
            this.chkShowPassword = new CheckBox { Text = "Hiện mật khẩu", Location = new Point(140, 100), AutoSize = true };
            this.chkShowPassword.CheckedChanged += ChkShowPassword_CheckedChanged;

            // Nút Đăng nhập & Nút Thoát
            this.btnLogin = new Button { Text = "Đăng nhập", Location = new Point(140, 140), Size = new Size(85, 30) };
            this.btnLogin.Click += BtnLogin_Click;

            this.btnExit = new Button { Text = "Thoát", Location = new Point(235, 140), Size = new Size(85, 30) };
            this.btnExit.Click += BtnExit_Click;

            this.errorProvider = new ErrorProvider();

            // Gán AcceptButton (Bấm Enter) & CancelButton (Bấm Esc)
            this.AcceptButton = btnLogin;
            this.CancelButton = btnExit;

            // Cấu hình cửa sổ Form
            this.Text = "Đăng nhập Hệ thống";
            this.Size = new Size(380, 230);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            this.Controls.AddRange(new Control[] { lblUsername, txtUsername, lblPassword, txtPassword, chkShowPassword, btnLogin, btnExit });
        }

        // Tick chọn -> Hiện mật khẩu (UseSystemPasswordChar = false), Bỏ chọn -> Ẩn mật khẩu (*)
        private void ChkShowPassword_CheckedChanged(object sender, EventArgs e)
        {
            txtPassword.UseSystemPasswordChar = !chkShowPassword.Checked;
        }

        // Xử lý Đăng nhập & Validation bằng ErrorProvider
        private void BtnLogin_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            bool isValid = true;

            // Kiểm tra trống Tên đăng nhập
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                errorProvider.SetError(txtUsername, "Không được để trống Tên đăng nhập!");
                isValid = false;
            }

            // Kiểm tra trống Mật khẩu
            if (string.IsNullOrWhiteSpace(txtPassword.Text))
            {
                errorProvider.SetError(txtPassword, "Không được để trống Mật khẩu!");
                isValid = false;
            }

            if (!isValid) return;

            // Kiểm tra tài khoản trong CSDL SQLite
            if (AuthenticateUser(txtUsername.Text.Trim(), txtPassword.Text.Trim()))
            {
                MessageBox.Show("Đăng nhập thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                
                // Mở Form Đăng ký học viên (Form2)
                this.Hide();
                Form2 form2 = new Form2();
                form2.ShowDialog();
                this.Close();
            }
            else
            {
                MessageBox.Show("Tài khoản hoặc mật khẩu không chính xác!", "Lỗi đăng nhập", MessageBoxButtons.OK, MessageBoxIcon.Error);
                txtPassword.Clear();
                txtPassword.Focus();
            }
        }

        // Truy vấn CSDL SQLite
        private bool AuthenticateUser(string username, string password)
        {
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string query = "SELECT COUNT(*) FROM Users WHERE Username = @u AND Password = @p";
                using (var cmd = new SqliteCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@u", username);
                    cmd.Parameters.AddWithValue("@p", password);
                    long count = (long)cmd.ExecuteScalar();
                    return count > 0;
                }
            }
        }

        // Xử lý khi ấn nút Thoát hoặc phím Esc
        private void BtnExit_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Bạn có muốn thoát ứng dụng không?", "Xác nhận", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }
    }
}