# API_Final

|        API        |    Description   | Request Paramters | Response Body |
| ----------------- | ---------------- | ----------------- | ------------- |
| GET /api/players/ | List all players | None              | JSON List     |
| GET /api/players/{name} | Get player from name | None | Player data |
| GET /api/players/id/{id} | Get specific player from ID | None | Player data |
| GET /api/players/onlinecount | Count number of online players | None | Integer |
| GET /api/players/onlinecount/{region} | Count number of online players in a particular region (NA or EU) | None | Integer |
| GET /api/players/matches | List all recorded matches | None | JSON List |
| GET /api/players/winrate/{id} | Get winrate of a specific player | None | Float point value |
| GET /api/players/matchcount/{id} | Get total number of matches played by a player | None | Integer |
| POST /api/players/onlinestatus | Set online status for specific player | id (string), online (boolean) | None |
| POST /api/players/register | Add player to player list | name (string), region (string) | Player data |
| POST /api/players/matchresult | Add match to match list | player1 (int), player2 (int), winner(int) | Match data |