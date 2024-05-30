using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Http;
using Newtonsoft.Json;


namespace CsharpApi
{
    public partial class Form1 : Form
    {
        private static readonly HttpClient client = new HttpClient();
        public Form1()
        {
            InitializeComponent();
        }

        private async void btnGet_Click(object sender, EventArgs e)
        {
            try
            {
                txtOutput.Clear();
                HttpResponseMessage response = await client.GetAsync("http://localhost/LiezaAPI/phpapi/LiezaAPI.php");
                response.EnsureSuccessStatusCode();
                string responseBody = await response.Content.ReadAsStringAsync();
                txtOutput.Text = responseBody;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void btnPost_Click(object sender, EventArgs e)
        {
            // Check if any of the textboxes are empty
            if (string.IsNullOrWhiteSpace(txtName.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) || string.IsNullOrWhiteSpace(txtYear.Text) || string.IsNullOrWhiteSpace(txtCourse.Text))
            {
                txtOutput.Text = "Error: All fields should not be empty.";
                return;
            }

            var userData = new { password = txtPassword.Text, name = txtName.Text, year = txtYear.Text, course_name = txtCourse.Text };
            string json = JsonConvert.SerializeObject(userData);
            HttpContent content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                HttpResponseMessage response = await client.PostAsync("http://localhost/LiezaAPI/phpapi/LiezaAPI.php", content);
                string responseBody = await response.Content.ReadAsStringAsync();

                if (response.StatusCode == System.Net.HttpStatusCode.Conflict || responseBody.Contains("Duplicate entry"))
                {
                    // Handle duplicate entry
                    txtOutput.Text = "Error: Duplicate entry. Please use a different username or email.";
                }
                else
                {
                    response.EnsureSuccessStatusCode();
                    txtOutput.Text = responseBody;

                    // Clear text boxes after successful post
                    txtPassword.Text = string.Empty;
                    txtName.Text = string.Empty;
                    txtYear.Text = string.Empty;
                    txtCourse.Text = string.Empty;
                }
            }
            catch (Exception ex)
            {
                txtOutput.Text = "Error: " + ex.Message;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
       
    }
}
