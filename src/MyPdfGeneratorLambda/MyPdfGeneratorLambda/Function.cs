using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Amazon.Lambda.Core;
using MyPdfGeneratorLambda.Dto;
using MyPdfGeneratorLambda.Model;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MyPdfGeneratorLambda
{
    public class Function
    {
        public Function()
        {
            PdfLogic.Init();
        }

        /// <summary>
        /// CSVファイルからPDFファイルを生成する
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public GeneratedPdf GeneratePdfFromCsv(PdfGenerationInput input, ILambdaContext context)
        {
            context.Logger.LogLine($"Arg : [{input}]");
            PdfGenerator generator = new PdfGenerator();
            return generator.GeneratePdfFromCsv(input);
        }

        /// <summary>
        /// PDFファイル生成で指定可能なパラメーターを取得する
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public Properties GetProperties(object input, ILambdaContext context)
        {
            return Properties.Init();
        }

        /// <summary>
        /// CSVのヘッダの項目一覧を取得する
        /// </summary>
        /// <param name="input"></param>
        /// <param name="context"></param>
        /// <returns></returns>
        public List<string> GetCsvHeaderItems(TextData input, ILambdaContext context)
        {
            context.Logger.LogLine($"Arg : [{input}]");
            TemporaryFileManager tempFileManager = TemporaryFileManager.GetInstance();
            string csvFilePath = tempFileManager.CreateFile(input.Data, "csv");
            CsvLoader loader = new CsvLoader();
            loader.LoadCsv(csvFilePath);
            List<string> headerItems = loader.GetHeaderItems();
            tempFileManager.DeleteFile(csvFilePath);
            return headerItems;
        }
    }
}
