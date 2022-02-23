using EventFlow.Aggregates;
using EventFlow.Aggregates.ExecutionResults;
using FoosballScore.Domain.Events;

namespace FoosballScore.Domain;

public class Game :
    AggregateRoot<Game, GameId>,
    IEmit<GameCreated>,
    IEmit<GoalScored>,
    IEmit<SetWon>,
    IEmit<GameWon>
{
    private bool _exists;
    private int _blueGoals;
    private int _redGoals;
    private int _blueSets;
    private int _redSets;
    private Team? _winner;

    public Game(GameId id) : base(id)
    {
    }

    public IExecutionResult CreateGame(string name)
    {
        if (_exists)
        {
            return ErrorCodeFailedExecutionResult.GameAlreadyExists;
        }

        Emit(new GameCreated(name, GetCurrentSet()));

        return ExecutionResult.Success();
    }

    public IExecutionResult ScoreGoal(Team team)
    {
        if (!_exists)
        {
            return ErrorCodeFailedExecutionResult.GameNotFound;
        }

        if (_winner.HasValue)
        {
            return ErrorCodeFailedExecutionResult.GameAlreadyFinished;
        }

        int goals = team == Team.Blue ? _blueGoals : _redGoals;
        Emit(new GoalScored(team, GetCurrentSet(), goals + 1));

        if (HasWonSet(team))
        {
            Emit(new SetWon(team, GetCurrentSet()));
        }

        if (HasWonGame(team))
        {
            Emit(new GameWon(team, GetCurrentSet()));
        }

        return ExecutionResult.Success();
    }

    public void Apply(GameCreated gameCreated)
    {
        _exists = true;
        _blueGoals = 0;
        _redGoals = 0;
        _blueSets = 0;
        _redSets = 0;
        _winner = null;
    }

    public void Apply(GoalScored goalScored)
    {
        if (goalScored.Team == Team.Blue)
        {
            _blueGoals = goalScored.Goals;
        }
        else
        {
            _redGoals = goalScored.Goals;
        }
    }

    public void Apply(SetWon setWon)
    {
        if (setWon.Winner == Team.Blue)
        {
            _blueSets++;
        }
        else
        {
            _redSets++;
        }

        _blueGoals = _redGoals = 0;
    }

    public void Apply(GameWon gameWon)
    {
        _winner = gameWon.Winner;
    }

    private int GetCurrentSet() => _blueSets + _redSets + 1;

    private bool HasWonSet(Team team)
    {
        const int goalsToWin = 10;

        if (team == Team.Blue)
        {
            return _blueGoals == goalsToWin;
        }

        return _redGoals == goalsToWin;
    }

    private bool HasWonGame(Team team)
    {
        const int setsToWin = 2;

        if (team == Team.Blue)
        {
            return _blueSets == setsToWin;
        }

        return _redSets == setsToWin;
    }
}