using FPOSDB.Attributes;
using FPOSDB.DTO;
using FPOSDB.Extensions;
using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace FPOSPriceUpdater.Helper
{
    public static class Serializer
    {
        private static readonly ILog _log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        public static void ExportDataToCsv<T>(List<T> data, string path) where T : IModel
        {
                WriteHeadersToCsv<T>(path);
                WriteRowsToCsv(data, path);
        }

        private static void WriteRowsToCsv<T>(List<T> data, string path) where T : IModel
        {
            using (StreamWriter writer = new StreamWriter(path, true))
            {
                var properties = typeof(T).GetProperties();
                foreach (T item in data)
                {
                    if (!String.IsNullOrEmpty(item.DisplayName) && !String.IsNullOrEmpty(item.PrimaryKey))
                    {
                        var row = string.Join(";",
                            properties
                            .Where(x => !Attribute.IsDefined(x, typeof(NotSerializable)))
                            .Select(x => x.GetValue(item, null).ToString().ToLiteral())
                            .ToArray()
                            );
                        writer.WriteLine(row);
                    }
                }
            }
        }

        private static void WriteHeadersToCsv<T>(string path) where T : IModel
        {
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                var properties = typeof(T).GetProperties();
                string headers = string.Join(";", properties
                    .Where(x => !Attribute.IsDefined(x, typeof(NotSerializable)))
                    .Select(x => x.Name)
                    .ToArray()
                    );
                writer.WriteLine(headers);
            }
        }

        public static void WriteToFile<T>(List<T> data, string fileName) where T : IModel
        {
            var path = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().Location) + "\\" + fileName;
            using (StreamWriter writer = new StreamWriter(path, false))
            {
                List<string> itemsDone = new List<string>();
                
                writer.WriteLine(DateTime.Now);
                foreach(var item in data)
                {
                    if (!itemsDone.Contains(item.DisplayName))
                    {
                        writer.WriteLine(item);
                        itemsDone.Add(item.DisplayName);
                    }
                }
            }
        }

        /// <summary>
        /// Converts a CSV file to a list of objects
        /// </summary>
        /// <param name="path">the full path to the csv file</param>
        /// <returns></returns>
        public static List<T> FromCSV<T>(string path) where T : new()
        {
            var models = new List<T>();
            using(StreamReader reader = new StreamReader(path))
            {
                T model;
                string[] headers = reader.ReadLine().Split(';');

                string line;
                while ((line = reader.ReadLine()) != null) {
                    string[] row = line.Split(';');
                    model = new T();
                    for (int i = 0; i < headers.Length; i++)
                    {
                        try
                        {
                            string newValue = row[i];
                            model.GetType().GetProperty(headers[i]).SetValue(model, newValue, null);
                        } catch(Exception ex) { 
                            _log.Error("Converting CSV cell to object resulted in an error",ex);
                            throw ex;
                        }
                    }
                    models.Add(model);  
                }
            }
            return models;
        }
    }
}
