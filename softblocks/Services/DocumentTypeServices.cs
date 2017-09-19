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

        private List<Field> FindDocumentType(List<Field> fields,  ObjectId documentTypeId)
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
                        return FindDocumentType(field.Fields, documentTypeId);
                    }
                }
            }
            return null;
        }

        public async Task<List<Field>> FindDocumentType(string appModuleId, ObjectId documentTypeId)
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
                            var fields = FindDocumentType(document.Fields, documentTypeId);
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
    }
}