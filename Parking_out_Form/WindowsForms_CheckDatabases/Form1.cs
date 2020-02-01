using MySql.Data.MySqlClient;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tesseract;
using System.IO;


namespace WindowsForms_CheckDatabases
{
    public partial class Form1 : Form
    {
        String in_time;
        String carNumber = "";
        MySqlConnection connection = 
            new MySqlConnection("Server=localhost;Database=member1;Uid=root;Pwd=1538;");
                                    //서버     //DB이름      // 유저이름  //비밀번호
        public Form1()
        {
            InitializeComponent();
        }

        public void image_Load()
        {
            pictureBoxIpl1.ImageIpl = new IplImage(@"C:\out\5.bmp", LoadMode.AnyColor);
        }

        public void parking_calculate()
        {
            // 정산 시작
            System.Globalization.CultureInfo provider = System.Globalization.CultureInfo.InvariantCulture;


            string insertQuery = "Select newtime from member_tb where car_number = " + "'" + carNumber + "'";
            connection.Open();

            MySqlCommand command = new MySqlCommand(insertQuery, connection);
            MySqlDataReader rdr = command.ExecuteReader();
            int tex = 1000;

            int temp_time;
            String intime;
            while (rdr.Read())
            {
                in_time = rdr[0].ToString();
            }
            try//예외 처리
            {

            }
            catch (Exception ex)
            {
                //MessageBox.Show(ex.Message);
            }

            rdr.Close();
            connection.Close();
            DateTime dt1 = DateTime.ParseExact(in_time, "yyyy-MM-dd HH:mm", provider);
            TimeSpan result2 = DateTime.Now - dt1;        // 현재 시각 - 입차 시각


            temp_time = (int)Math.Round(result2.TotalMinutes, 0, MidpointRounding.ToEven);   //소수점 제거
            int result_time = temp_time / 10;

            MessageBox.Show("차량 번호 : " + carNumber + "\n요금 : " + (result_time * tex).ToString());

            // 정산 끝
        }
        public void parking_delete()
        {
            string deleteQuery = "DELETE FROM member_tb WHERE car_number =" + "'" + carNumber + "'";

            connection.Open();
            MySqlCommand command = new MySqlCommand(deleteQuery, connection);

            try//예외 처리
            {
                // 만약에 내가처리한 Mysql에 정상적으로 들어갔다면 메세지를 보여주라는 뜻이다
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("정상적으로 삭제되었습니다.");
                }
                else
                {
                    MessageBox.Show("존재하지 않습니다.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            connection.Close();
        }
        public void refresh_data()
        {
            string insertQuery = "Select * from member_tb";
            MySqlDataAdapter sqlAdapter = new MySqlDataAdapter(insertQuery, connection);
            MySqlCommandBuilder builder = new MySqlCommandBuilder(sqlAdapter);

            DataSet ds = new DataSet("member1");                      // 데이터셋생성
            sqlAdapter.Fill(ds, "member1");

            dataGridView1.DataSource = ds.Tables["member1"].DefaultView;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            image_Load();
            text_Check();
            parking_calculate();
            parking_delete();
            refresh_data();
            for (;true;)
            {
                break;

            }
        }

        private void Exit_button_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        public void text_Check()
        {
            // 글자 인식 부분 시작



            String whitelist = "0123456789";
            var ocr = new TesseractEngine("./tessdata", "kor", EngineMode.Default);
            Bitmap img = new Bitmap(pictureBoxIpl1.Image);
            //var ocr = new TesseractEngine("./tessdata", "eng", EngineMode.TesseractAndCube);
            ocr.DefaultPageSegMode = Tesseract.PageSegMode.Auto;
            ocr.SetVariable("tessedit_char_whitelist", whitelist);
            var texts = ocr.Process(img);


            String aa = texts.GetText();
            String fix_text = "";
            char[] splitChar = aa.ToCharArray();

            for (int i = 5; i < splitChar.Length; i++)
            {
                fix_text += aa[i].ToString();
            }
            carNumber = fix_text;//fix_text; 
            // 글자 인식 부분 끝
            /*if(carNumber == "")
            {
                //parking_in_process();
                carNumber="0000";
            }
            else
                delete_File();*/
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            text_Check();
        }

        private void Label2_Click(object sender, EventArgs e)
        {

        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            refresh_data();
        }

        public void delete_File()
        {
            if (System.IO.Directory.Exists(@"C:\Out"))

            {

                string[] files = System.IO.Directory.GetFiles(@"C:\Out");

                foreach (string s in files)

                {

                    string fileName = System.IO.Path.GetFileName(s);

                    string deletefile = @"C:\Out\" + fileName;

                    System.IO.File.Delete(deletefile);



                }

            }
        }

    }
}
