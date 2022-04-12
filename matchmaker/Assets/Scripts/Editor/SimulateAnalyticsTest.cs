using ServerObjects;
using GPF;
using System.Threading.Tasks;
using GPF.ServerObjects;
using GPFEditor;
using NUnit.Framework;
using System.Collections.Generic;
using System;

namespace MatchmakerTests
{
    [TestFixture]
    [TestOf(typeof(MatchPlayerSO))]
    sealed class SimulateAnalyticsTest : ServerObjectTest
    {
        class AnalyticsContext : AnalyticsMessage
        {
        }

        const int TOTAL_PLAYERS = 5;
        static TimeSpan PLAYER_SPAWN_RATE = TimeSpan.FromSeconds(10);
        const int GAMES_PER_SESSION = 5;
        const int SESSIONS_PER_PLAYER = 5;
        static TimeSpan TIME_BETWEEN_SESSIONS = TimeSpan.FromMinutes(7);
        static TimeSpan TOTAL_TEST_LENGTH = TimeSpan.FromMinutes(60);
        const int USER_INTERACTION_DELAY = 1100;

        public override async Task Run()
        {
            if (RuntimeSettings.DeployEnv.IsSimulated)
            {
                if (DateTime.Now - timeService.Now > TimeSpan.FromHours(2))
                {
                    await Delay(DateTime.Now - timeService.Now - TimeSpan.FromHours(1));
                }
            }
            var tasks = new List<Task>();

            var matchmakerId = MatchmakerSO.GetTestSuffix();

            for (int i = 0; i < TOTAL_PLAYERS; i++)
            {
                var name = $"Player-{i}";
                var task = PlayerBehaviour(
                    CreateSyncer(name),
                    MatchPlayerSO.GetSOID(matchmakerId)
                    );
                tasks.Add(task);
                await Delay(PLAYER_SPAWN_RATE);
            }

            await Delay(TOTAL_TEST_LENGTH);
        }

        async Task PlayerBehaviour(Syncer syncer, SOID<MatchPlayerSO> id)
        {
            var rand = new System.Random();

            var player = await syncer.Sync(id);

            AnalyticsContext context = GetContext();

            for (int i = 0; i < SESSIONS_PER_PLAYER; i++)
            {
                LogInfo($"Session {i}");
                for (int j = 0; j < GAMES_PER_SESSION; j++)
                {
                    LogInfo($"Playing Game {j}");

                    await DelayUserInteraction();

                    // Request a match
                    syncer.Send(id, AddAnalytics(new MatchPlayerSO.Match(), context));

                    do
                    {
                        // Wait for the matchmaker to find a match
                        await syncer.WaitFor(player, nameof(MatchPlayerSO.state), FieldIs.Equal(MatchPlayerSO.State.TABLE_OFFERED));

                        await DelayUserInteraction();

                        // Accept the offer to join the game
                        await syncer.SendWait(id, AddAnalytics(new MatchPlayerSO.Accept(), context));

                        // Wait for the other player to accept the match
                        await syncer.WaitFor(player, nameof(MatchPlayerSO.state), FieldIs.NotEqual(MatchPlayerSO.State.TABLE_ACCEPTED));
                    }
                    while (player.state != MatchPlayerSO.State.GAME_STARTED);

                    await DelayUserInteraction();

                    // submit our move
                    await syncer.SendWait(id, AddAnalytics(new MatchPlayerSO.Move { move = GetRandomMove() }, context));

                    // Wait for the game to resolve
                    await syncer.WaitFor(player, nameof(MatchPlayerSO.state), FieldIs.NotEqual(MatchPlayerSO.State.GAME_WAITING));

                    await DelayUserInteraction();

                    var won = player.state == MatchPlayerSO.State.GAME_WON;

                    // Leave table
                    await syncer.SendWait(id, AddAnalytics(new MatchPlayerSO.LeaveTable {}, context));

                    // Simulate showing the purchase screen occasionally
                    if (j == GAMES_PER_SESSION - 2)
                    {
                        await syncer.SendWait(id, new MatchPlayerSO.GoToPurchaseOffer());
                        if (won || rand.Next(3) == 0)
                            await syncer.SendWait(id, AddAnalytics(new MatchPlayerSO.MakePurchase { }, context));
                        else
                            await syncer.SendWait(id, AddAnalytics(new MatchPlayerSO.RejectPurchase { }, context));
                    }
                }
                await Delay(TIME_BETWEEN_SESSIONS);
            }
        }

        MatchGameSO.Move GetRandomMove()
        {
            Array values = Enum.GetValues(typeof(MatchGameSO.Move));
            Random random = new System.Random();
            var result = (MatchGameSO.Move)values.GetValue(random.Next(values.Length-1)+1);
            return result;
        }

        ServerObjectMessage AddAnalytics(AnalyticsMessage msg, AnalyticsContext context)
        {
            var fields = typeof(AnalyticsMessage).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            foreach (var field in fields)
            {
                object value = field.GetValue(context);
                field.SetValue(msg, value);
            }

            msg.Element = msg.GetType().Name.ToLower();
            switch(msg.GetType().Name)
            {
                case "Match":
                    msg.Label = "Join Matchmaking";
                    break;
                case "Accept":
                    msg.Label = "Accept";
                    break;
                case "Move":
                    var move = msg as MatchPlayerSO.Move;
                    msg.Label = move.move.ToString();
                    break;
                case "LeaveTable":
                    msg.Label = "Leave";
                    break;
                case "MakePurchase":
                    msg.Label = "Buy";
                    break;
                case "RejectPurchase":
                    msg.Label = "No Thanks";
                    break;
            }

            return msg;
        }

        AnalyticsContext GetContext()
        {
            AnalyticsContext context = new AnalyticsContext
            {
                DeviceId = Guid.NewGuid().ToString()
            };

            var platforms = new string[] { "Android", "iOS", "Windows", "Xbox", };
            var interactions = new string[] { "touch", "touch", "click", "click", };
            Random rand = new System.Random();
            int choice = rand.Next(platforms.Length);
            context.Platform = platforms[choice];
            context.Interaction = interactions[choice];

            var languages = new string[] { "English", "Spanish", "French", "German", "Mandarin", "Hindi" };
            int languageChoice = rand.Next(languages.Length);
            context.Language = languages[languageChoice];


            var deviceNames = new string[,] { { "Galaxy", "Pixel" }, { "iPhone", "iPad" }, { "Surface", "Razor" }, { "Series S", "Series X" } };
            var resolutions = new string[,] { { "3200 × 1440", "2400 x 1080" }, { "1125 x 2436", "2048 x 2732" }, { "3000 x 2000", "3240 x 2160" }, { "1920 x 1080", "3240 x 2160" } };
            int deviceChoice = rand.Next(2);
            context.DeviceName = deviceNames[choice, deviceChoice];
            context.Resolution = resolutions[choice, deviceChoice];

            return context;
        }

        async Task DelayUserInteraction()
        {
            // TODO: Add noise
            await Delay(USER_INTERACTION_DELAY);
        }
    }
}