using System;
using System.Collections.Generic;
using EventFlow.Queries;
using FoosballScore.Api.ReadModels;

namespace FoosballScore.Api.Queries;

public record GetGamesQuery(DateTimeOffset? CreatedAfter, DateTimeOffset? CreatedBefore) : IQuery<IReadOnlyCollection<GameReadModel>>;