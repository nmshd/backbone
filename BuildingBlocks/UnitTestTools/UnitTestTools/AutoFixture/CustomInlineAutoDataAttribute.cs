using AutoFixture.Xunit2;

namespace Enmeshed.UnitTestTools.AutoFixture
{
    public class CustomInlineAutoDataAttribute : InlineAutoDataAttribute
    {
        public CustomInlineAutoDataAttribute(params object[] values) : base(new CustomAutoDataAttribute(), values)
        {
        }
    }
}