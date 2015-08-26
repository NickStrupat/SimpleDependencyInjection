using System;
using System.Collections.Generic;

namespace DependencyInjection.Examples
{
    public interface IVisitorService
    {
        IEnumerable<string> GetNewVisitor(int num);
    }
}