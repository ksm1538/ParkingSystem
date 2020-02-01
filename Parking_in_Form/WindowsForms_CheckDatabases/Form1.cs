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

namespace WindowsForms_CheckDatabases
{
    public partial class Form1 : Form
    {
        String carNumber = "";
        MySqlConnection connection = 
            new MySqlConnection("Server=localhost;Database=member1;Uid=root;Pwd=1538;");
                                    //서버     //DB이름      // 유저이름  //비밀번호
        public Form1()
        {
            InitializeComponent();
        }

        public void image_Load()      {
            pictureBoxIpl1.ImageIpl = new IplImage(@"C:\out\5.bmp", LoadMode.AnyColor);
        }

        public void parking_in()
        {
            //데베 주차 시작 부분
            String time = DateTime.Now.ToString("yyyy-MM-dd HH:mm");
            //칼럼에 추가하는 커리문 insertQuery
            string insertQuery = "INSERT INTO member_tb(car_number, newtime) VALUES('" + carNumber + "','" + time + "')";

            /* 추가한다    테이블 member_tb 테이블에  name 과 age 라는 항목의 값을 그값은 NameBox.Text 와  AgeBox.Text 에입력
             된 값이다*/

            connection.Open();
            MySqlCommand command = new MySqlCommand(insertQuery, connection);

            try//예외 처리
            {
                // 만약에 내가처리한 Mysql에 정상적으로 들어갔다면 메세지를 보여주라는 뜻이다
                if (command.ExecuteNonQuery() == 1)
                {
                    MessageBox.Show("차량 번호 : "+carNumber+"\n정상적으로 추가되었습니다.");
                }
                else
                {
                    MessageBox.Show("오류 발생");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }


            connection.Close();
            //데베 주차 끝 부분


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
            parking_in_process();
            for (;true;)
            {
                break;

            }
        }
        public void parking_in_process()
        {
            image_Load();
            text_Check();
            parking_in();
            refresh_data();
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

        private void Label1_Click(object sender, EventArgs e)
        {

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

        private void Button1_Click_1(object sender, EventArgs e)
        {
            text_Check();
            MessageBox.Show(carNumber);
        }

        private void Refresh_Click(object sender, EventArgs e)
        {
            refresh_data();
        }
    }
}
