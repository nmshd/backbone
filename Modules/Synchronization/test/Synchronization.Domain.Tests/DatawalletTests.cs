using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Synchronization.Domain.DomainEvents.Outgoing;
using Backbone.Modules.Synchronization.Domain.Entities;

namespace Backbone.Modules.Synchronization.Domain.Tests;

public class DatawalletTests : AbstractTestsBase
{
    [Fact]
    public void Cannot_upgrade_to_version_lower_than_current_version()
    {
        var datawallet = CreateDatawallet(new Datawallet.DatawalletVersion(2));

        var acting = () => datawallet.Upgrade(new Datawallet.DatawalletVersion(1));

        acting.Should().Throw<DomainException>().WithMessage("*it is not possible to upgrade to lower versions*");
    }

    [Fact]
    public void First_added_modification_should_have_index_0()
    {
        var datawallet = CreateDatawallet();

        var modification = AddModificationToDatawallet(datawallet);

        modification.Index.Should().Be(0);
    }

    [Fact]
    public void New_datawallet_should_have_all_properties_set()
    {
        var owner = CreateRandomIdentityAddress();
        var version = new Datawallet.DatawalletVersion(2);

        var datawallet = new Datawallet(version, owner);

        datawallet.Id.Should().NotBeNull();
        datawallet.Version.Should().Be(version);
        datawallet.Owner.Should().Be(owner);
        datawallet.Modifications.Should().NotBeNull();
    }

    [Fact]
    public void New_datawallet_should_have_no_modifications()
    {
        var datawallet = CreateDatawallet();

        datawallet.Modifications.Should().HaveCount(0);
    }

    [Fact]
    public void Second_added_modification_should_have_index_1()
    {
        var datawallet = CreateDatawallet();

        AddModificationToDatawallet(datawallet);
        var secondModification = AddModificationToDatawallet(datawallet);

        secondModification.Index.Should().Be(1);
    }

    [Fact]
    public void Upgrade_sets_version_to_target_version()
    {
        var datawallet = CreateDatawallet(new Datawallet.DatawalletVersion(1));

        datawallet.Upgrade(new Datawallet.DatawalletVersion(2));

        datawallet.Version.Should().Be(new Datawallet.DatawalletVersion(2));
    }

    [Fact]
    public void Raises_a_domain_event_on_adding_a_modification()
    {
        var datawallet = CreateDatawallet();
        var modification = AddModificationToDatawallet(datawallet);

        var domainEvent = datawallet.Should().HaveASingleDomainEvent<DatawalletModifiedDomainEvent>();
        domainEvent.Identity.Should().Be(datawallet.Owner);
        domainEvent.ModifiedByDevice.Should().Be(modification.CreatedByDevice);
    }

    [Fact]
    public void Adding_multiple_modifications_only_raises_single_domain_event()
    {
        var datawallet = CreateDatawallet();

        AddModificationToDatawallet(datawallet);
        AddModificationToDatawallet(datawallet);

        datawallet.DomainEvents.Should().HaveCount(1);
    }

    private static Datawallet CreateDatawallet()
    {
        return new Datawallet(new Datawallet.DatawalletVersion(1), CreateRandomIdentityAddress());
    }

    private static Datawallet CreateDatawallet(Datawallet.DatawalletVersion version)
    {
        return new Datawallet(version, CreateRandomIdentityAddress());
    }

    private static DatawalletModification AddModificationToDatawallet(Datawallet datawallet)
    {
        return datawallet.AddModification(DatawalletModificationType.Create, new Datawallet.DatawalletVersion(1), "aCollection", "anId", "aPayloadCategory", CreateRandomBytes(),
            CreateRandomDeviceId());
    }
}
