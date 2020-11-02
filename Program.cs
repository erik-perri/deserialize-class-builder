using DeserializeClassBuilder.Deserialize;
using DeserializeClassBuilder.Deserialize.Enums;
using DeserializeClassBuilder.Deserialize.Exceptions;
using DeserializeClassBuilder.Deserialize.Record;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;

namespace DeserializeClassBuilder
{
    internal class Program
    {
        private static bool confirmedOverwrite;

        /// <summary>
        /// Attempt to dump a set of classes that can be used to deserialize a .NET binary stream
        /// </summary>
        /// <param name="argument">The file to parse</param>
        /// <param name="classPrefix">Search for classes with the specified prefix (supports multiple)</param>
        /// <param name="fullScan">Search every possible class structure</param>
        /// <param name="namespaceToOutput">The namespace to use in the output files</param>
        /// <param name="outputPath">Where to output the files, if not supplied will create an 'generated' directory in the input directory</param>
        /// <returns></returns>
        private static int Main(
            FileInfo argument,
            string[] classPrefix,
            bool fullScan,
            string namespaceToOutput = "DeserializeClassBuilder",
            DirectoryInfo outputPath = null
        )
        {
            if (!fullScan && (classPrefix == null || classPrefix.Length < 1))
            {
                Console.WriteLine("Either --full-scan or --class-prefix must be specified");
                return 1;
            }

            if (fullScan && classPrefix != null && classPrefix.Length > 0)
            {
                Console.WriteLine("You must choose either --full-scan or --class-prefix, not both.");
                return 1;
            }

            try
            {
                var outputPathString = outputPath == null
                    ? Path.Combine(Path.GetDirectoryName(argument.FullName), $"{argument.Name}.generated")
                    : outputPath.FullName;

                if (!Directory.Exists(outputPathString) && !Directory.CreateDirectory(outputPathString).Exists)
                {
                    Console.WriteLine($"Failed to create output path: \"{argument.FullName}\"");
                    return 1;
                }

                using var fileStream = File.OpenRead(argument.FullName);
                using var reader = new DeserializeReader(fileStream);

                // Make sure we have a valid header before looking
                // TODO Improve the structure checking in SerializationHeaderRecord, it currenly only checks the first
                //      byte while checking the type.
                var header = new SerializationHeaderRecord(reader);

                // Search through the file for positions to check
                var positions = FindPositions(fileStream, fullScan, classPrefix);

                Console.WriteLine(
                    $"Found {positions.Length.ToString("N0", CultureInfo.InvariantCulture)} locations to check in" +
                    $" \"{argument.Name}\""
                );

                // Store the percent so we only re-render when changed
                var renderedPercent = 0.0;

                // Check the found positions for class structures
                var classes = ScanForClasses(reader, positions, (int currentIndex) =>
                {
                    var percent = Math.Round((double)currentIndex / (double)positions.Length * 100, 0);
                    if (renderedPercent != percent)
                    {
                        Console.Write($"\r({percent}%) ");
                        renderedPercent = percent;
                    }
                });

                // Overwrite the last % line
                Console.Write($"\r{new string(' ', "(100.00%) ".Length)}\r");

                foreach (var structure in classes)
                {
                    WriteClass(structure, outputPathString, namespaceToOutput);
                }

                return 0;
            }
            catch (Exception e) when (e is DirectoryNotFoundException || e is FileNotFoundException)
            {
                Console.WriteLine($"File not found: \"{argument.FullName}\"");
                return 1;
            }
            catch (DeserializeException e)
            {
                Console.WriteLine($"Unexpected file structure: {e.Message}");
                return 1;
            }
#pragma warning disable CA1031 // Do not catch general exception types
            catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
            {
                Console.WriteLine($"Fatal error: {e.Message}");
#if DEBUG
                Debugger.Break();
#endif
                return 1;
            }
        }

        private static int[] FindPositions(Stream inputStream, bool fullScan, string[] classPrefixes)
        {
            using var memoryStream = new MemoryStream();

            inputStream.Position = 0;
            inputStream.CopyTo(memoryStream);

            var buffer = memoryStream.GetBuffer();

            if (fullScan)
            {
                var searchBytes = new byte[] { (byte)RecordType.ClassWithMembersAndTypes };
                return BoyerMoore.IndexesOf(buffer, searchBytes);
            }
            else
            {
                var potentialPositions = new List<int>();

                foreach (var prefix in classPrefixes)
                {
                    var searchBytes = System.Text.Encoding.UTF8.GetBytes(prefix);
                    var prefixPositions = BoyerMoore.IndexesOf(buffer, searchBytes);

                    if (prefixPositions.Length < 1)
                    {
                        continue;
                    }

                    foreach (var currentPosition in prefixPositions)
                    {
                        var updatedPosition = currentPosition;
                        updatedPosition -= 1; // Skip backwards past potential Name size (single byte, hopefully)
                        updatedPosition -= 4; // Skip backwards past potential Object id (int32)
                        updatedPosition -= 1; // Skip backwards past potential RecordType (byte)
                        if (updatedPosition > 0)
                        {
                            potentialPositions.Add(updatedPosition);
                        }
                    }
                }

                potentialPositions.Sort();

                return potentialPositions.ToArray();
            }
        }

        private static ClassStructure[] ScanForClasses(DeserializeReader reader, int[] positions, Action<int> positionUpdate)
        {
            var classes = new List<ClassStructure>();
            var positionIndex = 0;

            foreach (var position in positions)
            {
                positionUpdate(positionIndex++);

                reader.BaseStream.Position = position;

                var type = reader.PeekEnum<RecordType>();

                if (type != RecordType.ClassWithMembersAndTypes)
                {
                    continue;
                }

                try
                {
                    var record = new ClassWithMemberAndTypes(reader);
                    var structure = ClassStructureFactory.BuildFromClassWithMemberAndTypes(record);

                    classes.Add(structure);

                    Console.WriteLine($"\rParsed {record.ClassInfo.ClassName} at {position}");
                }
#pragma warning disable CA1031 // Do not catch general exception types
                catch (Exception e)
#pragma warning restore CA1031 // Do not catch general exception types
                {
                    Debug.WriteLine($"Failed to parse at {position} {e.Message}");
                }
            }

            return classes.ToArray();
        }

        private static void WriteClass(ClassStructure structure, string outputPath, string namespaceString)
        {
            if (structure.Extends != null)
            {
                WriteClass(structure.Extends, outputPath, namespaceString);
            }

            var classFile = $"{structure.Name}.cs";

            if (!string.IsNullOrWhiteSpace(structure.Namespace))
            {
                outputPath = Path.Combine(outputPath, structure.Namespace);
                if (!Directory.Exists(outputPath))
                {
                    Directory.CreateDirectory(outputPath);
                }
            }

            var classFilePath = Path.Combine(outputPath, classFile);

            if (File.Exists(classFilePath) && !confirmedOverwrite)
            {
                string confirmation = "";

                Console.WriteLine();
                Console.WriteLine($"The output file \"{classFile}\" already exists.");
                Console.WriteLine();

                while (confirmation != "y" && confirmation != "n" && confirmation != "a" && confirmation != "all")
                {
                    Console.WriteLine("Do you want to overwrite the existing file? [y/N/all] ");
                    confirmation = Console.ReadLine().ToLowerInvariant();

                    if (string.IsNullOrEmpty(confirmation))
                    {
                        confirmation = "n";
                        break;
                    }

                    if (confirmation != "y" && confirmation != "n" && confirmation != "a" && confirmation != "all")
                    {
                        Console.WriteLine("Invalid input");
                    }
                }

                if (confirmation == "n")
                {
                    Console.WriteLine("Skipping file");
                    return;
                }

                if (confirmation == "a")
                {
                    Console.WriteLine("Overwriting all");
                    confirmedOverwrite = true;
                }
                else
                {
                    Console.WriteLine("Overwriting file");
                }
            }

            structure.WriteTo(classFilePath, namespaceString);
        }
    }
}