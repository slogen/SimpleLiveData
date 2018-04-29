using System;
using System.ComponentModel.DataAnnotations;

namespace DataSys.App.DataModel
{
    public abstract class AbstractEntity
    {
        protected AbstractEntity()
        {
        }

        protected AbstractEntity(Guid id)
        {
            Id = id;
        }

        [Key] public Guid Id { get; set; }
    }
}