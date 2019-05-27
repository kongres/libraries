namespace Kongrevsky.Infrastructure.Repository.Triggers.Relationships
{
    using System;
    using System.Collections.Generic;

    public static class TriggersTuple<Db> where Db : KongrevskyDbContext
    {
        public static void ConfigureBeforeSaveChanges(Action<IEnumerable<Tuple<object, object>>> addedRelationshipsAction, Action<IEnumerable<Tuple<object, object>>> removedRelationshipsAction)
        {
            KongrevskyUnitOfWork<Db>.AddedBeforeSaveChanges = addedRelationshipsAction;
            KongrevskyUnitOfWork<Db>.RemovedBeforeSaveChanges = removedRelationshipsAction;
        }


        public static void ConfigureAfterSaveChanges(Action<IEnumerable<Tuple<object, object>>> addedRelationshipsAction, Action<IEnumerable<Tuple<object, object>>> removedRelationshipsAction)
        {
            KongrevskyUnitOfWork<Db>.AddedAfterSaveChanges = addedRelationshipsAction;
            KongrevskyUnitOfWork<Db>.RemovedAfterSaveChanges = removedRelationshipsAction;
        }
    }
}