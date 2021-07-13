using FluentAssertions;
using Marktek.Fluent.Testing.Engine.Tests.Models;
using MarkTek.Fluent.Testing.RecordGeneration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Polly;
using System;

namespace Marktek.Fluent.Testing.Engine.Tests
{
    [TestClass]
    [TestCategory("Integration")]

    public class RecordServiceTests
    {
        private IRecordService<Guid> service;
        private Guid aggregateId { get; set; }

        [TestInitialize]
        public void Setup()
        {
            var retryIfException = Policy
                .Handle<ArgumentNullException>()
                .Or<InvalidOperationException>()
                .Or<Exception>()
                .WaitAndRetry(5, retryAttempt => TimeSpan.FromSeconds(Math.Pow(3, retryAttempt)));

            aggregateId = Guid.NewGuid();
            service = new RecordService<Guid>(aggregateId, retryIfException);
        }

        [TestMethod]
        public void Can_Get_AggregateId()
        {
            service.GetAggregateId().Should().Be(aggregateId);
        }

        [TestMethod]
        public void Can_Get_Assigned_AggregateId()
        {
            service
                .CreateRecord(new CreateDummyExample())
                .AssignAggregateId();

            service.GetAggregateId().Should().NotBe(aggregateId);
        }

        [TestMethod]
        public void Can_Get_Assigned_AggregateId_Even_When_Creating_RelatedRecords()
        {
            service
                .CreateRecord(new CreateDummyExample())
                .CreateRelatedRecord(new CreateDummyExampleRelated());

            service.GetAggregateId().Should().Be(aggregateId);
            service.GetRecordCount().Should().Be(2);
        }

        [TestMethod]
        public void Can_Create_Related_Record_And_Pass_Parent_As_Object()
        {
            service
                .CreateRecord(new CreateDummyExample())
                .CreatedRelatedRecord<DummyModel, DummyRelatedModel>(new CreateDummyExampleRelated());

            service.GetAggregateId().Should().Be(aggregateId);
            service.GetRecordCount().Should().Be(2);
        }

        [TestMethod]
        public void Implements_Correct_Interface()
        {
            service.Should().BeAssignableTo(typeof(IRecordService<Guid>));
        }

        [DataTestMethod]
        [DataRow(true)]
        [DataRow(false)]
        public void Can_Execute_Action_Against_Aggregate(bool useAggregate)
        {
            service
               .CreateRecord(new CreateDummyExample())
               .ExecuteAction(new ExecuteDummyAction(), useAggregate);

            service.GetRecordCount().Should().Be(1);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void Cannot_Execute_Action_Even_When_First_Call()
        {
            service.ExecuteAction(new ExecuteDummyAction(), false);
            service.GetRecordCount().Should().Be(0);
        }

        [ExpectedException(typeof(ArgumentException))]
        [TestMethod]
        public void Error_When_Assigning_AggregateId_When_No_Previous_Operation()
        {
            service.AssignAggregateId();
        }

        [TestMethod]
        public void Can_Validate_Using_The_Aggregate()
        {
            service
                .CreateRecord(new CreateDummyExample())
                .AssertAgainst(new DummyAssertionExample(service.GetAggregateId()));
        }

        [TestMethod]
        public void Can_Validate_Using_The_Aggregate_With_Custom_Policy()
        {
            Policy testPolicy = Policy
           .Handle<AssertFailedException>()
           .WaitAndRetry(4, retryAttempt => TimeSpan.FromSeconds(1));

            service
                .CreateRecord(new CreateDummyExample())
                .AssertAgainst(new DummyAssertionWithCustomPolicy(service.GetAggregateId()), testPolicy);
        }


        [TestMethod]
        public void Can_Cleanup_When_No_Records_Created()
        {
            service
               .CreateRecord(new CreateDummyExample())
               .Cleanup(new DummyCleanup());
        }

        [TestMethod]
        public void Can_Run_PreCondition_Code()
        {
            service
              .PreExecutionAction(new DummyPreExecute());

            service.GetRecordCount().Should().Be(0);
        }

        [TestMethod]
        public void Can_Run_WaitFor_Code()
        {
            service
              .WaitFor(new DummyWaitFor());

            service.GetRecordCount().Should().Be(0);
        }

        [TestMethod]
        public void Can_Run_WaitFor_Code_With_Policy()
        {
            Policy testPolicy = Policy
               .Handle<Exception>()
               .WaitAndRetry(4, retryAttempt => TimeSpan.FromSeconds(1));

            service
              .WaitFor(new DummyWaitForWithCustomPolicy(), testPolicy);

            service.GetRecordCount().Should().Be(0);
        }

        [DataTestMethod]
        [DataRow(1)]
        [DataRow(0)]
        public void Can_Run_If_Conditions(int expected)
        {
            service
              .If(expected == 1, x => x.CreateRecord(new CreateDummyExample()));

            service.GetRecordCount().Should().Be(expected);
        }


        [DataTestMethod]
        public void Can_Handle_Multiple_Keys()
        {
            service.CreateRecord(new CreateDummyExample())
                .CreateRelatedRecord(new CreateDummyExampleRelated())
                .CreateRelatedRecord(new CreateDummyExampleComposite());

            service.GetRecordCount().Should().Be(3);
        }

    }
}
