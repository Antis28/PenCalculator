using Newtonsoft.Json;
using PenCalculator.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PenCalculator.Infrastructure.Services
{
    internal class JsonService
    {
        private static readonly string[] notAvailableSymbols = { "/", "\\", ":", "*", "?", "<", ">", "|" };

        public static string Save(DataFile df)
        {
            string fileName = df.FileName;
            try
            {
                fileName = ClearString(fileName);
                var jsonText = JsonConvert.SerializeObject(df, Formatting.Indented);
                var wr = File.CreateText($"{fileName}.JSON");

                wr.AutoFlush = true;
                wr.Write(jsonText);
                wr.Close();
            }
            catch (Exception e) { Console.WriteLine(e); }
            return fileName;
        }

        private static string ClearString(string dirtyString)
        {
            foreach (string s in notAvailableSymbols)
            {
                var isDirty = dirtyString.Contains(s);
                if (isDirty)
                {
                    dirtyString = dirtyString.Replace(s, "");
                }
            }

            return dirtyString;
        }
    }
}
