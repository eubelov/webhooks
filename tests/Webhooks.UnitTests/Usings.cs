global using Xunit;
global using AutoBogus;
global using FluentAssertions;
global using Mapster;
global using MapsterMapper;
global using Webhooks.Commands.Workman;
global using Webhooks.Engine.ThirdParty.Magnit.Mappers;
global using Microsoft.EntityFrameworkCore;
global using Microsoft.Extensions.Logging.Abstractions;
global using Webhooks.Commands;
global using Webhooks.Commands.Enums;
global using Webhooks.Engine.Entities;
global using Webhooks.Engine.Infrastructure;
global using Webhooks.Engine.Requests;
global using Webhooks.Engine.ThirdParty.Mappers;
global using Webhooks.Engine.Services;
global using Webhooks.Engine.Notifications;
global using Webhooks.Engine.ThirdParty.Magnit.Contracts;
global using IMediator = Mediator.IMediator;
global using System.Net;
global using FakeItEasy;
global using RichardSzalay.MockHttp;
global using Webhooks.Engine.ThirdParty.Builders;


