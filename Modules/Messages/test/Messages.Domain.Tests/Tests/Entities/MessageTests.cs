using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Backbone.UnitTestTools.Data;
using FluentAssertions;
using Xunit;

namespace Backbone.Modules.Messages.Domain.Tests.Tests.Entities;

public class MessageTests
{
    [Fact]
    public void Message_should_return_a_decrypted_body_provided_correct_symmetric_key_has_been_passed_as_a_parameter()
    {
        var address = TestDataGenerator.CreateRandomIdentityAddress();
    }
}

