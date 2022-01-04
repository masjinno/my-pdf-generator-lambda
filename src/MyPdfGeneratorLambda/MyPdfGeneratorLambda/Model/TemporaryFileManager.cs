using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MyPdfGeneratorLambda.Model
{
    public sealed class TemporaryFileManager
    {
        private static TemporaryFileManager _temporaryFileManager = new TemporaryFileManager();

        private List<string> filePaths;

        public static TemporaryFileManager GetInstance()
        {
            return TemporaryFileManager._temporaryFileManager;
        }

        private TemporaryFileManager()
        {
            this.filePaths = new List<string>();
        }

        /// <summary>
        /// 一時ファイルを生成する(テキスト)
        /// </summary>
        /// <param name="content">書き出す内容</param>
        /// <param name="ext">拡張子</param>
        /// <returns>ファイルパス</returns>
        public string CreateFile(string content, string ext)
        {
            string filePath = this.GenerateFilePath(ext);
            File.WriteAllText(filePath, content);
            return filePath;
        }

        /// <summary>
        /// 一時ファイルを生成する(バイナリ)
        /// </summary>
        /// <param name="content">書き出す内容</param>
        /// <param name="ext">拡張子</param>
        /// <returns>ファイルパス</returns>
        public string CreateFile(byte[] content, string ext)
        {
            string filePath = this.GenerateFilePath(ext);
            using (FileStream fs = new FileStream(filePath, FileMode.Create))
            using (BinaryWriter bw = new BinaryWriter(fs))
            {
                bw.Write(content);
            }
            return filePath;
        }

        /// <summary>
        /// ファイルを削除する
        /// </summary>
        /// <param name="path">ファイルパス</param>
        /// <returns></returns>
        public bool DeleteFile(string path)
        {
            bool isDeleted = true;
            if (File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    isDeleted = false;
                }
            }
            else
            {
                isDeleted = false;
            }
            return isDeleted;
        }

        /// <summary>
        /// ファイルパスを生成する。管理登録も行う。
        /// </summary>
        /// <param name="ext">拡張子</param>
        /// <returns>ファイルパス</returns>
        public string GenerateFilePath(string ext)
        {
            string folder = "/tmp/";
            string folderFullPath = Path.GetFullPath(folder);
            if (!Directory.Exists(folderFullPath))
            {
                Directory.CreateDirectory(folderFullPath);
            }

            StringBuilder sb = new StringBuilder().Append(System.Guid.NewGuid().ToString());
            string fileName = string.IsNullOrEmpty(ext) ? sb.ToString() : sb.Append(".").Append(ext).ToString();
            string filePath = Path.GetFullPath(new StringBuilder(folder).Append(fileName).ToString());
            this.filePaths.Add(filePath);
            return filePath;
        }

        /// <summary>
        /// 【デストラクタ】
        /// 書き出したファイルを削除する
        /// </summary>
        ~TemporaryFileManager()
        {
            foreach (string path in this.filePaths)
            {
                this.DeleteFile(path);
            }
        }
    }
}
