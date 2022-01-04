using iTextSharp.text;
using MyPdfGeneratorLambda.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyPdfGeneratorLambda.Model
{
    class PdfGenerator
    {
        private PdfLogic pdfLogic;
        public PdfGenerator()
        {
            this.pdfLogic = new PdfLogic();
        }

        /// <summary>
        /// CSVデータからPDFファイルを生成する
        /// </summary>
        /// <param name="inputData"></param>
        /// <returns></returns>
        public GeneratedPdf GeneratePdfFromCsv(PdfGenerationInput inputData)
        {
            this.ValidatePdfGenerationInput(inputData);
            TemporaryFileManager tempFileManager = TemporaryFileManager.GetInstance();

            string csvFilePath = tempFileManager.CreateFile(inputData.CsvData, "csv");
            CsvLoader loader = new CsvLoader();
            loader.LoadCsv(csvFilePath);

            string tempPdfFilePath = tempFileManager.GenerateFilePath("pdf");
            this.SetPdfSettings(inputData.PageSetting, inputData.HeaderSetting, inputData.ContentSetting);
            List<List<string>> targetContentTable = this.FilterTable(loader, inputData.HeaderSetting.TargetItems);
            pdfLogic.ConvertCsvToPdf(inputData.HeaderSetting.TargetItems, targetContentTable, tempPdfFilePath);

            GeneratedPdf genPdf = new GeneratedPdf();
            byte[] pdfData = this.LoadBinaryFile(tempPdfFilePath);
            genPdf.PdfFileData = Convert.ToBase64String(pdfData, Base64FormattingOptions.None);

            tempFileManager.DeleteFile(csvFilePath);
            tempFileManager.DeleteFile(tempPdfFilePath);

            return genPdf;
        }

        /// <summary>
        /// 入力値のヴァリデーション
        /// </summary>
        /// <param name="input"></param>
        private void ValidatePdfGenerationInput(PdfGenerationInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (input.CsvData == null) throw new ArgumentNullException(nameof(input.CsvData));
            if (input.PageSetting == null) throw new ArgumentNullException(nameof(input.PageSetting));
            if (input.HeaderSetting == null) throw new ArgumentNullException(nameof(input.HeaderSetting));
            if (input.ContentSetting == null) throw new ArgumentNullException(nameof(input.ContentSetting));
            if (input.HeaderSetting.FontFamily == null) throw new ArgumentNullException(nameof(input.HeaderSetting.FontFamily));
            if (input.ContentSetting.FontFamily == null) throw new ArgumentNullException(nameof(input.ContentSetting.FontFamily));

            if (input.CsvData == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.CsvData));
            if (input.PageSetting.Size == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.PageSetting.Size));
            if (input.PageSetting.Orientation == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.PageSetting.Orientation));
            if (input.HeaderSetting.FontFamily == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.HeaderSetting.FontFamily));
            if (input.ContentSetting.FontFamily == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.ContentSetting.FontFamily));
        }

        /// <summary>
        /// テーブルの内容を、出力内容に沿うようフィルタリングする
        /// </summary>
        /// <param name="csvLoader">CSVローダー</param>
        /// <param name="targetItems">出力対象の項目一覧</param>
        /// <returns></returns>
        private List<List<string>> FilterTable(CsvLoader csvLoader, List<string> targetItems)
        {
            List<List<string>> retTable = new List<List<string>>();
            List<string> header = csvLoader.GetHeaderItems();
            csvLoader.GetContentTable().ForEach(srcRow =>
            {
                List<string> dstRow = new List<string>();
                targetItems.ForEach(item =>
                {
                    int itemIndex = header.IndexOf(item);
                    if (itemIndex < 0) throw new ArgumentOutOfRangeException(nameof(targetItems), nameof(targetItems) + " has invalid value.");
                    dstRow.Add(srcRow[itemIndex]);
                });
                retTable.Add(dstRow);
            });
            return retTable;
        }

        /// <summary>
        /// バイナリファイルをbyte[]形式として読み込む
        /// </summary>
        /// <param name="filePath">ファイルパス</param>
        /// <returns>ファイルの内容</returns>
        private byte[] LoadBinaryFile(string filePath)
        {
            if (!File.Exists(filePath)) throw new FileNotFoundException("Temporary pdf file not found.", filePath);
            using (var fs = new FileStream(filePath, FileMode.Open))
            using (var br = new BinaryReader(fs))
            {
                int len = (int)fs.Length;
                byte[] data = new byte[len];
                br.Read(data, 0, len);
                return data;
            }
        }

        /// <summary>
        /// PDF設定を登録する
        /// </summary>
        /// <param name="pageSetting">ページ設定</param>
        /// <param name="headerSetting">ヘッダ設定</param>
        /// <param name="contentSetting">内容の設定</param>
        private void SetPdfSettings(PageSetting pageSetting, CsvHeaderSetting headerSetting, CsvContentSetting contentSetting)
        {
            this.pdfLogic.SetDstPageSize(PageSize.GetRectangle(pageSetting.Size));
            this.pdfLogic.SetIsDstPageRotate(pageSetting.Orientation.Equals("横向き"));
            this.pdfLogic.SetDstMargin(pageSetting.Margin);

            this.pdfLogic.SetDstHeaderFontSize(headerSetting.FontSize);
            this.pdfLogic.SetDstHeaderFontFamily(headerSetting.FontFamily);
            this.pdfLogic.SetDstHeaderMarkupStart(headerSetting.MarkupStart);
            this.pdfLogic.SetDstHeaderMarkupEnd(headerSetting.MarkupEnd);

            this.pdfLogic.SetDstContentFontSize(contentSetting.FontSize);
            this.pdfLogic.SetDstContentFontFamily(contentSetting.FontFamily);
        }
    }
}
