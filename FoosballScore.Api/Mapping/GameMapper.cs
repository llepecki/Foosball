using System;
using System.Collections.Generic;
using System.Linq;
using FoosballScore.Api.Models;
using FoosballScore.Api.ReadModels;
using FoosballScore.Domain;

namespace FoosballScore.Api.Mapping;

internal static class GameMapper
{
    public static GameViewModel ToViewModel(this GameReadModel readModel)
    {
        Team? winner = readModel.GameWinner != null ? Enum.Parse<Team>(readModel.GameWinner) : null;
        return new GameViewModel(readModel.Id, readModel.Name, GetSets(readModel).ToArray(), winner, readModel.Created, readModel.Modified);
    }

    public static IEnumerable<GameViewModel> ToViewModel(this IEnumerable<GameReadModel> readModels)
    {
        foreach (GameReadModel readModel in readModels)
        {
            yield return readModel.ToViewModel();
        }
    }

    private static IEnumerable<SetViewModel> GetSets(GameReadModel readModel)
    {
        Team? firstSetWinner = readModel.FirstSetWinner != null ? Enum.Parse<Team>(readModel.FirstSetWinner) : null;

        yield return new SetViewModel
        {
            Set = 1,
            Winner = firstSetWinner,
            BlueGoals = readModel.FirstSetBlueGoals,
            RedGoals = readModel.FirstSetRedGoals
        };

        if (firstSetWinner.HasValue)
        {
            Team? secondSetWinner = readModel.SecondSetWinner != null ? Enum.Parse<Team>(readModel.SecondSetWinner) : null;

            yield return new SetViewModel
            {
                Set = 2,
                Winner = secondSetWinner,
                BlueGoals = readModel.SecondSetBlueGoals,
                RedGoals = readModel.SecondSetRedGoals
            };

            Team? thirdSetWinner = readModel.ThirdSetWinner != null ? Enum.Parse<Team>(readModel.ThirdSetWinner) : null;

            if (secondSetWinner.HasValue && readModel.GameWinner == null || thirdSetWinner.HasValue)
            {
                yield return new SetViewModel
                {
                    Set = 3,
                    Winner = thirdSetWinner,
                    BlueGoals = readModel.ThirdSetBlueGoals,
                    RedGoals = readModel.ThirdSetRedGoals
                };
            }
        }
    }
}