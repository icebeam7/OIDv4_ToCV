using System;
using System.IO;
using System.Drawing;
using System.Collections.Generic;

namespace OIDv4_ToCV
{
    class Program
    {
        static string DatasetPath = @"C:\Users\tony_\objectdetection\OIDv4_ToolKit\OID\Dataset";

        static void Main(string[] args)
        {
            var testPath = Path.Combine(DatasetPath, "test2");
            var tags = new List<string>();

            foreach (var directory in Directory.GetDirectories(testPath, "*", SearchOption.TopDirectoryOnly))
            {
                var tag = new DirectoryInfo(directory).Name;
                Console.WriteLine($"----- {tag} Class-----");

                var classFolder = Path.Combine(testPath, tag);
                var labelFolder = Path.Combine(classFolder, "Label");
                var normalizedLabelFolder = Directory.CreateDirectory(Path.Combine(classFolder, "normalizedLabel"));

                var tagImages = Directory.GetFiles(classFolder);

                foreach (var tagImage in tagImages)
                {
                    Console.WriteLine($"\tImage: {Path.GetFileName(tagImage)}");
                    float imageWidth, imageHeight;

                    using (var file = new FileStream(tagImage, FileMode.Open, FileAccess.Read))
                    {
                        using (var image = Image.FromStream(stream: file, useEmbeddedColorManagement: false, validateImageData: false))
                        {
                            imageWidth = image.PhysicalDimension.Width;
                            imageHeight = image.PhysicalDimension.Height;
                        }
                    }

                    var imageLabelFile = $"{Path.GetFileNameWithoutExtension(tagImage)}.txt";
                    var imageObjects = File.ReadAllLines(Path.Combine(labelFolder, imageLabelFile));

                    var normalizedContent = new List<string>();

                    foreach (var objectInfo in imageObjects)
                    {
                        var normalizedObject = string.Empty;

                        var info = objectInfo.Split();

                        if (info[0] == tag)
                        {
                            var roundingBoxLeftX = float.Parse(info[1]);
                            var roundingBoxTopY = float.Parse(info[2]);
                            var roundingBoxRightX = float.Parse(info[3]);
                            var roundingBoxBottomY = float.Parse(info[4]);

                            var normalizedLeft = roundingBoxLeftX / imageWidth;
                            var normalizedTop = roundingBoxTopY / imageHeight;
                            var normalizedWidth = (roundingBoxRightX - roundingBoxLeftX) / imageWidth;
                            var normalizedHeight = (roundingBoxBottomY - roundingBoxTopY) / imageHeight;

                            normalizedObject = $"{normalizedLeft} {normalizedTop} {normalizedWidth} {normalizedHeight}";
                            Console.WriteLine($"\t\tNormalized Data: {normalizedObject}");
                        }
                        else
                        {
                            // different tag... can actually happen?
                            var roundingBoxLeftX = float.Parse(info[2]);
                            var roundingBoxTopY = float.Parse(info[3]);
                            var roundingBoxRightX = float.Parse(info[4]);
                            var roundingBoxBottomY = float.Parse(info[5]);

                            var normalizedLeft = roundingBoxLeftX / imageWidth;
                            var normalizedTop = roundingBoxTopY / imageHeight;
                            var normalizedWidth = (roundingBoxRightX - roundingBoxLeftX) / imageWidth;
                            var normalizedHeight = (roundingBoxBottomY - roundingBoxTopY) / imageHeight;

                            normalizedObject = $"{normalizedLeft} {normalizedTop} {normalizedWidth} {normalizedHeight}";
                            Console.WriteLine($"\t\tNormalized Data: {normalizedObject}");
                        }

                        normalizedContent.Add(normalizedObject);
                    }

                    File.WriteAllLines(Path.Combine(normalizedLabelFolder.FullName, imageLabelFile), normalizedContent);
                }

                tags.Add(tag);
            }

            File.WriteAllLines(Path.Combine(testPath, "tags.txt"), tags);

            Console.WriteLine("Complete! Press a key to exit.");
            Console.ReadKey();
        }
    }
}
