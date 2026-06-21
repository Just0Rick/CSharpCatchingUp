using CardGame.PhaseOne;
using Microsoft.AspNetCore.Http.HttpResults;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<Game>();
var app = builder.Build();

app.MapGet("/", () => Results.Redirect("/game/deck"));

var gameGroup = app.MapGroup("/game");
gameGroup.MapGet("/deck", (Game game) =>
{
   var deck = game.Shuffle(game.BuildDeck()); 
   return TypedResults.Ok(deck);
});
gameGroup.MapPost("/hand", async (Game game, List<Card> baseDeck, int cardCount, CancellationToken ct) =>
{
    var hand = await game.DrawHandAsync(new Deck(baseDeck), cardCount, ct);
    return TypedResults.Ok(hand);
});
gameGroup.MapPost("/hand/result", (Game game, List<Card> hand) =>
{
    return TypedResults.Ok(game.HandResult(hand));
});
gameGroup.MapGet("/card/{suit}/{rank}", Results<Ok<Card>, NotFound<string>> (string suit, int rank) =>
{
    if (rank < 1 || rank > 13)
        return TypedResults.NotFound("Invalid rank");
    if (Enum.TryParse<SuitType>(suit, out var parsedSuit))
    {
        return TypedResults.Ok(new Card(parsedSuit, rank));
    }
    return TypedResults.NotFound("Invalid suit");
});


app.Run();
