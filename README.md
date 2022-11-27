# Multiplayer Game API

This API is used to manage a database keeping records of all the players of a multiplayer game as well as the matches they've played.
Through this API, players can be registered into the system, as well as the results of matches they played. Information about the players, including their total winrate (percentage of matches they played where they were the winner) and the number of matches they've played can be retrieved as well.
Player names must be unique.

These are the HTTP methods currently supported:

|        API        |    Description   | Request Paramters | Response Body |
| ----------------- | ---------------- | ----------------- | ------------- |
| GET /api/players/ | List all players | None              | List of player data  |
| GET /api/players/name/{name} | Get specific player from name | None | Player data |
| GET /api/players/id/{id} | Get specific player from ID | None | Player data |
| GET /api/players/onlinecount | Count number of online players | None | Integer |
| GET /api/players/onlinecount/{region} | Count number of online players in a particular region (NA or EU) | None | Integer |
| GET /api/players/matches | List all recorded matches | None | List of match data |
| GET /api/players/match/{id} | Get specific match data | None | Match data |
| GET /api/players/winrate/{id} | Get winrate of a specific player | None | Float point value |
| GET /api/players/matchcount/{id} | Get total number of matches played by a player | None | Integer |
| PATCH /api/players/onlinestatus | Set online status for specific player | id (string), online (boolean) | Player data |
| POST /api/players/register | Add player to player list | name (string), region (string) | Player data |
| POST /api/players/matchresult | Add match to match list | player1 (int), player2 (int), winner(int) | Match data |


Player data consists of the player's ID, their name, their matchmaking region and their online status. 
Sample:
```
{"playerId":1,"playerName":"Robert","isOnline":true,"playerRegion":"NA"}
```

Match data consists of the match's ID, its two players, the winner between them, and the timestamp of the match. Players are referred to by their ID numbers.
Sample:
```
{"matchId":1,"player1":1,"player2":2,"winner":1,"matchTimestamp":"2022-11-26T14:52:07"}
```

Sample output of a call to this API:
```
{
    "statusCode": 200,
    "statusDescription": "OK",
    "data": {
        "playerId": 2,
        "playerName": "Johnson",
        "isOnline": false,
        "playerRegion": "EU"
    }
}
```
