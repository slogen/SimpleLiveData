using System;
using System.ComponentModel.DataAnnotations;

namespace SimpleLiveData.App.DataModel
{
    public abstract class AbstractEntity
    {
        [Key]
        public Guid Id { get; set; }

        protected AbstractEntity(Guid id)
        {
            Id = id;
        }
    }
}