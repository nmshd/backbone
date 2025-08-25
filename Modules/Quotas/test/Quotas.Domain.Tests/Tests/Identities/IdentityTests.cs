using Backbone.BuildingBlocks.Domain.Exceptions;
using Backbone.Modules.Quotas.Domain.Aggregates.Identities;
using Backbone.Modules.Quotas.Domain.Aggregates.Tiers;
using Backbone.Modules.Quotas.Domain.Metrics;
using Backbone.Tooling;
using Backbone.UnitTestTools.Extensions;
using Backbone.UnitTestTools.Shouldly.Extensions;
using MetricKey = Backbone.Modules.Quotas.Domain.Aggregates.Metrics.MetricKey;

namespace Backbone.Modules.Quotas.Domain.Tests.Tests.Identities;

public class IdentityTests : AbstractTestsBase
{
    #region Creation

    [Fact]
    public void Can_create_identity_with_valid_properties()
    {
        // Act
        var identity = new Identity("some-address", TierId.Parse("TIRsomeTierId1111111"));

        // Assert
        identity.Address.ShouldBe("some-address");
        identity.TierId.ShouldBe(TierId.Parse("TIRsomeTierId1111111"));
    }

    #endregion

    #region CreateIndividualQuota

    [Fact]
    public void Creating_a_quota_with_duplicate_quota_metric_period_throws_domain_exception()
    {
        // Arrange
        var metricKey = MetricKey.NUMBER_OF_SENT_MESSAGES;
        var identity = CreateIdentity();
        identity.CreateIndividualQuota(metricKey, 5, QuotaPeriod.Hour);

        // Act
        var acting = () => identity.CreateIndividualQuota(metricKey, 5, QuotaPeriod.Hour);

        // Assert
        acting.ShouldThrow<DomainException>().ShouldHaveError("error.platform.quotas.duplicateQuota");
    }

    #endregion

    #region AssignTierQuotaFromDefinition

    [Fact]
    public void Can_assign_tier_quota_from_definition_to_identity()
    {
        // Arrange
        var identity = CreateIdentity();
        var tierQuotaDefinition1 = new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day);
        var tierQuotaDefinition2 = new TierQuotaDefinition(MetricKey.USED_FILE_STORAGE_SPACE, 2, QuotaPeriod.Hour);

        // Act
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition1);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition2);

        // Assert
        identity.TierQuotas.ShouldHaveCount(2);

        identity.TierQuotas.First().MetricKey.ShouldBe(tierQuotaDefinition1.MetricKey);
        identity.TierQuotas.First().Max.ShouldBe(tierQuotaDefinition1.Max);
        identity.TierQuotas.First().Period.ShouldBe(tierQuotaDefinition1.Period);

        identity.TierQuotas.Second().MetricKey.ShouldBe(tierQuotaDefinition2.MetricKey);
        identity.TierQuotas.Second().Max.ShouldBe(tierQuotaDefinition2.Max);
        identity.TierQuotas.Second().Period.ShouldBe(tierQuotaDefinition2.Period);
    }

    #endregion

    #region DeleteIndividualQuota

    [Fact]
    public void Deleting_individual_Quota()
    {
        // Arrange
        var identity = CreateIdentity();
        var createdQuota = identity.CreateIndividualQuota(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day);

        // Act
        identity.DeleteIndividualQuota(createdQuota.Id);

        // Assert
        identity.IndividualQuotas.ShouldHaveCount(0);
    }

    [Fact]
    public void Deleting_individual_Quota_only_deletes_Quota_with_given_id()
    {
        // Arrange
        var identity = new Identity("some-address", TierId.Parse("tier-id"));
        var firstQuota = identity.CreateIndividualQuota(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day);
        var secondQuota = identity.CreateIndividualQuota(MetricKey.NUMBER_OF_FILES, 1, QuotaPeriod.Day);

        // Act
        identity.DeleteIndividualQuota(firstQuota.Id);

        // Assert
        identity.IndividualQuotas.ShouldHaveCount(1);
        identity.IndividualQuotas.ShouldContain(secondQuota);
    }

    [Fact]
    public void Trying_to_delete_inexistent_individual_quota_throws_DomainException()
    {
        // Arrange
        var identity = CreateIdentity();

        // Act
        var result = identity.DeleteIndividualQuota(QuotaId.Generate());

        // Assert
        result.IsFailure.ShouldBeTrue();
        result.Error.Code.ShouldBe("error.platform.recordNotFound");
        result.Error.Message.ShouldStartWith("IndividualQuota");
    }

    #endregion

    #region DeleteTierQuotaFromDefinitionId

    [Fact]
    public void Deleting_Tier_Quota_by_definition_id()
    {
        // Arrange
        var identity = CreateIdentity();
        var tierQuotaDefinition = new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);

        // Act
        identity.DeleteTierQuotaFromDefinitionId(tierQuotaDefinition.Id);

        // Assert
        identity.TierQuotas.ShouldHaveCount(0);
    }

    [Fact]
    public void Deleting_Tier_Quota_by_definition_id_only_deletes_Quota_with_given_definition_id()
    {
        // Arrange
        var identity = new Identity("some-address", TierId.Parse("tier-id"));
        var tierQuotaDefinition1 = new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day);
        var tierQuotaDefinition2 = new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition1);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition2);

        // Act
        identity.DeleteTierQuotaFromDefinitionId(tierQuotaDefinition1.Id);

        // Assert
        identity.TierQuotas.ShouldHaveCount(1);
        identity.TierQuotas.ElementAt(0).DefinitionId.ShouldBe(tierQuotaDefinition2.Id);
    }

    [Fact]
    public void Trying_to_delete_Tier_Quota_with_inexistent_definition_id_throws_DomainException()
    {
        // Arrange
        var identity = CreateIdentity();

        // Act
        var acting = () => identity.DeleteTierQuotaFromDefinitionId(TierQuotaDefinitionId.Create("TQDsomeInexistentIdx").Value);

        // Assert
        acting.ShouldThrow<DomainException>();
    }

    #endregion

    #region UpdateMetricStatuses

    [Fact]
    public async Task Updating_a_non_existing_MetricStatus_creates_a_MetricStatus()
    {
        // Arrange
        var identity = CreateIdentity();

        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Hour));

        // Act
        var metricCalculatorFactoryStub = new MetricCalculatorFactoryStub(new MetricCalculatorStub(0));

        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], metricCalculatorFactoryStub, MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.ShouldHaveCount(1);
        identity.MetricStatuses.First().MetricKey.ShouldBe(MetricKey.NUMBER_OF_SENT_MESSAGES);
    }

    [Fact]
    public async Task Updating_a_non_existing_MetricStatus_with_no_corresponding_quotas_sets_exhaustion_date_to_null()
    {
        // Arrange
        var identity = CreateIdentity();

        // Act
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.ShouldHaveCount(1);
        identity.MetricStatuses.First().MetricKey.ShouldBe(MetricKey.NUMBER_OF_SENT_MESSAGES);
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBe(ExhaustionDate.UNEXHAUSTED);
    }

    [Fact]
    public async Task Updating_an_existing_MetricStatus_with_no_corresponding_quotas_sets_exhaustion_date_to_null()
    {
        // Arrange
        var identity = CreateIdentity();
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(), MetricUpdateType.All);

        // Act
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.ShouldHaveCount(1);
        identity.MetricStatuses.First().MetricKey.ShouldBe(MetricKey.NUMBER_OF_SENT_MESSAGES);
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBe(ExhaustionDate.UNEXHAUSTED);
    }

    [Fact]
    public async Task Updating_a_MetricStatus_with_1_exhausted_Quota_sets_its_exhaustion_date_to_the_end_of_the_Quotas_period()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = new Identity("some-address", TierId.Parse("tier-id"));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Hour));

        // Act
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(1), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBeEndOfHour();
    }

    [Fact]
    public async Task Updating_a_MetricStatus_with_1_exhausted_Quota_and_1_non_exhausted_Quota_sets_its_exhaustion_date_to_the_end_of_the_exhausted_Quotas_period()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = CreateIdentity();
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Hour));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 2, QuotaPeriod.Day));

        // Act
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(1), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBeEndOfHour();

        // To make sure that the order of the added Quotas does not matter, we do the same with reversed order
        // Arrange
        identity = new Identity("some-address", TierId.Parse("tier-id"));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 2, QuotaPeriod.Day));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Hour));

        // Act
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(1), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBeEndOfHour();
    }

    [Fact]
    public async Task Updating_a_MetricStatus_with_multiple_exhausted_Quotas_sets_its_exhaustion_date_to_the_end_of_the_greatest_period()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = CreateIdentity();
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Hour));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day));

        // Act
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(1), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBeEndOfDay();

        // To make sure that the order of the added Quotas does not matter, we do the same with reversed order
        // Arrange
        identity = CreateIdentity();
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Hour));

        // Act
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(1), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBeEndOfDay();
    }

    [Fact]
    public async Task Updating_a_MetricStatus_with_Quota_with_max_0()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = CreateIdentity();
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 0, QuotaPeriod.Hour));

        // Act
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(0), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBeEndOfHour();
    }

    [Fact]
    public async Task Updating_an_already_exhausted_MetricStatus_with_an_even_higher_usage_recalculates_exhaustion_date()
    {
        // Even though this case will probably never happen in reality (because once the MetricStatus is exhausted, there will be no more updates), we should still test it

        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = CreateIdentity();
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Hour));
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 2, QuotaPeriod.Day));

        // first update; only the hourly Quota is exhausted
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(1), MetricUpdateType.All);

        // Act
        // second update; now the daily Quota is exhausted as well
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(2), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBeEndOfDay();
    }

    [Fact]
    public async Task Updating_a_MetricStatus_that_was_exhausted_in_the_past_with_non_exhausted_Quotas_sets_the_exhaustion_date_to_null()
    {
        // Arrange
        SystemTime.Set(DateTime.Parse("2023-01-15T12:00:00"));

        var identity = CreateIdentity();
        identity.AssignTierQuotaFromDefinition(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Hour));

        // first update; the Quota is exhausted
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(1), MetricUpdateType.All);

        // Act
        // second update; the Quota is not exhausted anymore
        await identity.UpdateMetricStatuses([MetricKey.NUMBER_OF_SENT_MESSAGES], new MetricCalculatorFactoryStub(0), MetricUpdateType.All);

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBe(ExhaustionDate.UNEXHAUSTED);
    }

    #endregion

    #region ChangeTier

    [Fact]
    public async Task Changing_Tier_updates_identity_with_new_tier()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var identity = new Identity(identityAddress, TierId.Parse("tier-id"));
        var newTier = new Tier(TierId.Parse("new-tier-id"), "New Tier");

        // Act
        await identity.ChangeTier(newTier, new MetricCalculatorFactoryStub(0), CancellationToken.None);

        // Assert
        identity.TierId.ShouldBe(newTier.Id);
    }

    /**
     * If the old Tier has a Quota the new Tier does not have, it should be removed.
     * If the new Tier has a Quota the old Tier does not have, it should be added.
     * If the new Tier has a Quota the old Tier has as well, it should be kept.
     */
    [Fact]
    public async Task Changing_Tier_removes_old_TierQuotas_and_adds_new_ones()
    {
        // Arrange
        var identity = CreateIdentity();
        await identity.AddUnexhaustedTierQuotaToIdentity(MetricKey.NUMBER_OF_FILES, 1);
        await identity.AddUnexhaustedTierQuotaToIdentity(MetricKey.NUMBER_OF_RELATIONSHIPS, 1);

        var newTier = new Tier(TierId.Parse("new-tier-id"), "New Tier");
        newTier.CreateQuota(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day);
        newTier.CreateQuota(MetricKey.NUMBER_OF_RELATIONSHIPS, 1, QuotaPeriod.Day);

        // Act
        await identity.ChangeTier(newTier, new MetricCalculatorFactoryStub(1), CancellationToken.None);

        // Assert
        identity.TierQuotas.ShouldHaveCount(2);
        identity.TierQuotas.ShouldContain(q => q.MetricKey == MetricKey.NUMBER_OF_SENT_MESSAGES);
        identity.TierQuotas.ShouldContain(q => q.MetricKey == MetricKey.NUMBER_OF_RELATIONSHIPS);
    }

    /**
     * - If the old Tier had an exhausted MetricStatus due to a TierQuota, and the new Tier has more
     *   TierQuota on the same MetricStatus, the MetricStatus after changing the Tier should be unexhausted
     * - If the old Tier had an unexhausted MetricStatus, and the new Tier has less
     *   TierQuota on the same MetricStatus, the MetricStatus after changing the Tier should be unexhausted
     */
    [Fact]
    public async Task Changing_Tier_updates_MetricStatuses()
    {
        // Arrange
        var identity = CreateIdentity();
        await identity.AddExhaustedTierQuotaToIdentity(MetricKey.NUMBER_OF_FILES, 1);
        await identity.AddUnexhaustedTierQuotaToIdentity(MetricKey.NUMBER_OF_SENT_MESSAGES, 3);

        var newTier = new Tier(TierId.Parse("new-tier-id"), "New Tier");
        newTier.Quotas.Add(new TierQuotaDefinition(MetricKey.NUMBER_OF_FILES, 3, QuotaPeriod.Day));
        newTier.Quotas.Add(new TierQuotaDefinition(MetricKey.NUMBER_OF_SENT_MESSAGES, 1, QuotaPeriod.Day));

        // Act
        await identity.ChangeTier(newTier, new MetricCalculatorFactoryStub(2), CancellationToken.None);

        // Assert
        identity.MetricStatuses.First().IsExhaustedUntil.ShouldBe(ExhaustionDate.UNEXHAUSTED);
        identity.MetricStatuses.Second().IsExhaustedUntil.ShouldNotBe(ExhaustionDate.UNEXHAUSTED);
    }

    [Fact]
    public async Task Changing_Tier_fails_when_old_and_new_tier_match()
    {
        // Arrange
        var identityAddress = CreateRandomIdentityAddress();
        var oldTier = new Tier(TierId.Parse("tier-id"), "Old Tier");
        var identity = new Identity(identityAddress, oldTier.Id);

        // Act
        var acting = async () => await identity.ChangeTier(oldTier, new MetricCalculatorFactoryStub(0), CancellationToken.None);

        // Assert
        var exception = await acting.ShouldThrowAsync<DomainException>();
        exception.ShouldHaveError("error.platform.validation.newAndOldMatch");
    }

    #endregion

    private static Identity CreateIdentity()
    {
        return new Identity(CreateRandomIdentityAddress(), TierId.Parse("tier-id"));
    }
}

public class MetricCalculatorFactoryStub : MetricCalculatorFactory
{
    private readonly MetricCalculatorStub _calculator;

    public MetricCalculatorFactoryStub()
    {
        _calculator = new MetricCalculatorStub(0);
    }

    public MetricCalculatorFactoryStub(int newUsage)
    {
        _calculator = new MetricCalculatorStub(newUsage);
    }

    public MetricCalculatorFactoryStub(MetricCalculatorStub calculator)
    {
        _calculator = calculator;
    }

    protected override IMetricCalculator CreateNumberOfFilesMetricCalculator()
    {
        return _calculator;
    }

    protected override IMetricCalculator CreateNumberOfRelationshipsMetricCalculator()
    {
        return _calculator;
    }

    protected override IMetricCalculator CreateNumberOfRelationshipTemplatesMetricCalculator()
    {
        return _calculator;
    }

    protected override IMetricCalculator CreateNumberOfSentMessagesMetricCalculator()
    {
        return _calculator;
    }

    protected override IMetricCalculator CreateNumberOfTokensMetricCalculator()
    {
        return _calculator;
    }

    protected override IMetricCalculator CreateUsedFileStorageSpaceCalculator()
    {
        return _calculator;
    }

    protected override IMetricCalculator CreateNumberOfStartedDeletionProcessesCalculator()
    {
        return _calculator;
    }

    protected override IMetricCalculator CreateNumberOfCreatedDatawalletModificationsCalculator()
    {
        return _calculator;
    }

    protected override IMetricCalculator CreateNumberOfCreatedDevicesCalculator()
    {
        return _calculator;
    }

    protected override IMetricCalculator CreateNumberOfCreatedChallengesCalculator()
    {
        return _calculator;
    }
}

public class MetricCalculatorStub : IMetricCalculator
{
    private readonly int _newUsage;

    public MetricCalculatorStub(int newUsage)
    {
        _newUsage = newUsage;
    }

    public Task<uint> CalculateUsage(DateTime from, DateTime to, string identityAddress, CancellationToken cancellationToken)
    {
        return Task.FromResult((uint)_newUsage);
    }
}

public static class IdentityExtensions
{
    public static async Task UpdateMetricStatuses(this Identity identity, IEnumerable<MetricKey> metrics, MetricCalculatorFactory factory, MetricUpdateType updateType)
    {
        await identity.UpdateMetricStatuses(metrics, factory, updateType, CancellationToken.None);
    }

    public static async Task AddUnexhaustedTierQuotaToIdentity(this Identity identity, MetricKey metricKey, int max)
    {
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, max, QuotaPeriod.Day);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);
        await identity.UpdateMetricStatuses([metricKey], new MetricCalculatorFactoryStub(0), MetricUpdateType.All);
    }

    public static async Task AddExhaustedTierQuotaToIdentity(this Identity identity, MetricKey metricKey, int max)
    {
        var tierQuotaDefinition = new TierQuotaDefinition(metricKey, max, QuotaPeriod.Day);
        identity.AssignTierQuotaFromDefinition(tierQuotaDefinition);
        await identity.UpdateMetricStatuses([metricKey], new MetricCalculatorFactoryStub(max), MetricUpdateType.All);
    }
}
