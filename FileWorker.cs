using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Text;

namespace service_client
{
    public class FileWorker
    {

        // название временного dbf-файла
        public string currentDbfFileName { get { return "file.dbf"; } set { } }
        // название временного csv-файла
        public string currentCsvFileName { get { return "file.csv"; } set { } }
        // название временного dbf-файла
        public string currentSortedDbfFileName { get { return "sorted"; } set { } }

        // класс для названий и номеров регионов
        Regions regions = new Regions();

        string dbfFileDirectory = Directory.GetCurrentDirectory();
        string connectionString = "";

        public FileWorker()
        {

            // ининциализация строки подключения
            connectionString = "Provider=Microsoft.Jet.OLEDB.4.0;dBase 5.0;DATABASE=" + dbfFileDirectory+ @"\";

        }

        // для вызова разных методов для обработки dbf-файла 
        public void Work(string selectedRegionName, string selectedRegionNumber)
        {
            // получить DataTable из dbf-файла
            DataTable commonDatatable = GetDatatableFromDbf();

            // если выборка это все регионы, то создать файлы для всех регионов
            if (selectedRegionName == new Regions().GetNameOfAll())
            {
                int count = 0;
                foreach (string regionNum in regions.GetRegionNumbers())
                {
                    // получить DataTable сортированные по регионам из DataTable 
                    DataTable dtRegion = GetDatatableOfRegion(commonDatatable, regionNum);

                    // название региона
                    string regionName = regions.GetRegionNames()[count];
                    // записать DataTable в dbf-файл
                    WriteDatatableToDbf(dtRegion, regionName);

                    // заменить название файла на название региона и переместить на рабочий стол
                    MoveFileToDesktop(currentSortedDbfFileName + ".dbf", regionName + ".dbf");

                    count++;
                }
            }
            else // иначе создать файл только для одного региона
            {

                // получить DataTable сортированные по регионам из DataTable 
                DataTable dtRegion = GetDatatableOfRegion(commonDatatable, selectedRegionNumber);

                // записать DataTable в dbf-файл
                WriteDatatableToDbf(dtRegion, selectedRegionName);

                // заменить название файла на название региона и переместить на рабочий стол
                MoveFileToDesktop(currentSortedDbfFileName + ".dbf", selectedRegionName + ".dbf");

            }
            
        }

        // для получения данных в виде DataTable из dbf-файла
        private DataTable GetDatatableFromDbf()
        {
            // DataTable объект
            DataTable dt = new DataTable();

            OleDbConnection сonnectionHandler = new OleDbConnection(connectionString);

            // Подключение
            сonnectionHandler.Open();

            // Если есть подключение...
            if (сonnectionHandler.State == ConnectionState.Open)
            {
                string mySQL = "select * from " + currentDbfFileName;  // file.dbf - это название таблицы и файла

                // ...то выбрать все значения из dbf-файла...
                OleDbCommand MyQuery = new OleDbCommand(mySQL, сonnectionHandler);
                OleDbDataAdapter DA = new OleDbDataAdapter(MyQuery);
                
                // ...и присвоит в объект DataTable
                DA.Fill(dt);

                // закрытие подключения
                сonnectionHandler.Close();
            }

            return dt;
        }

        // для возвращения DataTable со строками из столбца по указанному региону
        private DataTable GetDatatableOfRegion(DataTable InputDatatable, string regionNum)
        {
            DataTable outputDatatable = CreateDatatableWithColumns(InputDatatable);

            //Debug.WriteLine(outputDatatable.Columns.Count);

            // перебор строк
            foreach (DataRow row in InputDatatable.Rows)
            {

                // сначала выбрать значение столбца AIL
                string columnValue = row["AIL"].ToString();

                // выбрать часть для сравнения
                string columnValueToCompare = columnValue.Substring(0, regionNum.Length);

                // если они совпадают...
                if (columnValueToCompare == regionNum)
                {

                    // ... то добавить эту строку в DataTable
                    outputDatatable.Rows.Add(row.ItemArray);

                }
            }

            return outputDatatable;
        }

        // для записи из DataTable в dbf-файл
        private void WriteDatatableToDbf(DataTable dt, string regionName)
        {


            // имена столбцов
            ArrayList columnsNames = new ArrayList();

            // для начала запрос на удаление таблицы с таким именем
            string deleteSql = "drop table " + currentSortedDbfFileName;

            // создание таблицы
            string createSql = "create table " + currentSortedDbfFileName + " (";

            // перебор столбцов
            int rowLength = dt.Rows[0].ItemArray.Length;
            int count = 0;
            // добавление столбцов для выходного DataTable
            foreach (DataColumn dc in dt.Columns)
            {

                
                // название столбца
                string fieldName = dc.ColumnName;

                // тип данных столбца
                string type = dc.DataType.ToString();

                switch (type)
                {
                    case "System.String":
                        type = "varchar(100)";
                        break;

                    case "System.Boolean":
                        type = "varchar(10)";
                        break;

                    case "System.Int32":
                        type = "int";
                        break;

                    case "System.Double":
                        type = "Double";
                        break;

                    case "System.DateTime":
                        type = "TimeStamp";
                        break;
                }

                // создание столбца
                createSql = createSql + "[" + fieldName + "]" + " " + type + ",";

                columnsNames.Add(fieldName);

                count++;
                if (rowLength == count) break;
            }

            //Debug.WriteLine(columnsNames.Count);

            createSql = createSql.Substring(0, createSql.Length - 1) + ")";

            // подключениe к dbf-файлу
            OleDbConnection con = new OleDbConnection(this.connectionString);

            OleDbCommand cmd = new OleDbCommand();

            cmd.Connection = con;

            con.Open();
            
            // удалить таблицу, пропустить если нет таблицы
            try
            {
                cmd.CommandText = deleteSql;
                cmd.ExecuteNonQuery();
            }
            catch
            {

            }
            

            // создать таблицу
            cmd.CommandText = createSql;

            cmd.ExecuteNonQuery();

            //Debug.WriteLine(createSql);

            // Записать элементы в таблицу
            // перебор строк DataTable
            foreach (DataRow row in dt.Rows)
            {
                // создание строки SQL-запроса для добавления элементов
                string insertSql = "insert into " + currentSortedDbfFileName + " values(";

                // перебор столбцов
                for (int i = 0; i < columnsNames.Count; i++)
                {
                    // элемент строки
                    insertSql = insertSql + "'" + ReplaceEscape(row[columnsNames[i].ToString()].ToString()) + "',";
                }

                insertSql = insertSql.Substring(0, insertSql.Length - 1) + ")";

                // выполнить SQL-запрос
                cmd.CommandText = insertSql;

                cmd.ExecuteNonQuery();
            }

            // это код не работает
            //////// округлить числа в таблице, потому что в оригинале нет лишьних нулей
            //////// перебор имён столбцов
            //////foreach (string columnName in columnsNames)
            //////{
                
            //////    string roundSql = "UPDATE " + currentSortedDbfFileName + " SET " +
            //////                      columnName + "= ROUND(" + columnName + ", 0)";
            //////}
            

            // закрыть подключение
            con.Close();
        }


        // для записи из DataTable в csv-файл
        private void WriteDatatableToCsv(DataTable dt)
        {
            // 
            StringBuilder sb = new StringBuilder();

            // названии столбцов
            IEnumerable<string> columnNames = dt.Columns.Cast<DataColumn>().
                                              Select(column => column.ColumnName);
            sb.AppendLine(string.Join(",", columnNames));

            // перебор строк
            foreach (DataRow row in dt.Rows)
            {
                IEnumerable<string> fields = row.ItemArray.Select(field => field.ToString());
                sb.AppendLine(string.Join(",", fields));
            }

            // запись в файл
            File.WriteAllText(currentCsvFileName, sb.ToString());
        }


        // для создания имён столбцов для нового DataTabel из указанного DataTable
        private DataTable CreateDatatableWithColumns(DataTable inputDatatable)
        {
            DataTable outputDatatable = new DataTable();

            int rowLength = inputDatatable.Rows[0].ItemArray.Length;
            int count = 0;
            // добавление столбцов для выходного DataTable
            foreach (DataColumn dc in inputDatatable.Columns)
            {
                outputDatatable.Columns.Add(dc.ColumnName, dc.DataType);
                count++;
                if (rowLength == count) break;
            }

            return outputDatatable;
        }

        // для изменения имени и перемещения файла 
        private void MoveFileToDesktop(string oldFileName, string newFileName)
        {
            // допускается что старый файл, находиться в текущем каталоге
            string oldFilePath = oldFileName;

            // путь файла на рабочий стол пользователья
            string newFilePath = @"C:\Users\" + Environment.UserName + @"\Desktop\" + newFileName;

            // удалить если существует файл с таким именем
            if (File.Exists(newFilePath))
            {
                File.Delete(newFilePath);
            }

            // переместить
            File.Move(oldFilePath, newFilePath);
        }

        // для удаления файла
        public void DeleteCommonFile(string fileName)
        {
            if (File.Exists(fileName))
            {
                File.Delete(fileName);
            }
        }

        private static string ReplaceEscape(string str)
        {
            str = str.Replace("'", "''");
            return str;
        }
    }
}

// (данные комментарии подлежат к обновлению)
// методы

// public:
//      Work

// private:
//      GetDatatableFromDbf
//      GetDatatableOfRegion
//      WriteDatatableToDbf
//      WriteDatatableToCsv
//      ReplaceEscape