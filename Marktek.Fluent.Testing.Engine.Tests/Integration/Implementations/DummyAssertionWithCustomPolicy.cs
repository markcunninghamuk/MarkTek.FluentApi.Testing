using Marktek.Fluent.Testing.Engine.Tests.Models;
using MarkTek.Fluent.Testing.RecordGeneration;
using System;
using System.Collections.Generic;

namespace Marktek.Fluent.Testing.Engine.Tests
{
    internal class DummyAssertionWithCustomPolicy : BaseValidator<Guid, DummyModel>
    {
        private int attempt = 0;
        private Guid id;

        public DummyAssertionWithCustomPolicy(Guid passedInExternalId)
        {
            this.id = passedInExternalId;
        }

        public override DummyModel GetRecord(Guid id)
        {
            attempt++;

            if (attempt < 4)
            {
                return new DummyModel
                {
                    Id = Guid.NewGuid()
                };
            }
            return new DummyModel
            {
                Id = id
            };
        }

        public override List<ISpecificationValidator<DummyModel>> GetValidators()
        {
            return new List<ISpecificationValidator<DummyModel>>
            {
                  new EnsureAggregateIdMatches(id)
            };
         
        }
    }
}