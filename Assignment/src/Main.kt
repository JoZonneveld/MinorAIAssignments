import kotlin.random.Random

fun main(args: Array<String>) {

    val mutationRate = 0.6
    val crossoverRate = 0.5
    val elitism = false
    val populationSize = 5
    val numIterations = 50

    var currentPopulation = IntRange(1, populationSize).map { createIndividual() }

    for (i in 0 until numIterations) {
        val fitness = currentPopulation.map { computeFitness(it) }

        var startIndex = 0

        val nextPopulation = ArrayList<String>()

        if (elitism) {
            startIndex = 1
            val populationWithFitness = currentPopulation.zip(fitness)
            val populationSorted = populationWithFitness.sortedByDescending { it.second }
            val bestIndividual = populationSorted.first()
            nextPopulation.add(bestIndividual.first)
        }

        for (j in startIndex until populationSize) {
            val parents = selectTwoParents(currentPopulation, fitness)

            var offspring : List<String>

            if(Random.nextDouble() < crossoverRate){
                offspring = crossover(parents)
            } else {
                offspring = parents
            }

            if(nextPopulation.size < populationSize){
                nextPopulation.add(mutation(offspring.first(), mutationRate))
            }
            if(nextPopulation.size < populationSize){
                nextPopulation.add(mutation(offspring.last(), mutationRate))
            }
        }
        currentPopulation = nextPopulation
    }

    val finalFitness = currentPopulation.zip(currentPopulation.map { computeFitness(it) })
    val bestIndividual = finalFitness.sortedByDescending { it.second }.first().first
    val bestFitness = finalFitness.sortedByDescending { it.second }.first().second
    val avg = finalFitness.map { it.second }.average()

    println("Best fitness = $bestFitness")
    println("Best individual = $bestIndividual")
    println("Average fitness = $avg")
}

fun createIndividual(): String {
    var output = ""
    val random = Random
    for (i in 0..4) {
        output += random.nextInt(0, 2)
    }

    return output
}

fun computeFitness(indu: String): Double {
    var fitness = 0.0
    val x = indu.toInt(2).toDouble()
    fitness = -(Math.pow(x, 2.0)) + (7 * x)

    return fitness
}

fun selectTwoParents(individuals: List<String>, fitness: List<Double>): List<String> {
    val minFitness = fitness.min()!!
    var cursor = minFitness

    val individualsWithFitness = individuals.zip(fitness)
    val individualsWithRanges = individualsWithFitness.map { (ind, fitness) ->
        val range = fitness + Math.abs(Math.min(minFitness, 0.0))
        val cursorEnd = cursor + range
        val res = (cursor to cursorEnd) to ind
        cursor += range
        res
    }

    return IntRange(1, 2).map {
        val point = Random.nextDouble(minFitness, cursor)
        individualsWithRanges.find { (range, _) ->
            range.first <= point && point < range.second
        }?.second ?: individualsWithRanges.first().second
    }
}

fun crossover(parents: List<String>): List<String> {
    val l = parents[0].length
    val pointer = Random.nextInt(1, l - 1)
    return listOf(
        parents[0].substring(0, pointer) + parents[1].substring(pointer, l),
        parents[1].substring(0, pointer) + parents[0].substring(pointer, l)
    )
}

fun mutation(individual: String, mutationRate: Double): String {
    var output = ""
    for (char in individual) {
        val rnd = Random.nextDouble(1.0)
        output += if (rnd <= mutationRate) {
            if (char == '1') {
                "0"
            } else {
                "1"
            }
        } else {
            char
        }
    }
    return output
}