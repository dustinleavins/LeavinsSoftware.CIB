// Copyright (c) 2013 Dustin Leavins
// See the file 'LICENSE.txt' for copying permission.
using LeavinsSoftware.Collection.Models;
using System;
using System.IO;
using System.Xml.Serialization;

namespace LeavinsSoftware.Collection.Persistence
{
    public sealed class ProgramOptionsPersistence : IProgramOptionsPersistence
    {
        public ProgramOptionsPersistence(string fileName)
        {
            FileName = fileName;
        }

        public string FileName { get; private set; }

        public void Update(ProgramOptions options)
        {
            options.Validate();
            var serializer = new XmlSerializer(typeof(ProgramOptions));

            using (var stream = File.Create(FileName))
            {
                serializer.Serialize(stream, options);
            }
        }

        public ProgramOptions Retrieve()
        {
            ProgramOptions returnOptions;

            if (File.Exists(FileName))
            {
                var serializer = new XmlSerializer(typeof(ProgramOptions));

                using (var stream = File.OpenRead(FileName))
                {
                    returnOptions = serializer.Deserialize(stream) as ProgramOptions;
                }
            }
            else
            {
                returnOptions = new ProgramOptions();
            }

            return returnOptions;
        }
    }
}
