using System;
using System.Threading;

namespace Allumettes
{
    class Program
    {

        public enum State
        {
            PLAYING = 0,
            OVER = 1,
        }

        static void Main(string[] args)
        {
            State m_gameState = State.PLAYING;
            
            Console.Write("La base doit être impaire\nDe combien est la base de votre jeu ? ");
            int _base = getImpairPositivNumber(Console.ReadLine());
            int lignes = (_base+1) / 2;
            int[] game = new int[lignes];
            FillTab(game, _base); // on remplit le tableau
            playingGame(game, m_gameState, _base);
        }

        private static void playingGame(int[] tab, State gameState, int baseNB)
        {
            while (gameState == State.PLAYING)
            {
                DisplayGame(tab, baseNB);
                humanPlay(tab, baseNB);
                DisplayGame(tab, baseNB);
                gameState = checkTab(tab);
                IAPlay(tab, baseNB);
                DisplayGame(tab, baseNB);
            }
        }

        private static State checkTab(int[] tab)
        {
            for(int i=0; i<tab.Length; i++)
            {
                if (tab[i] != 0)
                    return State.PLAYING;

            }
            return State.OVER;
        }

        private static void pullBars(int[] tab, int line, int nb)
        {
            tab[line] = tab[line] - nb;
        }

        private static void humanPlay (int[] tab, int baseNB)
        {
            Console.Write("C'est à votre tour, sur quelle ligne voulez-vous retirer une allumette ? ");
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

        private static void IAPlay(int[] tab, int baseNB)
        {
            /*int WhatLine = 0;
            int HowMany = 0;

            int nbLignesPair = 0;
            int nbLignesImpair = 0;
            int nbLignesAvecUneSeuleBarre = 0;*/



            //pullBars(tab, WhatLine, HowMany);
            Console.WriteLine("L'IA réfléchit ...");
            Thread.Sleep(2000);
        }


        private static void FillTab(int[] tab, int baseNB)
        {
            int nbBase = baseNB;

            for (int i = tab.Length-1; i >= 0; i--)
            {
                tab[i] = nbBase;
                nbBase = nbBase - 2;
            }
        }

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

        private static void DisplayTop(int nb)
        {
            DisplaySpace(2);
            for (int i = 0; i < nb; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n");
        }

        private static void DisplayI(int nb)
        {
            for (int i = 0; i < nb; i++)
            {
                Console.Write("I");
            }
        }

        private static void DisplaySpace(int nb)
        {
            for (int i = 0; i < nb; i++)
            {
                Console.Write(" ");
            }
        }

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

        /*
         * @description Vérifie que le nombre est plus petit que l'autre
         * @return true plus petit ou false plus grand ou egal
         */

        private static bool isSmaller(int x, int y)
        {
            if (x <= y)
                return true;
            else
                return false;
        }


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

        /*
         * @description Vérifie que le nombre est impair ou non
         * @return true impair ou false pair
         */

        private static bool isImpair(int number)
        {
            float n = (float)number;

            if (n%2f == 0)
                return false;
            else
                return true;
        }

        /*
         * @description Vérifie que le nombre est positif ou non
         * @return true positif ou false négatif
         */

        private static bool isPositiv(int number)
        {
            if (number > 0)
                return true;
            else
                return false;
        }
    }
}
