using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using BLL.DTO;
using BLL.Interfaces;
using BLL.Mappers;
using DAL.DTO;
using DAL.Interfaces;
using static BLL.Mappers.BllMapper;

namespace BLL.Services
{
    public class FileTypeService : IFileTypeService
    {

        private readonly IUnitOfWork uow;
        private readonly IFileTypeRepository fileTypeRepository;

        public FileTypeService(IUnitOfWork uow, IFileTypeRepository repository)
        {
            this.uow = uow;
            fileTypeRepository = repository;
        }

        public IEnumerable<DtoFileType> GetAllFileTypes() 
            => fileTypeRepository.GetAll().Select(item => item.ToDtoFileType());
        

        public DtoFileType GetFileTypeById(long key)
            => fileTypeRepository.GetById(key).ToDtoFileType();


        public IEnumerable<DtoFileType> GetFileTypesByPredicate(Expression<Func<DtoFileType, bool>> func)
            => fileTypeRepository.GetByPredicate(Convert<DtoFileType, DalFileType>(func))
                    .Select(item => item.ToDtoFileType());

        public DtoFileType CreateFileType(DtoFileType e)
        {
            var fileType = fileTypeRepository.Create(e.ToDalFileType());
            uow.Commit();

            return fileType.ToDtoFileType();
        }

        public void DeleteFileType(DtoFileType e)
        {
            fileTypeRepository.Delete(e.ToDalFileType());
            uow.Commit();
        }

        public void UpdateFileType(DtoFileType e)
        {
            fileTypeRepository.Update(e.ToDalFileType());
            uow.Commit();
        }
    }
}
