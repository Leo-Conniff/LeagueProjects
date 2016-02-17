using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;

namespace League_Game_Data
{
    class LeagueGame
    {
        private string[][] _stats;
        private LeagueDB _db;
        private string _gameId;
        public LeagueGame(string[][] data, Tuple<string[], string, string, string> gameData)
        {
            _stats = data;

            var connString = ConfigurationManager.AppSettings["connString"];
            var sqlite = ConfigurationManager.AppSettings["sqlite"] == "true";

            _db = new LeagueDB(connString, sqlite);

            createGame(gameData);
        }

        public void createGame(Tuple<string[], string, string, string> gameData)
        {
            var query = @"INSERT INTO Games (BlueTeam,RedTeam,GameDate, Duration, MatchHistory) VALUES (@blue, @red, @date, @duration, @matchHistory)";
            var sqlParameters = new List<Tuple<string, DbType, string, Type>>();

            //Tuple is Item1 Teams, Item2 Match History URL, Item3 date of game, Item4 match length
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("blue", DbType.String, gameData.Item1[0].Trim(), typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("red", DbType.String, gameData.Item1[1].Trim(), typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("matchHistory", DbType.String, gameData.Item2, typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("date", DbType.DateTime, gameData.Item3, typeof(DateTime)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("duration", DbType.Time, gameData.Item4, typeof(TimeSpan)));
                        
            _gameId = _db.insertGame(query, sqlParameters).ToString();
        }

        public void processBans(List<string> bans)
        {
            var query = "";
            bool allBansPresent = bans.Count == 6;
            if(allBansPresent)
            {
                query = "INSERT INTO Bans (Team,GameId, ChampionId) VALUES (@team,@gameId,@champId)";
            }
            else
            {
                query = "INSERT INTO Bans (GameId, ChampionId) VALUES (@gameId,@champId)";
            }

            int count = 0;
            foreach(var ban in bans)
            { 
                
                var sqlParameters = new List<Tuple<string, DbType, string, Type>>();
                
                sqlParameters.Add(new Tuple<string, DbType, string, Type>("gameId", DbType.Int32, _gameId, typeof(int)));
                sqlParameters.Add(new Tuple<string, DbType, string, Type>("champId", DbType.String, ban, typeof(string)));
                if (allBansPresent)
                {
                    sqlParameters.Add(new Tuple<string, DbType, string, Type>("team", DbType.Boolean, (count > 2).ToString(), typeof(Boolean)));
                    count++;
                }
                _db.executeNonQuery(query, sqlParameters);
            }
        }

        public void processStats()
        {
            foreach (var stat in _stats)
            {
                processCombat(stat);
                processDamageDealt(stat);
                processDamageTaken(stat);
                processWards(stat);
                processIncome(stat);
            }
            _db.dbDispose();
        }
        public void processCombat(string[] stats)
        {
            var sqlParameters = new List<Tuple<string, DbType, string, Type>>();
            var query = "INSERT INTO Combat VALUES (@gameId,@playerId,@champId,@kills,@deaths,@assists,@lks,@lmk,@firstblood)";
            var kda = stats[0].Split(new string[] { @"/" }, 3, StringSplitOptions.RemoveEmptyEntries);
            
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("gameId", DbType.Int32, _gameId, typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("playerId", DbType.String, stats[29], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("champId", DbType.String, stats[28], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("kills", DbType.Int32, kda[0], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("deaths", DbType.Int32, kda[1], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("assists", DbType.Int32, kda[2], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("lks", DbType.Int32, stats[1], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("lmk", DbType.Int32, stats[2], typeof(int)));
            stats[3] = stats[3] == "●" ? "true" : "false";
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("firstblood", DbType.Boolean, stats[3], typeof(Boolean)));

            _db.executeNonQuery(query, sqlParameters);
        }

        public void processDamageDealt(string[] stats)
        {
            var sqlParameters = new List<Tuple<string, DbType, string, Type>>();
            var query = "INSERT INTO DamageDealt VALUES (@gameId,@playerId,@champId,@TDC,@PDC,@MDC,@TrDC,@DamageDealt,@PhyDamageDealt,@MagicDamageDealt,@TrueDamageDealt,@LCS)";
            
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("gameId", DbType.Int32, _gameId, typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("playerId", DbType.String, stats[29], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("champId", DbType.String, stats[28], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("TDC", DbType.Int32, stats[4], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("PDC", DbType.Int32, stats[5], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("MDC", DbType.Int32, stats[6], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("TrDC", DbType.Int32, stats[7], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("DamageDealt", DbType.Int32, stats[8], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("PhyDamageDealt", DbType.Int32, stats[9], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("MagicDamageDealt", DbType.Int32, stats[10], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("TrueDamageDealt", DbType.Int32, stats[11], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("LCS", DbType.Int32, stats[12], typeof(int)));
            _db.executeNonQuery(query, sqlParameters);
        }

        public void processDamageTaken(string[] stats)
        {
            var sqlParameters = new List<Tuple<string, DbType, string, Type>>();
            var query = "INSERT INTO DamageTaken VALUES (@gameId,@playerId,@champId,@DamageHealed,@DamageTaken,@PhyDamageTaken,@MagicDamageTaken,@TrueDamageTaken)";

            sqlParameters.Add(new Tuple<string, DbType, string, Type>("gameId", DbType.Int32, _gameId, typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("playerId", DbType.String, stats[29], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("champId", DbType.String, stats[28], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("DamageHealed", DbType.Int32, stats[13], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("DamageTaken", DbType.Int32, stats[14], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("PhyDamageTaken", DbType.Int32, stats[15], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("MagicDamageTaken", DbType.Int32, stats[16], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("TrueDamageTaken", DbType.Int32, stats[17], typeof(int)));
            _db.executeNonQuery(query, sqlParameters);
        }

        public void processWards(string[] stats)
        {
            var sqlParameters = new List<Tuple<string, DbType, string, Type>>();
            var query = "INSERT INTO Wards VALUES (@gameId,@playerId,@champId,@wardsPlaced,@WardsDestroyed,@SWardsPurchased,@VWardsPurchased)";

            sqlParameters.Add(new Tuple<string, DbType, string, Type>("gameId", DbType.Int32, _gameId, typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("playerId", DbType.String, stats[29], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("champId", DbType.String, stats[28], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("wardsPlaced", DbType.Int32, stats[18], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("WardsDestroyed", DbType.Int32, stats[19], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("SWardsPurchased", DbType.Int32, stats[20], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("VWardsPurchased", DbType.Int32, stats[21], typeof(int)));
            _db.executeNonQuery(query, sqlParameters);
        }

        public void processIncome(string[] stats)
        {
            var sqlParameters = new List<Tuple<string, DbType, string, Type>>();
            var query = "INSERT INTO Income VALUES (@gameId,@playerId,@champId,@GoldEarned,@GoldSpent,@MinionsKilled,@NMK,@NMKTeam, @NMKEnemy)";

            sqlParameters.Add(new Tuple<string, DbType, string, Type>("gameId", DbType.Int32, _gameId, typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("playerId", DbType.String, stats[29], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("champId", DbType.String, stats[28], typeof(string)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("GoldEarned", DbType.Int32, stats[22], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("GoldSpent", DbType.Int32, stats[23], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("MinionsKilled", DbType.Int32, stats[24], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("NMK", DbType.Int32, stats[25], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("NMKTeam", DbType.Int32, stats[26], typeof(int)));
            sqlParameters.Add(new Tuple<string, DbType, string, Type>("NMKEnemy", DbType.Int32, stats[27], typeof(int)));
            _db.executeNonQuery(query, sqlParameters);
        }
    }
}
