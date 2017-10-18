using MongoDB.Bson;
using softblocks.data.Interface;
using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace softblocks.Services
{
    public class DocumentTypeServices
    {
        private IAppModuleRepository _appModuleRepository;

        public DocumentTypeServices(IAppModuleRepository _appModuleRepository)
        {
            this._appModuleRepository = _appModuleRepository;
        }

        private List<Field> FindDocumentTypeFields(List<Field> fields,  ObjectId documentTypeId)
        {
            if (fields != null)
            {
                foreach (var field in fields.Where(n => n.DataType == "Document Type"))
                {
                    if (field.Id == documentTypeId)
                    {
                        return field.Fields;
                    }
                    else
                    {
                        return FindDocumentTypeFields(field.Fields, documentTypeId);
                    }
                }
            }
            return null;
        }

        public async Task<List<Field>> FindDocumentTypeFields(string appModuleId, ObjectId documentTypeId)
        {
            var app = await _appModuleRepository.Get(appModuleId);
            if (app != null)
            {
                if (app.DocumentTypes != null)
                {
                    if (app.DocumentTypes.Any(n => n.Id == documentTypeId))
                    {
                        return app.DocumentTypes.FirstOrDefault(n => n.Id == documentTypeId).Fields;
                    }
                    else
                    {
                        foreach (var document in app.DocumentTypes)
                        {
                            var fields = FindDocumentTypeFields(document.Fields, documentTypeId);
                            if (fields != null)
                            {
                                return fields;
                            }
                        }
                    }
                }
            }
            return null;
        }

        public async Task<string> FindSubDocumentHierarchy(string appModuleId, ObjectId documentTypeId, string subDocumentName, string rootDocumentName)
        {
            var parentName = await FindParentDocumentTypeName(appModuleId, documentTypeId, subDocumentName);
            if (string.IsNullOrEmpty(parentName)) //root
            {
                return subDocumentName;
            }
            else
            {
                var name = subDocumentName;
                while (true)
                {
                    name = parentName + "." + name;
                    parentName = await FindParentDocumentTypeName(appModuleId, documentTypeId, parentName);
                    if (string.IsNullOrEmpty(parentName)) // root
                    {
                        break;
                    }
                }
                return name;
            }
        }

        private string FindParentDocumentTypeName(string subDocumentName, List<Field> fields, string parentName)
        {
            foreach (var field in fields)
            {
                if (field.Name == subDocumentName)
                {
                    return parentName;
                }
                else
                {
                    if (field.Fields != null) 
                    {
                        return FindParentDocumentTypeName(subDocumentName, field.Fields, field.Name);
                    }                    
                }
            }
            return null;
        }

        public async Task<string> FindParentDocumentTypeName(string appModuleId, ObjectId documentTypeId, string subDocumentName)
        {
            var app = await _appModuleRepository.Get(appModuleId);
            if (app != null)
            {
                if (app.DocumentTypes != null)
                {
                    if (app.DocumentTypes.Any(n => n.Id == documentTypeId))
                    {
                        var documentType = app.DocumentTypes.FirstOrDefault(n => n.Id == documentTypeId);
                        foreach (var field in documentType.Fields)
                        {
                            if (field.Fields != null)
                            {
                                var name = FindParentDocumentTypeName(subDocumentName, field.Fields, field.Name);
                                if (name != null)
                                {
                                    return name;
                                }
                            }
                        }
                    }
                }
            }
            return "";
        }

        private string FindDocumentTypeName(List<Field> fields, ObjectId documentTypeId)
        {
            if (fields != null)
            {
                foreach (var field in fields.Where(n => n.DataType == "Document Type"))
                {
                    if (field.Id == documentTypeId)
                    {
                        return field.Name;
                    }
                    else
                    {
                        return FindDocumentTypeName(field.Fields, documentTypeId);
                    }
                }
            }
            return null;
        }

        public async Task<string> FindDocumentTypeName(string appModuleId, ObjectId documentTypeId)
        {
            var app = await _appModuleRepository.Get(appModuleId);
            if (app != null)
            {
                if (app.DocumentTypes != null)
                {
                    if (app.DocumentTypes.Any(n => n.Id == documentTypeId))
                    {
                        return app.DocumentTypes.FirstOrDefault(n => n.Id == documentTypeId).Name;
                    }
                    else
                    {
                        foreach (var document in app.DocumentTypes)
                        {
                            var docName = FindDocumentTypeName(document.Fields, documentTypeId);
                            if (docName != null)
                            {
                                return docName;
                            }
                        }
                    }
                }
            }
            return null;
        }
    }
}