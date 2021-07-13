using Marktek.Fluent.Testing.Engine;
using Marktek.Fluent.Testing.Engine.Interfaces;
using Polly;
using System;
using System.Collections.Generic;

namespace MarkTek.Fluent.Testing.RecordGeneration
{
    /// <summary>
    /// The generic interface 
    /// </summary>
    /// <typeparam name="TID"></typeparam>
    public interface IRecordService<TID>
    {
        /// <summary>
        /// A generic interface for fluent record creation 
        /// </summary>
        /// <param name="spec"></param>
        /// <returns></returns>
        IRecordService<TID> AssertAgainst<TType>(BaseValidator<TID, TType> spec);

        /// <summary>
        /// Assert against one or more assertions with a custom policy.
        /// </summary>
        /// <typeparam name="TType"></typeparam>
        /// <param name="spec"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        IRecordService<TID> AssertAgainst<TType>(BaseValidator<TID, TType> spec, Policy policy);

        /// <summary>
        /// Performs an action and waits for it to complete before proceeding.
        /// </summary>
        /// <param name="implemetation"></param>
        /// <returns></returns>
        IRecordService<TID> WaitFor(IWaitableAction implemetation);

        /// <summary>
        /// Performs an action and waits for it to complete before proceeding.
        /// </summary>
        /// <param name="implemetation"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        IRecordService<TID> WaitFor(IWaitableAction implemetation, Policy policy);

        /// <summary>
        /// Sets the aggregate Id to be the record that was last created
        /// </summary>
        /// <returns></returns>
        TID GetAggregateId();

        //Gets the AggregateId
        IRecordService<TID> AssignAggregateId();

        /// <summary>
        /// Creates a record of type T where T is a class
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="implementation"></param>
        /// <returns></returns>
        IRecordService<TID> CreateRecord<T>(IRecordCreator<T, TID> implementation);

        /// <summary>
        /// Creates a related record of type T where T is a class and passes in the previously created id from the CreateRecord method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="implementation"></param>
        /// <returns></returns>
        IRecordService<TID> CreateRelatedRecord<T>(IRelatedRecordCreator<T, TID> implementation);

        /// <summary>
        /// Creates a related record of type T where T is a class and passes in all of the previously created Ids
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="implementation"></param>
        /// <returns></returns>
        IRecordService<TID> CreateRelatedRecord<T>(IRelatedRecordCreatorComposite<T, TID> implementation);


        int GetRecordCount();

        IRecordService<TID> CreatedRelatedRecord<TParent,T>(IRelatedRecordCreator<TParent, T, TID> record);


        /// <summary>
        /// Creates a related record of type T where T is a class and passes in the previously created id from the CreateRecord method
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="implementation"></param>
        /// <returns></returns>
        IRecordService<TID> ExecuteAction(IExecutableAction<TID> implementation, bool executeOnAggregate = false);
              
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cond"></param>
        /// <param name="builder"></param>
        /// <returns></returns>
        IRecordService<TID> If(bool cond, Func<IRecordService<TID>, IRecordService<TID>> builder);
        
        /// <summary>
        /// Cleans up records Created during the record service, Uses the Aggregate Id to retrieve the record and related children and cleandown.
        /// </summary>
        /// <param name="cleanup"></param>
        void Cleanup(IRecordCleanup<TID> cleanup);

        /// <summary>
        /// Cleans up records Created during the record service, Uses the Aggregate Id to retrieve the record and related children and cleandown.
        /// </summary>
        /// <param name="cleanup"></param>
        IRecordService<TID> PreExecutionAction(IPreExecution execute);

    }
}