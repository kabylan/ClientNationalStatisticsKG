using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace service_client
{
    class Regions
    {
        // массивы для названий регионов и их номеров
        // данные по умолчанию
        private string[] regionsNames = { "Бишкек шаары", "Ош шаары", "Чүй", "Ош облусу", "Жалалабад",
                                          "Ысыккөл", "Нарын", "Баткен", "Талас" };

        private string[] regionsNumbers = { "41711", "41721", "41708", "41706", "41703",
                                            "41702", "41704", "41705", "41707"};

        private string nameOfAll = "Баардыгы";

        // для указания названий и номеров
        public void SetNamesAndNumbers(string[] names, string[] numbers)
        {
            // исключение при не одинаковой длины массивов
            if (names.Length != numbers.Length)
            {
                string exceptionString = "Метод SetNamesAndNumbers класса Regions: " +
                                         "принимает массивы names и numbers - одинаковой длины";

                throw new Exception(exceptionString);

            }
            else
            {
                this.regionsNames = names;
                this.regionsNumbers = numbers;
            }
        }

        // для получения названий регионов
        public string[] GetRegionNames()
        {
            return this.regionsNames;
        }

        // для получения номеров регионов
        public string[] GetRegionNumbers()
        {
            return this.regionsNumbers;
        }

        public string GetNameOfAll()
        {
            return this.nameOfAll;
        }

    }
}
