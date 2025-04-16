using Backbone.BuildingBlocks.Domain;
using Backbone.BuildingBlocks.Domain.StronglyTypedIds.Records;

namespace Backbone.Modules.Files.Domain.Entities;

public record FileOwnershipToken(string Value) : StronglyTypedId(Value)
{
    public static FileOwnershipToken New()
    {
        //TODO: refactor this please
        var allCharactersList = new List<char>();
        allCharactersList.AddRange(DEFAULT_VALID_CHARS);
        allCharactersList.AddRange(SPECIAL_CHARACTERS);
        var allChars = allCharactersList.ToArray();

        var stringValue = StringUtils.Generate(allChars, DEFAULT_MAX_LENGTH);
        return new FileOwnershipToken(stringValue);
    }
}
