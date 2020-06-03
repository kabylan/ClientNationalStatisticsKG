using System;
using System.Net;
using System.Windows.Forms;

namespace service_client
{
    public partial class Form1 : Form
    {


        public Form1()
        {
            InitializeComponent();

            // для выбора региона
            comboBox1.Items.Add(new Regions().GetNameOfAll());
            comboBox1.Items.AddRange(new Regions().GetRegionNames());
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
            
            //Debug.WriteLine(selectedRegionName + " " + selectedRegionNumber);

            // Обработчик dbf-файла
            FileWorker fw = new FileWorker();

            // название скачанного dbf-файла
            string downloadedDbfFileName = fw.currentDbfFileName;

            // Cкачать файл, с даты от и до
            using (var client = new WebClient())
            {
                // даты
                string startDate = this.startDate.Value.ToString("dd/MM/yyyy");
                string endDate = this.endDate.Value.ToString("dd/MM/yyyy");
                
                // веб-адресс
                string dates = "T_Month?startDate=" + startDate + "&expirationDate=" + endDate;
                string url = "http://report.stat.kg/api/report/download/" + dates;

                // скачать файл
                client.DownloadFile(url, downloadedDbfFileName);

            }


            // Дальнейщяя обработка файла
            fw.Work(selectedRegionName, selectedRegionNumber);

            // Удалить скачанный общий(common) файл после удаления
            fw.DeleteCommonFile(downloadedDbfFileName);
        }

    }
}
