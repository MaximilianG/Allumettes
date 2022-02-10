using System;
using System.Threading;

namespace Allumettes
{
    class Program
    {
        // Les variables et les enums
        #region ETATS DU JEU
        public enum State
        {
            PLAYING = 0,
            OVER = 1,
        }

        public enum Difficulty
        {
            EASY = 0,
            MEDIUM = 1,
            HARD = 2
        }

        public enum Joueurs
        {
            PLAYER_1 = 0,
            IA = 1,
            PLAYER_2 = 2
        }

        public enum GameMode
        {
            PVIA= 0, // mode contre l'IA
            PVP = 1 // Mode joueur contre joueur
        }

        public enum GameTime
        {
            EARLYGAME = 0,
            ENDGAME = 1
        }
        #endregion

        // L'execution du program
        #region MAIN
        static void Main(string[] args)
        {
            Setup();
        }
        #endregion

        // Les fonctions standards
        #region FONCTIONS
        // Indique les boites de puissance de 2 de chaque ligne dans un tableau (avec un entier 1 et le reste 0)
        private static void sortTab(int[,] exitTab, int[] sourceTab)
        {
            for(int i=0; i<sourceTab.Length; i++)
            {
                for(int j=0;j<4; j++)
                    exitTab[i, j] = 0;              
            }

            for(int i=0; i<sourceTab.Length; i++)
            {
                if (puissanceDeuxMax(sourceTab[i]) == 3)
                {
                    exitTab[i, 3] = 1;
                    if(sourceTab[i] - 8 > 0)
                    {
                        if (puissanceDeuxMax(sourceTab[i]-8) == 2)
                        {
                            exitTab[i, 2] = 1;
                            if (sourceTab[i] - 12 > 0)
                            {
                                if (puissanceDeuxMax(sourceTab[i]-12) == 1)
                                {
                                    exitTab[i, 1] = 1;
                                    if (sourceTab[i] - 14 > 0)
                                    {
                                            exitTab[i, 0] = 1;
                                    }
                                }
                            }
                        }
                        else 
                        {
                            if (puissanceDeuxMax(sourceTab[i] - 8) == 1)
                            {
                                exitTab[i, 1] = 1;
                                if (sourceTab[i] - 10 > 0)
                                {
                                        exitTab[i, 0] = 1;
                                }
                            }
                            else
                            {
                                exitTab[i, 0] = 1;
                            }
                        }
                    }
                }
                else
                {
                    if (puissanceDeuxMax(sourceTab[i]) == 2)
                    {
                        exitTab[i, 2] = 1;
                        if (sourceTab[i] - 4 > 0)
                        {
                            if (puissanceDeuxMax(sourceTab[i]-4) == 1)
                            {
                                exitTab[i, 1] = 1;
                                if (sourceTab[i] - 6 > 0)
                                {
                                        exitTab[i, 0] = 1;
                                }
                            }
                            else
                            {
                                exitTab[i, 0] = 1;
                            }
                        }
                    }
                    else
                    {
                        if (puissanceDeuxMax(sourceTab[i]) == 1)
                        {
                            exitTab[i, 1] = 1;
                            if (sourceTab[i] - 2 > 0)
                            {
                                    exitTab[i, 0] = 1;
                            }
                        }
                        else
                        {
                            if (sourceTab[i]>0)
                                exitTab[i, 0] = 1;
                        }
                    }
                }
            }
        }

        private static int puissanceDeuxMax(int x) //retourne la puissance max de 2 d'une valeur
        {
            double puissance = Math.Log2((double)x);

            return (int)puissance;
        }

        // Parcours un tableau et retourne la somme de ses valeurs
        private static int SommeTab(int[] tab)
        {
            int somme = 0;

            for (int i = 0; i < tab.Length; i++)
                somme = somme + tab[i];

            return somme;
        }

        private static int SommePuissance(int[,] tab, int x, int taille)
        {
            int somme = 0;

            for(int i=0; i< taille; i++)
            {
                somme = somme + tab[i, x];
            }

            return somme;
        }

        // Verification du tableau si full 0 ou pas
        private static State checkTab(int[] tab)
        {
            for(int i=0; i<tab.Length; i++)
            {
                if (tab[i] != 0)
                    return State.PLAYING;

            }
            return State.OVER;
        }

        // Parcours un tableau et retourne le nombre de lignes égales à 0
        private static int howManyZeroTab(int[] tab)
        {
            int nbZero = 0;
            for (int i=0; i<tab.Length; i++)
            {
                if (tab[i] == 0)
                    nbZero++;
            }

            return nbZero;
        }

        // Parcours un tableau et retourne le nombre de lignes différentes de 0
        private static int HowManyNoneZeroTab(int[] tab)
        {
            int nbNoneZero = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i] != 0)
                    nbNoneZero++;
            }

            return nbNoneZero;
        }

        // Parcours un tableau et retourne le nombre de lignes égales à 1
        private static int HowManySingleTab(int[] tab)
        {
            int nbSingle = 0;
            for (int i = 0; i < tab.Length; i++)
            {
                if (tab[i] == 1)
                    nbSingle++;
            }

            return nbSingle;
        }

        // On retire les allumettes du tableau à telle ligne de tel montant
        private static void pullBars(int[] tab, int line, int nb)
        {
            tab[line] = tab[line] - nb;
        }

        // On remet les allumettes du tableau à telle ligne de tel montant (contraire de pullBars)
        private static void addBars(int[] tab, int line, int nb)
        {
            tab[line] = tab[line] + nb;
        }

        // Remplissage du tableau de jeu
        private static void FillTab(int[] tab, int baseNB)
        {
            int nbBase = baseNB;

            for (int i = tab.Length - 1; i >= 0; i--)
            {
                tab[i] = nbBase;
                nbBase = nbBase - 2;
            }
        }

        // check si tab des paquets de puissances de 2 est équilibré ou non
        private static bool checkBalance(int[,] tab, int size)
        {
            bool balance = false;

            if (isImpair(SommePuissance(tab, 0, size)) == false &&
                isImpair(SommePuissance(tab, 1, size)) == false &&
                isImpair(SommePuissance(tab, 2, size)) == false &&
                isImpair(SommePuissance(tab, 3, size)) == false)
                balance = true;

            return balance;
        }

        #endregion

        // Le lancement du jeu et les program joueur + IA
        #region EXECUTIONS DU JEU
        //setup du jeu
        private static void Setup()
        {
            State m_gameState = State.PLAYING;
            Joueurs m_winner;

            DisplayTop(10);
            Console.Write("\nChoisissez le mode de jeu :\nJcJ (Joueur contre Joueur) ou IA (Joueur contre IA) ? ");
            GameMode m_gameMode = GetGameMode(Console.ReadLine());
            DisplayTop(10);

            Console.Write("La base doit être impaire et au maximum de 15\nDe combien est la base de votre jeu ? ");
            int _base = getImpairPositivNumber(Console.ReadLine());
            int lignes = (_base + 1) / 2;
            int[] game = new int[lignes];
            FillTab(game, _base); // on remplit le tableau

            m_winner = LaunchGame(game, m_gameState, _base, m_gameMode);
            Console.WriteLine("La partie est maintenant terminée ! Le vainqueur est : " + m_winner);
        }

        // Lancement du Jeu
        private static Joueurs LaunchGame(int[] tab, State gameState, int baseNB, GameMode gameMode)
        {
            Joueurs gagnant = Joueurs.PLAYER_1;

            if (gameMode == GameMode.PVIA)
            {
                Console.Write("\nChoisissez le niveau de difficulté de votre adversaire l'IA.\nFacile, normal ou difficile ? ");
                Difficulty _difficulty = GetDifficulty(Console.ReadLine());
                gagnant = LaunchGameVsIA(tab, gameState, baseNB, _difficulty);
            }
            else
                gagnant = LaunchGameJcJ(tab, gameState, baseNB);

            return gagnant;
        }

        // Execution du jeu en mode JcJ + decision du vainqueur
        private static Joueurs LaunchGameJcJ(int[] tab, State gameState, int baseNB)
        {
            bool tourDuJoueur1 = true;
            Joueurs dernierJoueur = Joueurs.PLAYER_1;
            Joueurs gagnant = Joueurs.PLAYER_1;

            DisplayGame(tab, baseNB);

            while (gameState == State.PLAYING)
            {
                if (tourDuJoueur1 == true)
                {
                    humanPlay(tab, baseNB, Joueurs.PLAYER_1);
                    tourDuJoueur1 = false;
                    dernierJoueur = Joueurs.PLAYER_1;
                }
                else
                {
                    humanPlay(tab, baseNB, Joueurs.PLAYER_2);
                    tourDuJoueur1 = true;
                    dernierJoueur = Joueurs.PLAYER_2;
                }
                DisplayGame(tab, baseNB);
                gameState = checkTab(tab);
            }

            if (dernierJoueur == Joueurs.PLAYER_1)
                gagnant = Joueurs.PLAYER_2;
            else
                gagnant = Joueurs.PLAYER_1;

            return gagnant;
        }

        // Execution du jeu en mdoe vs IA + décision du vainqueur
        private static Joueurs LaunchGameVsIA(int[] tab, State gameState, int baseNB, Difficulty difficultyMode)
        {
            bool tourDuJoueur1 = true;
            Joueurs dernierJoueur = Joueurs.PLAYER_1;
            Joueurs gagnant = Joueurs.PLAYER_1;

            DisplayGame(tab, baseNB);

            while (gameState == State.PLAYING)
            {               
                if(tourDuJoueur1 == true)
                {
                    humanPlay(tab, baseNB, dernierJoueur);
                    tourDuJoueur1 = false;
                    dernierJoueur = Joueurs.PLAYER_1;
                }
                else
                {
                    IAPlay(tab, baseNB, difficultyMode);
                    tourDuJoueur1 = true;
                    dernierJoueur = Joueurs.IA;
                }
                DisplayGame(tab, baseNB);
                gameState = checkTab(tab);
            }

            if (dernierJoueur == Joueurs.PLAYER_1)
                gagnant = Joueurs.IA;
            else
                gagnant = Joueurs.PLAYER_1;

            return gagnant;
        }

        // Tour du joueur de jouer
        private static void humanPlay (int[] tab, int baseNB, Joueurs joueur)
        {
            Console.Write("C'est au tour du " + joueur + " sur quelle ligne voulez-vous retirer une allumette ? ");
            int WhatLine = (getLineNumber(Console.ReadLine(), (baseNB + 1)/2))-1;

            while(tab[WhatLine] == 0)
            {
                Console.Write("\nCette ligne est déjà vide, veuillez en choisir une autre : ");
                WhatLine = (getLineNumber(Console.ReadLine(), (baseNB + 1) / 2))-1;
            }

            Console.Write("Combien d'alumettes ? ");
            int howMany = getLineNumber(Console.ReadLine(), tab[WhatLine]);

            pullBars(tab, WhatLine, howMany);
            Console.Clear();
        }

        // Tour de l'IA de jouer
        private static void IAPlay(int[] tab, int baseNB, Difficulty difficultyMode)
        {
            GameTime _gametime = GameTime.EARLYGAME;

            int nbDeLignes = (baseNB + 1) / 2;
            int WhatLine=0;
            int HowMany=0;
            int[,] sortedGame = new int[tab.Length, 4];
            sortTab(sortedGame,tab);
            int nbPuissance3;
            nbPuissance3 = SommePuissance(sortedGame, 3, tab.Length);
            int nbPuissance2;
            nbPuissance2 = SommePuissance(sortedGame, 2, tab.Length);
            int nbPuissance1;
            nbPuissance1 = SommePuissance(sortedGame, 1, tab.Length);
            int nbPuissance0;
            nbPuissance0 = SommePuissance(sortedGame, 0, tab.Length);

            bool nbDelignesIsImpair = isImpair(nbDeLignes);
            bool nbLignesNonNulles_isImpair = isImpair(HowManyNoneZeroTab(tab));
            int nbLignesSingleAllumette = HowManySingleTab(tab);


            Console.WriteLine("L'IA réfléchit ...");
            Thread.Sleep(2000);

            if (difficultyMode == Difficulty.EASY)
            {
                Random rand = new Random();
                WhatLine = rand.Next(1, nbDeLignes + 1);

                while (tab[WhatLine - 1] == 0)
                {
                    WhatLine = rand.Next(1, nbDeLignes + 1);
                }
                if (tab[WhatLine - 1] == 1)
                    HowMany = 1;
                else
                    HowMany = rand.Next(1, tab[WhatLine - 1]);

            } //---------------------------------- MODE MEDIUM -------------------------------------------------------
            else if (difficultyMode == Difficulty.MEDIUM)
            {
                bool found = false;

                if (isImpair(HowManySingleTab(tab)) == false && (HowManyNoneZeroTab(tab) - HowManySingleTab(tab)) == 1 ) // si le nombre de lignes de 1 seule allumette est PAIR ET qu'UNE SEULE AUTRE LIGNE A PLUSIEURS ALUMETTES
                    _gametime = GameTime.ENDGAME;

                if ( _gametime == GameTime.EARLYGAME)
                {
                    if (isImpair(nbPuissance3) == false && isImpair(nbPuissance2) == false && isImpair(nbPuissance1) == false && isImpair(nbPuissance0) == false && found == false)
                    {
                        HowMany = 1;
                        WhatLine = tab.Length;
                    }

                    if(isImpair(nbPuissance3) == true && found == false)
                    {
                        if (isImpair(nbPuissance2) == false && isImpair(nbPuissance1) == false && isImpair(nbPuissance0) == false)
                        {
                            for (int i = tab.Length - 1; i >= 0; i--) // Je parcours le jeu de bas en haut
                            {
                                if (sortedGame[i, 3] == 1) // si le tab rangé possède un 1 en case 3 (donc possède 8 allumettes mini)
                                {
                                    WhatLine = i + 1; // je valide la ligne 
                                    HowMany = 8; // Je valide 8 comme nombre d'allumettes à retirer
                                    found = true; // j'ai trouvé donc je set la variable pour sortir de la boucle
                                }
                            }
                        }
                        else
                        {
                            if(isImpair(nbPuissance0) == true)
                            {
                                HowMany = 7;
                                for (int i = tab.Length - 1; i >= 0; i--)
                                {
                                    if(isImpair(sortedGame[i,0]) == true)
                                    {
                                        WhatLine = i;
                                    }
                                }
                            }

                        }

                    
                    }

                    if (isImpair(nbPuissance3) == true && found == false) // si le nombre de paquet de 8 est impaire
                    {
                        while (found == false)
                        {
                            for (int i = tab.Length - 1; i >= 0; i--) // Je parcours le jeu de bas en haut
                            {
                                if (sortedGame[i, 3] == 1) // si le tab rangé possède un 1 en case 3 (donc possède 8 allumettes mini)
                                {
                                    WhatLine = i + 1; // je valide la ligne 
                                    HowMany = 8; // Je valide 8 comme nombre d'allumettes à retirer
                                    found = true; // j'ai trouvé donc je set la variable pour sortir de la boucle
                                }
                            }
                        }
                    }

                    if (isImpair(nbPuissance2) == true && found == false)
                    {
                        while (found == false)
                        {
                            for (int i = tab.Length - 1; i >= 0; i--)
                            {
                                if (sortedGame[i, 2] == 1)
                                {
                                    WhatLine = i + 1;
                                    HowMany = 4;
                                    found = true;
                                }
                            }
                        }
                    }

                    if (isImpair(nbPuissance1) == true && found == false)
                    {
                        while (found == false)
                        {
                            for (int i = tab.Length - 1; i >= 0; i--)
                            {
                                if (sortedGame[i, 1] == 1)
                                {
                                    WhatLine = i + 1;
                                    HowMany = 2;
                                    found = true;
                                }
                            }
                        }
                    }

                    if (isImpair(nbPuissance0) == true && found == false)
                    {
                        while (found == false)
                        {
                            for (int i = tab.Length - 1; i >= 0; i--)
                            {
                                if (sortedGame[i, 0] == 1)
                                {
                                    WhatLine = i + 1;
                                    HowMany = 1;
                                    found = true;
                                }
                            }
                        }
                    }
                }
                else // SI ON EST EN ENDGAME    
                {
                    for(int i=0; i < tab.Length; i++)
                    {
                        if (tab[i] > 1)
                        {
                            WhatLine = i + 1;
                            HowMany = tab[i] - 1;
                            found = true;
                        }
                    }
                }
            }
            else if(difficultyMode == Difficulty.HARD)
            {
                _gametime = GameTime.EARLYGAME; 

                int[] copieTab = new int[tab.Length];
                Array.Copy(tab, copieTab, tab.Length);               

                int[,] copieSortedGame = new int[tab.Length,4];
                Array.Copy(sortedGame, copieSortedGame, tab.Length);

                if (nbLignesNonNulles_isImpair == false) // s'il reste que deux lignes non nulles
                    _gametime = GameTime.ENDGAME;

                if(_gametime == GameTime.EARLYGAME)
                {
                    if (checkBalance(sortedGame, tab.Length) == true) // si le tableau est équilibré au début de notre tour de jeu, on enlève seulement une allumette aléatoirement
                    {
                        Random rand = new Random();
                        WhatLine = rand.Next(1, nbDeLignes + 1);

                        while (tab[WhatLine - 1] == 0)
                        {
                            WhatLine = rand.Next(1, nbDeLignes + 1);
                        }

                        HowMany = 1;
                    }
                    else // faire une simulation de retraits d'allumettes jusqu'à ce qu'en un seul moove, checkbalance retourne TRUE
                    {
                        bool found = false;
                        int i = 0;
                        int j = 1;

                        while (i<copieTab.Length || found == false)
                        {
                            Array.Copy(tab, copieTab, tab.Length);
                            Array.Copy(sortedGame, copieSortedGame, tab.Length);

                            if (tab[i] == 0 && i< copieTab.Length-1)
                                i++;

                            while (j<=tab[i] && found == false && copieTab[i]>=0)
                            {
                                Array.Copy(tab, copieTab, tab.Length);
                                Array.Copy(sortedGame, copieSortedGame, tab.Length);

                                pullBars(copieTab, i, j);
                                sortTab(copieSortedGame, copieTab);
                            
                                if (checkBalance(copieSortedGame, tab.Length) == true)
                                {
                                    found = true;
                                    HowMany = j;
                                    WhatLine = i + 1;
                                }
                                else
                                {
                                    j++;
                                }
                            }

                            j = 1;
                            i++;
                        }
                    }

                }
                else // Si endgame
                {
                    // si une des deux lignes = 1, on clear l'autre
                    if (nbLignesSingleAllumette == 1)
                    {
                        for(int i=0; i<tab.Length; i++)
                        {
                            if (tab[i] >= 2)
                            {
                                WhatLine = i + 1;
                                HowMany = tab[i];
                            }
                        }
                    }

                    // si les deux lignes sont supérieurs à 1, on équilibre.

                    if (nbLignesSingleAllumette == 0)
                    {
                        bool found = false;
                        int i = 0;
                        int j = 1;

                        while (i < copieTab.Length || found == false)
                        {
                            Array.Copy(tab, copieTab, tab.Length);
                            Array.Copy(sortedGame, copieSortedGame, tab.Length);

                            if (tab[i] == 0 && i < copieTab.Length - 1)
                                i++;

                            while (j <= tab[i] && found == false && copieTab[i] >= 0)
                            {
                                Array.Copy(tab, copieTab, tab.Length);
                                Array.Copy(sortedGame, copieSortedGame, tab.Length);

                                pullBars(copieTab, i, j);
                                sortTab(copieSortedGame, copieTab);

                                if (checkBalance(copieSortedGame, tab.Length) == true)
                                {
                                    found = true;
                                    HowMany = j;
                                    WhatLine = i + 1;
                                }
                                else
                                {
                                    j++;
                                }
                            }

                            j = 1;
                            i++;
                        }
                    }
                }

            }
 

            Console.Clear();
            Console.WriteLine("L'IA a retiré " + HowMany + " allumettes sur la ligne " + WhatLine);

            pullBars(tab, WhatLine - 1, HowMany);
            
        } 
        #endregion

        // Les fonctions d'affichages des espaces, allumettes, tirets, etc .. (mise en forme visuel)
        #region FONCTION D'AFFICHAGES
        // Affichage des lignes du tableau
        private static void DisplayLine(int nombreI, int max)
        {
            if (isImpair(nombreI) == true)
            {
                int espaces = (max - nombreI) / 2;
                Console.Write("|");
                DisplaySpace(espaces);
                DisplayI(nombreI);
                DisplaySpace(espaces);
                Console.Write("|");
            }
            else
            {
                int espaces = (max - (nombreI+1))/2;
                Console.Write("|");
                DisplaySpace(espaces);
                DisplayI(nombreI);
                DisplaySpace(espaces+1);
                Console.Write("|");
            }          
        }

        // Affichage du jeu
        private static void DisplayGame(int[] tab, int baseNB)
        {
            DisplayTop(baseNB + 2);

            for (int i = 0; i<tab.Length; i++)
            {
                Console.Write((i+1) + " ");
                DisplayLine(tab[i], baseNB);
                Console.Write("\n");
            }

            DisplayTop(baseNB + 2);

        }

        // Affichage des tirets du haut et du bas
        private static void DisplayTop(int nb)
        {
            DisplaySpace(2);
            for (int i = 0; i < nb; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n");
        }

        // Affichage des allumettes
        private static void DisplayI(int nb)
        {
            for (int i = 0; i < nb; i++)
            {
                Console.Write("I");
            }
        }

        // Affichage des espaces
        private static void DisplaySpace(int nb)
        {
            for (int i = 0; i < nb; i++)
            {
                Console.Write(" ");
            }
        }
        #endregion

        // Fonctions de lecture des entrées utilisateur ainsi que leur vérifications
        #region RECUPERATION ET VERIFICATION
        // Récupération + vérification du mode de jeu
        private static GameMode GetGameMode(string entry)
        {
            while (entry.ToLower() != "jcj" && entry.ToLower() != "ia")
            {
                Console.Write("Entrée incorrecte, veuillez écrire \"JcJ\" ou \"IA\"");
                entry = Console.ReadLine();
            }

            if (entry.ToLower() == "jcj")
                return GameMode.PVP;
            else
                return GameMode.PVIA;
        }

        // Récupération + vérification de la difficulté de l'IA
        private static Difficulty GetDifficulty(string entry)
        {
            while (entry.ToLower() != "facile" && entry.ToLower() != "normal" && entry.ToLower() != "difficile")
            {
                Console.Write("Entrée incorrecte, veuillez écrire \"facile\", \"normal\" ou \"difficile\"");
                entry = Console.ReadLine();
            }

            if (entry.ToLower() == "facile")
                return Difficulty.EASY;
            else if (entry.ToLower() == "normal")
                return Difficulty.MEDIUM;
            else
                return Difficulty.HARD;
        }

        // Récupération + vérification de la ligne dans la quelle retirer des allumettes
        private static int getLineNumber(string entry, int maximum)
        {
            bool state = int.TryParse(entry, out int q);
            bool positiv = isPositiv(q);
            bool smaller = isSmaller(q, maximum);

            while (state == false || positiv == false || smaller == false || q == 0)
            {
                Console.Write("Valeur entrée incorrecte, veuillez entrer un chiffre entre 1 et " + maximum + " : ");
                entry = Console.ReadLine();
                state = int.TryParse(entry, out q);
                positiv = isPositiv(q);
                smaller = isSmaller(q, maximum);
            }

            return q;
        }


        // Récupération + vérification du nombre d'allumettes de la base du jeu
        private static int getImpairPositivNumber(string entry)
        {
            bool state = int.TryParse(entry, out int q);
            bool positiv = isPositiv(q);
            bool impair = isImpair(q);
            bool smaller = isSmaller(q,15);



            while (state == false || positiv == false || impair == false || smaller == false)
            {
                Console.Write("Valeur entrée incorrecte, veuillez entrer un nombre POSITIF IMPAIRE et PLUS PETIT OU EGAL A 15 : ");
                entry = Console.ReadLine();
                state = int.TryParse(entry, out q);
                positiv = isPositiv(q);
                impair = isImpair(q);
                smaller = isSmaller(q, 15);
            }

            return q;
        }

        // Vérification si un nombre est plus petit que l'autre

        private static bool isSmaller(int x, int y)
        {
            if (x <= y)
                return true;
            else
                return false;
        }

        // Vérification si un nombre est impair ou non

        private static bool isImpair(int number)
        {
            float n = (float)number;

            if (n%2f == 0)
                return false;
            else
                return true;
        }

        // Vérification si un nombre est positif ou non

        private static bool isPositiv(int number)
        {
            if (number > 0)
                return true;
            else
                return false;
        }
        #endregion
    }
}
