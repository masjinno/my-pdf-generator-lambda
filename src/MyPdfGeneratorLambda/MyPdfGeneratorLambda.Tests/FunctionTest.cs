using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Xunit;
using Amazon.Lambda.Core;
using Amazon.Lambda.TestUtilities;

using MyPdfGeneratorLambda;
using MyPdfGeneratorLambda.Dto;

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
            var generatedPdf = function.GeneratePdfFromCsv(input, context);

            Assert.Equal("xxx", generatedPdf.PdfFileData);
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
                "ècå¸Ç´", "â°å¸Ç´"
            };

            Assert.Equal(expectedPageSizes, props.PageSizes);
            Assert.Equal(expectedOrientations, props.Orientations);
        }
    }
}
