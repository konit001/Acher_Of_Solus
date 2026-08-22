public interface IMoveSpeedModifiable
{
    float MoveSpeedMultiplier { get; set; }

    // players and bosses only get slowed by freeze, never stopped outright
    bool ResistsFullStop { get; }
}
