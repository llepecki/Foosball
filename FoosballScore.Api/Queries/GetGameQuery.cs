using System;
using EventFlow.Queries;
using FoosballScore.Api.ReadModels;

namespace FoosballScore.Api.Queries;

public record GetGameQuery(Guid Id) : IQuery<GameReadModel?>;