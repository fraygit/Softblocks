using softblocks.data.Interface;
using softblocks.data.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace softblocks.library.Services
{
    public class DocumentTypeService
    {
        private IDocumentTypeRepository _documentTypeRepository;

        public DocumentTypeService(IDocumentTypeRepository _documentTypeRepository)
        {
            this._documentTypeRepository = _documentTypeRepository;
        }

        public async Task<DocumentType> Create(DocumentType documentType)
        {
            await _documentTypeRepository.CreateSync(documentType);
            return documentType;
        }

        public async Task<DocumentType> Get(string id)
        {
            var documentType = await _documentTypeRepository.Get(id);
            return documentType;
        }

        //public async Task<DocumentType> AddField(string id, Field field)
        //{
        //    var documentType = await _documentTypeRepository.Get(id);
        //    if (documentType != null)
        //    {
        //        if (documentType.Fields == null)
        //        {
        //            documentType.Fields = new List<Field>();
        //        }
        //        field.FieldId = Guid.NewGuid().ToString();
        //        documentType.Fields.Add(field);
        //        await _documentTypeRepository.Update(id, documentType);
        //        return documentType;
        //    }
        //    throw new Exception("Document type does not exists!");
        //}
    }
}
