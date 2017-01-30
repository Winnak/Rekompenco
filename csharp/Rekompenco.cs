/* MIT License
 *
 * Copyright (c) 2017 Erik Høyrup Jørgensen
 *
 * Permission is hereby granted, free of charge, to any person obtaining a copy
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software without restriction, including without limitation the rights
 * to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 * copies of the Software, and to permit persons to whom the Software is
 * furnished to do so, subject to the following conditions:
 *
 * The above copyright notice and this permission notice shall be included in all
 * copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
 * SOFTWARE.
 */

/// <summary>
/// A cross platform,
///   cross game engine,
///   cross game universe
/// <c>achievement framework</c>.
///  And hopefully also lightweight.
/// </summary>
namespace Rekompenco
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    /// <summary>
    /// The Achievement struct for the individual achievements.
    /// </summary>
    public struct Achievement
    {
        /// <summary>
        /// The ID of the <see cref="Achievement"/>.
        /// </summary>
        public readonly string ID;
        /// <summary>
        /// The human readable name of the <see cref="Achievement"/>.
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The description of the <see cref="Achievement"/>.
        /// </summary>
        public readonly string Description;
        /// <summary>
        /// Describes what kind of data the <see cref="Data"/>-field is.
        /// Please check https://github.com/Winnak/Rekompenco for a list of the standard types.
        /// </summary>
        public readonly UInt16 DataType;
        /// <summary>
        /// An optional data field, that can contain an image of the <see cref="Achievement"/>.
        /// <remark>Base64 encoded</remark>
        /// </summary>
        public readonly string Data;

        /// <summary>
        /// Constructor for the Achievement struct.
        /// </summary>
        /// <param name="id"><see cref="ID"/>.</param>
        /// <param name="name"><see cref="Name"/>.</param>
        /// <param name="description"><see cref="Description"/>.</param>
        /// <param name="datatype"><see cref="DataType"/>.</param>
        /// <param name="data"><see cref="data"/>.</param>
        public Achievement(string id, string name, string description, UInt16 datatype, string data)
        {
            ID = id;
            Name = name;
            Description = description;
            DataType = datatype;
            Data = data;
        }

        /// <summary>
        /// Constructor for the Achievement struct. (with no custom data)
        /// </summary>
        /// <param name="id"><see cref="ID"/>.</param>
        /// <param name="name"><see cref="Name"/>.</param>
        /// <param name="description"><see cref="Description"/>.</param>
        public Achievement(string id, string name, string description = "")
        {
            ID = id;
            Name = name;
            Description = description;
            DataType = 0;
            Data = string.Empty;
        }


        /// <summary>
        /// Gets the human readable <see cref="Name"/> of the <see cref="Achievement"/>.
        /// </summary>
        /// <returns><see cref="Name"/>.</returns>
        public override string ToString()
        {
            return Name;
        }

        /// <summary>
        /// Compares the <see cref="ID"/> of the <see cref="Achievement"/>s.
        /// </summary>
        /// <param name="obj">Another <see cref="Achievement"/>.</param>
        /// <returns><c>True</c> if the achievements are equal otherwise; <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (!(obj is Achievement))
            {
                return false;
            }
            return ID.Equals(obj);
        }

        /// <summary>
        /// Gets the hash code of the achievement (corresponding to the hash code of the <see cref="ID"/>).
        /// </summary>
        /// <returns>The hash code of <see cref="ID"/>.</returns>
        public override int GetHashCode()
        {
            return ID.GetHashCode();
        }

        /// <summary>
        /// Formats the <see cref="Achievement"/> so that it can be saved in the <see cref="Rekompenco.FileName"/>.
        /// </summary>
        /// <returns>A semicolon separated line containing all the fields of the <see cref="Achievement"/></returns>
        public string ToSavedFormat()
        {
            return string.Concat(
                ID, "|",
                Name, "|",
                Description, "|",
                DataType.ToString(), "|",
                Data);
        }
    }

    /// <summary>
    /// Provides a set of helper functions to read and write achievements.
    /// </summary>
    public static class Utility
    {
        /// <summary>
        /// A dictionary with all the gained achievements.
        /// The key is the same as <see cref="Achievement.ID"/>.
        /// <remark>
        /// Recommended use is to use the helper functions:
        /// <see cref="HasAchivement(string)"/> and <see cref="UnlockAchievement(string, Achievement)"/>.
        /// </remark>
        /// </summary>
        public static readonly Dictionary<string, Achievement> LoadedAchievements;

        /// <summary>
        /// Just the file name of the <see cref="Rekompenco"/> system.
        /// </summary>
        public const string FileName = "default_rekompenco.rkpc";
        /// <summary>
        /// The full path of the <see cref="Rekompenco"/> system.
        /// Usual Windows path: C:\Users\[USERNAME]\AppData\Roaming\Rekompenco\default_rekompenco.rkpc
        /// </summary>
        public static readonly string FilePath = string.Concat(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            Path.DirectorySeparatorChar,
            "Rekompenco",
            Path.DirectorySeparatorChar,
            FileName);

        static Utility()
        {
            if (!File.Exists(FilePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
                File.CreateText(FilePath).Close();
                LoadedAchievements = new Dictionary<string, Achievement>();
                return;
            }

            var row = File.ReadAllLines(FilePath);
            LoadedAchievements = new Dictionary<string, Achievement>(row.Length);
            for (int i = 0; i < row.Length; i++)
            {
                var line = row[i].Split(new char[] { '|' }, StringSplitOptions.None);

                if (line.Length != 5)
                {
                    throw new FileLoadException(
                        string.Concat("File had a malformed a row (Line: ", i.ToString(),
                        ") without either the id, name, description or data."),
                        FilePath);
                }

                LoadedAchievements.Add(line[0],
                    new Achievement(
                        /* ID */          line[0],
                        /* Name */        line[1],
                        /* Description */ line[2],
                        /* Data Type */   UInt16.Parse(line[3]),
                        /* Data */        line[4]
                    ));
            }
        }

        /// <summary>
        /// Checks if a certain <see cref="Achievement"/> has been collected.
        /// </summary>
        /// <remark>Will also initialize the system if it hasn't been done yet.</remark>
        /// <param name="achievementID">The achievement ID corresponding to the <see cref="Achievement.ID"/>.</param>
        /// <returns><c>True</c> if the achievement has been found otherwise; <c>false</c>.</returns>
        public static bool HasAchivement(string achievementID)
        {
            return LoadedAchievements.ContainsKey(achievementID);
        }

        /// <summary>
        /// Unlocks an <see cref="Achievement"/> and saves it to the <see cref="FileName"/>.
        /// </summary>
        /// <param name="achievmentID"><see cref="Achievement.ID"/>.</param>
        /// <param name="achievement"><see cref="Achievement"/>.</param>
        /// <returns><c>False</c> if the achievement has already been achieved otherwise; <c>true</c>.</returns>
        public static bool UnlockAchievement(string achievmentID, Achievement achievement)
        {
            if (LoadedAchievements.ContainsKey(achievmentID))
            {
                return false;
            }
            var achievementEntry = achievement.ToSavedFormat();
            if (achievementEntry.Length > 512)
            {
                return false;
            }
            var file = File.AppendText(FilePath);
            file.WriteLine(achievementEntry);
            file.Close();
            LoadedAchievements.Add(achievmentID, achievement);
            return true;
        }
    }
}
