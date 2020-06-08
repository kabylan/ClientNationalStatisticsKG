using System;
using System.Diagnostics;
using System.Net;
using System.Windows.Forms;

namespace service_client
{
    public partial class Form1 : Form
    {

        // обработчик файла
        FileWorker fw = new FileWorker();
        // название скачанного dbf-файла
        string downloadedDbfFileName = new FileWorker().currentDbfFileName;

        public Form1()
        {
            InitializeComponent();

            // для выбора региона
            comboBox1.Items.Add(new Regions().GetNameOfAll());
            comboBox1.Items.AddRange(new Regions().GetRegionNames());
            comboBox1.SelectedIndex = 0; 



        }

        private void groupBox1_Enter_1(object sender, EventArgs e)
        {

        }

        private void dateTimePicker1_ValueChanged(object sender, EventArgs e)
        {

        }


        private void end_date_Click(object sender, EventArgs e)
        {

        }

        private void attention_Click(object sender, EventArgs e)
        {

        }


        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        // Скачать файл из веб-адреса и дальнейщяя оброботка
        private void btn_save_Click(object sender, EventArgs e)
        {
            if (!CheckForInternetConnection())
            {
                progressLabel.Text = "Интернет байланыш узулгондой корунот. \nТекшерип кайрадан аракет кылып корунуз";
                return;
            }

            // Display the ProgressBar control.
            progressBar1.Visible = true;
            // Set Minimum to 1 to represent the first file being copied.
            progressBar1.Minimum = 1;
            // Set Maximum to the total number of files to copy.
            progressBar1.Maximum = 5;
            // Set the initial value of the ProgressBar.
            progressBar1.Value = 1;
            // Set the Step property to a value of 1 to represent each file being copied.
            progressBar1.Step = 1;

            progressLabel.Text = "Интернетке туташуу";


            // даты
            string startDate = this.startDate.Value.ToString("dd/MM/yyyy");
            string endDate = this.endDate.Value.ToString("dd/MM/yyyy");

            // веб-адресс
            string dates = "T_Month?startDate=" + startDate + "&expirationDate=" + endDate;
            string url = "http://report.stat.kg/api/report/download/" + dates;


            progressLabel.Text = "Файл жуктолуудо";

            // скачать файл
            DownloadWithProgress(url, downloadedDbfFileName);


        }

        private void progressBar1_Click(object sender, EventArgs e)
        {

        }


        // метод отображения прогресса загрузки
        private void DownloadWithProgress(string url, string downloadedDbfFileName)
        {

            progressBar1.PerformStep();


            // Cкачать файл, с даты от и до
            using (var client = new WebClient())
            {
                Uri uri = new Uri(url);
                // скачать файл
                // ассинхронно, чтобы программа не зависла
                client.DownloadFileAsync(uri, downloadedDbfFileName);
                // при завершении загрузки продолжить работу с файлом
                client.DownloadFileCompleted += DownloadComplited;
            }

            progressBar1.PerformStep();

        }

        // происходит при завершении загрузки
        private void  DownloadComplited(object sender, EventArgs e)
        {


            // выбранный регион
            string selectedRegionName = comboBox1.SelectedItem.ToString();
            string selectedRegionNumber = "";
            // индекс на 1 меньше, потому что добавляется один лишьний элемент
            // (это код подлежит к обновлению)
            if (selectedRegionName != new Regions().GetNameOfAll())
            {
                int indexOfNumber = comboBox1.SelectedIndex;
                selectedRegionNumber = new Regions().GetRegionNumbers()[indexOfNumber];
            }

            progressBar1.PerformStep();
            progressLabel.Text = "Файлдан \"" + selectedRegionName + "\" алынууда, куто турунуз";

            // Дальнейщяя обработка файла
            fw.Work(selectedRegionName, selectedRegionNumber);

            progressBar1.PerformStep();
            progressLabel.Text = "Бирааз куто турунуз";


            // Удалить скачанный общий(common) файл после обработки
            fw.DeleteCommonFile(downloadedDbfFileName);

            progressBar1.PerformStep();
            progressLabel.Text = "Иштетме тактага жуктолду";



        }

        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://google.com/generate_204"))
                    return true;
            }
            catch
            {
                return false;
            }
        }

    }
}
