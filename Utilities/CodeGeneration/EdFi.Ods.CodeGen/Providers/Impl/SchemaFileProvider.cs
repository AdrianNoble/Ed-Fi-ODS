﻿// SPDX-License-Identifier: Apache-2.0
// Licensed to the Ed-Fi Alliance under one or more agreements.
// The Ed-Fi Alliance licenses this file to you under the Apache License, Version 2.0.
// See the LICENSE and NOTICES files in the project root for more information.
 
using System.IO;
using Castle.Core.Logging;
using EdFi.Ods.Common;

namespace EdFi.Ods.CodeGen.Providers.Impl
{
    public class SchemaFileProvider : ISchemaFileProvider
    {
        private const string EdFiSchemaName = "Ed-Fi-Core.xsd";
        private const string EdFiSchemaAnnotationName = "SchemaAnnotation.xsd";
        private const string ExtensionSchemaName = "EXTENSION-Ed-Fi-Extended-Core.xsd";

        private readonly string _standardSchemaFolder;

        private ILogger Logger { get; set; } = NullLogger.Instance;

        public SchemaFileProvider(IMetadataFolderProvider metadataFolderProvider)
        {
            Preconditions.ThrowIfNull(metadataFolderProvider, nameof(metadataFolderProvider));
            _standardSchemaFolder = metadataFolderProvider.GetStandardSchemaFolder();
        }

        private string GetExceptionMessage(string folder, string file)
            => $"Unable to find XSD file '{file}'.  Please make certain the XSD files exist in the {folder} folder.";

        public string GetEdFiSchema()
        {
            var file = Path.Combine(_standardSchemaFolder, EdFiSchemaName);

            return File.Exists(file)
                ? file
                : throw new FileNotFoundException(GetExceptionMessage(_standardSchemaFolder, EdFiSchemaName));
        }

        public string GetEdFiSchemaAnnotation()
        {
            var file = Path.Combine(_standardSchemaFolder, EdFiSchemaAnnotationName);

            return File.Exists(file)
                ? file
                : throw new FileNotFoundException(GetExceptionMessage(_standardSchemaFolder, EdFiSchemaAnnotationName));
        }

        public string GetExtensionSchema(string folder)
        {
            var file = Path.Combine(folder, ExtensionSchemaName);

            if (File.Exists(file))
            {
                return file;
            }

            // Extension schemas are optional
            Logger.Warn(GetExceptionMessage(folder, ExtensionSchemaName));
            return null;
        }
    }
}
