﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#pragma warning disable 219, 612, 618
#nullable enable

namespace Backbone.Modules.Messages.Infrastructure.CompiledModels.Postgres
{
    public partial class MessagesDbContextModel
    {
        partial void Initialize()
        {
            var attachment = AttachmentEntityType.Create(this);
            var message = MessageEntityType.Create(this);
            var recipientInformation = RecipientInformationEntityType.Create(this);
            var relationship = RelationshipEntityType.Create(this);

            AttachmentEntityType.CreateForeignKey1(attachment, message);
            RecipientInformationEntityType.CreateForeignKey1(recipientInformation, message);
            RecipientInformationEntityType.CreateForeignKey2(recipientInformation, relationship);

            AttachmentEntityType.CreateAnnotations(attachment);
            MessageEntityType.CreateAnnotations(message);
            RecipientInformationEntityType.CreateAnnotations(recipientInformation);
            RelationshipEntityType.CreateAnnotations(relationship);

            AddAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);
            AddAnnotation("ProductVersion", "7.0.11");
            AddAnnotation("Relational:MaxIdentifierLength", 63);
        }
    }
}
