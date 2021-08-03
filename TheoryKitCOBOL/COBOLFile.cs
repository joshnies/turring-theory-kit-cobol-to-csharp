// Author: Joshua Nies
//
// Copyright © Turring 2021. All Rights Reserved.
//
using System;
using System.IO;
using System.Linq;

namespace TheoryKitCOBOL
{
    /// <summary>
    /// Sort direction.
    /// </summary>
    public enum SortDirection
    {
        Ascending,
        Descending
    }

    /// <summary>
    /// Wrapper object for COBOL-compatible file handling.
    /// </summary>
    public class COBOLFile
    {
        /// <summary>
        /// File path.
        /// </summary>
        public readonly string path;

        /// <summary>
        /// Attached data used for implicit writes.
        /// </summary>
        public dynamic attachedData;

        /// <summary>
        /// Create a new COBOL-compatible file.
        /// Creates a new file if one doesn't exist yet at the given path.
        /// </summary>
        /// <param name="path">Relative file path.</param>
        /// <param name="attachedData">Attached COBOL group data.</param>
        public COBOLFile(string path, dynamic attachedData = null)
        {
            var dirName = Path.Join(
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),
                "cobol_files"
            );
            this.path = Path.Join(dirName, path);
            this.attachedData = attachedData;

            // Create directory if it doesnt exist yet
            if (!Directory.Exists(dirName))
            {
                Directory.CreateDirectory(dirName);
            }

            // Create file if it doesnt exist yet
            if (!File.Exists(this.path))
            {
                File.Create(this.path);
            }
        }

        /// <summary>
        /// Attach data for implicit writes.
        /// </summary>
        /// <param name="data">COBOL group to use as attached data.</param>
        public void AttachData(dynamic data = null)
        {
            attachedData = data;
        }

        /// <summary>
        /// Read all contents.
        /// </summary>
        /// <returns>File contents.</returns>
        public string ReadAll()
        {
            using var reader = new StreamReader(path);
            return reader.ReadToEnd();
        }

        /// <summary>
        /// Append attached data to this file (implicit write).
        /// </summary>
        public void Append()
        {
            if (attachedData == null)
            {
                throw new Exception(
                    $"Failed to implicitly write COBOLFile with path " +
                    "\"{path}\" without attached data."
                );
            }

            using var writer = File.AppendText(path);
            writer.Write(attachedData);
        }

        /// <summary>
        /// Append the given data to this file.
        /// </summary>
        /// <param name="data"></param>
        public void Append(dynamic data)
        {
            using var writer = File.AppendText(path);
            writer.Write(data);
        }

        /// <summary>
        /// Append attached data and a new line to this file (implicit write).
        /// </summary>
        public void AppendLine()
        {
            if (attachedData == null)
            {
                throw new Exception(
                    $"Failed to implicitly write COBOLFile with path " +
                    "\"{path}\" without attached data."
                );
            }

            using var writer = File.AppendText(path);
            writer.WriteLine(attachedData);
        }

        /// <summary>
        /// Append the given data and a new line to this file.
        /// </summary>
        public void AppendLine(dynamic data)
        {
            using var writer = File.AppendText(path);
            writer.WriteLine(data);
        }

        /// <summary>
        /// Delete this file.
        /// </summary>
        public void Delete()
        {
            File.Delete(path);
        }

        /// <summary>
        /// Sort lines by the given variables' ranges.
        /// </summary>
        /// <param name="path">File path.</param>
        /// <param name="sortedBy">COBOL variables used to sort the file.</param>
        public void Sort(SortDirection direction, params COBOLVar[] sortedBy)
        {
            if (sortedBy.Length == 0) return;

            var newContents = "";

            // Read lines
            using (var reader = new StreamReader(path))
            {
                var lines = reader.ReadToEnd().Split('\n');
                var firstCvar = sortedBy[0];

                // Sort by first var (required)
                var ordered = COBOLUtils.OrderBy(
                    lines,
                    x =>
                    {
                        if (x == "") return x;

                        // Pad line
                        x = x.PadRight(Math.Max(firstCvar.startIndex + firstCvar.size + 1 - x.Length, 0), ' ');

                        return x.Substring(firstCvar.startIndex, firstCvar.size);
                    },
                    direction
                );

                // Sort by remaining vars (if any)
                foreach (var svar in sortedBy.Skip(1))
                {
                    ordered = COBOLUtils.ThenOrderBy(
                        ordered,
                        x =>
                        {
                            if (x == "") return x;

                            // Pad line
                            x = x.PadRight(Math.Max(svar.startIndex + svar.size + 1 - x.Length, 0), ' ');

                            return x.Substring(svar.startIndex, svar.size);
                        },
                        direction
                    );
                }

                lines = ordered.ToArray();
                newContents = string.Join('\n', lines);
            }

            // Write new contents
            var writer = new StreamWriter(path);
            writer.Write(newContents);
            writer.Close();
        }
    }
}
