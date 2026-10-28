#nullable disable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Data.Sqlite;

namespace QuanLyHocVienv
{
    public class CourseItem
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; }
    }

    public partial class Form2 : Form
    {
        private Label lblName, lblPhone, lblBirthDate, lblCourse;
        private TextBox txtName;
        private MaskedTextBox mtxtPhone;
        private DateTimePicker dtpBirthDate;
        private ComboBox cboCourse;
        private Button btnRegister;

        private string connectionString = "Data Source=AppDB.db";

        public Form2()
        {
            InitializeComponentCustom();
            LoadCourses();
        }

        private void InitializeComponentCustom()
        {
            this.lblName = new Label { Text = "Họ và tên:", Location = new Point(30, 30), AutoSize = true };
            this.txtName = new TextBox { Location = new Point(140, 27), Size = new Size(200, 23) };

            // mtxtPhone: Mask SDT (000) 000-0000
            this.lblPhone = new Label { Text = "Số điện thoại:", Location = new Point(30, 70), AutoSize = true };
            this.mtxtPhone = new MaskedTextBox { Location = new Point(140, 67), Size = new Size(200, 23), Mask = "(000) 000-0000" };

            // dtpBirthDate: Format Short (dd/MM/yyyy)
            this.lblBirthDate = new Label { Text = "Ngày sinh:", Location = new Point(30, 110), AutoSize = true };
            this.dtpBirthDate = new DateTimePicker 
            { 
                Location = new Point(140, 107), 
                Size = new Size(200, 23),
                Format = DateTimePickerFormat.Custom,
                CustomFormat = "dd/MM/yyyy"
            };

            // cboCourse: Nạp danh sách Khóa học (DisplayMember, ValueMember)
            this.lblCourse = new Label { Text = "Khóa học:", Location = new Point(30, 150), AutoSize = true };
            this.cboCourse = new ComboBox { Location = new Point(140, 147), Size = new Size(200, 23), DropDownStyle = ComboBoxStyle.DropDownList };

            // Nút "Đăng ký"
            this.btnRegister = new Button { Text = "Đăng ký", Location = new Point(140, 190), Size = new Size(100, 32) };
            this.btnRegister.Click += BtnRegister_Click;

            this.Text = "Bài 4.2 & 5.2: Form Đăng Ký Học Viên";
            this.Size = new Size(400, 280);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;

            this.Controls.AddRange(new Control[] { lblName, txtName, lblPhone, mtxtPhone, lblBirthDate, dtpBirthDate, lblCourse, cboCourse, btnRegister });
        }

        private void LoadCourses()
        {
            var courses = new List<CourseItem>
            {
                new CourseItem { CourseId = 1, CourseName = "Lập trình .NET C#" },
                new CourseItem { CourseId = 2, CourseName = "Lập trình Web React" },
                new CourseItem { CourseId = 3, CourseName = "Cơ sở dữ liệu SQL Server" }
            };

            cboCourse.DataSource = courses;
            cboCourse.DisplayMember = "CourseName";
            cboCourse.ValueMember = "CourseId";
        }

        private void BtnRegister_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Vui lòng nhập họ tên!", "Cảnh báo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string name = txtName.Text.Trim();
            string phone = mtxtPhone.Text;
            string birthDate = dtpBirthDate.Value.ToString("dd/MM/yyyy");
            string courseName = cboCourse.Text;

            // Lưu dữ liệu vào SQLite (Bài 5.2)
            using (var conn = new SqliteConnection(connectionString))
            {
                conn.Open();
                string sql = "CREATE TABLE IF NOT EXISTS Students (Id INTEGER PRIMARY KEY AUTOINCREMENT, FullName TEXT, Phone TEXT, BirthDate TEXT, CourseName TEXT);";
                using (var cmd = new SqliteCommand(sql, conn)) { cmd.ExecuteNonQuery(); }

                string insertSql = "INSERT INTO Students (FullName, Phone, BirthDate, CourseName) VALUES (@name, @phone, @dob, @course);";
                using (var cmd = new SqliteCommand(insertSql, conn))
                {
                    cmd.Parameters.AddWithValue("@name", name);
                    cmd.Parameters.AddWithValue("@phone", phone);
                    cmd.Parameters.AddWithValue("@dob", birthDate);
                    cmd.Parameters.AddWithValue("@course", courseName);
                    cmd.ExecuteNonQuery();
                }
            }

            // Hiển thị thông báo (Bài 4.2)
            string msg = $"--- THÔNG TIN ĐĂNG KÝ ---\n\n" +
                         $"• Họ và tên: {name}\n" +
                         $"• Số điện thoại: {phone}\n" +
                         $"• Ngày sinh: {birthDate}\n" +
                         $"• Khóa học: {courseName}\n\n" +
                         $"Đã lưu dữ liệu vào CSDL!";

            MessageBox.Show(msg, "Thông báo đăng ký", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
    }
}