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
            TemporaryFileManager tempFileManager = TemporaryFileManager.GetInstance();

            string csvFilePath = tempFileManager.CreateFile(inputData.CsvData, "csv");
            CsvLoader loader = new CsvLoader();
            loader.LoadCsv(csvFilePath);

            string tempPdfFilePath = tempFileManager.GenerateFilePath("pdf");
            pdfLogic.ConvertCsvToPdf(loader.GetHeaderItems(), loader.GetContentTable(), tempPdfFilePath);

            GeneratedPdf genPdf = new GeneratedPdf();
            byte[] pdfData = this.LoadBinaryFile(tempPdfFilePath);
            genPdf.PdfFileData = Convert.ToBase64String(pdfData, Base64FormattingOptions.None);
            genPdf.Guid = System.Guid.NewGuid().ToString();

            return genPdf;
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
    }
}
