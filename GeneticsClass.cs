using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using System.Collections;

namespace Project11
{
    internal class GeneticsClass
    {
        private string GenerateRandomBirdDNA()
        {
            const string validCharacters = "ACGT"; // DNA base pairs: adenine, cytosine, guanine, thymine
            int length = 120;

            StringBuilder builder = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < length; i++)
            {
                int index = random.Next(validCharacters.Length);
                char randomChar = validCharacters[index];
                builder.Append(randomChar);
            }

            return builder.ToString();
        }

        private List<string> GenerateChromosomes()
        {
            List<string> chromosomes = new List<string>();

            for (int i = 0; i < 20; i++)
            {
                string chromosome = GenerateRandomBirdDNA();
                chromosomes.Add(chromosome);
            }

            return chromosomes;
        }

        List<string> chromosomeList = GenerateChromosomes();

        private List<string> BreedChromosomes(List<string> parent1Chromosomes, List<string> parent2Chromosomes)
        {
            List<string> progenyChromosomes = new List<string>();

            // Make sure both parent lists have the same length
            int chromosomeCount = Math.Min(parent1Chromosomes.Count, parent2Chromosomes.Count);

            // Iterate over the chromosomes
            for (int i = 0; i < chromosomeCount; i++)
            {
                string parent1Chromosome = parent1Chromosomes[i];
                string parent2Chromosome = parent2Chromosomes[i];

                // Randomly select between the paired chromosomes
                string selectedChromosome = (new Random().Next(2) == 0) ? parent1Chromosome : parent2Chromosome;

                progenyChromosomes.Add(selectedChromosome);
            }

            return progenyChromosomes;
        }

        // Assuming you have two parent chromosome lists: parent1Chromosomes and parent2Chromosomes

        List<string> progenyChromosomes = BreedChromosomes(parent1Chromosomes, parent2Chromosomes);

// Print the progeny chromosomes
foreach (string chromosome in progenyChromosomes)
{
    Console.WriteLine(chromosome);
}

// How and where to store the genomes as sprites spawn and die
        public string Genotype_1
        {
                get
                {
                return Genotype_1;
                }
                set
                {

                }
        }
}
