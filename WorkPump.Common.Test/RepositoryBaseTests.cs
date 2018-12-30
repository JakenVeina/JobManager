using System;
using System.Collections.Generic;
using System.Linq;
using Moq;
using NUnit.Framework;
using Shouldly;

namespace WorkPump.Common.Test
{
    [TestFixture]
    public class RepositoryBaseTests
    {
        #region Test Context

        public static (TestEntity[] uutEntities, RepositoryBase<TestEntity, ulong> uut) BuildTestContext(ulong[] uutEntityIds)
        {
            var uut = new Mock<RepositoryBase<TestEntity, ulong>>()
            {
                CallBase = true
            }.Object;

            var uutEntities = uutEntityIds
                .Select(id => new TestEntity(id))
                .ToArray();

            uut.Insert(uutEntities);

            return (uutEntities, uut);
        }

        public class TestEntity : EntityBase<ulong>
        {
            public TestEntity(ulong id)
                : base(id) { }
        }

        #endregion Test Context

        #region Test Data

        public static readonly ulong[][] UUTEntityIdsTestCases
            = new[]
            {
                new ulong[0],
                new[] { 1UL },
                new[] { 2UL },
                new[] { 3UL },
                new[] { 1UL, 2UL },
                new[] { 1UL, 3UL },
                new[] { 2UL, 3UL },
                new[] { 1UL, 2UL, 3UL }
            };
            

        public static readonly object[][] ValidIdTestCases
            = new[]
            {
                new object[] { new[] { 1UL },           1UL },
                new object[] { new[] { 2UL },           2UL },
                new object[] { new[] { 3UL },           3UL },
                new object[] { new[] { 1UL, 2UL },      1UL },
                new object[] { new[] { 1UL, 2UL },      2UL },
                new object[] { new[] { 1UL, 3UL },      1UL },
                new object[] { new[] { 1UL, 3UL },      3UL },
                new object[] { new[] { 2UL, 3UL },      2UL },
                new object[] { new[] { 2UL, 3UL },      3UL },
                new object[] { new[] { 1UL, 2UL, 3UL }, 1UL },
                new object[] { new[] { 1UL, 2UL, 3UL }, 2UL },
                new object[] { new[] { 1UL, 2UL, 3UL }, 3UL }
            };

        public static readonly object[][] InvalidIdTestCases
            = new[]
            {
                new object[] { new ulong[0],       1UL },
                new object[] { new ulong[0],       2UL },
                new object[] { new ulong[0],       3UL },
                new object[] { new[] { 1UL },      2UL },
                new object[] { new[] { 1UL },      3UL },
                new object[] { new[] { 2UL },      1UL },
                new object[] { new[] { 2UL },      3UL },
                new object[] { new[] { 3UL },      1UL },
                new object[] { new[] { 3UL },      2UL },
                new object[] { new[] { 1UL, 2UL }, 3UL },
                new object[] { new[] { 1UL, 3UL }, 2UL },
                new object[] { new[] { 2UL, 3UL }, 1UL }
            };

        public static readonly object[][] AllIdsExistTestCases
            = new[]
            {
                new object[] { new[] { 1UL },           new[] { 1UL } },
                new object[] { new[] { 2UL },           new[] { 2UL } },
                new object[] { new[] { 3UL },           new[] { 3UL } },
                new object[] { new[] { 1UL, 2UL },      new[] { 1UL } },
                new object[] { new[] { 1UL, 2UL },      new[] { 2UL }  },
                new object[] { new[] { 1UL, 2UL },      new[] { 1UL, 2UL }  },
                new object[] { new[] { 1UL, 3UL },      new[] { 1UL } },
                new object[] { new[] { 1UL, 3UL },      new[] { 3UL } },
                new object[] { new[] { 1UL, 3UL },      new[] { 1UL, 3UL } },
                new object[] { new[] { 2UL, 3UL },      new[] { 2UL } },
                new object[] { new[] { 2UL, 3UL },      new[] { 3UL } },
                new object[] { new[] { 2UL, 3UL },      new[] { 2UL, 3UL } },
                new object[] { new[] { 1UL, 2UL, 3UL }, new[] { 1UL } },
                new object[] { new[] { 1UL, 2UL, 3UL }, new[] { 2UL } },
                new object[] { new[] { 1UL, 2UL, 3UL }, new[] { 3UL } },
                new object[] { new[] { 1UL, 2UL, 3UL }, new[] { 1UL, 2UL } },
                new object[] { new[] { 1UL, 2UL, 3UL }, new[] { 1UL, 3UL } },
                new object[] { new[] { 1UL, 2UL, 3UL }, new[] { 2UL, 3UL } },
                new object[] { new[] { 1UL, 2UL, 3UL }, new[] { 1UL, 2UL, 3UL } }
            };

        public static readonly object[][] SomeIdsExistAndSomeDoNotExistTestCases
            = new[]
            {
                new object[] { new[] { 1UL },      new[] { 1UL, 2UL } },
                new object[] { new[] { 1UL },      new[] { 1UL, 3UL } },
                new object[] { new[] { 1UL },      new[] { 1UL, 2UL, 3UL } },
                new object[] { new[] { 2UL },      new[] { 1UL, 2UL } },
                new object[] { new[] { 2UL },      new[] { 2UL, 3UL } },
                new object[] { new[] { 2UL },      new[] { 1UL, 2UL, 3UL } },
                new object[] { new[] { 3UL },      new[] { 1UL, 3UL } },
                new object[] { new[] { 3UL },      new[] { 2UL, 3UL } },
                new object[] { new[] { 3UL },      new[] { 1UL, 2UL, 3UL } },
                new object[] { new[] { 1UL, 2UL }, new[] { 1UL, 3UL } },
                new object[] { new[] { 1UL, 2UL }, new[] { 2UL, 3UL } },
                new object[] { new[] { 1UL, 2UL }, new[] { 1UL, 2UL, 3UL } },
                new object[] { new[] { 1UL, 3UL }, new[] { 1UL, 2UL } },
                new object[] { new[] { 1UL, 3UL }, new[] { 2UL, 3UL } },
                new object[] { new[] { 1UL, 3UL }, new[] { 1UL, 2UL, 3UL } },
                new object[] { new[] { 2UL, 3UL }, new[] { 1UL, 2UL } },
                new object[] { new[] { 2UL, 3UL }, new[] { 1UL, 3UL } },
                new object[] { new[] { 2UL, 3UL }, new[] { 1UL, 2UL, 3UL } }
            };

        public static readonly object[][] AllEntityIdsDoNotExistTestCases
            = new[]
            {
                new object[] { new ulong[0],       new[] { 1UL } },
                new object[] { new ulong[0],       new[] { 2UL } },
                new object[] { new ulong[0],       new[] { 3UL } },
                new object[] { new ulong[0],       new[] { 1UL, 2UL } },
                new object[] { new ulong[0],       new[] { 1UL, 3UL } },
                new object[] { new ulong[0],       new[] { 2UL, 3UL } },
                new object[] { new ulong[0],       new[] { 1UL, 2UL, 3UL } },
                new object[] { new[] { 1UL },      new[] { 2UL } },
                new object[] { new[] { 1UL },      new[] { 3UL } },
                new object[] { new[] { 1UL },      new[] { 2UL, 3UL } },
                new object[] { new[] { 2UL },      new[] { 1UL } },
                new object[] { new[] { 2UL },      new[] { 3UL } },
                new object[] { new[] { 2UL },      new[] { 1UL, 3UL } },
                new object[] { new[] { 3UL },      new[] { 1UL } },
                new object[] { new[] { 3UL },      new[] { 2UL } },
                new object[] { new[] { 3UL },      new[] { 1UL, 2UL } },
                new object[] { new[] { 1UL, 2UL }, new[] { 3UL } },
                new object[] { new[] { 1UL, 3UL }, new[] { 2UL } },
                new object[] { new[] { 2UL, 3UL }, new[] { 1UL } }
            };

        #endregion Test Data

        #region Count Tests

        [TestCaseSource(nameof(UUTEntityIdsTestCases))]
        public void Count_Always_ReturnsUUTEntitiesCount(ulong[] uutEntityIds)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var result = uut.Count;

            result.ShouldBe(uutEntities.Length);
        }

        #endregion Count Tests

        #region Get() Tests

        [TestCaseSource(nameof(InvalidIdTestCases))]
        public void Get_IdIsNotValid_ThrowsException(ulong[] uutEntityIds, ulong id)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            Should.Throw<KeyNotFoundException>(() =>
            {
                var result = uut.Get(id);
            });
        }

        [TestCaseSource(nameof(ValidIdTestCases))]
        public void Get_IdIsValid_ReturnsMatchingEntity(ulong[] uutEntityIds, ulong id)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var result = uut.Get(id);

            result.ShouldBeSameAs(uutEntities.First(x => x.Id == id));
        }

        #endregion Get() Tests

        #region TryGet() Tests

        [TestCaseSource(nameof(InvalidIdTestCases))]
        public void TryGet_IdIsNotValid_ReturnsNull(ulong[] uutEntityIds, ulong id)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var result = uut.TryGet(id);

            result.ShouldBeNull();
        }

        [TestCaseSource(nameof(ValidIdTestCases))]
        public void TryGet_IdIsValid_ReturnsMatchingEntity(ulong[] uutEntityIds, ulong id)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var result = uut.TryGet(id);

            result.ShouldBeSameAs(uutEntities.First(x => x.Id == id));
        }

        #endregion TryGet() Tests

        #region Insert() Tests

        [TestCaseSource(nameof(ValidIdTestCases))]
        public void Insert_EntityExists_ReturnsFalseAndDoesNotInsertEntities(ulong[] uutEntityIds, ulong entityId)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var entity = uutEntities.First(x => x.Id == entityId);

            var result = uut.Insert(entity);

            result.ShouldBeFalse();
            uut.Count.ShouldBe(uutEntities.Length);
            uutEntities.EachShould(uutEntity =>
                uut.TryGet(uutEntity.Id).ShouldBeSameAs(uutEntity));
        }

        [TestCaseSource(nameof(ValidIdTestCases))]
        public void Insert_EntityHasDuplicateId_ThrowsExceptionAndDoesNotInsertEntities(ulong[] uutEntityIds, ulong entityId)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var entity = new TestEntity(entityId);

            Should.Throw<ArgumentException>(() =>
            {
                var result = uut.Insert(entity);
            });

            uut.Count.ShouldBe(uutEntities.Length);
            uutEntities.EachShould(uutEntity =>
                uut.TryGet(uutEntity.Id).ShouldBeSameAs(uutEntity));
        }

        [TestCaseSource(nameof(InvalidIdTestCases))]
        public void Insert_EntityDoesNotExist_ReturnsTrueAndInsertsEntity(ulong[] uutEntityIds, ulong entityId)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var entity = new TestEntity(entityId);

            var result = uut.Insert(entity);

            result.ShouldBeTrue();
            uut.Count.ShouldBe(uutEntities.Length + 1);
            uut.Get(entityId).ShouldBeSameAs(entity);
            uutEntities.EachShould(uutEntity =>
                uut.TryGet(uutEntity.Id).ShouldBeSameAs(uutEntity));
        }

        [TestCaseSource(nameof(AllIdsExistTestCases))]
        public void Insert_AllEntitiesExist_ReturnsFalseAndDoesNotInsertEntities(ulong[] uutEntityIds, ulong[] entityIds)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var entities = uutEntities
                .Where(uutEntity => entityIds.Contains(uutEntity.Id));

            var result = uut.Insert(entities);

            result.ShouldBeFalse();
            uut.Count.ShouldBe(uutEntities.Length);
            uutEntities.EachShould(uutEntity =>
                uut.TryGet(uutEntity.Id).ShouldBeSameAs(uutEntity));
        }

        [TestCaseSource(nameof(SomeIdsExistAndSomeDoNotExistTestCases))]
        public void Insert_AnyEntitiesHaveDuplicateId_ThrowsExceptionAndDoesNotInsertEntities(ulong[] uutEntityIds, ulong[] entityIds)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var entities = entityIds
                .Select(entityId => new TestEntity(entityId));

            Should.Throw<ArgumentException>(() =>
            {
                var result = uut.Insert(entities);
            });

            uut.Count.ShouldBe(uutEntities.Length);
            uutEntities.EachShould(uutEntity =>
                uut.TryGet(uutEntity.Id).ShouldBeSameAs(uutEntity));
        }

        [TestCaseSource(nameof(SomeIdsExistAndSomeDoNotExistTestCases))]
        [TestCaseSource(nameof(AllEntityIdsDoNotExistTestCases))]
        public void Insert_AnyEntitiesDoNotExist_ReturnsTrueAndInsertsNewEntities(ulong[] uutEntityIds, ulong[] entityIds)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var entities = entityIds
                .Select(entityId => uutEntities.FirstOrDefault(uutEntity => uutEntity.Id == entityId)
                    ?? new TestEntity(entityId))
                .ToArray();

            var result = uut.Insert(entities);

            var newEntities = entities
                .Where(entity => !uutEntities.Any(uutEntity => uutEntity.Id == entity.Id))
                .ToArray();

            result.ShouldBeTrue();
            uut.Count.ShouldBe(uutEntities.Length + newEntities.Length);
            newEntities.EachShould(newEntity =>
                uut.TryGet(newEntity.Id).ShouldBeSameAs(newEntity));
            uutEntities.EachShould(uutEntity =>
                uut.TryGet(uutEntity.Id).ShouldBeSameAs(uutEntity));
        }

        #endregion Insert() Tests

        #region Remove() Tests

        [TestCaseSource(nameof(InvalidIdTestCases))]
        public void Remove_IdIsNotValid_ReturnsFalseAndDoesNotRemoveEntities(ulong[] uutEntityIds, ulong id)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var result = uut.Remove(id);

            result.ShouldBeFalse();
            uut.Count.ShouldBe(uutEntities.Length);
            uutEntities.EachShould(uutEntity =>
                uut.TryGet(uutEntity.Id).ShouldBe(uutEntity));
        }

        [TestCaseSource(nameof(ValidIdTestCases))]
        public void Remove_IdIsValid_ReturnsTrueAndRemovesEntity(ulong[] uutEntityIds, ulong id)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var result = uut.Remove(id);

            result.ShouldBeTrue();
            uut.Count.ShouldBe(uutEntities.Length - 1);
            uutEntities
                .Where(uutEntity => uutEntity.Id != id)
                .EachShould(uutEntity =>
                    uut.TryGet(uutEntity.Id).ShouldBe(uutEntity));
        }

        [TestCaseSource(nameof(AllEntityIdsDoNotExistTestCases))]
        public void Remove_AllIdsExist_ReturnsFalseAndDoesNotRemoveEntities(ulong[] uutEntityIds, ulong[] ids)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var result = uut.Remove(ids);

            result.ShouldBeFalse();
            uut.Count.ShouldBe(uutEntities.Length);
            uutEntities.EachShould(uutEntity =>
                uut.TryGet(uutEntity.Id).ShouldBe(uutEntity));
        }

        [TestCaseSource(nameof(AllIdsExistTestCases))]
        [TestCaseSource(nameof(SomeIdsExistAndSomeDoNotExistTestCases))]
        public void Remove_AnyIdsExist_ReturnsTrueAndRemovesMatchingEntities(ulong[] uutEntityIds, ulong[] ids)
        {
            (var uutEntities, var uut) = BuildTestContext(uutEntityIds);

            var result = uut.Remove(ids);

            var removedEntities = uutEntities
                .Where(uutEntity => ids.Contains(uutEntity.Id))
                .ToArray();

            result.ShouldBeTrue();
            uut.Count.ShouldBe(uutEntities.Length - removedEntities.Length);
            removedEntities.EachShould(removedEntity
                => uut.TryGet(removedEntity.Id).ShouldBeNull());
            uutEntities
                .Where(uutEntity => !ids.Contains(uutEntity.Id))
                .EachShould(uutEntity =>
                    uut.TryGet(uutEntity.Id).ShouldBe(uutEntity));
        }

        #endregion Remove() Tests
    }
}
