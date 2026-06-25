using NUnit.Framework;
using OTS2026_PT1_GrupaA;
using OTS2026_PT1_GrupaA.Exceptions;
using OTS2026_PT1_GrupaA.Models;
using System;
using System.Numerics;
using System.Security.Cryptography.X509Certificates;
namespace OTS2026_PT1_GrupaB.Test
{
    [TestFixture]
    public class GameTest
    {
        [TestCase(0, 0)]
        [TestCase(24, 12)]
        [TestCase(10, 13)]
        [TestCase(19, 19)]
        public void Game_PlayerAndBoatInsideLandZone_GameCreatedSuccessfully(int x, int y)
        {
            Position playerposition = new Position(x, y);
            Position boatposition = new Position(0, 0);

            Game game = new Game(playerposition, boatposition);

            Assert.That(game.Player.Position, Is.EqualTo(playerposition));
        }

        [TestCase(25, 12)]
        [TestCase(24, 13)]
        [TestCase(9, 13)]
        [TestCase(20, 13)]
        [TestCase(10, 20)]
        [TestCase(5, 20)]
        public void Game_PlayerOutsideLandZone_ThrowsInvalidPlayerPositionException(int x, int y)
        {
            Position playerposition = new Position(x, y);
            Position boatposition = new Position(0, 0);


            Exception ex = Assert.Throws<InvalidPlayerPositionException>((TestDelegate)(() => new Game(playerposition, boatposition)));
            Assert.That(ex.Message, Is.EqualTo("Player and boat must be in the Land zone!"));


        }
        [TestCase(25, 12)]
        [TestCase(10, 20)]
        public void Game_BoatOutsideLandZone_ThrowsInvalidPlayerPositionException(int x, int y)
        {
            Position playerposition = new Position(0, 0);
            Position boatposition = new Position(x, y);

            Exception ex = Assert.Throws<InvalidPlayerPositionException>((TestDelegate)(() => new Game(playerposition, boatposition)));
            Assert.That(ex.Message, Is.EqualTo("Player and boat must be in the Land zone!"));
        }

        [TestCase(-1, 0)]
        [TestCase(0, -1)]
        [TestCase(0, 30)]
        [TestCase(30, 0)]


        public void Game_PlayerOutsideMapBoundaries_ThrowsIndexOutOfRangeException(int x, int y)
        {
            Position playerposition = new Position(x, y);
            Position boatposition = new Position(0, 0);

            Assert.Throws<IndexOutOfRangeException>((TestDelegate)(() => new Game(playerposition, boatposition)));
        }

        [Test]
        public void ValidatePosition_LandZone_ReturnsTrue()
        {
            Game game = new Game(new Position(0, 0), new Position(0, 0));

            bool result = game.ValidatePosition(new Position(5, 5));

            Assert.That(result, Is.True);
        }

        [TestCase(-1, 5)]
        [TestCase(5, -1)]
        [TestCase(30, 5)]
        [TestCase(5, 30)]
        public void ValidatePosition_OutsideMapBoundaries_ReturnsFalse(int x, int y)
        {
            Game game = new Game(new Position(0, 0), new Position(0, 0));

            bool result = game.ValidatePosition(new Position(x, y));

            Assert.That(result, Is.False);
        }
        [TestCase(25, 12)]
        [TestCase(24, 13)]
        public void ValidatePosition_InvalidZone_ReturnsFalse(int x, int y)
        {

            Game game = new Game(new Position(0, 0), new Position(0, 0));
            bool result = game.ValidatePosition(new Position(x, y));
            Assert.That(result, Is.False);

        }

        [Test]
        public void ValidatePosition_PondZoneWithoutBoat_ReturnsFalse()
        {
            Game game = new Game(new Position(0, 0), new Position(0, 0));

            bool result = game.ValidatePosition(new Position(10, 25));

            Assert.That(result, Is.False);
        }
        [Test]
        public void ValidatePosition_PondZoneWithBoat_ReturnsTrue()
        {
            Game game = new Game(new Position(0, 0), new Position(0, 0));
            game.Player.HasBoat = true;

            bool result = game.ValidatePosition(new Position(10, 25));

            Assert.That(result, Is.True);
        }


        [TestCase(10, 19, true)]
        [TestCase(10, 20, false)]
        public void ValidatePosition_LandPondBoundary_ReturnsExpected(int x, int y, bool expected)
        {
            Game game = new Game(new Position(0, 0), new Position(0, 0));

            bool result = game.ValidatePosition(new Position(x, y));

            Assert.That(result, Is.EqualTo(expected));
        }
        [Test]
        public void MovePlayer_InvalidPosition_PlayerPositionUnchanged()
        {
            Position playerStart = new Position(0, 0);
            Game game = new Game(playerStart, new Position(0, 0));

            game.MovePlayer(Move.Up);

            Assert.That(game.Player.Position, Is.EqualTo(playerStart));
        }
        [Test]
        public void MovePlayer_ValidPosition_PlayerPositionChanged()
        {
            Game game = new Game(new Position(5, 5), new Position(0, 0));

            game.MovePlayer(Move.Right);

            Assert.That(game.Player.Position, Is.EqualTo(new Position(6, 5)));
        }

        [Test]
        public void ResolvePlayerPosition_BaitOnField_BaitCollectedAndFieldEmptied()
        {
            Game game = new Game(new Position(5, 5), new Position(0, 0));
            game.Map.AddContentToFieldOnPosition(FieldContent.Bait, new Position(5, 5));

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfBait, Is.EqualTo(1));
            Assert.That(game.Map.Fields[5, 5].Content, Is.EqualTo(FieldContent.Empty));
        }

        [Test]
        public void ResolvePlayerPosition_BoatOnField_HasBoatTrueAndFieldEmptied()
        {
            Game game = new Game(new Position(5, 5), new Position(0, 0));
            game.Map.AddContentToFieldOnPosition(FieldContent.Boat, new Position(5, 5));

            game.ResolvePlayerPosition();

            Assert.That(game.Player.HasBoat, Is.True);
            Assert.That(game.Map.Fields[5, 5].Content, Is.EqualTo(FieldContent.Empty));
        }
        [Test]
        public void ResolvePlayerPosition_FishOnFieldWithBait_FishCaughtAndBaitDecreased()
        {
            Game game = new Game(new Position(10, 19), new Position(0, 0));
            game.Player.HasBoat = true;
            game.Player.AmountOfBait = 3;
            game.MovePlayer(Move.Down);
            game.Map.AddContentToFieldOnPosition(FieldContent.Fish, new Position(10, 20));

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfFish, Is.EqualTo(1));
            Assert.That(game.Player.AmountOfBait, Is.EqualTo(2));
            Assert.That(game.Map.Fields[10, 20].Content, Is.EqualTo(FieldContent.Empty));
        }

        [Test]
        public void ResolvePlayerPosition_FishOnFieldWithoutBait_FishNotCaught()
        {
            Game game = new Game(new Position(10, 19), new Position(0, 0));
            game.Player.HasBoat = true;
            game.Player.AmountOfBait = 0;
            game.MovePlayer(Move.Down);
            game.Map.AddContentToFieldOnPosition(FieldContent.Fish, new Position(10, 20));

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfFish, Is.EqualTo(0));
            Assert.That(game.Player.AmountOfBait, Is.EqualTo(0));
        }
        [Test]
      
        public void ResolvePlayerPosition_EmptyField_NothingChanges()
        {
            Game game = new Game(new Position(5, 5), new Position(0, 0));

            game.ResolvePlayerPosition();

            Assert.That(game.Player.AmountOfBait, Is.EqualTo(0));
            Assert.That(game.Player.AmountOfFish, Is.EqualTo(0));
            Assert.That(game.Player.HasBoat, Is.False);
        }

        [TestCase(16, 0, false, Game.Score.Good)]
        [TestCase(15, 12, true, Game.Score.Good)]
        [TestCase(9, 12, true, Game.Score.Good)]
        [TestCase(8, 12, true, Game.Score.Average)]
        [TestCase(11, 12, false, Game.Score.Bad)]
        [TestCase(0, 11, true, Game.Score.Bad)]
        [TestCase(0, 0, false, Game.Score.Bad)]
        public void CalculateIncome_VariousInputs_ReturnsExpectedScore(int fish, int bait, bool hasBoat, Game.Score expected)
        {
            Game game = new Game(new Position(0, 0), new Position(0, 0));
            game.Player.AmountOfFish = fish;
            game.Player.AmountOfBait = bait;
            game.Player.HasBoat = hasBoat;

            Game.Score result = game.CalculateIncome();

            Assert.That(result, Is.EqualTo(expected));
        }
    }
}