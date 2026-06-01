using FurnitureStore.API.Models.Enums;

namespace FurnitureStore.API.Models.Entities
{
    public class BaseEntity
    {
        public Guid Id { get; set; } = Guid.CreateVersion7();

        public DateTime Created { get; set; }

        public DateTime LastModified { get; set; }

        public Guid CreatedBy { get; set; }
        public Guid LastModifiedBy { get; set; }
        public StatusEntity Status { get; set; } = StatusEntity.Approved;

    }
}

