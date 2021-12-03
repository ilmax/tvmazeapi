﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#pragma warning disable 219, 612, 618
#nullable disable

namespace TvMaze.Data.OptimizedModel
{
    public partial class TvMazeDbContextModel
    {
        partial void Initialize()
        {
            var actorShow = ActorShowEntityType.Create(this);
            var actor = ActorEntityType.Create(this);
            var jobRunMetadata = JobRunMetadataEntityType.Create(this);
            var show = ShowEntityType.Create(this);

            ActorShowEntityType.CreateForeignKey1(actorShow, actor);
            ActorShowEntityType.CreateForeignKey2(actorShow, show);

            ActorEntityType.CreateSkipNavigation1(actor, show, actorShow);
            ShowEntityType.CreateSkipNavigation1(show, actor, actorShow);

            ActorShowEntityType.CreateAnnotations(actorShow);
            ActorEntityType.CreateAnnotations(actor);
            JobRunMetadataEntityType.CreateAnnotations(jobRunMetadata);
            ShowEntityType.CreateAnnotations(show);

            AddAnnotation("ProductVersion", "6.0.0");
            AddAnnotation("Relational:MaxIdentifierLength", 128);
            AddAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);
        }
    }
}