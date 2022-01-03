using iTextSharp.text;
using iTextSharp.text.pdf;
using MyPdfGeneratorLambda.Dto;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyPdfGeneratorLambda.Model
{
    public class PdfLogic
    {
        private Rectangle dstPageSize;
        private Direction dstMargin;
        private bool isDstPageRotate;
        private float dstHeaderFontSize;
        private string dstHeaderFontName;
        private string dstHeaderMarkupStart;
        private string dstHeaderMarkupEnd;
        private float dstContentFontSize;
        private string dstContentFontName;

        public PdfLogic()
        {
            this.Init();
        }

        public void Init()
        {
            FontFactory.RegisterDirectories();
        }

        public void ConvertCsvToPdf(List<string> csvHeader, List<List<string>> csvContent, string pdfFilePath)
        {
            Rectangle pageSize = this.dstPageSize;
            if (this.isDstPageRotate) pageSize = pageSize.Rotate();

            using (FileStream fs = new FileStream(pdfFilePath, FileMode.Create))
            {
                Document doc = new Document(pageSize, this.dstMargin.Left, this.dstMargin.Right, this.dstMargin.Top, this.dstMargin.Bottom);

                PdfWriter pdfWriter = PdfWriter.GetInstance(doc, fs);
                doc.Open();

                // header paragraph setting
                Font headerFont = this.GetHeaderFont();
                List<Paragraph> headers = new List<Paragraph>();
                foreach (string h in csvHeader)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.Append(this.dstHeaderMarkupStart).Append(h).Append(this.dstHeaderMarkupEnd);
                    headers.Add(new Paragraph(sb.ToString(), headerFont));
                }

                // write document
                Font contentFont = this.GetContentFont();
                for (int rowIndex = 0; rowIndex < csvContent.Count; rowIndex++)
                {
                    string pageStartMessage = new StringBuilder().Append("No.").Append(rowIndex + 1).Append("\r\n\r\n").ToString();
                    doc.Add(new Paragraph(pageStartMessage, contentFont));

                    for (int columnIndex = 0; columnIndex < headers.Count; columnIndex++)
                    {
                        doc.Add(headers[columnIndex]);
                        StringBuilder contentSB = new StringBuilder();
                        contentSB.Append(csvContent[rowIndex][columnIndex]).Append("\r\n\r\n");
                        doc.Add(new Paragraph(contentSB.ToString(), contentFont));
                    }
                    doc.NewPage();
                }

                string docEndMessage = new StringBuilder().Append("以上、").Append(csvContent.Count).Append("データ").ToString();
                doc.Add(new Paragraph(docEndMessage, contentFont));

                doc.Close();
            }
        }

        public void SetDstPageSize(Rectangle pageSize)
        {
            this.dstPageSize = pageSize;
        }

        public void SetDstMargin(Direction margin)
        {
            this.dstMargin = margin;
        }

        public void SetIsDstPageRotate(bool rotate)
        {
            this.isDstPageRotate = rotate;
        }

        public void SetDstHeaderFontSize(float fontSize)
        {
            this.dstHeaderFontSize = fontSize;
        }

        public void SetDstHeaderFontFamily(string fontName)
        {
            this.dstHeaderFontName = fontName;
        }

        public void SetDstHeaderMarkupStart(string markupStart)
        {
            this.dstHeaderMarkupStart = markupStart;
        }

        public void SetDstHeaderMarkupEnd(string markupEnd)
        {
            this.dstHeaderMarkupEnd = markupEnd;
        }

        public void SetDstContentFontSize(float fontSize)
        {
            this.dstContentFontSize = fontSize;
        }

        public void SetDstContentFontFamily(string fontName)
        {
            this.dstContentFontName = fontName;
        }

        private Font GetHeaderFont()
        {
            return this.GetFont(this.dstHeaderFontName, this.dstHeaderFontSize);
        }

        private Font GetContentFont()
        {
            return this.GetFont(this.dstContentFontName, this.dstContentFontSize);
        }

        private Font GetFont(string fontName, float fontSize)
        {
            Font ret = FontFactory.GetFont(fontName, BaseFont.IDENTITY_H, BaseFont.NOT_EMBEDDED, fontSize);
            return ret;
        }
    }
}
