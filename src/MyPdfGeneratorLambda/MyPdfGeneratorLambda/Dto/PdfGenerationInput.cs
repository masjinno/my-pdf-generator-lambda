using System;
using System.Collections.Generic;
using System.Text;

namespace MyPdfGeneratorLambda.Dto
{
    public class PdfGenerationInput
    {
        public string CsvData { get; set; }
        public PageSetting PageSetting { get; set; }
        public CsvHeaderSetting HeaderSetting { get; set; }
        public CsvContentSetting ContentSetting { get; set; }
    }
}
