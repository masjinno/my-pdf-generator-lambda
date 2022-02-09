using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using MyPdfGeneratorLambda;
using MyPdfGeneratorLambda.Dto;
using MyPdfGeneratorLambda.Model;
using System.IO;

namespace MyPdfGeneratorLambda.Tests
{
    public class FunctionTest
    {
        [Fact]
        public void TestGeneratePdfFromCsv()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            var input = new PdfGenerationInput();
            input.CsvData = "header1,header2,header3\ncontent1-1,content1-2,content1-3\ncontent2-1,content2-2,content2-3";
            input.PageSetting = new PageSetting()
            {
                Size = "A4",
                Orientation = "縦向き",
                Margin = new Margin() { Top = 20F, Left = 20F, Right = 20F, Bottom = 20F}
            };
            input.HeaderSetting = new CsvHeaderSetting()
            {
                FontFamily = "メイリオ",
                FontSize = 14F,
                MarkupStart = "■",
                MarkupEnd = "",
                TargetItems = new List<string>() { "header3", "header1" }
            };
            input.ContentSetting = new CsvContentSetting()
            {
                FontSize = 11F,
                FontFamily = "メイリオ"
            };
            var generatedPdf = function.GeneratePdfFromCsv(input, context);

            Assert.False(string.IsNullOrEmpty(generatedPdf.PdfFileData));
            // Assert.Equal("xxx", generatedPdf.PdfFileData);
        }

        [Fact]
        public void TestGetProperties()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            object input = null;
            var props = function.GetProperties(input, context);

            var expectedPageSizes = new List<string>()
            {
                "A0", "A1", "A2", "A3", "A4", "A5",
                "A6", "A7", "A8", "A9", "A10",
                "B0", "B1", "B2", "B3", "B4", "B5",
                "B6", "B7", "B8", "B9", "B10"
            };
            var expectedOrientations = new List<string>()
            {
                "縦向き", "横向き"
            };

            Assert.Equal(expectedPageSizes, props.PageSizes);
            Assert.Equal(expectedOrientations, props.Orientations);
        }

        [Fact]
        public void TestGetCsvHeaderItems()
        {
            // Invoke the lambda function and confirm the string was upper cased.
            var function = new Function();
            var context = new TestLambdaContext();
            TextData input = new TextData();
            input.Data = "header1,\"header2\",\"header3L1\nheader3L2\"\ncontent1-1,content1-2,content1-3\ncontent2-1,content2-2,content2-3";
            var props = function.GetCsvHeaderItems(input, context);

            var expectedHeaderItems = new List<string>()
            {
                "header1", "header2", "header3L1\nheader3L2"
            };

            Assert.Equal(expectedHeaderItems, props);
        }

        //[Fact]
        public void CreatePdfFromBase64()
        {
            string base64 = "{Base64化したPDFファイルの内容}";
            byte[] bin = System.Convert.FromBase64String(base64);
            TemporaryFileManager tfm = TemporaryFileManager.GetInstance();
            string filePath = tfm.GenerateFilePath("pdf");
            BinaryWriter bw = new BinaryWriter(new FileStream(filePath, FileMode.Create));
            bw.Write(bin);
            bw.Close();
            tfm.DeleteFile(filePath);
        }
    }
}
