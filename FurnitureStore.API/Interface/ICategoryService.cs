using FurnitureStore.API.Models.Entities;
using FurnitureStore.API.Models.Form;
using FurnitureStore.API.Services.Base;

namespace FurnitureStore.API.Interface
{
    public interface ICategoryService : ICrudService<Category, CategoryForm>
    {
    }
}
