using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Text;

namespace MyPdfGeneratorLambda.Model
{
    public class CsvLoader
    {
        private List<List<string>> table;

        private List<string> headerItems;

        /// <summary>
        /// CSVファイルを読み込む
        /// </summary>
        /// <param name="csvFilePath">CSVファイルパス</param>
        public void LoadCsv(string csvFilePath)
        {
            this.table = new List<List<string>>();
            using (TextFieldParser parser = new TextFieldParser(csvFilePath))
            {
                parser.Delimiters = new string[] { "," };
                while (!parser.EndOfData)
                {
                    List<string> fieldList = new List<string>(parser.ReadFields());
                    this.table.Add(fieldList);
                }
            }

            this.headerItems = new List<string>();
            foreach (string head in this.table[0])
            {
                this.headerItems.Add(head);
            }
        }

        /// <summary>
        /// CSVの内容部分(ヘッダは除く)を取得する
        /// </summary>
        /// <returns></returns>
        public List<List<string>> GetContentTable()
        {
            if (this.table.Count == 0) throw new NotSupportedException("Call LoadCsv method before calling GetHeaderList method.");

            return table.GetRange(1, table.Count - 1);
        }
    }
}
