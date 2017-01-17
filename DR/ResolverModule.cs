using System.Data.Entity;
using BLL.Interfaces;
using BLL.Services;
using DAL.Interfaces;
using DAL.Repositories;
using Logger;
using Ninject;
using Ninject.Web.Common;
using ORM;


namespace DR
{
    public static class ResolverModule
    {
        public static void ConfigurateResolver(this IKernel kernel)
        {
            kernel.Bind<IUnitOfWork>().To<UnitOfWork>().InRequestScope();
            kernel.Bind<DbContext>().To<StorageEntities>().InRequestScope();

            kernel.Bind<ILogAdapter>().To<NLogAdapter>();

            kernel.Bind<IUserService>().To<UserService>();
            kernel.Bind<IFileService>().To<FileService>();
            kernel.Bind<IFileTypeService>().To<FileTypeService>();
            kernel.Bind<IFolderService>().To<FolderService>();
            kernel.Bind<IRoleService>().To<RoleService>();

            kernel.Bind<IUserRepository>().To<UserRepository>();
            kernel.Bind<IFileRepository>().To<FileRepository>();
            kernel.Bind<IFileTypeRepository>().To<FileTypeRepository>();
            kernel.Bind<IFolderRepository>().To<FolderRepository>();
            kernel.Bind<IRoleRepository>().To<RoleRepository>();
        }
    }
}