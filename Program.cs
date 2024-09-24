using System.Diagnostics;

namespace DiceGame
{
    internal class Program
    {
        //Random number variable
        static readonly Random rand = new();

        //Card variables
        static List<int> deck = Enumerable.Range(1, 16)
            .SelectMany(s => Enumerable.Range(1, 13)).ToList();
        enum CardType
        {
            Ace,
            Two,
            Three,
            Four,
            Five,
            Six,
            Seven,
            Eight,
            Nine,
            Ten,
            Jack,
            Queen,
            King
        }
        static int currentCard = 0;

        //Money variables
        static int playerMoney = 1000;
        static string betMoneyString = "";
        static int betMoney = 0;
        static bool fraud = false;

        //Score variables
        static int playerHandValue = 0;
        static int dealerHandValue = 0;
        static bool dealerStand = false;

        //Player input
        static string playerInput = "";

        //Hand variables
        static List<CardType> playerHand = new();
        static List<int> playerAceValues = new();

        static List<CardType> dealerHand = new();
        static List<int> dealerAceValues = new();

        //Game ended and restart variables
        static bool gameEnded = false;
        static bool blackjack = false;
        static bool playerRestart = false;

        static void Main()
        {
            StartGame();

            //Forever true while loop
            while (true)
            {
                //If player has restarted
                if (playerRestart)
                {
                    //Ask the player to bet and draw the first two cards for the player and dealer
                    StartGame();

                    //Set restart to false as the game has started
                    playerRestart = false;
                }

                //If player wants to hit
                if (playerInput.Trim().ToLower() == "hit")
                {
                    //Draw player and dealer card. Check if player goes above 21 before dealer draws card

                    //Draw player card
                    PlayerDraw();
                    ShowPlayerHand();

                    //Check if either player or dealer has gone above 21
                    PlayerBustCheck();
                    DealerBustCheck();

                    //Draw dealer card
                    if (playerHandValue < 22)
                    {
                        //Stand if dealer is above 17
                        if (dealerHandValue >= 17)
                        {
                            dealerStand = true;
                        }
                    
                        if (!dealerStand)
                        {
                            //Draw card if dealer hasn't stood
                            DealerDraw();
                            ShowDealerHand();
                        }
                        else
                        {
                            //Tell player the dealer stands
                            Console.WriteLine("The dealer stands because their hand's value is 17 or more");
                        }
                    }

                    //Check if dealer goes above 21
                    DealerBustCheck();

                    //Stand if dealer is above 17
                    if (dealerHandValue >= 17)
                    {
                        if (dealerHand[0] == CardType.Ace && dealerHand[1] == CardType.Ace && dealerHand.Count == 2)
                        {
                            //Do nothing, this is only here to make sure one ace becomes a 1 if there are only two cards,
                            //both of which are aces

                        }
                        else
                        {
                            //Stands if the dealer doesn't have two cards, both of which are aces.
                            //If that happens one of the aces should be a 1 instead of an 11
                            dealerStand = true;
                        }
                    }
                }

                //If game hasn't ended, ask player to hit or stand
                if (!gameEnded)
                {
                    Console.WriteLine("Do you want to hit or stand?\n");
                    playerInput = Console.ReadLine();
                }

                //Make dealer roll until they're above 17 if player stands when the dealer is below 17
                while (playerInput.ToLower().Trim() == "stand" && dealerHandValue < 17)
                {
                    //Draw dealer card and show the dealer's hand
                    DealerDraw();
                    ShowDealerHand();

                    //End the game
                    gameEnded = true;
                }

                //End game if player stands
                if (playerInput.ToLower().Trim() == "stand")
                {
                    gameEnded = true;
                }

                //Logic for ending the game
                if (gameEnded)
                {
                    //Write out the value of the player and dealer hand
                    Console.WriteLine("The value of your hand is: " + playerHandValue);
                    Console.WriteLine("The value of the dealer's hand is: " + dealerHandValue);

                    //Break while loop if the player has no money left
                    if (playerMoney <= 0)
                    {
                        Console.WriteLine("You have been kicked out of the casino because you have no money left!");
                        break;
                    }

                    //Check if player commited fraud
                    if (fraud)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("\nYou have been arrested for betting more money than you have!");
                        break;
                    }

                    //Calculate who wins if Blackjack didn't occur
                    if (!blackjack)
                    {
                        if (dealerHandValue >= playerHandValue && dealerHandValue <= 21)
                        {
                            //Dealer has more score than player and is under 21. Dealer wins
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Dealer wins!");
                            playerMoney -= betMoney;
                            Console.WriteLine("You lost " + betMoney + " dollars which means you have " + playerMoney + " dollars left!");
                            Console.ResetColor();
                        }
                        else if (dealerHandValue == playerHandValue)
                        {
                            //Dealer and player score is equal. Dealer wins
                            Console.ForegroundColor = ConsoleColor.Red;
                            playerMoney -= betMoney;
                            Console.WriteLine("You lost " + betMoney + " dollars which means you have " + playerMoney + " dollars left!");
                            Console.ResetColor();
                        }
                        else if (playerHandValue > 21 && dealerHandValue <= 21)
                        {
                            //Player goes over 21 and dealer doesn't. Dealer wins
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Dealer wins!");
                            playerMoney -= betMoney;
                            Console.WriteLine("You lost " + betMoney + " dollars which means you have " + playerMoney + " dollars left!");
                            Console.ResetColor();
                        }
                        else
                        {
                            //All scenarios where the dealer doesn't have a winning condition. Player wins
                            Console.ForegroundColor = ConsoleColor.Green;
                            Console.WriteLine("You win!");
                            playerMoney += betMoney;
                            Console.WriteLine("You won " + betMoney + " dollars! You have " + playerMoney + " dollars now!");
                            Console.ResetColor();
                        }
                    }
                    Console.WriteLine("\nDo you want to play again? y/n");

                    playerInput = Console.ReadLine();

                    //Check if player wants to play again
                    if (playerInput == "y" || playerInput == "yes")
                    {
                        betMoney = 0;
                        playerHandValue = 0;
                        dealerHandValue = 0;

                        playerHand.Clear();
                        playerAceValues.Clear();

                        dealerHand.Clear();
                        dealerAceValues.Clear();

                        dealerStand = false;
                        gameEnded = false;
                        playerRestart = true;
                        blackjack = false;

                        Console.Clear();
                        Console.WriteLine("\x1b[3J");
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        break;
                    }
                }
            }
        }

        //Logic for starting the game, includes betting and drawing the first two cards for both participants
        static void StartGame()
        {
            Console.WriteLine("Welcome to Black Jack!\n");

            deck = Enumerable.Range(1, 16)
                        .SelectMany(s => Enumerable.Range(1, 13)).ToList();
            deck = GenerateRandomLoop(deck);

            // Bet logic

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("How much do you want to bet? You currently have " + playerMoney + " dollars.");

            betMoneyString = Console.ReadLine();
            bool betIsNumber = int.TryParse(betMoneyString, out betMoney);

            //Ask the player to input a NUMBER and not something else
            if (!betIsNumber)
            {
                Console.WriteLine("That is not a number. Please write a how much you want to bet as a NUMBER");
                betMoney = int.Parse(Console.ReadLine());
            }

            //Check if the player commits fraud by betting more money than they have
            if (betMoney > playerMoney)
            {
                fraud = true;
            }

            //Make sure player isn't betting negative money,
            if (betMoney < 0)
            {
                Console.WriteLine("You cannot bet a negative amount of money. Please write a POSITIVE number");
                betMoney = int.Parse(Console.ReadLine());
            }

            //Write how much the player has bet
            Console.WriteLine("You have bet " + betMoney + " dollars!\n");
            Console.ResetColor();

            //Draw first two cards

            //Draw player card
            for (int i = 0; i < 2; i++)
            {
                PlayerDraw();
            }
            ShowPlayerHand();

            //Draw dealer cards
            for (int i = 0; i < 2; i++)
            {
                DealerDraw();
            }
            ShowDealerHand();

            //Check if anyone got Blackjack
            if (playerHandValue == 21 && dealerHandValue != 21)
            {
                //Player has Blackjack and dealer doesn't

                playerMoney += (int)(betMoney * 1.5f);

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You got Blackjack!\n" +
                    "You won " + (betMoney * 1.5f) + " dollars which means you now have " + playerMoney + " dollars!");
                Console.ResetColor();


                gameEnded = true;
                blackjack = true;
            }
            else if (dealerHandValue == 21 && playerHandValue != 21)
            {
                //Dealer has Blackjack and player doesn't

                playerMoney -= betMoney;

                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("The dealer got Blackjack!\n" +
                    "You lost " + betMoney + " dollars which means you have " + playerMoney + " dollars left!");
                Console.ResetColor();

                gameEnded = true;
                blackjack = true;
            }
            else if (playerHandValue == 21 && dealerHandValue == 21)
            {
                //Both the dealer and the player have Blackjack

                Console.ForegroundColor = ConsoleColor.DarkYellow;
                Console.WriteLine("Both you and the dealer got Blackjack!\n" +
                    "You got your bet of " + betMoney + " dollars back which means you still have " + playerMoney + " dollars left!");

                Console.ResetColor();

                gameEnded = true;
                blackjack = true;
            }
        }

        //Draw a card for the player
        static void PlayerDraw()
        {
            currentCard = deck[0];
            deck.RemoveAt(0);

            if (currentCard == 1)
            {
                playerHandValue += 11;
                Console.WriteLine("You drew an ace!");
                playerHand.Add(CardType.Ace);
                playerAceValues.Add(11);
            }
            else if (currentCard == 11)
            {
                playerHandValue += 10;
                Console.WriteLine("You drew a jack!");
                playerHand.Add(CardType.Jack);
            }
            else if (currentCard == 12)
            {
                playerHandValue += 10;
                Console.WriteLine("You drew a queen!");
                playerHand.Add(CardType.Queen);
            }
            else if (currentCard == 13)
            {
                playerHandValue += 10;
                Console.WriteLine("You drew a king!");
                playerHand.Add(CardType.King);
            }
            else
            {
                playerHandValue += currentCard;
                Console.WriteLine("You drew a " + currentCard + "!");

                switch (currentCard)
                {
                    case 2:
                        playerHand.Add(CardType.Two);
                        break;
                    case 3:
                        playerHand.Add(CardType.Three);
                        break;
                    case 4:
                        playerHand.Add(CardType.Four);
                        break;
                    case 5:
                        playerHand.Add(CardType.Five);
                        break;
                    case 6:
                        playerHand.Add(CardType.Six);
                        break;
                    case 7:
                        playerHand.Add(CardType.Seven);
                        break;
                    case 8:
                        playerHand.Add(CardType.Eight);
                        break;
                    case 9:
                        playerHand.Add(CardType.Nine);
                        break;
                    case 10:
                        playerHand.Add(CardType.Ten);
                        break;
                }
            }
        }

        //Show the player's hand
        static void ShowPlayerHand()
        {
            Console.WriteLine("\nYour hand consists of: ");
            Console.ForegroundColor = ConsoleColor.DarkCyan;

            //Write out the player's cards
            foreach (CardType _c in playerHand)
            {
                Console.WriteLine(_c);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        //Calculate what the value should be of the player's aces
        static void CalculatePlayerAces()
        {
            //Calculate value of player aces
            for (int i = 0; i < playerAceValues.Count; i++)
            {
                if (playerAceValues[i] == 11)
                {
                    playerAceValues[i] = 1;
                    playerHandValue -= 10;
                    break;
                }
            }

            //End the game if the player goes above 21. Not sure if this ever triggers, mostly here as a failsafe
            if (playerHandValue > 21)
            {

                Console.WriteLine("You went over 21");
                gameEnded = true;
            }
        }

        //Check if the player has gone above 21
        static void PlayerBustCheck()
        {
            //End game if player is above 21
            if (playerHandValue > 21)
            {
                if (playerHand.Contains(CardType.Ace))
                {
                    //Check if any player aces can become 1 instead of 11 to lower the player score
                    CalculatePlayerAces();

                    if (playerHandValue > 21)
                    {
                        //End the game if the player goes above 21
                        Console.WriteLine("You went over 21");
                        gameEnded = true;
                    }
                }
                else
                {
                    //End the game if the player goes above 21
                    Console.WriteLine("You went over 21");
                    gameEnded = true;
                }
            }
        }


        //Draw a card for the dealer
        static void DealerDraw()
        {
            currentCard = deck[0];
            deck.RemoveAt(0);

            if (currentCard == 1)
            {
                dealerHandValue += 11;
                Console.WriteLine("The dealer drew an ace!");
                dealerHand.Add(CardType.Ace);
                dealerAceValues.Add(11);

            }
            else if (currentCard == 11)
            {
                dealerHandValue += 10;
                Console.WriteLine("The dealer drew a jack!");
                dealerHand.Add(CardType.Jack);
            }
            else if (currentCard == 12)
            {
                dealerHandValue += 10;
                Console.WriteLine("The dealer drew a queen!");
                dealerHand.Add(CardType.Queen);
            }
            else if (currentCard == 13)
            {
                dealerHandValue += 10;
                Console.WriteLine("The dealer drew a king!");
                dealerHand.Add(CardType.King);
            }
            else
            {
                dealerHandValue += currentCard;
                Console.WriteLine("The dealer drew a " + currentCard + ".");

                switch (currentCard)
                {
                    case 2:
                        dealerHand.Add(CardType.Two);
                        break;
                    case 3:
                        dealerHand.Add(CardType.Three);
                        break;
                    case 4:
                        dealerHand.Add(CardType.Four);
                        break;
                    case 5:
                        dealerHand.Add(CardType.Five);
                        break;
                    case 6:
                        dealerHand.Add(CardType.Six);
                        break;
                    case 7:
                        dealerHand.Add(CardType.Seven);
                        break;
                    case 8:
                        dealerHand.Add(CardType.Eight);
                        break;
                    case 9:
                        dealerHand.Add(CardType.Nine);
                        break;
                    case 10:
                        dealerHand.Add(CardType.Ten);
                        break;
                }
            }
        }

        //Show the dealer's hand
        static void ShowDealerHand()
        {
            Console.WriteLine("\nThe dealer's hand consists of: ");
            Console.ForegroundColor = ConsoleColor.DarkRed;

            //Write out the dealer's cards
            foreach (CardType _c in dealerHand)
            {
                Console.WriteLine(_c);
            }

            Console.WriteLine("");
            Console.ResetColor();
        }

        //Calculate what the value should be of the dealer's aces
        static void CalculateDealerAces()
        {
            //Calculate value of dealer aces
            for (int i = 0; i < dealerAceValues.Count; i++)
            {
                if (dealerAceValues[i] == 11)
                {
                    dealerAceValues[i] = 1;
                    dealerHandValue -= 10;
                    break;
                }
            }

            //End the game if the dealer goes above 21. Not sure if this ever triggers, mostly here as a failsafe
            if (dealerHandValue > 21)
            {
                Console.WriteLine("The dealer has gone over 21");
                gameEnded = true;
            }
        }

        //Check if the dealer has gone above 21
        static void DealerBustCheck()
        {          
            if (dealerHandValue > 21)
            {
                if (dealerHand.Contains(CardType.Ace))
                {
                    //Check if any dealer aces can become 1 instead of 11 to lower dealer score
                    CalculateDealerAces();

                    if (dealerHandValue > 21)
                    {
                        //End the game if the dealer goes over 21
                        Console.WriteLine("The dealer went over 21");
                        gameEnded = true;
                    }
                }
                else
                {
                    //End the game if the dealer goes over 21
                    Console.WriteLine("The dealer went over 21");
                    gameEnded = true;
                }
            }
        }


        //Randomize list order
        static List<int> GenerateRandomLoop(List<int> listToShuffle)
        {
            for (int i = listToShuffle.Count - 1; i > 0; i--)
            {
                var k = rand.Next(i + 1);
                var value = listToShuffle[k];
                listToShuffle[k] = listToShuffle[i];
                listToShuffle[i] = value;
            }
            return listToShuffle;
        }
    }
}