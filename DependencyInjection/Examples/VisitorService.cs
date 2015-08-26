using System;
using System.Collections.Generic;

using RandomNameGeneratorLibrary;

namespace DependencyInjection.Examples
{
    public class VisitorService : IVisitorService
    {
        private readonly IPersonNameGenerator _generator;

        public VisitorService(IPersonNameGenerator generator)
        {
            _generator = generator;
        }

        public IEnumerable<string> GetNewVisitor(int num)
        {
            return _generator.GenerateMultipleFirstAndLastNames(num);
        }
    }
}