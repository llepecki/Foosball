using System;
using EventFlow.Queries;

namespace FoosballScore.Api.Queries;

public record GameExistsQuery(Guid Id) : IQuery<bool>;