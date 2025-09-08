using System.Text.Json;
using Backbone.DevelopmentKit.Identity.ValueObjects;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.Tooling;
using Xunit.Sdk;
using static Backbone.Modules.Synchronization.Domain.Entities.DatawalletModificationType;

namespace Backbone.Modules.Synchronization.Domain.Tests.DatawalletModifications;

public class CanBeCleanedUpTests : AbstractTestsBase
{
    [Theory]
    [ClassData(typeof(TheoryData))]
    public void Test(TheoryData.TestData input)
    {
        // Arrange
        var datawallet = CreateDatawallet();

        var modificationsThatCanBeCleanedUp = new List<DatawalletModification>();
        var modificationsThatCannotBeCleanedUp = new List<DatawalletModification>();

        foreach (var modificationData in input.Items)
        {
            var modification = datawallet.AddModification(modificationData.Type, modificationData.Collection, modificationData.ObjectIdentifier, modificationData.PayloadCategory,
                modificationData.CreatedAt);

            if (modificationData.CanBeCleanedUp)
                modificationsThatCanBeCleanedUp.Add(modification);
            else
                modificationsThatCannotBeCleanedUp.Add(modification);
        }

        // Act
        var resultsOfModificationsThatCanBeCleanedUp = modificationsThatCanBeCleanedUp.Select(m => m.CanBeCleanedUp()).ToList();
        var resultsOfModificationsThatCannotBeCleanedUp = modificationsThatCannotBeCleanedUp.Select(m => m.CanBeCleanedUp()).ToList();

        // Assert
        resultsOfModificationsThatCanBeCleanedUp.ShouldAllBe(x => x == true);
        resultsOfModificationsThatCannotBeCleanedUp.ShouldAllBe(x => x == false);
    }
}

public class TheoryData : TheoryData<TheoryData.TestData>
{
    public TheoryData()
    {
        var nowMinus29Days = SystemTime.UtcNow.AddDays(-29);
        Add("Does not delete modifications younger than 30 days",
        [
            new DatawalletModificationData { Type = Create, CreatedAt = nowMinus29Days, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = CacheChanged, CreatedAt = nowMinus29Days, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Update, CreatedAt = nowMinus29Days, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Delete, CreatedAt = nowMinus29Days, Collection = "c1", ObjectIdentifier = "o1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Create, CreatedAt = nowMinus29Days, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = CacheChanged, CreatedAt = nowMinus29Days, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Update, CreatedAt = nowMinus29Days, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Delete, CreatedAt = nowMinus29Days, Collection = "c1", ObjectIdentifier = "o1", CanBeCleanedUp = false },
        ]);

        Add("Never deletes the last DELETE",
        [
            new DatawalletModificationData { Type = Delete, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false }
        ]);

        Add("Deletes everything before the last DELETE",
        [
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c2", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Delete, Collection = "c1", ObjectIdentifier = "o1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c2", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c2", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = CacheChanged, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c2", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Delete, Collection = "c1", ObjectIdentifier = "o1", CanBeCleanedUp = false }
        ]);

        Add("Deletes all but the last UPDATE",
        [
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", CanBeCleanedUp = false }
        ]);

        Add("Deletes all but the last CREATE",
        [
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", CanBeCleanedUp = false }
        ]);

        Add("If the latest modification is an UPDATE, deletes all previous CREATEs/UPDATEs",
        [
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false }
        ]);

        Add("If the latest modification is a CREATE, deletes all previous CREATEs/UPDATEs",
        [
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false }
        ]);

        Add("Does not delete when other CREATE/UPDATE is for different collection",
        [
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Update, Collection = "c2", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Create, Collection = "c3", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false }
        ]);

        Add("Does not delete when other DELETE is for different collection",
        [
            new DatawalletModificationData { Type = Delete, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Delete, Collection = "c2", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false }
        ]);

        Add("Does not delete when other CREATE/UPDATE is for different object identifier",
        [
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o2", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o3", PayloadCategory = "c1", CanBeCleanedUp = false }
        ]);

        Add("Does not delete when other DELETE is for different object identifier",
        [
            new DatawalletModificationData { Type = Delete, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Delete, Collection = "c1", ObjectIdentifier = "o2", PayloadCategory = "c1", CanBeCleanedUp = false }
        ]);

        Add("Does not delete when other CREATE/UPDATE is for different payload category",
        [
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c2", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c3", CanBeCleanedUp = false }
        ]);

        Add("Does not touch CACHE_CHANGEDs",
        [
            new DatawalletModificationData { Type = CacheChanged, Collection = "c1", ObjectIdentifier = "o1", CanBeCleanedUp = false },
            new DatawalletModificationData { Type = Create, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = true },
            new DatawalletModificationData { Type = Update, Collection = "c1", ObjectIdentifier = "o1", PayloadCategory = "c1", CanBeCleanedUp = false }
        ]);
    }

    private void Add(string displayName, IEnumerable<DatawalletModificationData> items)
    {
        Add(new TestData(displayName, items));
    }

    public class TestData : IXunitSerializable
    {
        private string _displayName = "";

        // ReSharper disable once UnusedMember.Global // Used by xUnit
        public TestData()
        {
        }

        public TestData(string displayName, IEnumerable<DatawalletModificationData> list)
        {
            _displayName = displayName;
            Items.AddRange(list);
        }

        public List<DatawalletModificationData> Items { get; set; } = new();

        public void Deserialize(IXunitSerializationInfo info)
        {
            var items = JsonSerializer.Deserialize<List<DatawalletModificationData>>(info.GetValue<string>("items")!)!;

            Items.AddRange(items);

            _displayName = info.GetValue<string>("displayName")!;
        }

        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("items", JsonSerializer.Serialize(Items));
            info.AddValue("displayName", _displayName);
        }

        public override string ToString()
        {
            return _displayName;
        }
    }

    public class DatawalletModificationData
    {
        public DatawalletModificationType Type { get; set; }
        public required string Collection { get; set; }
        public required string ObjectIdentifier { get; set; }
        public string? PayloadCategory { get; set; }
        public DateTime CreatedAt { get; set; } = SystemTime.UtcNow.AddDays(-31);
        public required bool CanBeCleanedUp { get; set; }
    }
}

file static class DatawalletExtensions
{
    public static DatawalletModification AddModification(this Datawallet datawallet, DatawalletModificationType type, string collection, string objectIdentifier,
        string? payloadCategory, DateTime createdAt)
    {
        SystemTime.Set(createdAt);
        var modification = datawallet.AddModification(
            type,
            datawallet.Version,
            collection,
            objectIdentifier,
            payloadCategory,
            null,
            DeviceId.New());
        SystemTime.UndoSet();
        return modification;
    }
}

file static class DatawalletModificationExtensions
{
    private static Func<DatawalletModification, bool>? _compiledCanBeCleanedUp;

    public static bool CanBeCleanedUp(this DatawalletModification modification)
    {
        _compiledCanBeCleanedUp ??= DatawalletModification.CanBeCleanedUp.Compile();

        return _compiledCanBeCleanedUp(modification);
    }
}
