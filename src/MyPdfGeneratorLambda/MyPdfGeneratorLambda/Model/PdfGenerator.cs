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
            pdfLogic.ConvertCsvToPdf(inputData.HeaderSetting.TargetItems, loader.GetContentTable(), tempPdfFilePath);

            GeneratedPdf genPdf = new GeneratedPdf();
            byte[] pdfData = this.LoadBinaryFile(tempPdfFilePath);
            genPdf.PdfFileData = Convert.ToBase64String(pdfData, Base64FormattingOptions.None);
            genPdf.Guid = System.Guid.NewGuid().ToString();

            tempFileManager.DeleteFile(csvFilePath);
            tempFileManager.DeleteFile(tempPdfFilePath);

            return genPdf;
        }

        private void ValidatePdfGenerationInput(PdfGenerationInput input)
        {
            if (input == null) throw new ArgumentNullException(nameof(input));
            if (input.CsvData == null) throw new ArgumentNullException(nameof(input.CsvData));
            if (input.PageSetting == null) throw new ArgumentNullException(nameof(input.PageSetting));
            if (input.HeaderSetting == null) throw new ArgumentNullException(nameof(input.HeaderSetting));
            if (input.ContentSetting == null) throw new ArgumentNullException(nameof(input.ContentSetting));
            if (input.HeaderSetting.FontFamily == null) throw new ArgumentNullException(nameof(input.HeaderSetting.FontFamily));

            if (input.CsvData == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.CsvData));
            if (input.PageSetting.Size == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.PageSetting.Size));
            if (input.PageSetting.Orientation == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.PageSetting.Orientation));
            if (input.HeaderSetting.FontFamily == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.HeaderSetting.FontFamily));
            if (input.ContentSetting.FontFamily == string.Empty) throw new ArgumentException("Empty parameter", nameof(input.ContentSetting.FontFamily));
        }

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
