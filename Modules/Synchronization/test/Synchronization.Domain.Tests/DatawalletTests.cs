using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities;
using Backbone.UnitTestTools.Shouldly.Extensions;

namespace Backbone.Modules.Synchronization.Domain.Tests;

public class DatawalletTests : AbstractTestsBase
{
    [Fact]
    public void Cannot_upgrade_to_version_lower_than_current_version()
    {
        var datawallet = CreateDatawallet(new Datawallet.DatawalletVersion(2));

        var acting = () => datawallet.Upgrade(new Datawallet.DatawalletVersion(1));

        acting.ShouldThrow<DomainException>().ShouldContainMessage("it is not possible to upgrade to lower versions");
    }

    [Fact]
    public void First_added_modification_should_have_index_0()
    {
        var datawallet = CreateDatawallet();

        var modification = AddModificationToDatawallet(datawallet);

        modification.Index.ShouldBe(0);
    }

    [Fact]
    public void New_datawallet_should_have_all_properties_set()
    {
        var owner = CreateRandomIdentityAddress();
        var version = new Datawallet.DatawalletVersion(2);

        var datawallet = new Datawallet(version, owner);

        datawallet.Id.ShouldNotBeNull();
        datawallet.Version.ShouldBe(version);
        datawallet.Owner.ShouldBe(owner);
        datawallet.Modifications.ShouldNotBeNull();
    }

    [Fact]
    public void New_datawallet_should_have_no_modifications()
    {
        var datawallet = CreateDatawallet();

        datawallet.Modifications.ShouldHaveCount(0);
    }

    [Fact]
    public void Second_added_modification_should_have_index_1()
    {
        var datawallet = CreateDatawallet();

        AddModificationToDatawallet(datawallet);
        var secondModification = AddModificationToDatawallet(datawallet);

        secondModification.Index.ShouldBe(1);
    }

    [Fact]
    public void Upgrade_sets_version_to_target_version()
    {
        var datawallet = CreateDatawallet(new Datawallet.DatawalletVersion(1));

        datawallet.Upgrade(new Datawallet.DatawalletVersion(2));

        datawallet.Version.ShouldBe(new Datawallet.DatawalletVersion(2));
    }

    [Fact]
    public void Raises_a_domain_event_on_adding_a_modification()
    {
        var datawallet = CreateDatawallet();
        var modification = AddModificationToDatawallet(datawallet);

        var domainEvent = datawallet.ShouldHaveASingleDomainEvent<DatawalletModifiedDomainEvent>();
        domainEvent.Identity.ShouldBe(datawallet.Owner);
        domainEvent.ModifiedByDevice.ShouldBe(modification.CreatedByDevice);
    }

    [Fact]
    public void Adding_multiple_modifications_only_raises_single_domain_event()
    {
        var datawallet = CreateDatawallet();

        AddModificationToDatawallet(datawallet);
        AddModificationToDatawallet(datawallet);

        datawallet.DomainEvents.ShouldHaveCount(1);
    }

    private static DatawalletModification AddModificationToDatawallet(Datawallet datawallet)
    {
        return datawallet.AddModification(DatawalletModificationType.Create, new Datawallet.DatawalletVersion(1), "aCollection", "anId", "aPayloadCategory", CreateRandomBytes(),
            CreateRandomDeviceId());
    }
}
