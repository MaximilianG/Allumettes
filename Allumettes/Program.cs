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
            PLAYER1 = 0,
            IA = 1,
            PLAYER2 = 2
        }

        public enum GameMode
        {
            PVIA= 0, // mode contre l'IA
            PVP = 1 // Mode joueur contre joueur
        }
        #endregion

        // L'execution du program
        #region MAIN
        static void Main(string[] args)
        {
            State m_gameState = State.PLAYING;
            Joueurs m_winner;

            DisplayTop(10);
            Console.Write("\nChoisissez le mode de jeu :\nJcJ (Joueur contre Joueur) ou IA (Joueur contre IA) ? ");
            GameMode m_gameMode = GetGameMode(Console.ReadLine());
            DisplayTop(10);

            Console.Write("La base doit être impaire\nDe combien est la base de votre jeu ? ");
            int _base = getImpairPositivNumber(Console.ReadLine());
            int lignes = (_base+1) / 2;
            int[] game = new int[lignes];
            FillTab(game, _base); // on remplit le tableau

            m_winner = LaunchGame(game, m_gameState, _base, m_gameMode);
            Console.WriteLine("La partie est maintenant terminée ! Le vainqueur est : " + m_winner);
        }
        #endregion

        // Les fonctions standards
        #region FONCTIONS
        // Parcours un tableau et retourne la somme de ses valeurs
        private static int SommeTab(int[] tab)
        {
            int somme = 0;

            for (int i = 0; i < tab.Length; i++)
                somme += tab[i];

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
        #endregion

        // Le lancement du jeu et les program joueur + IA
        #region EXECUTIONS DU JEU
        // Lancement du Jeu
        private static Joueurs LaunchGame(int[] tab, State gameState, int baseNB, GameMode gameMode)
        {
            Joueurs gagnant = Joueurs.PLAYER1;

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
            Joueurs dernierJoueur = Joueurs.PLAYER1;
            Joueurs gagnant = Joueurs.PLAYER1;

            DisplayGame(tab, baseNB);

            while (gameState == State.PLAYING)
            {
                if (tourDuJoueur1 == true)
                {
                    humanPlay(tab, baseNB, Joueurs.PLAYER1);
                    tourDuJoueur1 = false;
                    dernierJoueur = Joueurs.PLAYER1;
                }
                else
                {
                    humanPlay(tab, baseNB, Joueurs.PLAYER2);
                    tourDuJoueur1 = true;
                    dernierJoueur = Joueurs.PLAYER2;
                }
                DisplayGame(tab, baseNB);
                gameState = checkTab(tab);
            }

            if (dernierJoueur == Joueurs.PLAYER1)
                gagnant = Joueurs.PLAYER2;
            else
                gagnant = Joueurs.PLAYER1;

            return gagnant;
        }

        // Execution du jeu en mdoe vs IA + décision du vainqueur
        private static Joueurs LaunchGameVsIA(int[] tab, State gameState, int baseNB, Difficulty difficultyMode)
        {
            bool tourDuJoueur1 = true;
            Joueurs dernierJoueur = Joueurs.PLAYER1;
            Joueurs gagnant = Joueurs.PLAYER1;

            DisplayGame(tab, baseNB);

            while (gameState == State.PLAYING)
            {               
                if(tourDuJoueur1 == true)
                {
                    humanPlay(tab, baseNB, dernierJoueur);
                    tourDuJoueur1 = false;
                    dernierJoueur = Joueurs.PLAYER1;
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

            if (dernierJoueur == Joueurs.PLAYER1)
                gagnant = Joueurs.IA;
            else
                gagnant = Joueurs.PLAYER1;

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
        }

        // Tour de l'IA de jouer
        private static void IAPlay(int[] tab, int baseNB, Difficulty difficultyMode)
        {
            int nbDeLignes = (baseNB + 1) / 2;
            int WhatLine=0;
            int HowMany=0;

            bool nbDelignesIsImpair = isImpair(nbDeLignes);
            bool nbLignesNonNulles_isImpair = isImpair(HowManyNoneZeroTab(tab));
            int nbLignesSingleAllumette = HowManySingleTab(tab);

            Console.WriteLine("L'IA réfléchit ...");
            Thread.Sleep(2000);

            if (difficultyMode == Difficulty.EASY)
            {
                Random rand = new Random();
                WhatLine = rand.Next(1, nbDeLignes);

                while (tab[WhatLine-1] == 0)
                {                    
                    WhatLine = rand.Next(1, nbDeLignes);
                }
                if (tab[WhatLine - 1] == 1)
                    HowMany = 1;
                else
                    HowMany = rand.Next(1, tab[WhatLine-1]);

            }
            else if (difficultyMode == Difficulty.HARD)
            {
                // Si le nombre de lignes de base est impair
                if (nbDelignesIsImpair == true)
                {
                    // Si le nombre de lignes non nulles est pair
                    if (nbLignesNonNulles_isImpair == false)
                    {
                        Random rand = new Random();
                        WhatLine = rand.Next(1, nbDeLignes);

                        while (tab[WhatLine - 1] == 0 || tab[WhatLine - 1] == 1)
                        {
                            WhatLine = rand.Next(1, nbDeLignes);
                        }

                        HowMany = tab[WhatLine-1];
                    }
                }
                else // Si le nombre de lignes de base est pair
                {

                }
            }

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

            while (state == false || positiv == false || impair == false)
            {
                Console.Write("Valeur entrée incorrecte, veuillez entrer un nombre POSITIF IMPAIRE: ");
                entry = Console.ReadLine();
                state = int.TryParse(entry, out q);
                positiv = isPositiv(q);
                impair = isImpair(q);
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
