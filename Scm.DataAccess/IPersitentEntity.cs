using System;

namespace Scm.DataAccess
{
    public interface IPersitentEntity
    {
        ISink Sink(Type targetType);
    }
}