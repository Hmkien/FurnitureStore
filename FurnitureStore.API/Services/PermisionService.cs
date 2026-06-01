using FurnitureStore.API.Interface;
using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Repositories;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Services
{
    public class PermisionService : CrudService<Permision, PermisionForm>, IPermisionService
    {
        public PermisionService(IRepository<Permision> repository) : base(repository)
        {
        }

        protected override Permision MapToNew(PermisionForm form) => new()
        {
            PermisionName = form.PermisionName,
            PermisionCode = form.PermisionCode,
            Description = form.Description
        };

        protected override void ApplyUpdate(Permision entity, PermisionForm form)
        {
            entity.PermisionName = form.PermisionName;
            entity.PermisionCode = form.PermisionCode;
            entity.Description = form.Description;
        }
    }
}
