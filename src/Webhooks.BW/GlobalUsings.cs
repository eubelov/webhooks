global using System.Threading.Tasks;
global using System;
global using MassTransit;
global using Webhooks.Engine.Requests;
global using Webhooks.BW.Commands;
global using Webhooks.Engine.Ports;
global using Microsoft.Extensions.Configuration;
global using Microsoft.Extensions.DependencyInjection;
global using Webhooks.BW.Consumers;
global using Webhooks.BW.Consumers.Workman;
global using Webhooks.BW.Settings;
global using Webhooks.Commands.Workman;
global using IMediator = Mediator.IMediator;
