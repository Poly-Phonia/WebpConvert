using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace WebpConvert
{
    internal class Program
    {
        static void Main(string[] args)
        {
            List<string> commandLines = Environment.GetCommandLineArgs().ToList();

            //パスの決定
            string path;
            if(commandLines.Count > 1)
            {
                path = commandLines[1];
            }
            else
            {
                Console.Write("Path? > ");
                path = Console.ReadLine();
            }
            Console.WriteLine($"Path:{path}");
            Console.WriteLine(string.Empty);

            //ディレクトリ有無確認
            if (!System.IO.Directory.Exists(path))
            {
                Console.WriteLine("Directory not exists.");
                Console.WriteLine("Failed.");
                Console.ReadLine();
                return;
            }

            //WebPファイルの取得
            var files = System.IO.Directory.GetFiles(path)
                                           .Where(n => Regex.IsMatch(n, @"\.webp$", RegexOptions.IgnoreCase))
                                           .OrderBy(f => f)
                                           .ToList();
            if (!files.Any())
            {
                Console.WriteLine("WebP Files not exists.");
                Console.WriteLine("Failed.");
                Console.ReadLine();
                return;
            }
            Console.WriteLine($"FileCount:{files.Count}");

            //出力フォルダ作成
            var resultPath = System.IO.Path.Combine(path, "Result");
            if (System.IO.Directory.Exists(resultPath))
            {
                try
                {
                    System.IO.Directory.Delete(resultPath, true);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Result directory remove failed.\r\n\r\n{ex.Message}\r\n");
                    Console.WriteLine("Failed.");
                    Console.ReadLine();
                    return;
                }
            }
            try
            {
                System.IO.Directory.CreateDirectory(resultPath);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Result directory creation failed.\r\n\r\n{ex.Message}\r\n");
                Console.WriteLine("Failed.");
                Console.ReadLine();
                return;
            }

            //読み込む
            Console.WriteLine("Processing...");
            using (var factory = new ImageProcessor.ImageFactory())
            {
                foreach (var webpFile in files)
                {
                    var outFileName = System.IO.Path.GetFileNameWithoutExtension(webpFile) + ".png";
                    using(var webpSr = new System.IO.FileStream(webpFile, System.IO.FileMode.Open, System.IO.FileAccess.ReadWrite))
                    {
                        using(var outSr = new System.IO.FileStream(System.IO.Path.Combine(resultPath, outFileName), System.IO.FileMode.CreateNew, System.IO.FileAccess.Write))
                        {
                            Console.CursorLeft = 0;
                            Console.Write(webpFile);

                            factory.Load(webpSr)
                                   .Format(new ImageProcessor.Plugins.WebP.Imaging.Formats.WebPFormat())
                                   .Save(outSr);

                            outSr.Close();
                        }
                        webpSr.Close();
                    }
                }
            }

            Console.WriteLine(string.Empty);
            Console.WriteLine(string.Empty);
            Console.WriteLine("Done.");
            Console.ReadLine();
            return;
        }
    }
}
