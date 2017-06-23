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
    }
}
